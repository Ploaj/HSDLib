using HSDRaw;
using HSDRaw.Common.Animation;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Tools.Animation;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer.Tools.Melee
{
    public class FighterBoneRemapper
    {
        private class AnimManager
        {
            private string FilePath { get; set; }

            private FighterAJManager Animation { get; set; }

            private HSDRawFile AJFile { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filePath"></param>
            public AnimManager(string filePath)
            {
                FilePath = filePath;

                if (!string.IsNullOrEmpty(filePath))
                {
                    // check if data or file
                    var f = new HSDRawFile(filePath);
                    Animation = new FighterAJManager();
                    if (f.Roots[0].Name.Contains("_figatree"))
                    {
                        Animation.ScanAJData(File.ReadAllBytes(filePath));
                    }
                    else
                    {
                        AJFile = f;
                        Animation.ScanAJFile(filePath);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            /// <param name="target"></param>
            /// <returns></returns>
            public bool Remap(JointMap source, JointMap target)
            {
                if (Animation == null)
                    return false;

                var symbols = Animation.GetAnimationSymbols().ToArray();

                foreach (var symbol in symbols)
                {
                    if (symbol.Contains("Taro"))
                        continue;

                    var d = Animation.GetAnimationData(symbol);

                    var f = new HSDRawFile(d);
                    var tree = f.Roots[0].Data as HSD_FigaTree;

                    var nodes = tree.Nodes;
                    var newNodes = new List<FigaTreeNode>();
                    for (int i = 0; i < target.Count; i++)
                    {
                        var name = target[i];
                        if (name == null)
                        {
                            newNodes.Add(new FigaTreeNode());
                            continue;
                        }

                        var index = source.IndexOf(name);
                        if (index == -1)
                        {
                            newNodes.Add(new FigaTreeNode());
                            continue;
                        }

                        if (index >= nodes.Count)
                        {
                            newNodes.Add(new FigaTreeNode());
                            continue;
                        }

                        newNodes.Add(nodes[index]);
                    }
                    tree.Nodes = newNodes;

                    using (MemoryStream output = new MemoryStream())
                    {
                        f.Save(output);
                        d = output.ToArray();
                    }

                    Animation.SetAnimation(symbol, d);
                }

                var rebuilt = Animation.RebuildAJFile(symbols, true);
                if (AJFile != null)
                {
                    AJFile.Roots[0].Data._s.SetData(rebuilt);
                    AJFile.Save(FilePath);
                }
                else
                {
                    File.WriteAllBytes(FilePath, rebuilt);
                }

                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="symbol"></param>
            /// <param name="current_offset"></param>
            /// <returns></returns>
            public (int, int) GetOffset(string symbol, int current_offset, int current_size)
            {
                if (Animation == null || string.IsNullOrEmpty(symbol))
                    return (current_offset, current_size);

                if (Animation.GetAnimationData(symbol) == null)
                    return (current_offset, current_size);

                var o = Animation.GetOffsetSize(symbol);
                return (o.Item1, o.Item2);
            }
        }

        private AnimManager Animation;
        private AnimManager IntroAnimation;
        private AnimManager EndingAnimation;
        private AnimManager WaitAnimation;
        private AnimManager ResultAnimation;

        private JointMap Source { get; set; }

        private JointMap Target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="animations"></param>
        public FighterBoneRemapper(JointMap source, 
            JointMap target, 
            string ajFilePath, 
            string rstFilePath, 
            string waitFilePath, 
            string introFilePath, 
            string endingFilePath)
        {
            Source = source;
            Target = target;

            if (!string.IsNullOrEmpty(ajFilePath))
                Animation = new AnimManager(ajFilePath);

            if (!string.IsNullOrEmpty(introFilePath))
                IntroAnimation = new AnimManager(introFilePath);

            if (!string.IsNullOrEmpty(endingFilePath))
                EndingAnimation = new AnimManager(endingFilePath);

            if (!string.IsNullOrEmpty(rstFilePath))
                ResultAnimation = new AnimManager(rstFilePath);

            if (!string.IsNullOrEmpty(waitFilePath))
                WaitAnimation = new AnimManager(waitFilePath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original_index"></param>
        /// <returns></returns>
        private int RemapBone(int original_index)
        {
            var src = Source[original_index];
            if (src == null)
                return -1;

            return Target.IndexOf(src);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="aj"></param>
        /// <param name="o"></param>
        /// <param name="n"></param>
        /// <param name="include_animation"></param>
        public void RemapAll(SBM_FighterData fd)
        {
            // edit animation
            Animation?.Remap(Source, Target);
            IntroAnimation?.Remap(Source, Target);
            EndingAnimation?.Remap(Source, Target);
            ResultAnimation?.Remap(Source, Target);
            WaitAnimation?.Remap(Source, Target);

            // model lookup 
            fd.ModelLookupTables.ItemHoldBone = (byte)RemapBone(fd.ModelLookupTables.ItemHoldBone);
            fd.ModelLookupTables.TopOfHeadBone = (byte)RemapBone(fd.ModelLookupTables.TopOfHeadBone);
            fd.ModelLookupTables.ShieldBone = (byte)RemapBone(fd.ModelLookupTables.ShieldBone);
            fd.ModelLookupTables.RightFootBone = (byte)RemapBone(fd.ModelLookupTables.RightFootBone);
            fd.ModelLookupTables.LeftFootBone = (byte)RemapBone(fd.ModelLookupTables.LeftFootBone);

            // Fighter Action Table
            {
                var actions = fd.FighterActionTable.Commands;

                foreach (var a in actions)
                {
                    // edit ftcmd
                    var data = a.SubAction._s.GetData();
                    SubactionManager.EditSubactionData(
                        ref data,
                        (Subaction sa, ref int[] p) =>
                        {
                            // create gfx
                            if (sa.Code == 10 << 2)
                                p[0] = Target.IndexOf(Source[p[0]]);
                            // create hitbox
                            if (sa.Code == 11 << 2 && p[4] == 0)
                                p[3] = Target.IndexOf(Source[p[3]]);
                            // set bone collision state
                            if (sa.Code == 28 << 2)
                                p[0] = Target.IndexOf(Source[p[0]]);
                            // enable ragdoll
                            if (sa.Code == 50 << 2)
                                p[0] = Target.IndexOf(Source[p[0]]);
                        },
                        SubactionGroup.Fighter);
                    a.SubAction._s.SetData(data);

                    // update animation
                    if (Animation != null)
                    {
                        var offsize = Animation.GetOffset(a.SymbolName?.Value, a.AnimationOffset, a.AnimationSize);
                        a.AnimationOffset = offsize.Item1;
                        a.AnimationSize = offsize.Item2;
                    }
                }

                fd.FighterActionTable.Commands = actions;
            }

            // Fighter Demo Table
            {
                var actions = fd.FighterActionTable.Commands;

                int action_index = 0;
                AnimManager[] stateOrder = new AnimManager[]
                {
                    ResultAnimation, // Win1
                    ResultAnimation, // Win1Wait
                    ResultAnimation, // Win2
                    ResultAnimation, // Win2Wait
                    ResultAnimation, // Win2Wait2
                    ResultAnimation, // Win3
                    ResultAnimation, // Win3Wait

                    ResultAnimation, // Selected
                    ResultAnimation, // Selected

                    ResultAnimation, // Lose

                    IntroAnimation, // Left
                    IntroAnimation, // Right

                    EndingAnimation,

                    WaitAnimation,
                };
                foreach (var a in actions)
                {
                    // edit ftcmd
                    var data = a.SubAction._s.GetData();
                    SubactionManager.EditSubactionData(
                        ref data,
                        (Subaction sa, ref int[] p) =>
                        {
                            // create gfx
                            if (sa.Code == 10 << 2)
                                p[0] = Target.IndexOf(Source[p[0]]);
                            // create hitbox
                            if (sa.Code == 11 << 2 && p[4] == 0)
                                p[3] = Target.IndexOf(Source[p[3]]);
                            // set bone collision state
                            if (sa.Code == 28 << 2)
                                p[0] = Target.IndexOf(Source[p[0]]);
                            // enable ragdoll
                            if (sa.Code == 50 << 2)
                                p[0] = Target.IndexOf(Source[p[0]]);
                        },
                        SubactionGroup.Fighter);
                    a.SubAction._s.SetData(data);

                    // update animation
                    if (action_index < stateOrder.Length)
                    {
                        var offsize = stateOrder[action_index].GetOffset(a.SymbolName?.Value, a.AnimationOffset, a.AnimationSize);
                        a.AnimationOffset = offsize.Item1;
                        a.AnimationSize = offsize.Item2;
                    }
                    action_index++;
                }

                fd.FighterActionTable.Commands = actions;
            }

            // ModelParts
            {
                var ar = fd.ModelPartAnimations.Array;
                foreach (var a in ar)
                {
                    a.StartingBone = (short)RemapBone(a.StartingBone);

                    var er = a.Entries;
                    for (int i = 0; i < er.Length; i++)
                        er[i] = (byte)RemapBone(er[i]);
                    a.Entries = er;
                }
                fd.ModelPartAnimations.Array = ar;
            }

            // TODO: Shield Pose

            // Physics
            {
                var ar = fd.Physics.DynamicDesc.Array;
                foreach (var a in ar)
                {
                    a.BoneIndex = RemapBone(a.BoneIndex);
                }
                fd.Physics.DynamicDesc.Array = ar;
            }
            {
                var ar = fd.Physics.Hitbubbles.Array;
                foreach (var a in ar)
                {
                    a.BoneIndex = RemapBone(a.BoneIndex);
                }
                fd.Physics.Hitbubbles.Array = ar;
            }

            // Hurtboxes
            {
                var ar = fd.Hurtboxes.Hurtboxes;
                foreach (var a in ar)
                {
                    a.BoneIndex = RemapBone(a.BoneIndex);
                }
                fd.Hurtboxes.Hurtboxes = ar;
            }

            // CenterBubble
            fd.CenterBubble.BoneIndex = RemapBone(fd.CenterBubble.BoneIndex);

            // CoinCollisionSphere
            {
                var ar = fd.CoinCollisionSpheres.Array;
                foreach (var a in ar)
                {
                    a.BoneIndex = RemapBone(a.BoneIndex);
                }
                fd.CoinCollisionSpheres.Array = ar;
            }

            // ECB
            fd.EnvironmentCollision.ECBBone1 = (short)RemapBone(fd.EnvironmentCollision.ECBBone1);
            fd.EnvironmentCollision.ECBBone2 = (short)RemapBone(fd.EnvironmentCollision.ECBBone2);
            fd.EnvironmentCollision.ECBBone3 = (short)RemapBone(fd.EnvironmentCollision.ECBBone3);
            fd.EnvironmentCollision.ECBBone4 = (short)RemapBone(fd.EnvironmentCollision.ECBBone4);
            fd.EnvironmentCollision.ECBBone5 = (short)RemapBone(fd.EnvironmentCollision.ECBBone5);
            fd.EnvironmentCollision.ECBBone6 = (short)RemapBone(fd.EnvironmentCollision.ECBBone6);

            // FighterBoneTable
            fd.FighterBoneTable.HeadBone = RemapBone(fd.FighterBoneTable.HeadBone);
            fd.FighterBoneTable.RightLeg = RemapBone(fd.FighterBoneTable.RightLeg);
            fd.FighterBoneTable.LeftLeg = RemapBone(fd.FighterBoneTable.LeftLeg);
            fd.FighterBoneTable.RightArm = RemapBone(fd.FighterBoneTable.RightArm);
            fd.FighterBoneTable.LeftArm = RemapBone(fd.FighterBoneTable.LeftArm);

            // FighterIK
            fd.FighterIK.RShoulderJ = (byte)RemapBone(fd.FighterIK.RShoulderJ);
            fd.FighterIK.LShoulderJ = (byte)RemapBone(fd.FighterIK.LShoulderJ);
            fd.FighterIK.RArmJ = (byte)RemapBone(fd.FighterIK.RArmJ);
            fd.FighterIK.LArmJ = (byte)RemapBone(fd.FighterIK.LArmJ);

            fd.FighterIK.RFootJ = (byte)RemapBone(fd.FighterIK.RFootJ);
            fd.FighterIK.LFootJ = (byte)RemapBone(fd.FighterIK.LFootJ);
            fd.FighterIK.RLegJ = (byte)RemapBone(fd.FighterIK.RLegJ);
            fd.FighterIK.LLegJ = (byte)RemapBone(fd.FighterIK.LLegJ);

            // TODO: MetalModel
        }
    }
}
