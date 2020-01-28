using OpenTK;
using System;
using System.Collections.Generic;
using HSDRawViewer.Tools;
using HSDRaw;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    /// <summary>
    /// Handles and processes subaction data for rendering
    /// </summary>
    public class SubactionProcessor
    {
        public class Command
        {
            public Subaction Action;

            public int[] Parameters;

            public HSDStruct Reference;

            public List<Command> ReferenceCommands = new List<Command>();
        }

        private List<Command> Commands = new List<Command>();

        public List<Hitbox> Hitboxes { get; internal set; } = new List<Hitbox>();

        private HSDStruct Struct;

        /// <summary>
        /// 
        /// </summary>
        public SubactionProcessor()
        {

        }
        
        public void SetStruct(HSDStruct str)
        {
            Struct = str;

            Commands = GetCommands(Struct);

            SetFrame(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private List<Command> GetCommands(HSDStruct str, HashSet<HSDStruct> references = null)
        {
            if (references == null)
                references = new HashSet<HSDStruct>();

            if (references.Contains(str))
                return new List<Command>();

            var data = str.GetData();

            references.Add(str);

            var Commands = new List<Command>();

            for (int i = 0; i < data.Length;)
            {
                var sa = SubactionManager.GetSubaction((byte)(data[i] >> 2));

                var cmd = new Command();

                foreach (var r in str.References)
                {
                    if (r.Key >= i && r.Key < i + sa.ByteSize)
                        if (cmd.Reference != null)
                            throw new NotSupportedException("Multiple References not supported");
                        else
                        {
                            cmd.Reference = r.Value;
                            cmd.ReferenceCommands = GetCommands(cmd.Reference, references);
                        }
                }

                var sub = new byte[sa.ByteSize];

                for (int j = 0; j < sub.Length; j++)
                    sub[j] = data[i + j];

                cmd.Parameters = sa.GetParameters(sub);
                cmd.Action = sa;
                Commands.Add(cmd);

                i += sa.ByteSize;
            }

            return Commands;
        }


        public void SetFrame(float frame)
        {
            Hitboxes.Clear();
            SetFrame(frame, 0, Commands);
        }

        /// <summary>
        /// 
        /// </summary>
        private float SetFrame(float frame, float time, List<Command> commands)
        {
            int loopAmt = 0;
            int loopPos = 0;
            for(int i = 0; i < commands.Count; i++)
            {
                var cmd = commands[i];
                switch (cmd.Action.Code)
                {
                    case 0: //end script
                        time = int.MaxValue;
                        break;
                    case 1: //synchronous
                        time += cmd.Parameters[0];
                        break;
                    case 2: //asynchronus
                        time = cmd.Parameters[0];
                        break;
                    case 3: //start loop
                        loopAmt = cmd.Parameters[0];
                        loopPos = i;
                        break;
                    case 4: //end loop
                        loopAmt -= 1;
                        if(loopAmt != 0)
                            i = loopPos;
                        break;
                    case 5: //subroutine
                        time = SetFrame(frame, time, cmd.ReferenceCommands);
                        break;
                    case 6: //return?
                        break;
                    case 7: //goto
                        time = SetFrame(frame, time, cmd.ReferenceCommands);
                        break;
                    case 11: // Create Hitbox
                        // remove the current hitbox with this id
                        Hitboxes.RemoveAll(e => e.ID == cmd.Parameters[0]);
                        // add hitbox
                        Hitboxes.Add(new Hitbox()
                        {
                            ID = cmd.Parameters[0],
                            BoneID = cmd.Parameters[2],
                            Size = (short)cmd.Parameters[5] / 150f,
                            Point1 = new Vector3((short)cmd.Parameters[8] / 150f, (short)cmd.Parameters[7] / 150f, (short)cmd.Parameters[6] / 150f),
                        });
                        break;
                    case 13: // adjust size
                        {
                            var hb = Hitboxes.Find(e=>e.ID == cmd.Parameters[0]);
                            if(hb != null)
                            {
                                hb.Size = (short)cmd.Parameters[1] / 150f;
                            }
                        }
                        break;
                    case 15:
                        Hitboxes.RemoveAll(e=>e.ID == cmd.Parameters[0]);
                        break;
                    case 16:
                        Hitboxes.Clear();
                        break;
                }

                if (time > frame)
                    break;
            }
            return time;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class Hitbox
    {
        public int ID;
        public int BoneID;
        public float Size;
        public Vector3 Point1;
        public Vector3 Point2;
    }
}
