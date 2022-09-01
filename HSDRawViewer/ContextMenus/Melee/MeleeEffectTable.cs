using HSDRaw.Melee.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class MeleeEffectTable : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_EffectTable) };
        
        public MeleeEffectTable() : base()
        {
            ToolStripMenuItem AddNewEffect = new ToolStripMenuItem("Add new Effect");
            AddNewEffect.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_EffectTable root)
                {
                    var f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);
                    if(f != null)
                    {
                        var file = new HSDRaw.HSDRawFile(f);
                        if(file.Roots[0].Data._s.Length == 0x14)
                        {
                            var mod = root.Models;
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
