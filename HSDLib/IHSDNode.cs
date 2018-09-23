using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDLib.Common;

namespace HSDLib
{
    public class IHSDNode
    {
        public IHSDNode()
        {

        }

        public virtual void Open(HSDReader Reader)
        {
            foreach (var prop in GetType().GetProperties())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);
                foreach (var attr in attrs)
                {
                    if (attr.Type.IsSubclassOf(typeof(IHSDNode)))
                    {
                        uint temp = Reader.Position() + 4;
                        IHSDNode field = (IHSDNode)Activator.CreateInstance(attr.Type);
                        dynamic changedObj = field;
                        uint Offset = Reader.Position();
                        if (!attr.InLine) Offset = Reader.ReadUInt32();
                        //Console.WriteLine(attr.Type + " " + Offset.ToString("X"));
                        prop.SetValue(this, Reader.ReadObject(Offset, changedObj));
                        if (!attr.InLine) Reader.Seek(temp);
                    }
                    else
                    if (attr.Type == typeof(uint))
                        prop.SetValue(this, Reader.ReadUInt32());
                    else
                    if (attr.Type == typeof(int))
                        prop.SetValue(this, Reader.ReadInt32());
                    else
                    if (attr.Type == typeof(float))
                        prop.SetValue(this, Reader.ReadSingle());
                    else
                    if (attr.Type == typeof(ushort))
                        prop.SetValue(this, Reader.ReadUInt16());
                    else
                    if (attr.Type == typeof(short))
                        prop.SetValue(this, Reader.ReadInt16());
                    else
                    if (attr.Type == typeof(bool))
                        prop.SetValue(this, Reader.ReadBoolean());
                    else
                    if (attr.Type == typeof(byte))
                    {
                        if (prop.PropertyType.IsEnum)
                        {
                            prop.SetValue(this, (Enum)Enum.ToObject(prop.PropertyType, Reader.ReadByte()));
                        }else
                            prop.SetValue(this, Reader.ReadByte());
                    }
                    else if (attr.Type.IsEnum)
                    {
                        prop.SetValue(this, (Enum)Enum.ToObject(attr.Type, Reader.ReadUInt32()));
                    }
                    else
                    {
                        throw new Exception("Failed to read " + attr.Type);
                    }
                }
            }
        }

        public virtual void Save(HSDWriter Writer)
        {
            // Write child node data first
            foreach (var prop in GetType().GetProperties().Reverse())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);
                foreach (var attr in attrs)
                {
                    if (attr.Type.IsSubclassOf(typeof(IHSDNode)))
                    {
                        if ((!attr.InLine) && prop.GetValue(this) != null)
                        {
                            Writer.WriteObject(((IHSDNode)prop.GetValue(this)));
                        }
                    }
                }
            }
            
            // Write this objects attributes
            Writer.AddObject(this);
            foreach (var prop in GetType().GetProperties())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);
                foreach (var attr in attrs)
                {
                    if (attr.Type.IsSubclassOf(typeof(IHSDNode)))
                    {
                        if (attr.InLine)
                        {
                            Writer.WriteObject(((IHSDNode)prop.GetValue(this)));
                        }
                        else
                        {
                            Writer.WritePointer(prop.GetValue(this));
                        }
                        /*uint temp = Reader.Position() + 4;
                        IHSDNode field = (IHSDNode)Activator.CreateInstance(attr.Type);
                        dynamic changedObj = field;
                        uint Offset = Reader.Position();
                        if (!attr.InLine) Offset = Reader.ReadUInt32();
                        //Console.WriteLine(attr.Type + " " + Offset.ToString("X"));
                        prop.SetValue(this, Reader.ReadObject(Offset, changedObj));
                        if (!attr.InLine) Reader.Seek(temp);*/
                    }
                    else
                    if (attr.Type == typeof(uint))
                        Writer.Write((uint)prop.GetValue(this));
                    else
                    if (attr.Type == typeof(int))
                        Writer.Write((int)prop.GetValue(this));
                    else
                    if (attr.Type == typeof(float))
                        Writer.Write((float)prop.GetValue(this));
                    else
                    if (attr.Type == typeof(ushort))
                        Writer.Write((ushort)prop.GetValue(this));
                    else
                    if (attr.Type == typeof(short))
                        Writer.Write((short)prop.GetValue(this));
                    else
                    if (attr.Type == typeof(bool))
                        Writer.Write((bool)prop.GetValue(this));
                    else
                    if (attr.Type == typeof(byte))
                    {
                        if (prop.PropertyType.IsEnum)
                        {
                            Writer.Write((byte)(int)prop.GetValue(this));
                        }
                        else
                        {
                            Writer.Write((byte)prop.GetValue(this));
                        }
                    }
                    else if (attr.Type.IsEnum)
                    {
                        Writer.Write((int)prop.GetValue(this));
                    }
                    else
                    {
                        throw new Exception("Failed to write " + attr.Type);
                    }
                }
            }
        }

        // for buffers
        public virtual void PreSave(HSDWriter Writer)
        {

        }

        //for textures
        public virtual void AfterSave(HSDWriter Writer)
        {

        }
    }

    public class IHSDList<T> : IHSDNode where T : IHSDList<T>
    {
        public virtual T Next { get; set; }

        public T[] List
        {
            get
            {
                List<T> list = new List<T>();
                T Current = (T)this;
                while(Current != null)
                {
                    list.Add(Current);
                    Current = Current.Next;
                }
                return list.ToArray();
            }
        }
    }

    public class IHSDTree<T> : IHSDList<T> where T : IHSDTree<T>
    {
        public virtual T Child { get; set; }

        public T[] Children
        {
            get
            {
                if (Child != null)
                    return Child.List;
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
}
