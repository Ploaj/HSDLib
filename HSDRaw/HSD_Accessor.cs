using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRaw
{
    /// <summary>
    /// Accessor for accesing structure data
    /// Extent for custom properties
    /// </summary>
    public class HSDAccessor
    {
        public HSDStruct _s;
        
        public virtual int TrimmedSize { get; } = -1;

        public HSDAccessor()
        {
            New();
        }

        public void New()
        {
            if (TrimmedSize != -1)
            {
                _s = new HSDStruct(new byte[TrimmedSize]);
            }
            else
                _s = new HSDStruct(new byte[0]);
        }

        public override bool Equals(object obj)
        {
            if (obj is HSDAccessor acc)
                return _s.Equals(acc._s);

            return false;
        }

        public override int GetHashCode()
        {
            return _s.GetHashCode();
        }
    }

    /// <summary>
    /// Special Accessor for handling simpler HSDStructs
    /// </summary>
    public class HSDDynamicAccessor : HSDAccessor
    {
        public string[] PropertyNames { get { return NameToStruct.Keys.ToArray(); } }

        private Dictionary<string, StructInfo> NameToStruct = new Dictionary<string, StructInfo>();

        private static Dictionary<string, Type> Types = new Dictionary<string, Type>() {
            { "int", typeof(int)}
        };

        public object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public void LoadStructureFromFile(string filePath)
        {
            /*var lines = System.IO.File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var args = line.Split(' ');
                var off = Convert.ToInt32(args[0], 16);
                var type = Types[args[1]];
                var name = args[2];

                NameToStruct.Add(name, new StructInfo() { Name = name, Type = type, Location = off });
            }*/
        }

        public object Get(string name)
        {
            if (NameToStruct.ContainsKey(name))
            {
                var structinfo = NameToStruct[name];
                if (structinfo.Type == typeof(int))
                    return _s.GetInt32(structinfo.Location);
            }

            return null;
        }

        public void Set(string name, object value)
        {
            if (NameToStruct.ContainsKey(name))
            {
                var structinfo = NameToStruct[name];

                if (structinfo.Type == typeof(int) && value is int i)
                    _s.SetInt32(structinfo.Location, i);
            }
        }
    }

    public class StructInfo
    {
        public string Name { get; set; }
        public int Location { get; set; }
        public Type Type { get; set; }
    }

    /// <summary>
    /// Accessor that implements a tree containing a next and child node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HSDTreeAccessor<T> : HSDListAccessor<T> where T : HSDTreeAccessor<T>
    {
        public virtual T Child { get; set; }

        public T[] Children
        {
            get
            {
                if (Child != null)
                    return Child.List.ToArray();
                return new T[0];
            }
        }

        public List<T> DepthFirstList
        {
            get
            {
                Queue<T> Que = new Queue<T>();
                foreach (T j in List)
                {
                    Enqueue(j, Que);
                }
                List<T> JOBJS = new List<T>();
                while (Que.Count > 0)
                    JOBJS.Add(Que.Dequeue());
                return JOBJS;
            }
        }

        private void Enqueue(T JOBJ, Queue<T> Que)
        {
            Que.Enqueue(JOBJ);
            if (JOBJ.Child == null) return;
            foreach (T Child in JOBJ.Child.List)
            {
                Enqueue(Child, Que);
            }
        }
    }

    /// <summary>
    /// Accessor that can be accessed like a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HSDListAccessor<T> : HSDAccessor where T : HSDListAccessor<T>
    {
        public virtual T Next { get; set; }

        public List<T> List
        {
            get
            {
                var list = new List<T>();
                var i = (T)this;
                while (i != null)
                {
                    list.Add(i);
                    i = i.Next;
                }

                return list;
            }
        }
    }

    /// <summary>
    /// Null terminated pointer list
    /// Accessor for accesing structure data
    /// Extend for custom properties
    /// </summary>
    public class HSDNullPointerArrayAccessor<T> : HSDAccessor where T : HSDAccessor
    {
        public int Length => ((_s.Length / 4) - 1);

        public T[] Array
        {
            get
            {
                T[] t = new T[Length];
                for (int i = 0; i < Length; i++)
                    t[i] = this[i];
                return t;
            }
            set
            {
                if (_s == null)
                    _s = new HSDStruct(new byte[value.Length * 4 + 4]);
                _s.SetData(new byte[value.Length * 4 + 4]);

                for (int i = 0; i < value.Length; i++)
                    this[i] = value[i];
            }
        }

        public T this[int i]
        {
            get
            {
                if (i < 0 || i > Length)
                    throw new IndexOutOfRangeException();
                return _s.GetReference<T>(i * 4);
            }
            set
            {
                if (i < 0 || i > Length)
                    throw new IndexOutOfRangeException();
                _s.SetReference(i * 4, value);
            }
        }

        //TODO: Add and Remove Functions
    }


    /// <summary>
    /// array of object data
    /// Accessor for accesing structure data
    /// Extend for custom properties
    /// </summary>
    public class HSDArrayAccessor<T> : HSDAccessor where T : HSDAccessor
    {
        public int Length => _s.Length / Stride;

        public int Stride => Activator.CreateInstance<T>().TrimmedSize;
        
        public T[] Array
        {
            get
            {
                if(Stride == -1)
                {
                    throw new Exception("Accessor has incorrest size");
                }
                T[] t = new T[Length];
                for (int i = 0; i < Length; i++)
                    t[i] = this[i];
                return t;
            }
            set
            {
                if (_s == null)
                    _s = new HSDStruct(new byte[value.Length * Stride]);

                _s.SetData(new byte[value.Length * Stride]);

                for (int i = 0; i < value.Length; i++)
                    this[i] = value[i];
            }
        }

        public T this[int i]
        {
            get
            {
                if (i < 0 || i > Length)
                    throw new IndexOutOfRangeException();
                var t = Activator.CreateInstance<T>();
                t._s = _s.GetEmbededStruct(i * Stride, Stride);
                return t;
            }
            set
            {
                if (i < 0 || i > Length)
                    throw new IndexOutOfRangeException();
                _s.SetEmbededStruct(i * Stride, value._s);
            }
        }
    }
}
