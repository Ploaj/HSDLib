using HSDRaw.Melee.Ef;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class MeleeEffectTable : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_EffectTable) };

        public MeleeEffectTable() : base()
        {
            ToolStripMenuItem AddNewEffect = new("Add new Effect");
            AddNewEffect.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_EffectTable root)
                {
                    string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new(f);
                        if (file.Roots[0].Data._s.Length == 0x14)
                        {
                            SBM_EffectModel[] mod = root.Models;
                            Array.Resize(ref mod, mod.Length + 1);
                            mod[mod.Length - 1] = new SBM_EffectModel()
                            {
                                _s = file.Roots[0].Data._s
                            };
                            root.Models = mod;
                        }
                    }
                }
            };
            Items.Add(AddNewEffect);
        }
    }
}
