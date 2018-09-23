using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.DAT.Animation;
using MeleeLib.DAT.MatAnim;
using MeleeLib.DAT;
using MeleeLib.GCX;

namespace MeleeLib.IO
{
    public class Compiler
    {

        public static void Compile(DATFile d, string Fname)
        {
            DATWriter o = new DATWriter();

            o.Int(0); // FileSize
            o.Int(0); // DataSize
            o.Int(0); // RelocationCount
            o.Int(d.Roots.Length); // RootCount
            o.Int(0); // ReferenceNodeCount
            o.Int(0); o.Int(0); o.Int(0);//Padding

            //Attribute Buffers
            List<byte[]> data = new List<byte[]>();
            List<DATRoot> Roots = d.GetAllSubRoots();
            for (int i = 0; i < Roots.Count; i++)
            {
                DATRoot root = Roots[i];
                foreach (GXAttribGroup g in root.Attributes)
                {
                    foreach(GXAttr a in g.Attributes)
                    {
                        if (a.DataBuffer != null && !data.Contains(a.DataBuffer))
                        {
                            data.Add(a.DataBuffer);
                            o.AddObject(a.DataBuffer);
                            o.Bytes(a.DataBuffer);
                            o.Align(0x20);
                        }
                    }
                }
            }
            //o.AddObject("RootBuffer");
            //if(d.DataBuffer != null)
            //    o.Bytes(d.DataBuffer);

            for (int i = 0; i < d.Roots.Length; i++)
            {
                DATRoot root = d.Roots[i];
                if (root.FighterData.Count > 0)
                    Console.WriteLine("Warning: Unsupported building of Fighter Script Data");


                foreach (DatNode extra in root.ExtraNodes)
                {
                    extra.Serialize(o);
                }

                foreach (DatNode n in root.Bones)
                {
                    n.Serialize(o);
                }
                List<byte[]> imaged = root.GetImageData();
                for (int j = imaged.Count-1; j >=0 ; j--)
                {
                    if (!o.ContainsObject(imaged[j]))
                    {
                        o.AddObject(imaged[j]);
                        o.Align(0x20);
                        o.Bytes(imaged[j]);
                        o.Align(0x20);
                    }
                }
                foreach(DatMatAnim a in root.MatAnims)
                {
                    a.SerializeData(o);
                }
                foreach (DatMatAnim a in root.MatAnims)
                {
                    a.Serialize(o, root.MatAnims);
                }

                List<byte[]> matimaged = root.GetMatAnimImageData();
                for (int j = 0; j < matimaged.Count; j++)
                {
                    if (!o.ContainsObject(matimaged[j]))
                    {
                        o.Align(32);
                        o.AddObject(matimaged[j]);
                        o.Bytes(matimaged[j]);
                    }
                }

                foreach (DatAnimation a in root.Animations)
                {
                    a.Serialize(o);
                }
            }

            //RelocationTable
            int RelocationTableOffset = o.Size();
            int PointerCount = o.WriteRelocationTable();

            // Footer
            DATWriter RootStrings = new DATWriter();
            for (int i = 0; i < d.Roots.Length; i++)
            {
                DATRoot root = d.Roots[i];
                if (root.Bones.Count > 0)
                    o.Object(root.Bones[0]);
                else
                if (root.MatAnims.Count > 0)
                    o.Object(root.MatAnims[0]);
                else
                if (root.Animations.Count > 0)
                    o.Object(root.Animations[0]);
                else
                if (root.ExtraNodes.Count > 0)
                    o.Object(root.ExtraNodes[0]);
                else
                    o.Int(0);
                o.Int(RootStrings.Size());
                RootStrings.String(root.Text);
            }
            o.WriteData(RootStrings);

            o.WriteIntAt(0, o.Size());
            o.WriteIntAt(4, RelocationTableOffset - 0x20);
            o.WriteIntAt(8, PointerCount);
            o.UpdateRelocationTable();

            o.Save(Fname);
        }
    }
}
