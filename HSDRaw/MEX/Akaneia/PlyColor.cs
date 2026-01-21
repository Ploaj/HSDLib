using System.Drawing;

namespace HSDRaw.MEX.Akaneia
{
    /// <summary>
    /// 
    /// </summary>
    public class HSDColorArray : HSDPrimitiveArray<Color>
    {
        public override int Stride => 4;

        protected override Color Get(int index)
        {
            return _s.GetColorRGBA(index * Stride);
        }

        protected override void Set(int index, Color value)
        {
            _s.SetColorRGBA(index * Stride, value);
        }
    }
}
