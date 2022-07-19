using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Rendering;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Controls
{
    public partial class LObjEditor : UserControl, IDrawable
    {
        public class LightPropBase
        {
            protected HSD_LOBJ _lobj;

            [Category("1. Light Properties"), Description("Determines if this light affects Diffuse color.")]
            public bool Diffuse
            {
                get => _lobj.Flags.HasFlag(LOBJ_Flags.LOBJ_DIFFUSE);
                set
                {
                    if (value)
                        _lobj.Flags |= LOBJ_Flags.LOBJ_DIFFUSE;
                    else
                        _lobj.Flags &= ~LOBJ_Flags.LOBJ_DIFFUSE;
                }
            }

            [Category("1. Light Properties"), Description("Determines if this light affects Specular color.")]
            public bool Specular
            {
                get => _lobj.Flags.HasFlag(LOBJ_Flags.LOBJ_SPECULAR);
                set
                {
                    if (value)
                        _lobj.Flags |= LOBJ_Flags.LOBJ_SPECULAR;
                    else
                        _lobj.Flags &= ~LOBJ_Flags.LOBJ_SPECULAR;
                }
            }

            [Category("1. Light Properties"), Description("Determines if this light affects Alpha channel.")]
            public bool Alpha
            {
                get => _lobj.Flags.HasFlag(LOBJ_Flags.LOBJ_ALPHA);
                set
                {
                    if (value)
                        _lobj.Flags |= LOBJ_Flags.LOBJ_ALPHA;
                    else
                        _lobj.Flags &= ~LOBJ_Flags.LOBJ_ALPHA;
                }
            }

            [Category("1. Light Properties"), Description("The color of this light.")]
            public Color Color
            {
                get => _lobj.LightColor;
                set => _lobj.LightColor = value;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropBase(HSD_LOBJ light)
            {
                _lobj = light;
                light.Flags &= ~LOBJ_Flags.LOBJ_SPOT;
                // light._s.SetReference(0x18, null);
            }

            public virtual void Draw()
            {

            }
        }

        public class LightPropEye : LightPropBase
        {
            [Category("2. Position Properties")]
            public float SourceX { get => _lobj.Position.V1; set => _lobj.Position.V1 = value; }

            [Category("2. Position Properties")]
            public float SourceY { get => _lobj.Position.V2; set => _lobj.Position.V2 = value; }

            [Category("2. Position Properties")]
            public float SourceZ { get => _lobj.Position.V3; set => _lobj.Position.V3 = value; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropEye(HSD_LOBJ light) : base(light)
            {
                // set data
                if (light.Position == null)
                    light.Position = new HSD_WOBJ();
            }
        }
        public class LightPropEyeInterest : LightPropEye
        {
            [Category("2. Position Properties")]
            public float TargetX { get => _lobj.Interest.V1; set => _lobj.Interest.V1 = value; }

            [Category("2. Position Properties")]
            public float TargetY { get => _lobj.Interest.V2; set => _lobj.Interest.V2 = value; }

            [Category("2. Position Properties")]
            public float TargetZ { get => _lobj.Interest.V3; set => _lobj.Interest.V3 = value; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropEyeInterest(HSD_LOBJ light) : base(light)
            {
                // set data
                if (light.Interest == null)
                    light.Interest = new HSD_WOBJ();
            }
        }

        public class LightPropAttenuation : LightPropEye
        {
            private HSD_LightAttn _attn;

            [Category("3. Attenuation Properties")]
            public float A0 { get => _attn.A0; set => _attn.A0 = value; }

            [Category("3. Attenuation Properties")]
            public float A1 { get => _attn.A1; set => _attn.A1 = value; }

            [Category("3. Attenuation Properties")]
            public float A2 { get => _attn.A2; set => _attn.A2 = value; }

            [Category("3. Attenuation Properties")]
            public float K0 { get => _attn.K0; set => _attn.K0 = value; }

            [Category("3. Attenuation Properties")]
            public float K1 { get => _attn.K1; set => _attn.K1 = value; }

            [Category("3. Attenuation Properties")]
            public float K2 { get => _attn.K2; set => _attn.K2 = value; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropAttenuation(HSD_LOBJ light) : base(light)
            {
                // set data
                _attn = light._s.GetCreateReference<HSD_LightAttn>(0x18);
                if (_attn._s.Length < _attn.TrimmedSize)
                    _attn._s.Resize(_attn.TrimmedSize);
            }
        }

        public class LightPropAttenuationInterest : LightPropEyeInterest
        {
            private HSD_LightAttn _attn;

            [Category("3. Attenuation Properties")]
            public float A0 { get => _attn.A0; set => _attn.A0 = value; }

            [Category("3. Attenuation Properties")]
            public float A1 { get => _attn.A1; set => _attn.A1 = value; }

            [Category("3. Attenuation Properties")]
            public float A2 { get => _attn.A2; set => _attn.A2 = value; }

            [Category("3. Attenuation Properties")]
            public float K0 { get => _attn.K0; set => _attn.K0 = value; }

            [Category("3. Attenuation Properties")]
            public float K1 { get => _attn.K1; set => _attn.K1 = value; }

            [Category("3. Attenuation Properties")]
            public float K2 { get => _attn.K2; set => _attn.K2 = value; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropAttenuationInterest(HSD_LOBJ light) : base(light)
            {
                // set data
                _attn = light._s.GetCreateReference<HSD_LightAttn>(0x18);
                if (_attn._s.Length < _attn.TrimmedSize)
                    _attn._s.Resize(_attn.TrimmedSize);
            }
        }

        public class LightPropInfinite : LightPropEye
        {
            private HSDAccessor _acc;

            [Category("3. Infinite Properties")]
            public float Shininess { get => _acc._s.GetFloat(0x00); set => _acc._s.SetFloat(0x00, value); }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropInfinite(HSD_LOBJ light) : base(light)
            {
                // set flags
                light.Flags |= LOBJ_Flags.LOBJ_INFINITE;

                // set data
                _acc = light._s.GetCreateReference<HSDAccessor>(0x18);
                if (_acc._s.Length < 4)
                    _acc._s.Resize(4);
            }
        }

        public class LightPropPoint : LightPropEye
        {
            private HSD_LightPoint _point;

            [Category("3. Point Properties"), DisplayName("Ref Brightness"), Description("Specifies the ratio of the brightness at the reference point.\nValid Values are between 0-1")]
            public float ReferenceBrightness { get => _point.RefBrightness; set => _point.RefBrightness = value; }

            [Category("3. Point Properties"), DisplayName("Ref Distance"), Description("Distance between the light and the reference point.\nValid values are >0")]
            public float ReferenceDistance { get => _point.RefDistance; set => _point.RefDistance = value; }

            [Category("3. Point Properties"), DisplayName("Distance Function"), Description("Defines how brightness decreases as a function of distance. The value GX_DA_OFF turns the distance attenuation feature off.")]
            public GXBrightnessDistance DistanceFunc { get => _point.Flag; set => _point.Flag = value; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropPoint(HSD_LOBJ light) : base(light)
            {
                // set flags
                light.Flags |= LOBJ_Flags.LOBJ_POINT;

                // set data
                _point = light._s.GetCreateReference<HSD_LightPoint>(0x18);
                if (_point._s.Length < _point.TrimmedSize)
                    _point._s.Resize(_point.TrimmedSize);
            }

            /// <summary>
            /// 
            /// </summary>
            public override void Draw()
            {
                Rendering.DrawShape.DrawSphere(
                    OpenTK.Matrix4.CreateTranslation(SourceX, SourceY, SourceZ), 
                    ReferenceDistance, 
                    16, 
                    16, 
                    new OpenTK.Vector3(Color.R / 255f, Color.G / 255f, Color.B / 255f),
                    0.7f);
            }
        }

        public class LightPropSpot : LightPropEyeInterest
        {
            private HSD_LightSpot _spot;

            [Category("2. Spotlight Properties"), DisplayName("Ref Brightness"), Description("Specifies the ratio of the brightness at the reference point.\nValid Values are between 0-1")]
            public float ReferenceBrightness { get => _spot.RefBrightness; set => _spot.RefBrightness = value; }

            [Category("2. Spotlight Properties"), DisplayName("Ref Distance"), Description("Distance between the light and the reference point.\nValid values are >0")]
            public float ReferenceDistance { get => _spot.RefDistance; set => _spot.RefDistance = value; }

            [Category("2. Spotlight Properties"), DisplayName("Distance Function"), Description("Defines how brightness decreases as a function of distance. The value GX_DA_OFF turns the distance attenuation feature off.")]
            public GXBrightnessDistance DistanceFunc { get => _spot.DistFunc; set => _spot.DistFunc = value; }


            [Category("2. Spotlight Properties"), DisplayName("Cutoff"), Description("Dpecifies cutoff angle of the spotlight in degrees.\nThe value for cutoff should be within (0.0 < cutoff <= 90.0)")]
            public float Cutoff { get => _spot.Cutoff; set => _spot.Cutoff = value; }

            [Category("2. Spotlight Properties"), DisplayName("Spotlight Function"), Description("Defines type of the illumination distribution within cutoff angle.")]
            public GXSpotFunc SpotFunc { get => _spot.SpotFunc; set => _spot.SpotFunc = value; }

            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="light"></param>
            public LightPropSpot(HSD_LOBJ light) : base(light)
            {
                // set flags
                light.Flags |= LOBJ_Flags.LOBJ_SPOT;

                // set data
                _spot = light._s.GetCreateReference<HSD_LightSpot>(0x18);
                if (_spot._s.Length < _spot.TrimmedSize)
                    _spot._s.Resize(_spot.TrimmedSize);
            }
        }

        private HSD_LOBJ _lobj;

        private ViewportControl _vp;

        public DrawOrder DrawOrder => DrawOrder.Last;

        /// <summary>
        /// 
        /// </summary>
        public LObjEditor()
        {
            InitializeComponent();

            //_vp = new ViewportControl();
            //_vp.DisplayGrid = true;
            //_vp.Dock = DockStyle.Fill;
            //groupBox2.Controls.Add(_vp);

            //_vp.AddRenderer(this);

            //Disposed += (sender, args) =>
            //{
            //    _vp.Dispose();
            //};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lobj"></param>
        public void SetLObj(HSD_LOBJ lobj)
        {
            _lobj = null;

            comboBox1.SelectedIndex = ((int)lobj.Flags & 0x3);
            checkBox1.Checked = lobj.AttenuationFlags.HasFlag(LOBJ_AttenuationFlags.LOBJ_LIGHT_ATTN);

            _lobj = lobj;

            UpdatePropertyBox();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lobj == null)
                return;

            var ob = _lobj._s.GetReference<HSDAccessor>(0x18);
            if (ob != null)
                ob._s.SetBytes(0, new byte[ob._s.Length]);

            UpdatePropertyBox();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                _lobj.AttenuationFlags = LOBJ_AttenuationFlags.LOBJ_LIGHT_ATTN;
            else
                _lobj.AttenuationFlags = LOBJ_AttenuationFlags.LOBJ_LIGHT_ATTN_NONE;

            UpdatePropertyBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdatePropertyBox()
        {
            if (_lobj == null)
                return;

            // set light based on type
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    propertyGrid1.SelectedObject = new LightPropBase(_lobj);
                    break;
                case 1:
                    propertyGrid1.SelectedObject = new LightPropInfinite(_lobj);
                    break;
                case 2:
                    propertyGrid1.SelectedObject = new LightPropPoint(_lobj);
                    break;
                case 3:
                    propertyGrid1.SelectedObject = new LightPropSpot(_lobj);
                    break;
            }

            // check is attenuation is set
            if (checkBox1.Checked)
            {
                if (comboBox1.SelectedIndex == 3)
                    propertyGrid1.SelectedObject = new LightPropAttenuationInterest(_lobj);
                else
                    propertyGrid1.SelectedObject = new LightPropAttenuation(_lobj);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="windowWidth"></param>
        /// <param name="windowHeight"></param>
        public void Draw(Camera cam, int windowWidth, int windowHeight)
        {
            var ob = propertyGrid1.SelectedObject;

            if (ob is LightPropBase prop)
                prop.Draw();
        }
    }
}
