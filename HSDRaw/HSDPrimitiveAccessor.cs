using System;

namespace HSDRaw
{
    /// <summary>
    /// 
    /// </summary>
    public class HSDPrimitiveArray<T> : HSDAccessor where T : struct
    {
        public int Length => _s.Length / Stride;

        public virtual int Stride => 4;

        public T[] Array
        {
            get
            {
                T[] v = new T[Length];
                for (int i = 0; i < Length; i++)
                    v[i] = this[i];
                return v;
            }
            set
            {
                _s.Resize(Stride * value.Length);
                for (int i = 0; i < value.Length; i++)
                    this[i] = value[i];
            }
        }

        public T this[int i]
        {
            get
            {
                if(i >= Length || i < 0)
                    return (T)Convert.ChangeType(0, typeof(T));

                return Get(i);
            }
            set
            {
                if (i * Stride + Stride > _s.Length)
                    _s.Resize(i * Stride + Stride);

                Set(i, value);
            }
        }

        protected virtual T Get(int index)
        {
            return (T)Convert.ChangeType(0, typeof(T));
        }

        protected virtual void Set(int index, T value)
        {
        }

        public void RemoveAt(int index)
        {
            var data = _s.GetData();
            var newData = new byte[data.Length - Stride];

            if (index > 0)
                System.Array.Copy(data, 0, newData, 0, index * Stride);

            if (index < Length - 1)
                System.Array.Copy(data, (index + 1) * Stride, newData, index * Stride, (Length - index) * Stride);

            _s.SetData(newData);

            if(_s.Length == 0)
                _s.Resize(0x04);
        }

        public void Add(T value)
        {
            this[Length] = value;
        }

        public override void New()
        {
            base.New();
            _s.Resize(0x04);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSDFloatArray : HSDPrimitiveArray<float>
    {
        public override int Stride => 4;

        protected override float Get(int index)
        {
            return _s.GetFloat(index * Stride);
        }

        protected override void Set(int index, float value)
        {
            _s.SetFloat(index * Stride, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSDUIntArray : HSDPrimitiveArray<uint>
    {
        public override int Stride => 4;

        protected override uint Get(int index)
        {
            return (uint)_s.GetInt32(index * Stride);
        }

        protected override void Set(int index, uint value)
        {
            _s.SetInt32(index * Stride, (int)value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSDIntArray :  HSDPrimitiveArray<int>
    {
        public override int Stride => 4;

        protected override int Get(int index)
        {
            return _s.GetInt32(index * Stride);
        }

        protected override void Set(int index, int value)
        {
            _s.SetInt32(index * Stride, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSDShortArray : HSDPrimitiveArray<short>
    {
        public override int Stride => 2;

        protected override short Get(int index)
        {
            return _s.GetInt16(index * Stride);
        }

        protected override void Set(int index, short value)
        {
            _s.SetInt16(index * Stride, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSDUShortArray : HSDPrimitiveArray<ushort>
    {
        public override int Stride => 2;
        protected override ushort Get(int index)
        {
            return (ushort)_s.GetInt16(index * Stride);
        }

        protected override void Set(int index, ushort value)
        {
            _s.SetInt16(index * Stride, (short)value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HSDByteArray : HSDPrimitiveArray<byte>
    {
        public override int Stride => 1;

        protected override byte Get(int index)
        {
            return _s.GetByte(index * Stride);
        }

        protected override void Set(int index, byte value)
        {
            _s.SetByte(index * Stride, value);
        }
    }
}
