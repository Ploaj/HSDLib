using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDLib
{
    public class IHSDNode
    {
        public IHSDNode()
        {

        }

        /// <summary>
        /// Returns all of the children IHSDNodes of this node
        /// Warning: slow
        /// </summary>
        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
        public IHSDNode[] Children
        {
            get
            {
                List<IHSDNode> children = new List<IHSDNode>();
                foreach (var prop in GetType().GetProperties())
                {
                    if (prop.PropertyType.IsSubclassOf(typeof(IHSDNode)) && prop.Name != "Next")
                    {
                        var value = prop.GetValue(this);
                        if(value != null)
                        {
                            var node = (IHSDNode)value;
                            children.Add(node);
                            children.AddRange(node.Siblings);
                        }
                    }
                }
                return children.ToArray();
            }
        }

        /// <summary>
        /// Returns all of the siblings for this now
        /// </summary>
        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
        private List<IHSDNode> Siblings
        {
            get
            {
                List<IHSDNode> children = new List<IHSDNode>();
                foreach (var prop in GetType().GetProperties())
                {
                    if (prop.PropertyType.IsSubclassOf(typeof(IHSDNode)) && prop.Name == "Next")
                    {
                        var value = prop.GetValue(this);
                        if (value != null)
                        {
                            children.Add((IHSDNode)value);
                            children.AddRange(((IHSDNode)value).Siblings);
                        }
                    }
                }
                return children;
            }
        }

        /// <summary>
        /// Reads properties from the HSDReader stream
        /// </summary>
        /// <param name="Reader"></param>
        public virtual void Open(HSDReader Reader)
        {
            foreach (var prop in GetType().GetProperties())
            {
                var attrs = (FieldData[])prop.GetCustomAttributes(typeof(FieldData), false);

                var type = prop.PropertyType;
                bool inLine = false;
                bool ignore = false;
                foreach (var attr in prop.GetCustomAttributes(false))
                {
                    if(attr is FieldData field)
                        type = field.Type;
                    if (attr is HSDInLineAttribute inline)
                        inLine = true;
                    if (attr is HSDParseIgnoreAttribute)
                        ignore = true;
                }
                if (ignore)
                    continue;
                if (type.IsSubclassOf(typeof(IHSDNode)))
                {
                    uint temp = Reader.Position() + 4;
                    IHSDNode field = (IHSDNode)Activator.CreateInstance(type);
                    dynamic changedObj = field;
                    uint Offset = Reader.Position();
                    if (!inLine) Offset = Reader.ReadUInt32();
                    prop.SetValue(this, Reader.ReadObject(Offset, changedObj));
                    if (!inLine) Reader.Seek(temp);
                }
                else
                    if (type == typeof(uint))
                    prop.SetValue(this, Reader.ReadUInt32());
                else
                    if (type == typeof(int))
                    prop.SetValue(this, Reader.ReadInt32());
                else
                    if (type == typeof(float))
                    prop.SetValue(this, Reader.ReadSingle());
                else
                    if (type == typeof(ushort))
                    prop.SetValue(this, Reader.ReadUInt16());
                else
                    if (type == typeof(short))
                    prop.SetValue(this, Reader.ReadInt16());
                else
                    if (type == typeof(bool))
                    prop.SetValue(this, Reader.ReadBoolean());
                else
                    if (type == typeof(string))
                    prop.SetValue(this, Reader.ReadString(Reader.ReadInt32()));
                else
                if (type == typeof(byte))
                {
                    if (type.IsEnum)
                    {
                        prop.SetValue(this, (Enum)Enum.ToObject(prop.PropertyType, Reader.ReadByte()));
                    }
                    else
                        prop.SetValue(this, Reader.ReadByte());
                }
                else if (type.IsEnum)
                {
                    prop.SetValue(this, (Enum)Enum.ToObject(type, Reader.ReadUInt32()));
                }
                else
                {
                    throw new InvalidOperationException("Failed to read " + type);
                }
            }
        }

        /// <summary>
        /// Saves properties to the HSDWriter
        /// </summary>
        /// <param name="Writer"></param>
        public virtual void Save(HSDWriter Writer)
        {
            // Write child node data first
            foreach (var prop in GetType().GetProperties().Reverse())
            {
                var type = prop.PropertyType;
                var inLine = false;
                var ignore = false;
                foreach (var attr in prop.GetCustomAttributes(false))
                {
                    if (attr is FieldData data)
                        type = data.Type;
                    if (attr is HSDParseIgnoreAttribute)
                        ignore = true;
                    if (attr is HSDInLineAttribute)
                        inLine = true;
                }
                if (ignore)
                    continue;
                if (type.IsSubclassOf(typeof(IHSDNode)))
                {
                    if ((!inLine) && prop.GetValue(this) != null)
                    {
                        Writer.WriteObject(((IHSDNode)prop.GetValue(this)));
                    }
                }
            }

            // Write this objects attributes
            Writer.AddObject(this);
            foreach (var prop in GetType().GetProperties())
            {
                var type = prop.PropertyType;
                var inLine = false;
                var ignore = false;
                foreach (var attr in prop.GetCustomAttributes(false))
                {
                    if (attr is FieldData data)
                        type = data.Type;
                    if (attr is HSDParseIgnoreAttribute)
                        ignore = true;
                    if (attr is HSDInLineAttribute)
                        inLine = true;
                }
                if (ignore)
                    continue;
                if (type.IsSubclassOf(typeof(IHSDNode)))
                {
                    if (inLine)
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
                if (type == typeof(uint))
                    Writer.Write((uint)prop.GetValue(this));
                else
                if (type == typeof(int))
                    Writer.Write((int)prop.GetValue(this));
                else
                if (type == typeof(float))
                    Writer.Write((float)prop.GetValue(this));
                else
                if (type == typeof(ushort))
                    Writer.Write((ushort)prop.GetValue(this));
                else
                if (type == typeof(short))
                    Writer.Write((short)prop.GetValue(this));
                else
                if (type == typeof(bool))
                    Writer.Write((bool)prop.GetValue(this));
                else
                if (type == typeof(string))
                    if ((string)prop.GetValue(this) != "")
                        Writer.WritePointer(prop.GetValue(this));
                    else
                        Writer.Write(0);
                else
                if (type == typeof(byte))
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
                else if (type.IsEnum)
                {
                    Writer.Write((int)prop.GetValue(this));
                }
                else
                {
                    throw new Exception("Failed to write " + type);
                }
            }

        }
        
        /// <summary>
        /// Gets a list of nodes of a given type (including sub nodes and children's now)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAllOfType<T>()
        {
            List<T> List = new List<T>();

            foreach (var prop in GetType().GetProperties())
            {
                var type = prop.PropertyType;
                var inLine = false;
                var ignore = false;
                foreach (var attr in prop.GetCustomAttributes(false))
                {
                    if (attr is FieldData data)
                        type = data.Type;
                    if (attr is HSDParseIgnoreAttribute)
                        ignore = true;
                    if (attr is HSDInLineAttribute)
                        inLine = true;
                }
                if (ignore)
                    continue;

                if (prop.GetValue(this) != null && prop.PropertyType == typeof(T))
                {
                    List.Add((T)prop.GetValue(this));
                }

                if (prop.GetValue(this) != null && type.IsSubclassOf(typeof(IHSDNode)))
                {
                    List.AddRange(((IHSDNode)prop.GetValue(this)).GetAllOfType<T>());
                }
            }

            return List;
        }
    }

    public class IHSDList<T> : IHSDNode where T : IHSDList<T>
    {
        /// <summary>
        /// Next object in the list, also know as the next sibling
        /// </summary>
        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
        public virtual T Next { get; set; }

        /// <summary>
        /// returns this list as an array to make easier to work with
        /// </summary>
        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
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
        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
        public virtual T Child { get; set; }

        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
        public T[] Children
        {
            get
            {
                if (Child != null)
                    return Child.List;
                return new T[0];
            }
        }

        [HSDParseIgnore()]
        [System.ComponentModel.Browsable(false)]
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

        /// <summary>
        /// adds new child to the tree
        /// </summary>
        /// <param name="Child"></param>
        public void AddChild(T Child)
        {
            if (this.Child == null)
                this.Child = Child;
            else
            {
                T child = this.Child;
                while (child.Next != null)
                {
                    child = child.Next;
                }
                child.Next = Child;
            }
        }
    }
}
