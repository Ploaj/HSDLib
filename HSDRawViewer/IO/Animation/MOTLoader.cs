using HSDRaw;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace HSDRawViewer.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class MOTLoader
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static short[] GetJointTable(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("No JCV file loaded", "JCV File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new short[0];
            }

            using BinaryReaderExt r = new(new FileStream(filePath, FileMode.Open));
            r.BigEndian = true;

            r.Seek(0x10);
            short count = r.ReadInt16();

            short[] vals = new short[count];

            for (int i = 0; i < vals.Length; i++)
                vals[i] = r.ReadInt16();

            return vals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<MOT_FILE> UnpackMOTs(string path)
        {
            List<MOT_FILE> anims = new();

            using (BinaryReaderExt r = new(new FileStream(path, FileMode.Open)))
            {
                r.BigEndian = true;

                r.ReadInt32(); // unknown

                int count = r.ReadInt32();
                uint headerSize = r.ReadUInt32();
                uint fileLength = r.ReadUInt32();

                if (fileLength != r.Length)
                    throw new Exception("File Length Mismatch");

                for (uint i = 0; i < count; i++)
                {
                    r.Seek(headerSize + i * 4);
                    r.Seek(r.ReadUInt32());

                    MOT_FILE anim = new();
                    if (r.Position != 0)
                        anim.Parse(r);
                    anims.Add(anim);
                }
            }
            return anims;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class MOT_FILE
    {
        public float PlaySpeed;

        public float EndTime;

        public List<MOT_JOINT> Joints = new();

        public static bool IsMotXML(string filePath)
        {
            XmlReaderSettings settings = new();
            settings.IgnoreWhitespace = true;
            using XmlReader r = XmlReader.Create(filePath, settings);
            while (r.Read())
            {
                switch (r.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (r.Name == "MOT")
                            {
                                return true;
                            }
                        }
                        break;
                }
            }

            return false;
        }

        public MOT_FILE()
        {

        }

        public MOT_FILE(string filePath)
        {
            if (Path.GetExtension(filePath).ToLower().Equals(".xml"))
            {
                XmlReaderSettings settings = new();
                settings.IgnoreWhitespace = true;
                using XmlReader r = XmlReader.Create(filePath, settings);
                ParseXML(r);
            }
            else
            {
                using BinaryReaderExt r = new(new FileStream(filePath, FileMode.Open));
                Parse(r);
            }
        }

        /// <summary>
        /// Saves to File
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using BinaryWriterExt w = new(new FileStream(filename, FileMode.Create));
            w.BigEndian = true;
            if (Joints.Count == 0)
                return;

            uint animStart = (uint)w.BaseStream.Position;
            w.Write(Joints.Count);
            w.Write(0x10);
            w.Write(PlaySpeed);
            w.Write(EndTime);

            // padding
            long headerStart = w.BaseStream.Position;
            w.Write(new byte[Joints.Count * 0x20]);

            for (int j = 0; j < Joints.Count; j++)
            {
                MOT_JOINT joint = Joints[j];

                uint temp = (uint)w.BaseStream.Position;
                w.Seek((uint)(headerStart + j * 0x20));
                w.Write(joint.Flag1);
                w.Write(joint.Flag2);
                w.Write((short)joint.TrackFlag);
                w.Write(joint.BoneID);
                w.Write((short)joint.Keys.Count);
                w.Write(joint.MaxTime);
                w.Write(joint.Unknown);
                w.Seek(temp);

                WriteAt(w, (int)(headerStart + j * 0x20 + 0x10), (int)(w.BaseStream.Position - animStart));
                foreach (MOT_KEY k in joint.Keys)
                {
                    w.Write(k.Time);
                }
                if (w.BaseStream.Position % 0x10 != 0)
                    w.Write(new byte[0x10 - w.BaseStream.Position % 0x10]);

                if (joint.Flag1 == 0x02)
                {
                    WriteAt(w, (int)(headerStart + j * 0x20 + 0x14), (int)(w.BaseStream.Position - animStart));
                    foreach (MOT_KEY k in joint.Keys)
                    {
                        w.Write((BitConverter.ToInt16(BitConverter.GetBytes(k.X), 2)));
                        w.Write((BitConverter.ToInt16(BitConverter.GetBytes(k.Y), 2)));
                        w.Write((BitConverter.ToInt16(BitConverter.GetBytes(k.Z), 2)));
                        w.Write((BitConverter.ToInt16(BitConverter.GetBytes(k.W), 2)));
                    }
                    if (w.BaseStream.Position % 0x10 != 0)
                        w.Write(new byte[0x10 - w.BaseStream.Position % 0x10]);
                }
            }

        }

        private void WriteXML(XmlWriter file)
        {
            float prevTime = Single.NegativeInfinity;

            file.WriteStartElement("MOT");
            file.WriteAttributeString("PlaySpeed", "" + PlaySpeed);
            file.WriteAttributeString("EndTime", "" + EndTime);

            foreach (MOT_JOINT j in Joints)
            {
                file.WriteStartElement("Joint");
                file.WriteAttributeString("Flag1", "" + j.Flag1);
                file.WriteAttributeString("Flag2", "" + j.Flag2);
                file.WriteAttributeString("TrackFlag", "" + j.TrackFlag);
                file.WriteAttributeString("BoneID", "" + j.BoneID);
                file.WriteAttributeString("MaxTime", "" + j.MaxTime);
                file.WriteAttributeString("Unknown", "" + j.Unknown);
                file.WriteEndElement();
            }

            do
            {
                float currTime = Single.PositiveInfinity;

                // Find the next keyframe
                foreach (MOT_JOINT j in Joints)
                {
                    foreach (MOT_KEY k in j.Keys)
                    {
                        if (k.Time > prevTime)
                        {
                            currTime = Math.Min(currTime, k.Time);
                            break;
                        }
                    }
                }

                List<MOT_KEY> keys = new();
                List<int> joints = new();

                int jointIdx = 0;
                // Find all joints that have this keyframe
                foreach (MOT_JOINT j in Joints)
                {
                    foreach (MOT_KEY k in j.Keys)
                    {
                        if (k.Time == currTime)
                        {
                            keys.Add(k);
                            joints.Add(jointIdx);
                            break;
                        }
                    }

                    jointIdx++;
                }

                Debug.Assert(keys.Count == joints.Count);
                if (keys.Count == 0)
                {
                    break;
                }

                file.WriteStartElement("Key");
                file.WriteAttributeString("Time", "" + currTime);
                for (int i = 0; i < keys.Count; i++)
                {
                    MOT_KEY k = keys[i];
                    int j = joints[i];

                    file.WriteStartElement("Joint");
                    file.WriteAttributeString("Index", "" + j);
                    file.WriteAttributeString("X", "" + k.X);
                    file.WriteAttributeString("Y", "" + k.Y);
                    file.WriteAttributeString("Z", "" + k.Z);
                    file.WriteAttributeString("W", "" + k.W);
                    file.WriteEndElement();
                }
                file.WriteEndElement();

                prevTime = currTime;
            } while (true);
            file.WriteEndElement();
        }

        public void ExportXML(string filename)
        {
            XmlWriterSettings settings = new();
            settings.Indent = true;
            settings.IndentChars = "\t";
            using XmlWriter file = XmlWriter.Create(filename, settings);
            WriteXML(file);
        }

        private void ParseXML(XmlReader r)
        {
            const int STATE_ENTER = 0;
            const int STATE_JOINTS = 1;
            const int STATE_KEYS = 2;
            int state = STATE_ENTER;
            float time = Single.NegativeInfinity;
            while (r.Read())
            {
                switch (r.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (state == STATE_ENTER)
                            {
                                if (r.Name != "MOT")
                                {
                                    throw new NotSupportedException("Expected Element \"MOT\"");
                                }

                                string strPlaySpeed = r.GetAttribute("PlaySpeed");
                                string strEndTime = r.GetAttribute("EndTime");

                                if (!float.TryParse(strPlaySpeed, out PlaySpeed))
                                {
                                    throw new NotSupportedException("Could not parse attribute \"PlaySpeed\"");
                                }

                                if (!float.TryParse(strEndTime, out EndTime))
                                {
                                    throw new NotSupportedException("Could not parse attribute \"EndTime\"");
                                }

                                state = STATE_JOINTS;
                            }
                            else if (state == STATE_JOINTS)
                            {
                                if (r.Name == "Joint")
                                {
                                    MOT_JOINT j = new();

                                    string strFlag1 = r.GetAttribute("Flag1");
                                    string strFlag2 = r.GetAttribute("Flag2");
                                    string strTrackFlag = r.GetAttribute("TrackFlag");
                                    string strBoneID = r.GetAttribute("BoneID");
                                    string strMaxTime = r.GetAttribute("MaxTime");
                                    string strUnknown = r.GetAttribute("Unknown");

                                    if (!byte.TryParse(strFlag1, out j.Flag1))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"Flag1\"");
                                    }

                                    if (!byte.TryParse(strFlag2, out j.Flag2))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"Flag2\"");
                                    }

                                    if (!Enum.TryParse<MOT_FLAGS>(strTrackFlag, out j.TrackFlag))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"Flag2\"");
                                    }

                                    if (!short.TryParse(strBoneID, out j.BoneID))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"BoneID\"");
                                    }

                                    if (!float.TryParse(strMaxTime, out j.MaxTime))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"MaxTime\"");
                                    }

                                    if (!int.TryParse(strUnknown, out j.Unknown))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"Unknown\"");
                                    }

                                    Joints.Add(j);
                                }
                                else if (r.Name == "Key")
                                {
                                    state = STATE_KEYS;
                                }
                            }

                            if (state == STATE_KEYS)
                            {
                                if (r.Name == "Key")
                                {
                                    string strTime = r.GetAttribute("Time");

                                    if (!float.TryParse(strTime, out time))
                                    {
                                        throw new NotSupportedException("Could not parse attribute \"Time\"");
                                    }
                                }
                                else if (r.Name == "Joint")
                                {
                                    if (Single.IsNegativeInfinity(time))
                                    {
                                        throw new NotSupportedException("No Key element defined");
                                    }

                                    string indexStr;
                                    string xStr, yStr, zStr, wStr;

                                    indexStr = r.GetAttribute("Index");
                                    if (!int.TryParse(indexStr, out int index))
                                    {
                                        throw new NotSupportedException("Unable to parse Index attribute");
                                    }

                                    xStr = r.GetAttribute("X");
                                    if (!float.TryParse(xStr, out float X))
                                    {
                                        throw new NotSupportedException("Unable to parse X attribute");
                                    }

                                    yStr = r.GetAttribute("Y");
                                    if (!float.TryParse(yStr, out float Y))
                                    {
                                        throw new NotSupportedException("Unable to parse Y attribute");
                                    }

                                    zStr = r.GetAttribute("Z");
                                    if (!float.TryParse(zStr, out float Z))
                                    {
                                        throw new NotSupportedException("Unable to parse Z attribute");
                                    }

                                    wStr = r.GetAttribute("W");
                                    if (!float.TryParse(wStr, out float W))
                                    {
                                        throw new NotSupportedException("Unable to parse W attribute");
                                    }

                                    MOT_KEY key = new();
                                    key.Time = time;
                                    key.X = X;
                                    key.Y = Y;
                                    key.Z = Z;
                                    key.W = W;

                                    Joints[index].Keys.Add(key);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void WriteAt(BinaryWriterExt ext, int pos, int value)
        {
            long tmp = ext.BaseStream.Position;
            ext.BaseStream.Position = pos;

            ext.Write(value);

            ext.BaseStream.Position = tmp;
        }

        public void Parse(BinaryReaderExt r)
        {
            r.BigEndian = true;

            uint start = r.Position;

            int sectionCount = r.ReadInt32();
            int sectionHeaderLength = r.ReadInt32();
            PlaySpeed = r.ReadSingle();
            EndTime = r.ReadSingle();

            for (int j = 0; j < sectionCount; j++)
            {
                MOT_JOINT joint = new();

                Joints.Add(joint);

                joint.Flag1 = r.ReadByte();
                joint.Flag2 = r.ReadByte();
                joint.TrackFlag = (MOT_FLAGS)r.ReadUInt16();
                joint.BoneID = r.ReadInt16();
                short floatCount = r.ReadInt16();

                joint.MaxTime = r.ReadSingle();
                joint.Unknown = r.ReadInt32();

                uint offset1 = r.ReadUInt32() + start;
                uint offset2 = r.ReadUInt32() + start;
                uint offset3 = r.ReadUInt32() + start;
                uint offset4 = r.ReadUInt32() + start;

                if (offset3 != start)
                    throw new NotSupportedException("Section 3 detected");

                if (offset4 != start)
                    throw new NotSupportedException("Section 4 detected");

                uint temp = r.Position;
                for (uint k = 0; k < floatCount; k++)
                {
                    MOT_KEY key = new();

                    r.Seek(offset1 + 4 * k);
                    key.Time = r.ReadSingle();

                    if (offset2 != start)
                    {
                        r.Seek(offset2 + 8 * k);

                        key.X = BitConverter.ToSingle(BitConverter.GetBytes(((r.ReadByte() & 0xFF) << 24) | ((r.ReadByte() & 0xFF) << 16)), 0);
                        key.Y = BitConverter.ToSingle(BitConverter.GetBytes(((r.ReadByte() & 0xFF) << 24) | ((r.ReadByte() & 0xFF) << 16)), 0);
                        key.Z = BitConverter.ToSingle(BitConverter.GetBytes(((r.ReadByte() & 0xFF) << 24) | ((r.ReadByte() & 0xFF) << 16)), 0);
                        key.W = BitConverter.ToSingle(BitConverter.GetBytes(((r.ReadByte() & 0xFF) << 24) | ((r.ReadByte() & 0xFF) << 16)), 0);
                    }

                    joint.Keys.Add(key);
                }


                r.Seek(temp);

            }
        }
    }

    [Flags]
    public enum MOT_FLAGS
    {
        TRANSLATE = 0x01,
        SCALE = 0x02,
        ROTATE = 0x08,
        ENABLED = 0x20,
        DISABLED = 0x40
    }

    public class MOT_JOINT
    {
        public byte Flag1 = 2;

        public byte Flag2 = 2;

        public MOT_FLAGS TrackFlag;

        public short BoneID;

        public float MaxTime;

        public int Unknown;

        public List<MOT_KEY> Keys = new();

        public MOT_KEY GetKey(float time)
        {
            if (Keys.Count == 0)
                return null;

            if (Keys.Count == 1)
                return Keys[0];

            if (Keys[0].Time > time)
                return Keys[0];

            // Keys.FindIndex should not return 0 here due to above check
            int index = Keys.FindIndex(e => e.Time > time) - 1;

            // If index is negative all keys come before the provided time, pick the last one
            if (index < 0)
                return Keys[Keys.Count - 1];

            // var weight = (time - Keys[index].Time) / (Keys[index + 1].Time - Keys[index].Time);

            float ftime = time - Keys[index].Time;
            float fterm = (Keys[index + 1].Time - Keys[index].Time);

            return new MOT_KEY()
            {
                Time = time,
                X = AnimationInterpolationHelper.Lerp(fterm, ftime, Keys[index].X, Keys[index + 1].X),
                Y = AnimationInterpolationHelper.Lerp(fterm, ftime, Keys[index].Y, Keys[index + 1].Y),
                Z = AnimationInterpolationHelper.Lerp(fterm, ftime, Keys[index].Z, Keys[index + 1].Z),
                W = AnimationInterpolationHelper.Lerp(fterm, ftime, Keys[index].W, Keys[index + 1].W),
            };
        }
    }

    public class MOT_KEY
    {
        public float Time;
        public float X;
        public float Y;
        public float Z;
        public float W;
    }

}
