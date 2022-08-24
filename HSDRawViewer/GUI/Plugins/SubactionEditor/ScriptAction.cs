using HSDRaw;
using HSDRaw.Melee.Pl;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace HSDRawViewer.GUI.Plugins.SubactionEditor
{
    public class ScriptAction
    {
        [Browsable(false)]
        public SBM_FighterAction _action { get; internal set; }

        [Browsable(false)]
        public HSDStruct _struct { get => _action.SubAction._s; set => _action.SubAction._s = value; }

        [Browsable(false)]
        public int AnimOffset { get => _action.AnimationOffset; set => _action.AnimationOffset = value; }

        [Browsable(false)]
        public int AnimSize { get => _action.AnimationSize; set => _action.AnimationSize = value; }

        [Browsable(false)]
        public uint Flags { get => _action.Flags; set => _action.Flags = value; }

        private string DisplayText
        {
            get
            {
                if (!string.IsNullOrEmpty(Symbol))
                    return Regex.Replace(_action.Name.Replace("_figatree", ""), @"Ply.*_Share_ACTION_", "");

                return "Null";
            }
        }

        [Category("Animation"), DisplayName("Figatree Symbol")]
        public string Symbol
        {
            get => _action.Name;
            set => _action.Name = value;
        }

        [Category("Display Flags"), DisplayName("Flags")]
        public string BitFlags { get => Flags.ToString("X"); set { uint v = Flags; uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out v); Flags = v; } }

        [Category("Flags"), DisplayName("Utilize animation-induced physics")]
        public bool AnimInducedPhysics { get => (Flags & 0x80000000) != 0; set => SetFlag(0x80000000, value); }

        [Category("Flags"), DisplayName("Loop Animation")]
        public bool LoopAnimation { get => (Flags & 0x40000000) != 0; set => SetFlag(0x40000000, value); }

        [Category("Flags"), DisplayName("Unknown")]
        public bool Unknown { get => (Flags & 0x20000000) != 0; set => SetFlag(0x20000000, value); }

        [Category("Flags"), DisplayName("Unknown Flag")]
        public bool UnknownFlag { get => (Flags & 0x10000000) != 0; set => SetFlag(0x10000000, value); }

        [Category("Flags"), DisplayName("Disable Dynamics")]
        public bool DisableDynamics { get => (Flags & 0x08000000) != 0; set => SetFlag(0x08000000, value); }

        [Category("Flags"), DisplayName("Unknown TransN Update")]
        public bool TransNUpdate { get => (Flags & 0x04000000) != 0; set => SetFlag(0x04000000, value); }

        [Category("Flags"), DisplayName("TransN Affected by Model Scale")]
        public bool AffectModelScale { get => (Flags & 0x02000000) != 0; set => SetFlag(0x02000000, value); }

        [Category("Flags"), DisplayName("Additional Bone Value")]
        public uint AdditionalBone { get => (Flags & 0x003FFE00) >> 9; set => Flags = (uint)((Flags & ~0x003FFE00) | ((value << 9) & 0x003FFE00)); }

        [Category("Flags"), DisplayName("Disable Blend on Bone Index")]
        public uint BoneIndex { get => (Flags & 0x1C0) >> 7; set => Flags = (uint)(Flags & ~0x1C0) | ((value << 7) & 0x1C0); }

        [Category("Flags"), DisplayName("Character ID")]
        public uint CharIDCheck { get => Flags & 0x3F; set => Flags = (Flags & 0xFFFFFFC0) | (value & 0x3F); }


        /// <summary>
        /// 
        /// </summary>
        public ScriptAction()
        {
            _action = new SBM_FighterAction();
        }

        /// <summary>
        /// 
        /// </summary>
        public ScriptAction(SBM_FighterAction v)
        {
            _action = v;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="set"></param>
        private void SetFlag(uint flag, bool set)
        {
            if (set)
            {
                Flags |= flag;
            }
            else
            {
                Flags &= ~flag;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayText;
        }
    }

    public class ScriptSubrountine
    {
        public HSDStruct _struct;

        /// <summary>
        /// 
        /// </summary>
        public ScriptSubrountine()
        {
            _struct = new HSDStruct();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Subroutine";
        }
    }
}
