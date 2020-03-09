﻿using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;

namespace HSDRaw
{
    /// <summary>
    /// Accessor for accesing structure data
    /// Extent for custom properties
    /// </summary>
    public class HSDAccessor
    {
        public HSDStruct _s;

        [Browsable(false)]
        public virtual int TrimmedSize { get; } = -1;

        public HSDAccessor()
        {
            New();
        }

        public virtual void New()
        {
            if (TrimmedSize != -1)
            {
                _s = new HSDStruct(new byte[TrimmedSize]);
            }
            else
                _s = new HSDStruct(new byte[0]);
        }

        /// <summary>
        /// Returns a deep clone of given accessor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static T DeepClone<T>(HSDAccessor a) where T : HSDAccessor
        {
            var clone = ((HSDAccessor)Activator.CreateInstance<T>());
            clone._s = a._s.DeepClone();
            return (T)clone;
        }

        public static bool operator ==(HSDAccessor obj1, HSDAccessor obj2)
        {
            if ((object)obj1 == null && (object)obj2 == null)
                return true;
            if ((object)obj1 == null || (object)obj2 == null)
                return false;
            return obj1.Equals(obj2);
        }

        public static bool operator !=(HSDAccessor obj1, HSDAccessor obj2)
        {
            return !(obj1 == obj2);
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
    /// Accessor that implements a tree containing a next and child node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HSDTreeAccessor<T> : HSDListAccessor<T> where T : HSDTreeAccessor<T>
    {
        public virtual T Child { get; set; }

        [Browsable(false)]
        public T[] Children
        {
            get
            {
                if (Child != null)
                    return Child.List.ToArray();
                return new T[0];
            }
        }

        [Browsable(false)]
        public List<T> BreathFirstList
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void AddChild(T t)
        {
            if (Child == null)
                Child = t;
            else
                Child.Add(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveChildAt(int index)
        {
            Console.WriteLine(index + " " + Children.Length);

            if (index > Children.Length)
                return;

            var prev = Children[index - 1];
            var next = Children[index].Next;

            if (index == 0)
                Child = next;
            else
                prev.Next = next; ;
        }
    }

    /// <summary>
    /// Accessor that can be accessed like a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HSDListAccessor<T> : HSDAccessor where T : HSDListAccessor<T>
    {
        public virtual T Next { get; set; }

        [Browsable(false)]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public void Add(T t)
        {
            if(Next == null)
            {
                Next = t;
            }
            else
            {
                Next.Add(t);
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
        public int Length => Math.Max(((_s.Length / 4) - 1), 0);

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
                _s.References.Clear();
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

        /// <summary>
        /// Sets value at index to given value
        /// Array is resized to fit index if it is smaller
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set(int index, T value)
        {
            if ((index + 1) * 4 >= _s.Length)
                _s.Resize((index + 2) * 4);

            this[index] = value;
        }

        //TODO: Add and Remove Functions

        public void Remove(T value)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.Remove(value);
            Array = arr.ToArray();
        }

        public void Add(T value)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.Add(value);
            Array = arr.ToArray();
        }
    }


    /// <summary>
    /// Null terminated pointer list
    /// Accessor for accesing structure data
    /// Extend for custom properties
    /// </summary>
    public class HSDFixedLengthPointerArrayAccessor<T> : HSDAccessor where T : HSDAccessor
    {
        public int Length => ((_s.Length / 4));

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
                    _s = new HSDStruct(new byte[value.Length * 4]);
                _s.References.Clear();
                _s.SetData(new byte[value.Length * 4]);

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

        /// <summary>
        /// Sets value at index to given value
        /// Array is resized to fit index if it is smaller
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set(int index, T value)
        {
            if ((index) * 4 >= _s.Length)
                _s.Resize((index + 1) * 4);

            this[index] = value;
        }

        //TODO: Add and Remove Functions

        public void Remove(T value)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.Remove(value);
            Array = arr.ToArray();
        }

        public void Add(T value)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.Add(value);
            Array = arr.ToArray();
        }
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

                _s.References.Clear();
                _s.SetData(new byte[value.Length * Stride]);

                for (int i = 0; i < value.Length; i++)
                    this[i] = value[i];

                // add dummy data if empty
                if (_s.Length == 0) 
                    _s.Resize(4);
            }
        }

        public T this[int i]
        {
            get
            {
                if (i < 0 || i > Length)
                    throw new IndexOutOfRangeException();
                var t = Activator.CreateInstance<T>();
                t._s = _s.GetEmbeddedStruct(i * Stride, Stride);
                return t;
            }
            set
            {
                if (i < 0 || i > Length)
                    throw new IndexOutOfRangeException();
                _s.SetEmbededStruct(i * Stride, value._s);
            }
        }

        /// <summary>
        /// Sets value at index to given value
        /// Array is resized to fit index if it is smaller
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void Set(int index, T value)
        {
            if((index + 1) * Stride > _s.Length)
                _s.Resize((index + 1) * Stride);

            this[index] = value;
        }

        public void RemoveAt(int index)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.RemoveAt(index);
            Array = arr.ToArray();
        }

        public void Remove(T value)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.Remove(value);
            Array = arr.ToArray();
        }

        public void Add(T value)
        {
            // temp; slow
            var arr = Array.ToList();
            arr.Add(value);
            Array = arr.ToArray();
        }
    }
}
