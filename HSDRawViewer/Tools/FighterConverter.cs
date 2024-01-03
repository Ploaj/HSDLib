using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using HSDRawViewer.Tools.KeyFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static HSDRawViewer.Tools.AnimationRetarget;

namespace HSDRawViewer.Tools
{
    public class FighterConverter
    {
        public delegate void AdditionalProcess(RetargetState state, JointAnimManager anim, JointAnimManager src_anim);

        public static void GenerateAnimations(
            string in_directory, 
            string out_directory, 
            string source_file, 
            string target_file, 
            string source_map, 
            string target_map, 
            string fightername,
            AdditionalProcess process,
            CustomRetargetCallback retargetCB)
        {
            List<string> files = new List<string>();

            if (File.Exists(in_directory))
            {
                files.Add(in_directory);
            }
            else
            {
                foreach (var f in Directory.GetFiles(in_directory))
                {
                    if (f.EndsWith(".chr0"))
                    {
                        files.Add(f);
                    }
                }
            }


            var src_map = new JointMap(source_map);
            var tar_map = new JointMap(target_map);

            var src_jobj = new HSDRawFile(source_file).Roots[0].Data as HSD_JOBJ;
            var tar_jobj = new HSDRawFile(target_file).Roots[0].Data as HSD_JOBJ;

            int toProcess = files.Count;

            using (ManualResetEvent resetEvent = new ManualResetEvent(false))
            {
                foreach (var f in files)
                {
                    RetargetState state = new RetargetState()
                    {
                        targetMap = tar_map,
                        sourceMap = src_map,
                        sourceModel = src_jobj,
                        targetModel = tar_jobj,

                        animationPath = f,
                        outputPath = out_directory,
                        FighterName = fightername,

                        AnimProcess = process,
                        RetargetCallback = retargetCB,
                    };

                    ThreadPool.QueueUserWorkItem(
                       new WaitCallback(x =>
                       {
                           ThreadProc(x);

                           // Safely decrement the counter
                           if (Interlocked.Decrement(ref toProcess) == 0)
                               resetEvent.Set();

                       }), state);
                }

                resetEvent.WaitOne();
            }
        }

        public class RetargetState
        {
            public string animationPath;
            public string outputPath;

            public string FighterName;

            public HSD_JOBJ targetModel;
            public HSD_JOBJ sourceModel;
            public JointMap targetMap;
            public JointMap sourceMap;

            public AdditionalProcess AnimProcess;
            public CustomRetargetCallback RetargetCallback;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateInfo"></param>
        static void ThreadProc(object stateInfo)
        {
            var state = stateInfo as RetargetState;

            // get animation name
            var fname = Path.GetFileNameWithoutExtension(state.animationPath);

            // retarget animation
            var anim = RetargetAnimation(state.animationPath, state);

            // apply euler filter
            foreach (var n in anim.Nodes)
            {
                DiscontinuityFilter.Filter(n.Tracks);
                EulerFilter.Filter(n.Tracks);
            }

            // apply appends
            PreProcess(state, anim);

            // optimize animation
            // anim.Optimize(state.targetModel, 0.01f);

            // change this animation's name
            if (fname.Equals("LandingHeavy"))
                fname = "Landing";

            // export animation
            var symbol = $"Ply{state.FighterName}5K_Share_ACTION_{fname}_figatree";
            ExportFigatree(state.outputPath + symbol + ".dat", anim, symbol, 0.001f);

            // perform post processing
            PostProcess(state, anim);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="anim"></param>
        private static void PreProcess(RetargetState state, JointAnimManager anim)
        {
            string inputDirectory = Path.GetDirectoryName(state.animationPath) + "\\";

            // get animation name
            var fname = Path.GetFileNameWithoutExtension(state.animationPath);
            var ext = Path.GetExtension(state.animationPath);

            // append charge
            if (fname.Equals("AttackHi4") || fname.Equals("AttackLw4") || fname.Equals("AttackS4S") || fname.Equals("AttackS4Hi") || fname.Equals("AttackS4Lw"))
            {
                var start_anim = inputDirectory + fname.Replace("4Hi", "4").Replace("4Lw", "4").Replace("4S", "4") + "Start" + ext;
                AppendAnimation(state, anim, RetargetAnimation(start_anim, state));

                var wait_anim = inputDirectory + "Wait1" + ext;
                var appendAnim = RetargetAnimation(wait_anim, state, 2);
                appendAnim.FrameCount = 1;
                AppendAnimation(state, anim, appendAnim);
            }

            // append swing
            if (fname.Equals("Swing4"))
            {
                var start_anim = inputDirectory + fname + "Start" + ext;
                AppendAnimation(state, anim, RetargetAnimation(start_anim, state));

                var wait_anim = inputDirectory + "Wait1" + ext;
                var appendAnim = RetargetAnimation(wait_anim, state, 2);
                appendAnim.FrameCount = 1;
                AppendAnimation(state, anim, appendAnim);
            }

            // append 1 frame
            if (AppendFrameList.ContainsKey(fname))
            {
                var file = inputDirectory + AppendFrameList[fname] + ext;

                if (File.Exists(file))
                {
                    var appendAnim = RetargetAnimation(file, state, 2);
                    appendAnim.FrameCount = 1;
                    AppendAnimation(state, anim, appendAnim);
                }
            }

            // resize
            if (SetFrameLengths.ContainsKey(fname))
            {
                anim.ApplyFSMs(new FrameSpeedMultiplier[]
                    {
                        new FrameSpeedMultiplier()
                        {
                            Rate = anim.FrameCount / (SetFrameLengths[fname] + 1)
                        }
                    });
                anim.FrameCount = SetFrameLengths[fname];
            }

            switch (fname)
            {
                // remove transN tracks from these animations
                case "WalkSlow":
                case "WalkMiddle":
                case "WalkFast":
                case "Run":
                    {
                        var transN = state.targetMap.IndexOf("TransN");

                        if (transN >= 0)
                            anim.Nodes[transN].Tracks.Clear();
                    }
                    break;
                case "HeavyWalk1":
                case "HeavyWalk2":
                    {
                        var transN = state.targetMap.IndexOf("TransN");

                        if (transN >= 0)
                        {
                            anim.Nodes[transN].Tracks.Clear();

                            anim.Nodes[transN].Tracks.Add(new FOBJ_Player()
                            {
                                JointTrackType = JointTrackType.HSD_A_J_TRAX,
                                Keys = new List<FOBJKey>()
                                {
                                    new FOBJKey()
                                    {
                                        Frame = 0,
                                        Value = 0,
                                        InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                                    },
                                    new FOBJKey()
                                    {
                                        Frame = anim.FrameCount,
                                        Value = 5,
                                        InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                                    },
                                }
                            });
                        }
                    }
                    break;
                case "Guard":
                    FixGuardAnimation(state, anim);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="anim"></param>
        /// <param name="append"></param>
        private static void AppendAnimation(RetargetState state, JointAnimManager anim, JointAnimManager append)
        {
            anim.FrameCount += append.FrameCount;

            for (int i = 0; i < anim.Nodes.Count; i++)
            {
                foreach (var t in anim.Nodes[i].Tracks)
                {
                    // shift all keys
                    foreach (var k in t.Keys)
                        k.Frame += append.FrameCount;

                    // add keys from state
                    var append_track = append.Nodes[i].Tracks.FirstOrDefault(e => e.JointTrackType == t.JointTrackType);

                    if (append_track != null)
                    {
                        for (int f = (int)append.FrameCount - 1; f >= 0; f--)
                        {
                            t.Keys.Insert(0, append_track.Keys[f]);
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="anim"></param>
        private static void FixGuardAnimation(RetargetState state, JointAnimManager anim)
        {
            if (anim.FrameCount >= 370)
                return;

            // add 10 frames
            anim.FrameCount += 10;

            // add 10 dead keys to start
            var nodes = anim.Nodes;
            for (int i = 0; i < anim.NodeCount; i++)
            {
                foreach (var t in nodes[i].Tracks)
                {
                    var keys = t.Keys;

                    foreach (var k in keys)
                        k.Frame += 10;

                    keys.Insert(0, new FOBJKey()
                    {
                        Frame = 0,
                        Value = keys[0].Value,
                        InterpolationType = GXInterpolationType.HSD_A_OP_CON
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="anim"></param>
        private static void PostProcess(RetargetState state, JointAnimManager anim)
        {
            var fname = Path.GetFileNameWithoutExtension(state.animationPath);

            // generate item screw for air and damage
            if (fname.Equals("ItemScrew"))
            {
                fname = "ItemScrewAir";
                var symbol = $"Ply{state.FighterName}5K_Share_ACTION_{fname}_figatree";
                ExportFigatree(state.outputPath + symbol + ".dat", anim, symbol);

                fname = "ItemScrewDamage";
                symbol = $"Ply{state.FighterName}5K_Share_ACTION_{fname}_figatree";
                ExportFigatree(state.outputPath + symbol + ".dat", anim, symbol);
            }

            // generate guard model
            if (fname.Equals("GuardOff"))
                GenerateGuardModel(state, anim);

            // generate entry anim
            if (fname.Equals("Wait1"))
                GenerateEntryAnim(state, anim);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="anim"></param>
        private static void GenerateGuardModel(RetargetState state, JointAnimManager anim)
        {
            // generate guard skeleton
            var guard_model = HSDAccessor.DeepClone<HSD_JOBJ>(state.targetModel);
            var index = 0;
            foreach (var j in guard_model.TreeList)
            {
                if (index >= anim.NodeCount)
                    break;

                j.Dobj = null;
                j.InverseWorldTransform = null;

                foreach (var t in anim.Nodes[index].Tracks)
                {
                    switch (t.JointTrackType)
                    {
                        case JointTrackType.HSD_A_J_TRAX: j.TX = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_TRAY: j.TY = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_TRAZ: j.TZ = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_ROTX: j.RX = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_ROTY: j.RY = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_ROTZ: j.RZ = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_SCAX: j.SX = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_SCAY: j.SY = t.Keys[0].Value; break;
                        case JointTrackType.HSD_A_J_SCAZ: j.SZ = t.Keys[0].Value; break;
                    }
                }
                index++;
            }

            guard_model.UpdateFlags();

            var guard_file = new HSDRawFile();
            guard_file.Roots.Add(new HSDRootNode() { Name = "guard_joint", Data = guard_model });
            guard_file.Save(state.outputPath + "guard_joint.dat");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ft"></param>
        private static void GenerateEntryAnim(RetargetState state, JointAnimManager anim)
        {
            var symbol = "Ply" + state.FighterName + "5K_Share_ACTION_Entry_figatree";

            var entry = new JointAnimManager();
            entry.FrameCount = 1;

            foreach (var n in anim.Nodes)
            {
                var node = new AnimNode();
                entry.Nodes.Add(node);

                foreach (var t in n.Tracks)
                {
                    node.Tracks.Add(new FOBJ_Player()
                    {
                        JointTrackType = t.JointTrackType,
                        Keys = new List<FOBJKey>()
                        {
                            new FOBJKey()
                            {
                                Frame = 0,
                                Value = t.GetValue(0),
                                InterpolationType = GXInterpolationType.HSD_A_OP_KEY
                            }
                        }
                    });
                }
            }

            var output = new HSDRawFile();
            output.Roots.Add(new HSDRootNode()
            {
                Name = symbol,
                Data = entry.ToFigaTree(0.001f)
            });
            output.Save(state.outputPath + symbol + ".dat");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_path"></param>
        /// <param name="anim"></param>
        /// <param name="symbol"></param>
        private static void ExportFigatree(string file_path, JointAnimManager anim, string symbol, float error = 0.001f)
        {

            var output = new HSDRawFile();
            output.Roots.Add(new HSDRootNode()
            {
                Name = symbol,
                Data = anim.ToFigaTree(error)
            });
            output.Save(file_path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static float ClampRotation(float v)
        {
            if (System.Math.Abs(System.Math.Abs(v) - Math3D.TwoPI) < 0.001)
            {
                if (v > 0)
                    v -= (float)Math3D.TwoPI;

                if (v < 0)
                    v += (float)Math3D.TwoPI;
            }
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animPath"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private static JointAnimManager RetargetAnimation(string animPath, RetargetState state, float frame_count = -1)
        {
            // wait until file is free
            while (true)
            {
                try
                {
                    /* open and process file */
                    JointAnimManager anim = Converters.Animation.JointAnimationLoader.LoadJointAnimFromFile(state.sourceMap, animPath);
                    if (frame_count != -1)
                        anim.FrameCount = frame_count;

                    foreach (var n in anim.Nodes)
                    {
                        foreach (var t in n.Tracks)
                        {
                            if (t.JointTrackType == JointTrackType.HSD_A_J_ROTX ||
                                t.JointTrackType == JointTrackType.HSD_A_J_ROTY ||
                                t.JointTrackType == JointTrackType.HSD_A_J_ROTZ)
                                foreach (var k in t.Keys)
                                {
                                    k.Value = ClampRotation(k.Value);
                                }
                        }
                    }

                    // retarget animation
                    var new_anim = AnimationRetarget.Retarget(anim, new LiveJObj(state.sourceModel), new LiveJObj(state.targetModel), state.sourceMap, state.targetMap, state.RetargetCallback);

                    // perform additional process callback
                    if (state.AnimProcess != null)
                        state.AnimProcess(state, new_anim, anim);

                    return new_anim;
                }
                catch (IOException e)
                {
                    /* file unavailable */
                }

                Thread.Sleep(100); // Wait for the file to be available
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, string> AppendFrameList = new Dictionary<string, string>()
        {
            {"Turn", "Wait1" },
            {"TurnRun", "Run" },
            {"Squat", "Wait1" },
            {"SquatRv", "SquatWait" },
            {"GuardOn", "Wait1" },
            {"GuardOff", "Guard" },
            {"HeavyThrowF", "HeavyWalk1" },
            {"HeavyThrowB", "HeavyWalk1" },
            {"HeavyThrowLw", "HeavyWalk1" },
            {"Swing1", "Wait1" },
            {"Swing3", "Wait1" },
            {"SwingDash", "Wait1" },
            {"ItemScopeStart", "Wait1" },
            {"ItemScopeAirStart", "Fall" },
            {"DamageHi1", "Wait1" },
            {"DamageHi2", "Wait1" },
            {"DamageHi3", "Wait1" },
            {"DamageN1", "Wait1" },
            {"DamageN2", "Wait1" },
            {"DamageN3", "Wait1" },
            {"DamageLw1", "Wait1" },
            {"DamageLw2", "Wait1" },
            {"DamageLw3", "Wait1" },
            {"DamageAir1", "Fall" },
            {"DamageAir2", "Fall" },
            {"DamageAir3", "Fall" },
            {"DownDamageU", "DownWaitU" },
            {"DownStandU", "DownWaitU" },
            {"DownAttackU", "DownWaitU" },
            {"DownForwardU", "DownWaitU" },
            {"DownBackU", "DownWaitU" },
            {"DownDamageD", "DownWaitD" },
            {"DownStandD", "DownWaitD" },
            {"DownAttackD", "DownWaitD" },
            {"DownForwardD", "DownWaitD" },
            {"DownBackD", "DownWaitD" },
            {"CliffClimbQuick", "CliffWait" },
            {"CliffClimbSlow", "CliffWait" },
            {"CliffAttackSlow", "CliffWait" },
            {"CliffAttackQuick", "CliffWait" },
            {"CliffEscapeSlow", "CliffWait" },
            {"CliffEscapeQuick", "CliffWait" },
            {"CliffJumpQuick1", "CliffWait" },
            //{"CliffJumpQuick2", "CliffWait" },
            {"CliffJumpSlow1", "CliffWait" },
            //{"CliffJumpSlow2", "CliffWait" },
            {"Catch", "Wait1" },
            {"CatchDash", "Run" },
            {"CatchCut", "CatchWait" },
            {"CapturePulledHi", "Fall" },
            {"CaptureDamageHi", "CaptureWaitHi" },
            {"CapturePulledLw", "Fall" },
            {"CaptureDamageLw", "CaptureWaitLw" },
            {"Dash", "Wait1" },
            {"EscapeB", "Guard" },
            {"EscapeF", "Guard" },
            {"EscapeN", "Guard" },
            {"FuraSleepEnd", "FuraSleepLoop" },
            {"FuraSleepStart", "Wait1" },

            // append and resize
            {"EscapeAir", "Fall" },
            {"LightGet", "Wait1" },
            {"LightThrowDash", "Run" },
            {"ItemShoot", "Wait1" },
            {"ItemShootAir", "Fall" },
            {"DownSpotU", "Wait1" },
            {"CatchAttack", "CatchWait" },
            {"HeavyThrowHi", "HeavyWalk1" },

            // no resizes
            { "ThrowF", "CatchWait" },
            { "ThrowB", "CatchWait" },
            { "ThrowHi", "CatchWait" },
            { "ThrowLw", "CatchWait" },
            { "JumpAerialF", "Fall" },
            { "JumpAerialB", "Fall" },
            { "LightThrowAirF", "Fall" },
            { "LightThrowAirB", "Fall" },
            { "LightThrowAirHi", "Fall" },
            { "LightThrowAirLw", "Fall" },
            { "LightThrowF", "Wait1" },
            { "LightThrowB", "Wait1" },
            { "LightThrowHi", "Wait1" },
            { "LightThrowLw", "Wait1" },
            { "LightThrowDrop", "Wait1" },
            { "RunBrake", "Run" },
            { "HeavyGet", "Wait1" },
            { "SpecialNStart", "Wait1" },
            { "SpecialAirNStart", "Fall" },
            { "SpecialSStart", "Wait1" },
            { "SpecialAirSStart", "Fall" },
            { "SpecialHiStart", "Wait1" },
            { "SpecialAirHiStart", "Fall" },
            { "SpecialLwStart", "Wait1" },
            { "SpecialAirLwStart", "Fall" },
            { "AttackDash", "Run" },
            { "Attack11", "Wait1" },
            { "Attack100Start", "Wait1" },
            { "AttackHi3", "Wait1" },
            { "AttackS3Hi", "Wait1" },
            { "AttackS3Lw", "Wait1" },
            { "AttackS3S", "Wait1" },
            { "AttackLw3", "Squat" },
            { "AttackAirN", "Fall" },
            { "AttackAirF", "Fall" },
            { "AttackAirB", "Fall" },
            { "AttackAirHi", "Fall" },
            { "AttackAirLw", "Fall"}
        };

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, int> SetFrameLengths = new Dictionary<string, int>()
        {
            {"DamageFlyTop", 60},
            {"CliffCatch", 8},

            {"Squat", 8 },
            {"SquatRv", 10 },
            {"GuardOn", 8 },
            {"GuardOff", 16 },
            {"HeavyThrowF", 40 },
            {"HeavyThrowB", 40 },
            {"HeavyThrowLw", 30 },
            {"Swing1", 24 },
            {"Swing3", 42 },
            {"SwingDash", 46 },
            {"ItemScopeStart", 16 },
            {"ItemScopeAirStart", 16 },
            {"DamageHi1", 12 },
            {"DamageHi2", 24 },
            {"DamageHi3", 30 },
            {"DamageN1", 12 },
            {"DamageN2", 24 },
            {"DamageN3", 30 },
            {"DamageLw1", 12 },
            {"DamageLw2", 24 },
            {"DamageLw3", 42 },
            {"DamageAir1", 12 },
            {"DamageAir2", 24 },
            {"DamageAir3", 30 },
            {"DownDamageU", 14 },
            {"DownStandU", 30 },
            {"DownAttackU", 50 },
            {"DownForwardU", 36 },
            {"DownBackU", 36 },
            {"DownDamageD", 14 },
            {"DownStandD", 30 },
            {"DownAttackD", 50 },
            {"DownForwardD", 36 },
            {"DownBackD", 36 },
            {"CliffClimbQuick", 35 },
            {"CliffClimbSlow", 60 },
            {"CliffAttackSlow", 70 },
            {"CliffAttackQuick", 56 },
            {"CliffEscapeSlow", 80 },
            {"CliffEscapeQuick", 50 },
            {"CliffJumpQuick1", 16 },
            {"CliffJumpQuick2", 25 },
            {"CliffJumpSlow1", 20 },
            {"CliffJumpSlow2", 25 },
            {"Catch", 30 },
            {"CatchDash", 40 },
            {"CatchCut", 30 },
            {"CapturePulledHi", 20 },
            {"CaptureDamageHi", 20 },
            {"CapturePulledLw", 20 },
            {"CaptureDamageLw", 20 },
            {"Dash", 20 },
            {"EscapeB", 32 },
            {"EscapeF", 32 },
            {"EscapeN", 32 },
            {"FuraSleepEnd", 60 },
            {"FuraSleepStart", 30 },

            // append and resize
            {"EscapeAir", 50 },
            {"LightGet", 8 },
            {"LightThrowDash", 40 },
            {"ItemShoot", 30 },
            {"ItemShootAir", 30 },
            {"DownSpotU", 20 },
            {"CatchAttack", 24 },
            {"HeavyThrowHi", 30 },
        };
    }
}
