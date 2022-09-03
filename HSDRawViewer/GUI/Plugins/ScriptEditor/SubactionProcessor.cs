using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using HSDRawViewer.Tools;
using HSDRaw;
using HSDRawViewer.Rendering.Models;
using System.Linq;
using HSDRawViewer.Rendering.Widgets;

namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{

    /// <summary>
    /// 
    /// </summary>
    public class Hitbox
    {
        public bool Active { get => _active; set { _active = value; } }
        private bool _active;
        public float CreatedOnFrame;
        public bool Interpolate;
        public int BoneID;
        public float Size;
        public int Angle;
        public int Element;
        public Vector3 Point1;

        public SubactionEvent EventSource;

        public bool PendingUpdate { get; internal set; } = false;

        public TranslationWidget _widget { get; internal set; } = new TranslationWidget();

        public bool UseLocalTransform = false;

        private Matrix4 StartTransform;
        private Vector3 StartPoint;

        /// <summary>
        /// 
        /// </summary>
        public Hitbox()
        {
            _widget.TransformUpdated += (t) =>
            {
                Point1 = StartPoint + (t * StartTransform.Inverted()).ExtractTranslation();
                // TODO: have editor apply updated event data
                ((CustomIntProperty)EventSource[7].Value).Value = (int)(Point1.X * 256);
                ((CustomIntProperty)EventSource[8].Value).Value = (int)(Point1.Y * 256);
                ((CustomIntProperty)EventSource[9].Value).Value = (int)(Point1.Z * 256);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public Vector3 GetWorldPosition(LiveJObj manager)
        {
            var transform = GetWorldTransform(manager);

            if (!_widget.Interacting)
            {
                if (UseLocalTransform)
                    _widget.Transform = transform.ClearScale();
                else
                    _widget.Transform = transform.ClearRotation().ClearScale();

                StartTransform = transform;
                StartPoint = Point1;
            }

            return Vector3.TransformPosition(Vector3.Zero, transform);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        private Matrix4 GetWorldTransform(LiveJObj manager)
        {
            if (manager == null)
                return Matrix4.Identity;

            var boneID = BoneID;
            if (boneID == 0)
                if (manager.GetJObjAtIndex(1) != null && manager.GetJObjAtIndex(1).Child == null) // special case for character like mewtwo with a leading bone
                    boneID = 2;
                else
                    boneID = 1;

            var transform = Matrix4.CreateTranslation(Point1) * manager.GetJObjAtIndex(boneID).WorldTransform;
            //transform = transform.ClearScale();

            return transform;
        }
    }

    /// <summary>
    /// Handles and processes subaction data for rendering
    /// </summary>
    public class SubactionProcessor
    {
        public static int MaxHitboxCount = 4;

        public class Command
        {
            public SubactionEvent Event;

            public List<Command> ReferenceCommands = new List<Command>();
        }

        public class GFXSpawn
        {
            public float Frame;
            public int Bone;
            public int ID;
            public Vector3 Position;
            public Vector3 Range;
        }

        private List<Command> Commands = new List<Command>();

        public Hitbox[] Hitboxes { get; internal set; } = new Hitbox[MaxHitboxCount];

        public bool HitboxesActive { get => Hitboxes.Any(e => e.Active); }

        public bool[] FighterFlagWasSetThisFrame { get; } = new bool[4];

        public int[] FighterFlagValues { get; } = new int[4];

        public bool AllowInterrupt { get; internal set; }

        public List<GFXSpawn> GFXOnFrame { get; internal set; } = new List<GFXSpawn>();

        public Vector3 OverlayColor { get; internal set; } = Vector3.One;

        public bool IsInvisible { get; internal set; } = false;

        public int BodyCollisionState { get; internal set; } = 0;

        public bool ThrownFighter { get; internal set; } = false;

        public Dictionary<int, int> BoneCollisionStates = new Dictionary<int, int>();


        public delegate void UpdateVIS(int structId, int objectId);
        public UpdateVIS UpdateVISMethod;


        public delegate void AnimateMaterial(int matindex, int frame, int matflag, int frameflag);
        public AnimateMaterial AnimateMaterialMethod;


        public delegate void AnimateModel(int part_index, int anim_index);
        public AnimateModel AnimateModelMethod;


        public delegate void SpawnGFX(int bone, int gfxid, float x, float y, float z, float range_x, float range_y, float range_z);
        public SpawnGFX SpawnGFXMethod;

        /// <summary>
        /// 
        /// </summary>
        public SubactionProcessor()
        {
            for (int i = 0; i < Hitboxes.Length; i++)
                Hitboxes[i] = new Hitbox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetState()
        {
            BodyCollisionState = 0;
            BoneCollisionStates.Clear();
            OverlayColor = Vector3.One;
            IsInvisible = false;
            ThrownFighter = false;
            AllowInterrupt = false;

            for (int i = 0; i < FighterFlagValues.Length; i++)
                FighterFlagValues[i] = 0;
            ClearFighterFlags();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearFighterFlags()
        {
            for (int i = 0; i < FighterFlagWasSetThisFrame.Length; i++)
                FighterFlagWasSetThisFrame[i] = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void SetStruct(List<SubactionEvent> events, SubactionGroup subGroup)
        {
            Commands = GetCommands(events, subGroup);
            ResetState();
            SetFrame(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private List<Command> GetCommands(List<SubactionEvent> events, SubactionGroup subGroup, Dictionary<HSDStruct, List<Command>> structToComman = null)
        {
            if (subGroup != SubactionGroup.Fighter)
                return new List<Command>();

            if (structToComman == null)
            {
                structToComman = new Dictionary<HSDStruct, List<Command>>();
            }

            List<Command> commands = new List<Command>();

            foreach (var ev in events)
            {
                var command = new Command()
                {
                    Event = ev,
                };
                commands.Add(command);

                // process pointer data
                var pointer = ev.GetPointer();
                if (pointer != null)
                {
                    if (structToComman.ContainsKey(pointer))
                    {
                        commands = structToComman[pointer];
                    }
                    else
                    {
                        structToComman.Add(pointer, new List<Command>());
                        var subcommand = GetCommands(SubactionEvent.GetEvents(subGroup, pointer).ToList(), subGroup, structToComman);
                        structToComman[pointer] = subcommand;
                        command.ReferenceCommands = subcommand;
                    }
                }
            }

            return commands;
        }


        // prevent recursion...
        private HashSet<List<Command>> CommandHashes = new HashSet<List<Command>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void SetFrame(float frame)
        {
            GFXOnFrame.Clear();
            // disable hitboxes
            foreach (var v in Hitboxes)
                v.Active = false;

            ResetState();
            CommandHashes.Clear();
            SetFrame(frame, 0, Commands);
        }

        /// <summary>
        /// 
        /// </summary>
        private float SetFrame(float frame, float time, List<Command> commands)
        {
            int loopAmt = 0;
            int loopPos = 0;

            float[] fighterFlagSetFrame = new float[4];

            // process commands
            for (int i = 0; i < commands.Count; i++)
            {
                var cmd = commands[i];
                var ev = cmd.Event;
                switch (cmd.Event.Code)
                {
                    case 0 << 2: //end script
                        time = int.MaxValue;
                        break;
                    case 1 << 2: //synchronous
                        time += ev.GetParameter(0);
                        break;
                    case 2 << 2: //asynchronus
                        time = ev.GetParameter(0);
                        break;
                    case 3 << 2: //start loop
                        loopAmt = ev.GetParameter(0);
                        loopPos = i;
                        break;
                    case 4 << 2: //end loop
                        loopAmt -= 1;
                        if (loopAmt > 0)
                            i = loopPos;
                        break;
                    case 5 << 2: //subroutine
                        time = SetFrame(frame, time, cmd.ReferenceCommands);
                        break;
                    case 6 << 2: //return?
                        return time;
                    case 7 << 2: //goto
                        time = SetFrame(frame, time, cmd.ReferenceCommands);
                        break;

                    // fighter specific
                    case 10 << 2: //spawn gfx
                        {
                            GFXOnFrame.Add(new GFXSpawn()
                            {
                                Frame = time,
                                Bone = ev.GetParameter(0),
                                ID = ev.GetParameter(4),
                                Position = new Vector3(ev.GetParameter(6) / 256f, ev.GetParameter(7) / 256f, ev.GetParameter(8) / 256f),
                                Range = new Vector3(ev.GetParameter(9) / 256f, ev.GetParameter(10) / 256f, ev.GetParameter(11) / 256f),
                            });
                        }
                        //SpawnGFXMethod(cmd.Parameters[0], cmd.Parameters[4], cmd.Parameters[6] / 256f, cmd.Parameters[7] / 256f, cmd.Parameters[8] / 256f, cmd.Parameters[9] / 256f, cmd.Parameters[10] / 256f, cmd.Parameters[11] / 256f);
                        break;
                    case 11 << 2: // Create Hitbox
                        // remove the current hitbox with this id
                        if (ev.GetParameter(0) < Hitboxes.Length)
                        {
                            var hb = Hitboxes[ev.GetParameter(0)];
                            hb.EventSource = ev;
                            hb.CreatedOnFrame = time;
                            hb.Active = true;
                            hb.BoneID = ev.GetParameter(3);
                            hb.Size = (short)ev.GetParameter(6) / 256f;
                            hb.Point1 = new Vector3(ev.GetParameter(7) / 256f, ev.GetParameter(8) / 256f, ev.GetParameter(9) / 256f);
                            hb.Angle = ev.GetParameter(10);
                            hb.Element = ev.GetParameter(19);
                        }
                        break;
                    case 13 << 2: // adjust size
                        {
                            if (ev.GetParameter(0) < Hitboxes.Length)
                                Hitboxes[ev.GetParameter(0)].Size = (short)ev.GetParameter(1) / 256f; //TODO: ? (short)cmd.Parameters[1] / 150f;
                        }
                        break;
                    case 15 << 2:
                        if (ev.GetParameter(0) < Hitboxes.Length)
                            Hitboxes[ev.GetParameter(0)].Active = false;
                        break;
                    case 16 << 2:
                        {
                            foreach (var hb in Hitboxes)
                                hb.Active = false;
                        }
                        break;
                    case 19 << 2:
                        if (ev.GetParameter(0) < FighterFlagValues.Length)
                        {
                            fighterFlagSetFrame[ev.GetParameter(0)] = time;
                            FighterFlagWasSetThisFrame[ev.GetParameter(0)] = true;
                            FighterFlagValues[ev.GetParameter(0)] = ev.GetParameter(1);
                        }
                        break;
                    case 20 << 2: // throw
                        ThrownFighter = true;
                        break;
                    case 23 << 2: // allow interrupt
                        AllowInterrupt = true;
                        break;
                    case 26 << 2:
                        BodyCollisionState = ev.GetParameter(0);
                        break;
                    case 27 << 2:
                        // i don't really know how many bone to assume...
                        for (int j = 0; j < 100; j++)
                        {
                            if (BoneCollisionStates.ContainsKey(j))
                                BoneCollisionStates[j] = ev.GetParameter(0);
                            else
                                BoneCollisionStates.Add(j, ev.GetParameter(0));
                        }
                        break;
                    case 28 << 2:
                        //if (cmd.Parameters.Length > 1)
                        {
                            if (BoneCollisionStates.ContainsKey(ev.GetParameter(0)))
                                BoneCollisionStates[ev.GetParameter(0)] = ev.GetParameter(1);
                            else
                                BoneCollisionStates.Add(ev.GetParameter(0), ev.GetParameter(1));
                        }
                        break;
                    case 31 << 2: // struct vis change
                        if (UpdateVISMethod != null)
                            UpdateVISMethod(ev.GetParameter(0), ev.GetParameter(2));
                        break;
                    case 37 << 2:
                        IsInvisible = ev.GetParameter(1) == 1;
                        break;
                    case 40 << 2:
                        if (AnimateMaterialMethod != null)
                            AnimateMaterialMethod(ev.GetParameter(1), ev.GetParameter(3), ev.GetParameter(0), ev.GetParameter(2));
                        break;
                    case 41 << 2:
                        if (AnimateModelMethod != null)
                            AnimateModelMethod(ev.GetParameter(0), ev.GetParameter(1));
                        break;
                    case 46 << 2: //overlay color
                        if (ev.GetParameter(0) == 1)
                        {
                        }
                        break;
                }

                if (time > frame)
                    break;

            }

            // Update hitbox interpolation
            foreach (var v in Hitboxes)
            {
                if (v.Active)
                {
                    v.Interpolate = v.CreatedOnFrame != frame;
                }
            }

            GFXOnFrame.RemoveAll(e => e.Frame != frame);
            for (int i = 0; i < fighterFlagSetFrame.Length; i++)
            {
                if (fighterFlagSetFrame[i] != frame)
                    FighterFlagWasSetThisFrame[i] = false;
            }

            return time;
        }

    }
}
