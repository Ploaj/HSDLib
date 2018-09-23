using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeleeLib.DAT;

namespace Modlee
{
    public partial class MaterialEditor : UserControl
    {
        public MaterialEditor()
        {
            InitializeComponent();
        }

        public void SetMaterial(DatMaterial mat)
        {
            pictureBox1.Image = mat.Textures[0].GetBitmap();
            buttonDIF.BackColor = mat.MaterialColor.DIF;
            buttonAMB.BackColor = mat.MaterialColor.AMB;
            buttonSPC.BackColor = mat.MaterialColor.SPC;
        }
    }
}
