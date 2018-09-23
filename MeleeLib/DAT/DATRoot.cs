using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;
using MeleeLib.GCX;
using MeleeLib.DAT.Animation;
using MeleeLib.DAT.MatAnim;
using MeleeLib.DAT.Script;
using MeleeLib.DAT.Melee;

namespace MeleeLib.DAT
{
    public class DATRoot : DatNode
    {
        public DATFile ParentDAT;
        public List<DatJOBJ> Bones = new List<DatJOBJ>();

        public List<GXAttribGroup> Attributes
        {
            get
            {
                List<GXAttribGroup> Group = new List<GXAttribGroup>();
                foreach(DatDOBJ d in GetDataObjects())
                {
                    foreach(DatPolygon p in d.Polygons)
                    {
                        if (p.AttributeGroup == null) continue;
                        if (!Group.Contains(p.AttributeGroup))
                            Group.Add(p.AttributeGroup);
                    }
                }
                Group.Reverse();
                return Group;
            }
        }

        public List<DatAnimation> Animations = new List<DatAnimation>();

        public List<DatMatAnim> MatAnims = new List<DatMatAnim>();

        public List<DatFighterData> FighterData = new List<DatFighterData>();

        public List<DatNode> ExtraNodes = new List<DatNode>();

        //Melee
        public Map_Head Map_Head = null;

        public DATRoot()
        {
        }

        public List<byte[]> GetImageData()
        {
            List<byte[]> Data = new List<byte[]>();
            foreach(DatDOBJ d in GetDataObjects())
            {
                foreach(DatTexture t in d.Material.Textures)
                {
                    byte[] data = t.ImageData.Data;
                    if(!Data.Contains(data))
                    Data.Add(data);
                }
            }
            return Data;
        }

        public List<byte[]> GetMatAnimImageData()
        {
            List<byte[]> Data = new List<byte[]>();
            foreach (DatMatAnim a in GetMatAnimsinOrder())
            {
                foreach(DatMatAnimGroup g in a.Groups)
                {
                    if (g.InformationStruct != null)
                        foreach (DatMatAnimTextureData t in g.InformationStruct.Textures)
                        {
                            Data.Add(t.Data);
                        }
                    
                    foreach(DatMatAnimData d in g.TextureData)
                    {
                        if(d.Info != null)
                        foreach (DatMatAnimTextureData t in d.Info.Textures)
                        {
                            Data.Add(t.Data);
                        }
                        foreach (DatMatAnimTextureData t in d.Textures)
                        {
                            Data.Add(t.Data);
                        }
                    }

                }
            }
            return Data;
        }
        
        public GXAttribGroup GetAttribute(int id, DATReader d)
        {
            foreach (GXAttribGroup t in Attributes)
            {
                if (t is null) continue;
                if (t.ID == id)
                    return t;
            }
            GXAttribGroup AttrGroup = new GXAttribGroup();
            AttrGroup.ID = id;

            d.Seek(id);
            GXAttr att = new GXAttr();
            att.Deserialize(d, this);
            while (att.Name != GXAttribName.GX_VA_NULL)
            {
                AttrGroup.Add(att);
                att = new GXAttr();
                att.Deserialize(d, this);
            }
            return AttrGroup;
        }

        public DatJOBJ GetJOBJ(int ID)
        {
            Queue<DatJOBJ> Que = new Queue<DatJOBJ>();
            foreach(DatNode n in Bones)
            {
                if (n is DatJOBJ) Que.Enqueue((DatJOBJ)n);
            }

            while(Que.Count > 0)
            {
                DatJOBJ jobj = Que.Dequeue();
                if (jobj.ID == ID) return jobj;
                foreach (DatJOBJ j in jobj.GetChildren())
                    Que.Enqueue(j);
            }
            return null;
        }

        public int GetJOBJIndex(int ID)
        {
            int i = 0;
            foreach(DatJOBJ j in GetJOBJinOrder())
            {
                if(j.ID == ID)
                    return i;
                i++;
            }
            return -1;
        }
        
        public DatJOBJ[] GetJOBJinOrder()
        {
            /*Queue<DatJOBJ> Que = new Queue<DatJOBJ>();
            List<DatJOBJ> JOBJS = new List<DatJOBJ>();
            foreach (DatJOBJ n in Bones)
            {
                Que.Enqueue(n);
            }
            while (Que.Count > 0)
            {
                DatJOBJ jobj = Que.Dequeue();
                JOBJS.Add(jobj);
                foreach (DatJOBJ j in jobj.GetChildren())
                    Que.Enqueue(j);
            }
            return JOBJS.ToArray();*/
            return GetJOBJinOrderBreath();
        }
        
        private DatJOBJ[] GetJOBJinOrderBreath()
        {
            Queue<DatJOBJ> Que = new Queue<DatJOBJ>();
            foreach(DatJOBJ j in Bones)
                Enqueue(j, Que);
            List<DatJOBJ> JOBJS = new List<DatJOBJ>();
            while (Que.Count > 0)
                JOBJS.Add(Que.Dequeue());
            return JOBJS.ToArray();
        }

        private void Enqueue(DatJOBJ jobj, Queue<DatJOBJ> Que)
        {
            Que.Enqueue(jobj);
            foreach (DatJOBJ j in jobj.GetChildren())
                Enqueue(j, Que);
        }

        public DatMatAnim[] GetMatAnimsinOrder()
        {
            Queue<DatMatAnim> Que = new Queue<DatMatAnim>();
            List<DatMatAnim> JOBJS = new List<DatMatAnim>();
            foreach (DatMatAnim n in MatAnims)
            {
                Que.Enqueue(n);
            }
            int i = 0;
            while (Que.Count > 0)
            {
                DatMatAnim jobj = Que.Dequeue();
                JOBJS.Add(jobj);
                foreach (DatMatAnim j in jobj.GetChildren())
                    Que.Enqueue(j);
                i++;
            }
            return JOBJS.ToArray();
        }

        public DatDOBJ[] GetDataObjects()
        {
            List<DatDOBJ> dobs = new List<DatDOBJ>();

            foreach(DatJOBJ j in GetJOBJinOrder())
            {
                dobs.AddRange(j.DataObjects);
            }

            return dobs.ToArray();
        }
    }
}
