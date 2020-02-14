using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw
{
    /// <summary>
    /// Manager for handling underlying structure data
    /// </summary>
    public class HSDStruct
    {
        private byte[] _data = new byte[0];

        public bool IsTextureBuffer = false;

        public Dictionary<int, HSDStruct> References { get => _references; }
        private Dictionary<int, HSDStruct> _references = new Dictionary<int, HSDStruct>();

        public int Length => _data.Length;
        
        /// <summary>
        /// returns a collection of all references and sub references in the struct
        /// </summary>
        /// <returns></returns>
        public List<HSDStruct> GetSubStructs()
        {
            List<HSDStruct> list = new List<HSDStruct>();
            var contain = new HashSet<HSDStruct>();
            Search(ref contain, ref list);
            //list.Reverse();
            return list;
        }

        /// <summary>
        /// recursivly gathers references and subreferences and stores them in given list
        /// </summary>
        /// <param name="contain"></param>
        /// <param name="list"></param>
        private void Search(ref HashSet<HSDStruct> contain, ref List<HSDStruct> list)
        {
            if (contain.Contains(this))
                return;
            contain.Add(this);
            list.Add(this);
            foreach (var v in _references)
                v.Value.Search(ref contain, ref list);
        }

        public HSDStruct()
        {

        }

        public HSDStruct(int size)
        {
            _data = new byte[size];
        }

        public HSDStruct(byte[] data)
        {
            _data = data;
        }

        /// <summary>
        /// Sets the contents of this struct to the contents of another
        /// </summary>
        /// <param name="str"></param>
        public void SetFromStruct(HSDStruct str)
        {
            _data = str._data;
            _references = str._references;
        }

        public byte[] GetData()
        {
            return _data;
        }

        public void SetData(byte[] data)
        {
            _data = data;
        }

        public override string ToString()
        {
            return $"HSDStruct: (Length: 0x{_data.Length.ToString("X")}, ReferenceCount: {_references.Count})";
        }

        /// <summary>
        /// gets a byte buffer at given location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public byte[] GetBuffer(int loc)
        {
            if (_references.ContainsKey(loc))
            {
                var reference = _references[loc];
                return reference.GetData();
            }
            return null;
        }

        /// <summary>
        /// sets a byte buffer at given location
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="data"></param>
        public void SetBuffer(int loc, byte[] data)
        {
            if(data == null)
            {
                SetInt32(loc, 0);
                _references.Remove(loc);
            }
            if (_references.ContainsKey(loc))
            {
                var reference = _references[loc];
                reference.IsTextureBuffer = true;
                reference.SetData(data);
            }
            else
            {
                _references.Add(loc, new HSDStruct(data) { IsTextureBuffer = true });
            }
        }

        /// <summary>
        /// sets reference to given accessor at given location
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="value"></param>
        public void SetReference(int loc, HSDAccessor value)
        {
            if (value == null)
                SetReferenceStruct(loc, null);
            else
                SetReferenceStruct(loc, value._s);
        }

        /// <summary>
        /// Sets reference to given struct at given location
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="value"></param>
        public void SetReferenceStruct(int loc, HSDStruct value)
        {
            if (!_references.ContainsKey(loc))
                _references.Add(loc, value);
            _references[loc] = value;
            if (value == null)
                _references.Remove(loc);
            SetInt32(loc, 0);
        }

        /// <summary>
        /// Gets a reference at location and returns null if one isn't found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc"></param>
        /// <returns></returns>
        public T GetReference<T>(int loc) where T : HSDAccessor
        {
            if (_references.ContainsKey(loc))
            {
                var reference = _references[loc];

                var newref = Activator.CreateInstance<T>();
                newref._s = reference;
                
                return newref;
            }
            return null;
        }

        /// <summary>
        /// Gets a reference at location and creates a reference if none exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc"></param>
        /// <returns></returns>
        public T GetCreateReference<T>(int loc) where T : HSDAccessor
        {
            var re = GetReference<T>(loc);
            if (re == null)
            {
                SetReference(loc, Activator.CreateInstance<T>());
                re = GetReference<T>(loc);
            }
            return re;
        }

        /// <summary>
        /// Removes all references to given struct
        /// </summary>
        /// <param name="strct"></param>
        public bool RemoveReferenceToStruct(HSDStruct strct)
        {
            foreach (var v in _references)
            {
                if (v.Value == strct)
                {
                    SetReferenceStruct(v.Key, null);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replaces all references to given struct with another
        /// </summary>
        /// <param name="oldStruct"></param>
        /// <param name="newStruct"></param>
        /// <returns>true if at least one struct was replaced</returns>
        public bool ReplaceReferenceToStruct(HSDStruct oldStruct, HSDStruct newStruct)
        {
            bool replaced = false;
            var keys = _references.Keys.ToArray();
            foreach (var k in keys)
            {
                var v = _references[k];
                if (v == oldStruct)
                {
                    SetReferenceStruct(k, newStruct);
                    replaced = true;
                }
            }
            return replaced;
        }

        /// <summary>
        /// Creates a new struct from a struct embedded into this one
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public HSDStruct GetEmbeddedStruct(int loc, int length)
        {
            HSDStruct s = new HSDStruct();

            s.SetData(GetBytes(loc, length));
            
            foreach(var v in _references)
            {
                if(v.Key >= loc && v.Key < loc + length)
                {
                    s.SetReferenceStruct(v.Key - loc, v.Value);
                }
            }

            return s;
        }
        
        /// <summary>
        /// Embeddeds a given <see cref="HSDStruct"/> at given location
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="str"></param>
        public void SetEmbededStruct(int loc, HSDStruct str)
        {
            if(str == null)
                return;

            // Sets the bytes for this range
            SetBytes(loc, str.GetData());

            // Remove references in this range
            for(int i = loc; i < loc + str.Length; i++)
            {
                if (References.ContainsKey(i))
                    References.Remove(i);
            }
            
            // Copy new references over
            foreach (var v in str._references)
            {
                SetReferenceStruct(loc + v.Key, v.Value);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public T[] GetEmbeddedAccessorArray<T>(int loc, int count) where T : HSDAccessor
        {
            int structSize = Activator.CreateInstance<T>().TrimmedSize;
            if (structSize == -1)
                throw new NotImplementedException("Size is not implemented for " + typeof(T).ToString());

            int length = structSize * count;
            if (Length < loc + length)
                throw new IndexOutOfRangeException("Embedded array reaches out of bound");

            T[] v = new T[count];

            for(int i = 0; i < count; i++)
            {
                v[i] = Activator.CreateInstance<T>();
                v[i]._s = GetEmbeddedStruct(loc + structSize * i, v[i].TrimmedSize);
            }

            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc"></param>
        /// <param name="values"></param>
        public void SetEmbeddedAccessorArray<T>(int loc, T[] values) where T : HSDAccessor
        {
            //TODO: allow multiple embed structs?

            var length = Activator.CreateInstance<T>().TrimmedSize;
            
            // clear existing struct references
            for (int i = loc; i < Length; i++)
                if (References.ContainsKey(i))
                    References.Remove(i);

            // resize struct to fix
            Resize(loc + values.Length * length);

            // embed data
            for(int i = 0; i < values.Length; i++)
            {
                if (values[i]._s.Length > length)
                    throw new OverflowException($"Struct size ({values[i].GetType()})=>({typeof(T)}) was larger than expected {values[i]._s.Length} > {length}");
                SetEmbededStruct(loc + i * length, values[i]._s);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc"></param>
        /// <returns></returns>
        public T[] GetNullPointerArray<T>(int loc) where T : HSDAccessor
        {
            var re = GetReference<HSDNullPointerArrayAccessor<T>>(loc);
            if (re == null)
                return null;
            return re.Array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc">location of array pointer</param>
        /// <param name="countLoc">location of count</param>
        /// <param name="value"></param>
        public void SetNullPointerArray<T>(int loc, T[] value) where T : HSDAccessor
        {
            if (value == null || value.Length == 0)
            {
                SetReference(loc, null);
            }
            else
            {
                var re = GetCreateReference<HSDArrayAccessor<T>>(loc);
                re.Array = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc"></param>
        /// <returns></returns>
        public T[] GetArray<T>(int loc) where T : HSDAccessor
        {
            var re = GetReference<HSDArrayAccessor<T>>(loc);
            if (re == null)
                return null;
            return re.Array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loc">location of array pointer</param>
        /// <param name="countLoc">location of count</param>
        /// <param name="value"></param>
        public void SetArray<T>(int loc, int countLoc, T[] value) where T : HSDAccessor
        {
            if (value == null || value.Length == 0)
            {
                if(countLoc != -1)
                    SetInt32(countLoc, 0);
                SetReference(loc, null);
            }
            else
            {
                if (countLoc != -1)
                    SetInt32(countLoc, value.Length);
                var re = GetCreateReference<HSDArrayAccessor<T>>(loc);
                re.Array = value;
            }
        }


        /// <summary>
        /// Gets string array at specificed pointer
        /// Size is automatically assumed
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public string[] GetStringArray(int loc)
        {
            var a = GetReference<HSDAccessor>(loc);
            if (a == null)
                return null;
            else
            {
                string[] s = new string[a._s.References.Count];
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = a._s.GetString(i * 4);
                }
                return s;
            }
        }

        /// <summary>
        /// Sets string array at pointer with optional embedded count location
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="value"></param>
        /// <param name="countloc"></param>
        public void SetStringArray(int loc, string[] value, int countloc = -1)
        {
            if (value == null)
            {
                SetReference(loc, null);
                if (countloc != -1)
                    SetInt32(countloc, 0);
            }
            else
            {
                if (countloc != -1)
                    SetInt32(countloc, countloc);

                var a = GetCreateReference<HSDAccessor>(loc);
                a._s.References.Clear();
                a._s.Resize(4 * value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    a._s.SetString(i * 4, value[i]);
                }
            }
        }

        public int GetInt32(int loc)
        {
            return BitConverter.ToInt32(EndianFix(GetBytes(loc, 4)), 0);
        }

        public void SetInt32(int loc, int value)
        {
            SetBytes(loc, EndianFix(BitConverter.GetBytes(value)));
        }

        public short GetInt16(int loc)
        {
            return BitConverter.ToInt16(EndianFix(GetBytes(loc, 2)), 0);
        }

        public void SetInt16(int loc, short value)
        {
            SetBytes(loc, EndianFix(BitConverter.GetBytes(value)));
        }

        public ushort GetUInt16(int loc)
        {
            return BitConverter.ToUInt16(EndianFix(GetBytes(loc, 2)), 0);
        }

        public void SetUInt16(int loc, ushort value)
        {
            SetBytes(loc, EndianFix(BitConverter.GetBytes(value)));
        }

        public byte GetByte(int loc)
        {
            return _data[loc];
        }

        public void SetByte(int loc, byte value)
        {
            _data[loc] = value;
        }

        /// <summary>
        /// Gets string value at pointer location
        /// </summary>
        /// <param name="refloc"></param>
        /// <returns></returns>
        public string GetString(int refloc)
        {
            var v = GetReference<HSDAccessor>(refloc);
            if (v == null)
                return null;
            else
            {
                var nullpoint = 0;
                foreach (var d in v._s.GetData())
                    if (d == 0)
                        break;
                    else
                        nullpoint++;
                return Encoding.UTF8.GetString(v._s.GetData(), 0, nullpoint);
            }
        }

        /// <summary>
        /// Sets pointer at location to string value
        /// </summary>
        /// <param name="refloc"></param>
        /// <param name="value"></param>
        public void SetString(int refloc, string value)
        {
            if (value == null)
                SetReference(refloc, null);
            else
            {
                var re = GetCreateReference<HSDAccessor>(refloc);
                re._s.SetData(Encoding.UTF8.GetBytes(value));
                re._s.Resize(re._s.Length + 1);
                if (re._s.Length % 4 != 0)
                    re._s.Resize(re._s.Length + (4 - (re._s.Length % 4)));
            }
        }

        public float GetFloat(int loc)
        {
            return BitConverter.ToSingle(EndianFix(GetBytes(loc, 4)), 0);
        }

        public void SetFloat(int loc, float value)
        {
            SetBytes(loc, EndianFix(BitConverter.GetBytes(value)));
        }

        public byte[] GetBytes(int loc, int length)
        {
            if (loc < 0 || loc > _data.Length || loc > _data.Length + length)
                throw new InvalidOperationException("Tried to get data out of struct range");

            byte[] bytes = new byte[length];
            for (int i = loc; i < Math.Min(_data.Length, loc + bytes.Length); i++)
                bytes[i - loc] = _data[i];

            return bytes;
        }

        public void SetBytes(int loc, byte[] bytes)
        {
            if (loc < 0 || loc > _data.Length + bytes.Length)
                throw new InvalidOperationException("Tried to set data out of struct range");

            for (int i = loc; i < loc + bytes.Length; i++)
                _data[i] = bytes[i - loc];
        }

        public void Resize(int size)
        {
            Array.Resize(ref _data, size);
        }

        private byte[] EndianFix(byte[] b)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return b;
        }



        /// <summary>
        /// Gets string array at specificed pointer
        /// Size is automatically assumed
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public int[] GetInt32Array(int loc)
        {
            var a = GetReference<HSDAccessor>(loc);
            if (a == null)
                return null;
            else
            {
                int[] s = new int[a._s.Length / 4];
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = a._s.GetInt32(i * 4);
                }
                return s;
            }
        }

        /// <summary>
        /// Sets string array at pointer with optional embedded count location
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="value"></param>
        /// <param name="countloc"></param>
        public void SetInt32Array(int loc, int[] value, int countloc = -1)
        {
            if (value == null)
            {
                SetReference(loc, null);
                if (countloc != -1)
                    SetInt32(countloc, 0);
            }
            else
            {
                if (countloc != -1)
                    SetInt32(countloc, countloc);

                var a = GetCreateReference<HSDAccessor>(loc);
                a._s.Resize(4 * value.Length);

                for (int i = 0; i < value.Length; i++)
                {
                    a._s.SetInt32(i * 4, value[i]);
                }
            }
        }
    }
}
