using HSDRaw;
using HSDRaw.Melee.Cmd;
using HSDRaw.Melee.Pl;
using HSDRaw.Tools.Melee;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public partial class SubactionEditor
    {
        public class Action
        {
            public HSDStruct _struct;

            private string DisplayText;

            [Category("Animation"), DisplayName("Figatree Symbol")]
            public string Symbol
            {
                get => _symbol;
                set
                {
                    _symbol = value;

                    if (!string.IsNullOrEmpty(_symbol))
                        DisplayText = Regex.Replace(_symbol.Replace("_figatree", ""), @"Ply.*_Share_ACTION_", "");
                }
            }

            private string _symbol;

            public bool Subroutine = false;

            public int Index;

            public int AnimOffset;

            public int AnimSize;

            public uint Flags;

            [Category("Display Flags"), DisplayName("Flags")]
            public string BitFlags { get => Flags.ToString("X"); set { uint v = Flags; uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.CurrentCulture, out v); Flags = v; } }

            [Category("Flags"), DisplayName("Utilize animation-induced physics")]
            public bool AnimInducedPhysics { get => (Flags & 0x80000000) != 0; set => Flags = (uint)((Flags & ~0x80000000) | 0x80000000); }

            [Category("Flags"), DisplayName("Loop Animation")]
            public bool LoopAnimation { get => (Flags & 0x40000000) != 0; set => Flags = (uint)((Flags & ~0x40000000) | 0x40000000); }

            [Category("Flags"), DisplayName("Unknown")]
            public bool Unknown { get => (Flags & 0x20000000) != 0; set => Flags = (uint)((Flags & ~0x20000000) | 0x20000000); }

            [Category("Flags"), DisplayName("Unknown Flag")]
            public bool UnknownFlag { get => (Flags & 0x10000000) != 0; set => Flags = (uint)((Flags & ~0x10000000) | 0x10000000); }

            [Category("Flags"), DisplayName("Disable Dynamics")]
            public bool DisableDynamics { get => (Flags & 0x08000000) != 0; set => Flags = (uint)((Flags & ~0x08000000) | 0x08000000); }

            [Category("Flags"), DisplayName("Unknown TransN Update")]
            public bool TransNUpdate { get => (Flags & 0x04000000) != 0; set => Flags = (uint)((Flags & ~0x04000000) | 0x04000000); }

            [Category("Flags"), DisplayName("TransN Affected by Model Scale")]
            public bool AffectModelScale { get => (Flags & 0x02000000) != 0; set => Flags = (uint)((Flags & ~0x02000000) | 0x02000000); }

            [Category("Flags"), DisplayName("Additional Bone Value")]
            public uint AdditionalBone { get => (Flags & 0x003FFE00) >> 9; set => Flags = (uint)((Flags & ~0x003FFE00) | ((value << 9) & 0x003FFE00)); }

            [Category("Flags"), DisplayName("Disable Blend on Bone Index")]
            public uint BoneIndex { get => (Flags & 0x1C0) >> 7; set => Flags = (uint)(Flags & ~0x1C0) | ((value << 7) & 0x1C0); }

            [Category("Flags"), DisplayName("Character ID")]
            public uint CharIDCheck { get => Flags & 0x3F; set => Flags = (Flags & 0xFFFFFFC0) | (value & 0x3F); }

            public override string ToString()
            {
                return DisplayText == null ? (Subroutine ? "Subroutine_" : "Function_") + Index : DisplayText;
            }
        }

        [Serializable]
        public class SubActionScript
        {
            public byte[] data;

            public HSDStruct Reference;

            private SubactionGroup SubactionGroup = SubactionGroup.Fighter;

            public SubActionScript(SubactionGroup SubactionGroup)
            {
                this.SubactionGroup = SubactionGroup;
            }

            public SubactionGroup GetGroup()
            {
                return SubactionGroup;
            }

            public string Name
            {
                get
                {
                    return SubactionDesc.Name;
                }
            }

            public int CodeID
            {
                get
                {
                    Bitreader r = new Bitreader(data);

                    return (byte)r.Read(6);
                }
            }

            public Subaction SubactionDesc
            {
                get
                {
                    Bitreader r = new Bitreader(data);

                    return SubactionManager.GetSubaction((byte)r.Read(8), SubactionGroup);
                }
            }

            public IEnumerable<string> GetParamsAsString(SubactionEditor editor)
            {
                var sa = SubactionManager.GetSubaction(data[0], SubactionGroup);

                StringBuilder sb = new StringBuilder();

                var dparams = sa.GetParameters(data);

                for (int i = 0; i < sa.Parameters.Length; i++)
                {
                    var param = sa.Parameters[i];

                    if (param.Name.Contains("None"))
                        continue;

                    var value = param.IsPointer ? 0 : dparams[i];

                    if (param.HasEnums && value < param.Enums.Length)
                        yield return (param.Name +
                            " : " +
                            param.Enums[value]);
                    else
                    if (param.IsPointer)
                        if (editor != null && editor.AllActions.Find(e => e._struct == Reference) != null)
                            yield return ("&" + editor.AllActions.Find(e => e._struct == Reference).ToString());
                        else
                            yield return ("POINTER->(Edit To View)");
                    else
                    if (param.IsFloat)
                        yield return (param.Name +
                            " : " +
                            BitConverter.ToSingle(BitConverter.GetBytes(value), 0));
                    else
                        yield return (param.Name +
                            " : " +
                            (param.Hex ? "0x" + value.ToString("X") : value.ToString()));
                }
            }

            public SubActionScript Clone()
            {
                return new SubActionScript(SubactionGroup)
                {
                    data = (byte[])data.Clone(),
                    Reference = Reference
                };
            }

            public override string ToString()
            {
                return Name + "(" + string.Join(", ", GetParamsAsString(null)) + ")";
            }

            public string Serialize(SubactionEditor editor)
            {
                return Name + "(" + string.Join(", ", GetParamsAsString(editor)) + ")";
            }

            public static void Deserialize(string script)
            {
                // TODO:
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly List<Action> AllActions = new List<Action>();

        /// <summary>
        /// 
        /// </summary>
        public int ActionCount
        {
            get
            {
                int index = 0;
                foreach (var v in AllActions)
                {
                    if (v.Subroutine)
                        break;
                    index++;
                }
                return index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Action SelectedAction
        {
            get
            {
                if (actionList.SelectedItem is Action a)
                    return a;
                else
                    return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Subactions"></param>
        private void LoadActions(SBM_FighterAction[] Subactions)
        {
            HashSet<HSDStruct> aHash = new HashSet<HSDStruct>();
            Queue<HSDStruct> extra = new Queue<HSDStruct>();

            int Index = -1;
            foreach (var v in Subactions)
            {
                Index++;

                if (v.SubAction == null)
                    v.SubAction = new SBM_FighterSubactionData();

                if (!aHash.Contains(v.SubAction._s))
                    aHash.Add(v.SubAction._s);

                AllActions.Add(new Action()
                {
                    _struct = v.SubAction._s,
                    AnimOffset = v.AnimationOffset,
                    AnimSize = v.AnimationSize,
                    Flags = v.Flags,
                    Symbol = v.Name,
                });

                foreach (var c in v.SubAction._s.References)
                {
                    if (!aHash.Contains(c.Value))
                    {
                        extra.Enqueue(c.Value);
                    }
                }

            }

            Index = 0;
            while (extra.Count > 0)
            {
                var v = extra.Dequeue();
                if (!aHash.Contains(v))
                {
                    aHash.Add(v);
                    AllActions.Add(new Action()
                    {
                        _struct = v,
                        Subroutine = true
                    });
                }
                foreach (var r in v.References)
                    if (!aHash.Contains(r.Value))
                        extra.Enqueue(r.Value);
                Index++;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public List<SubActionScript> GetScripts(Action script)
        {
            // get subaction data
            var data = script._struct.GetData();
            List<SubActionScript> scripts = new List<SubActionScript>();

            // process data
            for (int i = 0; i < data.Length;)
            {
                // get subaction
                var sa = SubactionManager.GetSubaction((byte)(data[i]), SubactionGroup);

                // create new script node
                var sas = new SubActionScript(SubactionGroup);

                // store any pointers within this subaction
                foreach (var r in script._struct.References)
                {
                    if (r.Key >= i && r.Key < i + sa.ByteSize)
                        if (sas.Reference != null)
                            throw new NotSupportedException("Multiple References not supported");
                        else
                            sas.Reference = r.Value;
                }

                // copy subaction data to script node
                var sub = new byte[sa.ByteSize];

                if (i + sub.Length > data.Length)
                    break;

                for (int j = 0; j < sub.Length; j++)
                    sub[j] = data[i + j];

                i += sa.ByteSize;

                sas.data = sub;

                // add new script node
                scripts.Add(sas);

                // if end of script then stop reading
                if (sa.Code == 0)
                    break;
            }

            return scripts;
        }


        /// <summary>
        /// 
        /// </summary>
        private void SaveAllActionChanges()
        {
            for (int i = 0; i < AllActions.Count; i++)
                SaveActionChanges(i, false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveSelectedActionChanges()
        {
            int index = actionList.SelectedIndex;
            if (index != -1)
                SaveActionChanges(index, true, true);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveActionChanges(int index, bool recompile, bool enableUndo)
        {
            var a = AllActions[index];
            AddActionToUndo();

            if (!a.Subroutine)
            {
                var ftcmd = new SBM_FighterAction();
                ftcmd._s = _node.Accessor._s.GetEmbeddedStruct(0x18 * index, ftcmd.TrimmedSize);

                ftcmd.Name = a.Symbol;
                ftcmd.AnimationOffset = a.AnimOffset;
                ftcmd.AnimationSize = a.AnimSize;
                ftcmd.Flags = a.Flags;
                if (a._struct == null || a._struct.Length == 0)
                    ftcmd.SubAction = null;
                else
                    ftcmd.SubAction = new SBM_FighterSubactionData() { _s = a._struct };

                if (_node.Accessor._s.Length <= 0x18 * index + 0x18)
                    _node.Accessor._s.Resize(0x18 * index + 0x18);

                _node.Accessor._s.SetEmbededStruct(0x18 * index, ftcmd._s);
            }

            // compile subaction
            if(recompile)
            {
                a._struct.References.Clear();
                List<byte> scriptData = new List<byte>();
                foreach (SubActionScript scr in subActionList.Items)
                {
                    // TODO: are all references in this position?
                    if (scr.Reference != null)
                    {
                        a._struct.References.Add(scriptData.Count + 4, scr.Reference);
                    }
                    scriptData.AddRange(scr.data);
                }

                // update struct
                a._struct.SetData(scriptData.ToArray());

                SubactionProcess.SetStruct(a._struct, SubactionGroup);
            }

            UpdateFrameTips();
        }
    }
}
