using HSDRaw;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

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

            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
            {
                r.BigEndian = true;

                r.Seek(0x10);
                var count = r.ReadInt16();

                short[] vals = new short[count];

                for (int i = 0; i < vals.Length; i++)
                    vals[i] = r.ReadInt16();

                return vals;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<MOT_FILE> UnpackMOTs(string path)
        {
            var anims = new List<MOT_FILE>();

            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(path, FileMode.Open)))
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

                    MOT_FILE anim = new MOT_FILE();
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

        public List<MOT_JOINT> Joints = new List<MOT_JOINT>();

        public MOT_FILE()
        {

        }

        public MOT_FILE(string filePath)
        {
            using (BinaryReaderExt r = new BinaryReaderExt(new FileStream(filePath, FileMode.Open)))
                Parse(r);
        }

        /// <summary>
        /// Saves to File
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            using (BinaryWriterExt w = new BinaryWriterExt(new FileStream(filename, FileMode.Create)))
            {
                w.BigEndian = true;
                if (Joints.Count == 0)
                    return;

                var animStart = (uint)w.BaseStream.Position;
                w.Write(Joints.Count);
                w.Write(0x10);
                w.Write(PlaySpeed);
                w.Write(EndTime);

                // padding
                var headerStart = w.BaseStream.Position;
                w.Write(new byte[Joints.Count * 0x20]);

                for (int j = 0; j < Joints.Count; j++)
                {
                    var joint = Joints[j];

                    var temp = (uint)w.BaseStream.Position;
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
                    foreach (var k in joint.Keys)
                    {
                        w.Write(k.Time);
                    }
                    if (w.BaseStream.Position % 0x10 != 0)
                        w.Write(new byte[0x10 - w.BaseStream.Position % 0x10]);

                    if (joint.Flag1 == 0x02)
                    {
                        WriteAt(w, (int)(headerStart + j * 0x20 + 0x14), (int)(w.BaseStream.Position - animStart));
                        foreach (var k in joint.Keys)
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

        }

        private void WriteAt(BinaryWriterExt ext, int pos, int value)
        {
            var tmp = ext.BaseStream.Position;
            ext.BaseStream.Position = pos;

            ext.Write(value);

            ext.BaseStream.Position = tmp;
        }

        public void Parse(BinaryReaderExt r)
        {
            r.BigEndian = true;

            var start = r.Position;

            var sectionCount = r.ReadInt32();
            var sectionHeaderLength = r.ReadInt32();
            PlaySpeed = r.ReadSingle();
            EndTime = r.ReadSingle();

            for (int j = 0; j < sectionCount; j++)
            {
                MOT_JOINT joint = new MOT_JOINT();

                Joints.Add(joint);

                joint.Flag1 = r.ReadByte();
                joint.Flag2 = r.ReadByte();
                joint.TrackFlag = (MOT_FLAGS)r.ReadUInt16();
                joint.BoneID = r.ReadInt16();
                var floatCount = r.ReadInt16();

                joint.MaxTime = r.ReadSingle();
                joint.Unknown = r.ReadInt32();

                var offset1 = r.ReadUInt32() + start;
                var offset2 = r.ReadUInt32() + start;
                var offset3 = r.ReadUInt32() + start;
                var offset4 = r.ReadUInt32() + start;

                if (offset3 != start)
                    throw new NotSupportedException("Section 3 detected");

                if (offset4 != start)
                    throw new NotSupportedException("Section 4 detected");

                var temp = r.Position;
                for (uint k = 0; k < floatCount; k++)
                {
                    MOT_KEY key = new MOT_KEY();

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

        public List<MOT_KEY> Keys = new List<MOT_KEY>();

        public MOT_KEY GetKey(float time)
        {
            if (Keys.Count == 0)
                return null;

            if (Keys.Count == 1)
                return Keys[0];

            if (Keys[0].Time > time)
                return Keys[0];

            // Keys.FindIndex should not return 0 here due to above check
            var index = Keys.FindIndex(e => e.Time > time) - 1;
            
            // If index is negative all keys come before the provided time, pick the last one
            if (index < 0)
                return Keys[Keys.Count - 1];

            var weight = (time - Keys[index].Time) / (Keys[index + 1].Time - Keys[index].Time);
            return new MOT_KEY()
            {
                Time = time,
                X = AnimationInterpolationHelper.Lerp(Keys[index].X, Keys[index + 1].X, weight),
                Y = AnimationInterpolationHelper.Lerp(Keys[index].Y, Keys[index + 1].Y, weight),
                Z = AnimationInterpolationHelper.Lerp(Keys[index].Z, Keys[index + 1].Z, weight),
                W = AnimationInterpolationHelper.Lerp(Keys[index].W, Keys[index + 1].W, weight),
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
