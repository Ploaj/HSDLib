using System.Collections.Generic;
using MeleeLib.DAT.Melee;

namespace MeleeLib.DAT
{
    /// <summary>
    /// DAT files used for Super Smash Bros. Melee
    /// Holds all data inside of .dat files.
    /// </summary>
    public class DATFile : DatNode
    {
        public byte[] DataBuffer;
        private List<DATRoot> _Roots = new List<DATRoot>();

        /// <summary>
        /// Read Only Array of the Roots
        /// </summary>
        public DATRoot[] Roots
        {
            get
            {
                return _Roots.ToArray();
            }
        }
        /// <summary>
        /// Adds a new Root to the DAT
        /// </summary>
        /// <param name="Root">The root to add</param>
        /// <returns></returns>
        public void AddRoot(DATRoot Root)
        {
            Root.ParentDAT = this;
            _Roots.Add(Root);
        }

        /// <summary>
        /// Gets all the DatRoots in the file including subroots
        /// </summary>
        /// <returns></returns>
        public List<DATRoot> GetAllSubRoots()
        {
            List<DATRoot> AllRoots = new List<DATRoot>();
            AllRoots.AddRange(Roots);
            foreach (DATRoot root in Roots)
            {
                if (root.Map_Head != null)
                {
                    foreach (Map_Object_Group g in root.Map_Head.NodeObjects)
                    {
                        if(g.BoneRoot != null)
                        AllRoots.Add(g.BoneRoot);
                    }
                    foreach (Map_Model_Group g in root.Map_Head.ModelObjects)
                    {
                        if (g.BoneRoot != null)
                            AllRoots.Add(g.BoneRoot);
                    }
                }
                foreach(DatNode n in root.ExtraNodes)
                {
                    AllRoots.AddRange(n.GetRoots());
                }
            }
            return AllRoots;
        }
    }
}
