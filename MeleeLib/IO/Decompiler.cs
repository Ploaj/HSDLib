using System;
using System.Collections.Generic;
using MeleeLib.GCX;
using MeleeLib.DAT;
using MeleeLib.DAT.Melee;
using MeleeLib.DAT.Animation;
using MeleeLib.DAT.MatAnim;
using MeleeLib.DAT.Script;

using MeleeLib.KAR;

namespace MeleeLib.IO
{
    public class Decompiler
    {
        public static DATFile Decompile(byte[] Data)
        {
            DATReader d = new DATReader(Data);
            d.EnableDebug();
            int FileSize = d.Int();
            int DataBlockSize = d.Int();
            int RelocationCount = d.Int();
            int RootCount = d.Int();
            int ReferenceNodeCount = d.Int();
            d.Skip(12); // 0 Padding/Reserved

            int DataBlockStart = d.Pos();
            d.Seek(DataBlockSize + 0x20);
            int RelocationOffset = d.Pos();

            DATFile dat = new DATFile();
            dat.Text = "DAT";

            //Handle RelocationTable
            int minData = 0;
            int firstoff = 0;
            for (int i = 0; i < RelocationCount; ++i)
            {
                int relocationOffset = RelocationOffset + i * 4;

                d.Seek(relocationOffset);
                int dataOffset = d.Int() + 0x20;
                // Hacky way to grab the entire data buffer
                d.WriteInt(relocationOffset, dataOffset);

                if (i == 0) firstoff = dataOffset;

                d.Seek(dataOffset);
                int off = d.Int() + 0x20;
                if(off <= firstoff)
                    minData = Math.Max(minData, off);

                d.Seek(dataOffset);
                d.WriteInt(dataOffset, off);
            }
            // hack to grab only vertexdata buffer
            //
            d.Seek(RelocationOffset + RelocationCount * 4); // skip relocation table
            int strOffset = d.Pos() + RootCount * 8 + ReferenceNodeCount * 8;

            //Create Roots
            RootCount += ReferenceNodeCount;
            int[] RootOffset = new int[RootCount];
            for (int i = 0; i < RootCount; i++)
            {
                RootOffset[i] = d.Int() + 0x20;
                DATRoot root = new DATRoot();
                dat.AddRoot(root);
                root.Text = d.String(strOffset + d.Int());

                int temp = d.Pos();
                if (root.Text.EndsWith("_figatree"))
                {
                    //animation
                    d.Seek(RootOffset[i]);
                    DatAnimation Anim = new DatAnimation();
                    Anim.Deserialize(d, root);
                    root.Animations.Add(Anim);
                }
                else
                if (root.Text.EndsWith("matanim_joint"))
                {
                    // Material Animation
                    d.Seek(RootOffset[i]);
                    DatMatAnim mat = new DatMatAnim();
                    mat.Deserialize(d, root, root.MatAnims);
                }
                else
                if (root.Text.EndsWith("_joint"))
                {
                    // Process NodeTree
                    d.Seek(RootOffset[i]);
                    ProcessRoot(d, root);
                }
                else
                if (root.Text.StartsWith("ftData"))
                {
                    // Process NodeTree
                    d.Seek(RootOffset[i]);
                    DatFighterData data = new DatFighterData();
                    data.Deserialize(d, root);
                }
                else
                if (root.Text.StartsWith("map_head"))
                {
                    // Process NodeTree
                    d.Seek(RootOffset[i]);
                    Map_Head data = new Map_Head();
                    data.Deserialize(d, root);
                }
                else
                if (root.Text.StartsWith("vcDataStar"))
                {
                    // Process NodeTree
                    d.Seek(RootOffset[i]);
                    KAR_HSD_Vehicle data = new KAR_HSD_Vehicle();
                    data.Deserialize(d, root);
                }
                else
                {
                    Console.WriteLine(root.Text + " 0x" + RootOffset[i].ToString("X"));
                    //Console.WriteLine("This DAT Cannot be opened yet due to unknown structure: " + root.Text);

                }
                d.Seek(temp);
            }

            // Gather all roots
            List<DATRoot> AllRoots = dat.GetAllSubRoots();

            // clean up and assign dobjs their jobjs
            Dictionary<int, int> OffsetSize = new Dictionary<int, int>();
            foreach (DATRoot root in AllRoots)
            {
                foreach (DatDOBJ dobj in root.GetDataObjects())
                {
                    foreach (DatPolygon p in dobj.Polygons)
                    {
                        // analyze to find exact size of attribute buffer
                        foreach(GXDisplayList dl in p.DisplayLists)
                        {
                            foreach(GXIndexGroup idx in dl.Indices)
                            {
                                for (int i = 0; i < p.AttributeGroup.Attributes.Count; i++)
                                {
                                    GXAttr a = p.AttributeGroup.Attributes[i];
                                    int size = (idx.Indices[i] + 1) * a.Stride;
                                    if (!OffsetSize.ContainsKey(a.Offset))
                                    {
                                        OffsetSize.Add(a.Offset, 0);
                                    }
                                    if (size > OffsetSize[a.Offset])
                                        OffsetSize[a.Offset] = size;
                                }
                            }
                        }
                        foreach (List<DatBoneWeight> bwl in p.BoneWeightList)
                        {
                            foreach (DatBoneWeight w in bwl)
                            {
                                if (w.jobj == null)
                                    w.jobj = root.GetJOBJ(w.Offset);
                            }
                        }
                    }
                }
            }

            // Get the data buffers
            Dictionary<int, byte[]> OffsetData = new Dictionary<int, byte[]>();
            foreach (int offset in OffsetSize.Keys)
            {
                OffsetData.Add(offset, d.getSection(offset + 0x20, OffsetSize[offset]));
            }

            // Set the Data buffers
            foreach (DATRoot root in AllRoots)
            {
                foreach(GXAttribGroup g in root.Attributes)
                    foreach(GXAttr a in g.Attributes)
                    {
                        if (a.AttributeType != GXAttribType.GX_DIRECT)
                            a.DataBuffer = OffsetData[a.Offset];
                    }
            }


            d.WriteRead("FileRead.bin");

            return dat;
        }

        private static void ProcessRoot(DATReader d, DATRoot root)
        {
            ProcessJOBJ(d, root);
        }

        private static void ProcessJOBJ(DATReader d, DATRoot parent)
        {
            DatJOBJ j = new DatJOBJ();
            parent.Bones.Add(j);
            j.Deserialize(d, parent);
        }

    }
}
