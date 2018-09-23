using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;

namespace MeleeLib.DAT.Script
{
    public class SubAction
    {
        public byte[] Data;
    }

    public class DatFighterScript : DatNode
    {
        public int Flags = 1;
        public int AnimationOffset;
        public int AnimationSize;

        public List<SubAction> SubActions = new List<SubAction>();

        // For Injecting
        public int Offset;
        public int SubActionOffset;
        public int SubActionSize;

        public void Deserialize(DATReader r, DATRoot Root)
        {
            Offset = r.Pos();
            int StringOffset = r.Int();
            AnimationOffset = r.Int();
            AnimationSize = r.Int();
            int ScriptOffset = r.Int();
            Flags = r.Int();
            int UnkOffset = r.Int();

            Text = r.String(StringOffset);

            // Read Script Data
            int temp = r.Pos();
            SubActionOffset = ScriptOffset;
            r.Seek(ScriptOffset);
            byte flag = r.Byte();
            while(flag != 0)
            {
                SubAction a = new SubAction();
                byte f = (byte)((int)flag >> 2);
                a.Data = r.getSection(r.Pos() - 1, MeleeCMD.GetSize(f));
                SubActions.Add(a);
                r.Skip(MeleeCMD.GetSize(f) - 1);
                flag = r.Byte();
            }
            r.Skip(3);
            SubActionSize = r.Pos() - SubActionOffset;
            r.Seek(temp);
            if (UnkOffset != 0) throw new Exception("What is this offset 0x" + UnkOffset.ToString("x"));
        }

        public override void Serialize(DATWriter Node)
        {

        }
    }

    public class DatFighterData : DatNode
    {
        public float[] CharacterAttributes = new float[0x61];
        public List<DatFighterScript> Scripts = new List<DatFighterScript>();
        
        public void Deserialize(DATReader r, DATRoot Root)
        {
            //Console.WriteLine("Data At 0x" + r.Pos().ToString("x"));
            Root.FighterData.Add(this);
            int AttributesOffset = r.Int();
            int AttributesOffset2 = r.Int();
            int Unk1 = r.Int();
            int SubActionOffsetStart = r.Int();
            int Unk2 = r.Int();
            int SubActionOffsetEnd = r.Int();
            // 18 more offsets to Sections....

            r.Seek(AttributesOffset);
            for (int i = 0; i < 0x61; i++)
                CharacterAttributes[i] = r.Float();

            r.Seek(SubActionOffsetStart);
            while(r.Pos() < SubActionOffsetEnd)
            {
                DatFighterScript s = new DatFighterScript();
                s.Deserialize(r, Root);
                Scripts.Add(s);
            }


            //Console.WriteLine("Data At 0x" + AttributesOffset.ToString("x"));
        }

        public override void Serialize(DATWriter Node)
        {
            
        }

    }
}
