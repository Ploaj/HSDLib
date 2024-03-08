using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw.MEX
{
    [Flags]
    public enum HSD_BUTTON
    {
        DPAD_LEFT = 0x0001,
        DPAD_RIGHT = 0x0002,
        DPAD_DOWN = 0x0004,
        DPAD_UP = 0x0008,
        TRIGGER_Z = 0x0010,
        TRIGGER_R = 0x0020,
        TRIGGER_L = 0x0040,
        BUTTON_A = 0x0100,
        BUTTON_B = 0x0200,
        BUTTON_X = 0x0400,
        BUTTON_Y = 0x0800,
        BUTTON_START = 0x1000,
        BUTTON_UP = 0x10000,
        BUTTON_DOWN = 0x20000,
        BUTTON_LEFT = 0x40000,
        BUTTON_RIGHT = 0x80000,
    }

    public class MEX_BGMModel : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public HSD_JOBJ Model { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public HSD_AnimJoint Animation { get => _s.GetReference<HSD_AnimJoint>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class MEX_Stock : HSDAccessor
    {
        public override int TrimmedSize => 0x10;

        public short Reserved { get => _s.GetInt16(0x00); set => _s.SetInt16(0x00, value); }

        public short Stride { get => _s.GetInt16(0x02); set => _s.SetInt16(0x02, value); }

        public HSD_MatAnimJoint MatAnimJoint { get => _s.GetReference<HSD_MatAnimJoint>(0x04); set => _s.SetReference(0x04, value); }

        public int CustomStockLength { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        public HSDArrayAccessor<MEX_StockEgg> CustomStockEntries { get => _s.GetReference<HSDArrayAccessor<MEX_StockEgg>>(0x0C); set => _s.SetReference(0x0C, value); }
    
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void GetFighterIcons(out List<List<HSD_TOBJ>> fighters, out List<HSD_TOBJ> reserved)
        {
            var keys = MatAnimJoint.MaterialAnimation.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();
            var tobjs = MatAnimJoint.MaterialAnimation.TextureAnimation.ToTOBJs();

            reserved = new List<HSD_TOBJ>(Reserved);
            fighters = new List<List<HSD_TOBJ>>(Stride);

            // get reserved images
            for (int i = 0; i < Reserved; i++)
            {
                var frame = keys.Find(e => e.Frame == i);
                if (frame != null) 
                    reserved.Add(tobjs[(int)frame.Value]);
            }

            // get stock icons
            for (int i = 0; i < Stride; i++)
            {
                fighters.Add(new List<HSD_TOBJ>());
                
                var colorCount = 0;

                while (keys.Find(e => e.Frame == Reserved + Stride * colorCount + i) != null)
                    colorCount++;

                for (int color = 0; color < colorCount; color++)
                {
                    var frame = keys.Find(e => e.Frame == Reserved + Stride * color + i);

                    if (frame != null)
                        fighters[i].Add(tobjs[(int)frame.Value]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void GenerateStockData(List<List<HSD_TOBJ>> fighters, List<HSD_TOBJ> reserved)
        {
            // set meta data
            Reserved = (short)reserved.Count;
            Stride = (short)fighters.Count;

            List<FOBJKey> keys = new List<FOBJKey>();
            List<HSD_TOBJ> tobjs = new List<HSD_TOBJ>();

            // reserved
            for (int i = 0; i < reserved.Count; i++)
            {
                if (reserved[i].ImageData == null)
                    continue;

                keys.Add(new FOBJKey() { Frame = i, InterpolationType = GXInterpolationType.HSD_A_OP_CON, Value = tobjs.Count });
                tobjs.Add(reserved[i]);
            }

            // get fighter stock icons
            for (int i = 0; i < fighters.Count; i++)
            {
                for (int j = 0; j < fighters[i].Count; j++)
                {
                    if (fighters[i][j] == null || fighters[i][j].ImageData == null)
                        break;

                    keys.Add(new FOBJKey() { Frame = Reserved + j * Stride + i, InterpolationType = GXInterpolationType.HSD_A_OP_CON, Value = tobjs.Count });
                    tobjs.Add(fighters[i][j]);
                }
            }

            // order keys
            keys = keys.OrderBy(e => e.Frame).ToList();

            // generate new tex anim
            var newTexAnim = new HSD_TexAnim();

            newTexAnim.AnimationObject = new HSD_AOBJ();
            newTexAnim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            newTexAnim.AnimationObject.FObjDesc.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TIMG);
            newTexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
            newTexAnim.AnimationObject.FObjDesc.Next.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TCLT);

            newTexAnim.FromTOBJs(tobjs, false);
            newTexAnim.Optimize();

            MatAnimJoint = new HSD_MatAnimJoint();
            MatAnimJoint.MaterialAnimation = new HSD_MatAnim();
            MatAnimJoint.MaterialAnimation.TextureAnimation = newTexAnim;
        }
    }

    public class MEX_StockEgg : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public byte ReserveredStockIndex { get => _s.GetByte(0x00); set => _s.SetByte(0x00, value); }

        public HSD_BUTTON Input { get => (HSD_BUTTON)(_s.GetInt32(0x00) & 0xFFFFFF); set => _s.SetInt32(0x00, (ReserveredStockIndex << 24) | ((int)value & 0xFFFFFF)); }
        
        public string Name
        {
            get
            {
                var data = _s.GetReference<HSDAccessor>(0x04);

                if(data != null)
                {
                    return ShiftJIS.ToUnicode(data._s.GetData());
                }

                return "";
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    _s.SetReference(0x04, null);
                }
                else
                {
                    // max of 8 chars
                    if (value.Length > 8)
                        value = value.Substring(0, 8);

                    // set data
                    var s = _s.GetCreateReference<HSDAccessor>(0x04)._s;
                    s.SetData(ShiftJIS.ToShiftJIS(value));

                    // null terminated
                    s.Resize(s.Length + 1);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
