using HSDRaw;
using HSDRaw.Melee.Pl;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.Tools.Animation;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
#if DEBUG
    public class SubactionTableRename
    {
        public string Name { get; set; }
    }
#endif
    public class SubactionTableContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_FighterActionTable) };

        public SubactionTableContextMenu() : base()
        {
            ToolStripMenuItem Export = new("Import Subaction Data From File");
            Export.Click += (sender, args) =>
            {
                string f = Tools.FileIO.OpenFile(ApplicationSettings.HSDFileFilter);

                if (f != null && MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    SBM_FighterActionTable dataToImport = new();

                    dataToImport._s = new HSDRawFile(f).Roots[0].Data._s;

                    if (dataToImport.Count == table.Count)
                    {
                        SBM_FighterAction[] importTable = dataToImport.Commands;
                        SBM_FighterAction[] newTable = table.Commands;
                        for (int i = 0; i < table.Count; i++)
                            newTable[i].SubAction = importTable[i].SubAction;
                        table.Commands = newTable;
                    }
                }
            };
            Items.Add(Export);

#if DEBUG

            ToolStripMenuItem rename = new ToolStripMenuItem("Rename Symbols");
            rename.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    var prop = new SubactionTableRename();
                    using (PropertyDialog d = new PropertyDialog("Fighter Symbol Rename", prop))
                    {
                        if (d.ShowDialog() == DialogResult.OK)
                        {
                            var tables = table.Commands;

                            foreach (var c in tables)
                            {
                                if (c.SymbolName != null && !string.IsNullOrEmpty(c.SymbolName.Value))
                                {
                                    var sym = c.SymbolName.Value;

                                    var newsym = System.Text.RegularExpressions.Regex.Replace(sym, @"(?=Ply)(.)*(?=5K)", prop.Name); ;

                                    c.SymbolName.Value = newsym;
                                }
                            }

                            table.Commands = tables;
                        }
                    }
                }
            };
            Items.Add(rename);


            ToolStripMenuItem bonemap = new ToolStripMenuItem("Remap Bone IDs");
            bonemap.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    var source = Tools.FileIO.OpenFile("Current Bone INI (*.ini)|*.ini");
                    if (source == null)
                        return;
                    var target = Tools.FileIO.OpenFile("New Bone INI (*.ini)|*.ini");
                    if (target == null)
                        return;

                    var sini = new JointMap(source);
                    var tini = new JointMap(target);

                    var tables = table.Commands;

                    foreach (var c in tables)
                    {
                        var data = c.SubAction._s.GetData();
                        Tools.SubactionManager.EditSubactionData(
                            ref data,
                            (Tools.Subaction sa, ref int[] p) =>
                            {
                                // create gfx
                                if (sa.Code == 10 << 2)
                                    p[0] = tini.IndexOf(sini[p[0]]);
                                // create hitbox
                                if (sa.Code == 11 << 2)
                                    p[3] = tini.IndexOf(sini[p[3]]);
                                // set bone collision state
                                if (sa.Code == 28 << 2)
                                    p[0] = tini.IndexOf(sini[p[0]]);
                                // enable ragdoll
                                if (sa.Code == 50 << 2)
                                    p[0] = tini.IndexOf(sini[p[0]]);
                            },
                            Tools.SubactionGroup.Fighter);
                        c.SubAction._s.SetData(data);
                    }

                    table.Commands = tables;
                }
            };
            Items.Add(bonemap);


            ToolStripMenuItem soundid = new ToolStripMenuItem("MEX: Make Sound IDs Portable ");
            soundid.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    {
                        {
                            var tables = table.Commands;

                            foreach (var c in tables)
                            {
                                var data = c.SubAction._s.GetData();
                                Tools.SubactionManager.EditSubactionData(
                                    ref data, 
                                    (Tools.Subaction sa, ref int[] p) =>
                                    {
                                        if (sa.Code == 17 << 2 && p[2] > 10000)
                                            p[2] = (p[2] % 1000) + 5000;

                                        if (sa.Code == 54 << 2 && p[1] > 10000)
                                            p[1] = (p[1] % 1000) + 5000;

                                        if (sa.Code == 55 << 2 && p[1] > 10000)
                                            p[1] = (p[1] % 1000) + 5000;
                                    }, 
                                    Tools.SubactionGroup.Fighter);
                                c.SubAction._s.SetData(data);
                            }

                            table.Commands = tables;
                        }
                    }
                }
            };
            Items.Add(soundid);

            ToolStripMenuItem disabledyn = new ToolStripMenuItem("Disable All Dynamics");
            disabledyn.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    var tables = table.Commands;

                    foreach (var c in tables)
                    {
                        if (c.SymbolName != null &&
                            !c.SymbolName.Value.Contains("ThrowN"))
                            c.Flags |= 0x08000000;
                    }

                    table.Commands = tables;
                }
            };
            Items.Add(disabledyn);

            ToolStripMenuItem enabledyn = new ToolStripMenuItem("Enable All Dynamics");
            enabledyn.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_FighterActionTable table)
                {
                    var tables = table.Commands;

                    foreach (var c in tables)
                    {
                        if (c.SymbolName != null &&
                            !c.SymbolName.Value.Contains("ThrowN"))
                            c.Flags = (uint)(c.Flags & ~0x08000000);
                    }

                    table.Commands = tables;
                }
            };
            Items.Add(enabledyn);
#endif

        }
    }
}
