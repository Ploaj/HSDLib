using HSDRaw.Common;
using System.ComponentModel;
using System.Drawing;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public class TObjProxy : ImageArrayItem
    {

        [DisplayName("Texture Data"), Category("1. Texture Data"), Description(""), TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_TOBJ TOBJ { get; set; }

        [DisplayName("Enable TEV"), Category("2. TEV"), Description(""), RefreshProperties(RefreshProperties.All)]
        public bool EnableTEV
        {
            get => TOBJ.TEV != null;
            set
            {
                if (value)
                {
                    TOBJ.TEV = _tev;
                }
                else
                {
                    TOBJ.TEV = null;
                }
            }
        }

        [DisplayName("TEV Data"), Category("2. TEV"), Description(""), TypeConverter(typeof(ExpandableObjectConverter))]
        public HSD_TOBJ_TEV tev { get => EnableTEV ? _tev : null; set => _tev = value; }
        private HSD_TOBJ_TEV _tev;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tobj"></param>
        public TObjProxy(HSD_TOBJ tobj)
        {
            TOBJ = tobj;

            if (tobj.TEV != null)
                _tev = tobj.TEV;
            else
            {
                _tev = new HSD_TOBJ_TEV()
                {
                    alpha_a_in = TOBJ_TEV_CA.GX_CC_ZERO,
                    alpha_c_in = TOBJ_TEV_CA.GX_CC_ZERO,
                    alpha_b_in = TOBJ_TEV_CA.GX_CC_ZERO,
                    alpha_d_in = TOBJ_TEV_CA.GX_CC_ZERO,
                    color_a_in = TOBJ_TEV_CC.GX_CC_ZERO,
                    color_b_in = TOBJ_TEV_CC.GX_CC_ZERO,
                    color_c_in = TOBJ_TEV_CC.GX_CC_ZERO,
                    color_d_in = TOBJ_TEV_CC.GX_CC_ZERO,
                };
            }
        }

        public Image ToImage()
        {
            return TOBJ.ToImage().ToBitmap();
        }

        public void Dispose()
        {

        }

        public override string ToString()
        {
            return $"{TOBJ.TexMapID} {TOBJ.Flags.ToString()}";
        }
    }
}
