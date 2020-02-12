namespace HSDRaw.Melee.Pl
{
    public class ByteTable : HSDAccessor
    {
        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public byte[] Entries
        {
            get
            {
                var a = _s.GetReference<HSDAccessor>(0x04);
                if (a == null)
                    return null;
                return a._s.GetBytes(0x00, Count);
            }
            set
            {
                if (value == null)
                {
                    Count = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    Count = value.Length;
                    var a = _s.GetCreateReference<HSDAccessor>(0x04);
                    var size = Count;
                    if (size % 4 != 0)
                        size += 4 - (size % 4);
                    a._s.SetData(value);
                    a._s.Resize(size);
                }
            }
        }
    }

    public class Int16Table : HSDAccessor
    {
        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public short[] Entries
        {
            get
            {
                var a = _s.GetReference<HSDAccessor>(0x04);
                if (a == null)
                    return null;

                var e = new short[Count];

                for (int i = 0; i < Count; i++)
                    e[i] = a._s.GetInt16(i * 2);

                return e;
            }
            set
            {
                if (value == null)
                {
                    Count = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    Count = value.Length;
                    var a = _s.GetCreateReference<HSDAccessor>(0x04);

                    var size = Count * 2;
                    if (size % 4 != 0)
                        size += 4 - (size % 4);

                    a._s.Resize(size);
                    for (int i = 0; i < Count; i++)
                        a._s.SetInt16(i * 2, value[i]);
                }
            }
        }
    }

    public class Int32Table : HSDAccessor
    {
        public int Count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public int[] Entries
        {
            get
            {
                var a = _s.GetReference<HSDAccessor>(0x04);
                if (a == null)
                    return null;

                var e = new int[Count];

                for (int i = 0; i < Count; i++)
                    e[i] = a._s.GetInt32(i * 4);

                return e;
            }
            set
            {
                if (value == null)
                {
                    Count = 0;
                    _s.SetReference(0x04, null);
                }
                else
                {
                    Count = value.Length;
                    var a = _s.GetCreateReference<HSDAccessor>(0x04);
                    a._s.Resize(4 * Count);
                    for (int i = 0; i < Count; i++)
                        a._s.SetInt32(i * 4, value[i]);
                }
            }
        }
    }
}
