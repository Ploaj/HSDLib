using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MeleeLib.IO;

namespace MeleeLib.DAT
{
    public class DatMaterialColor : DatNode
    {
        public Color DIF = Color.White, AMB = Color.White, SPC = Color.White;
        public float Transparency = 1, Glossiness = 1;

        public void Deserialize(DATReader d, DATRoot Root)
        {
            AMB = Color.FromArgb(d.Byte(), d.Byte(), d.Byte());
            d.Byte();
            DIF = Color.FromArgb(d.Byte(), d.Byte(), d.Byte());
            d.Byte();
            SPC = Color.FromArgb(d.Byte(), d.Byte(), d.Byte());
            d.Byte();
            Transparency = d.Float();
            Glossiness = d.Float();
        }

        public override void Serialize(DATWriter Node)
        {
            Node.AddObject(this);
            Node.Byte(AMB.R); Node.Byte(AMB.G); Node.Byte(AMB.B); Node.Byte(AMB.A);
            Node.Byte(DIF.R); Node.Byte(DIF.G); Node.Byte(DIF.B); Node.Byte(DIF.A);
            Node.Byte(SPC.R); Node.Byte(SPC.G); Node.Byte(SPC.B); Node.Byte(SPC.A);
            Node.Float(Transparency);
            Node.Float(Glossiness);
        }
    }
}
