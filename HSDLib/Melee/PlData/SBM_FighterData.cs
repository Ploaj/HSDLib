using HSDLib.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDLib.Melee.PlData
{
    public class SBM_JOBJPointer : IHSDNode
    {
        public HSD_JOBJ SkeletonJOBJ { get; set; }
    }

    public class SBM_FighterData : IHSDNode
    {
        public SBM_FighterAttributes FighterAttributes { get; set; }

        public uint FighterAttributes2 { get; set; } // 0xDc long

        public uint Padding { get; set; } // potentialy more attributes for some characters

        [Browsable(false)]
        public uint SubActionOffset { get; set; }

        public uint Unknown0x10 { get; set; } // size 0x28c

        public uint WinSubActionOffset { get; set; }

        public uint Unknown0x18 { get; set; } // has 7 32 bit values all 0 for ness

        public uint Unknown0x1C { get; set; }

        public SBM_JOBJPointer Unknown0x20 { get; set; }

        public uint Unknown0x24 { get; set; }

        public uint Unknown0x28 { get; set; }

        public uint Unknown0x2C { get; set; }

        public uint Unknown0x30 { get; set; }

        public uint Unknown0x34 { get; set; }

        public uint Unknown0x38 { get; set; }

        public uint Unknown0x3C { get; set; }

        public uint Unknown0x40 { get; set; }

        public uint Unknown0x44 { get; set; }

        public SBM_FighterArticles Articles { get; set; }

        public uint Unknown0x4C { get; set; }

        public uint Unknown0x50 { get; set; }

        public uint Unknown0x54 { get; set; }

        public uint Unknown0x58 { get; set; }

        public HSD_JOBJ ShadowModel { get; set; }

        //TODO this section is 0x64 long

        public List<SBM_FighterSubAction> FightSubActions = new List<SBM_FighterSubAction>();
        public List<SBM_FighterSubAction> WinSubActions = new List<SBM_FighterSubAction>();

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);

            Reader.Seek(SubActionOffset);
            for(int i = 0; i < 0x146; i++)
            {
                SBM_FighterSubAction sa = new SBM_FighterSubAction();
                sa.Open(Reader);
                FightSubActions.Add(sa);
            }
            Reader.Seek(WinSubActionOffset);
            for (int i = 0; i < 0xE; i++)
            {
                SBM_FighterSubAction sa = new SBM_FighterSubAction();
                sa.Open(Reader);
                WinSubActions.Add(sa);
            }
        }

        public override void Save(HSDWriter Writer)
        {
            foreach (var sa in FightSubActions)
                sa.Save(Writer);
            
            foreach (var sa in WinSubActions)
                sa.Save(Writer);

            base.Save(Writer);

            Writer.WritePointerAt((int)(Writer.BaseStream.Position - 0x58), FightSubActions[0]);

            Writer.WritePointerAt((int)(Writer.BaseStream.Position - 0x50), WinSubActions[0]);
        }
    }
}
