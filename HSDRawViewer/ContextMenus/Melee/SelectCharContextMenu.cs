using HSDRaw.Melee.Mn;
using System;

namespace HSDRawViewer.ContextMenus
{
    public class SelectCharContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_SelectChrDataTable) };

        /// <summary>
        /// TODO: incomplete
        /// </summary>
        public SelectCharContextMenu() : base()
        {
            /*MenuItem CreateGeckoCode = new MenuItem("Create JOBJPosition Gecko Code for v1.02 NTSC");
            CreateGeckoCode.Click += (sender, args) =>
            {
                Console.WriteLine("Click");
                if (MainForm.SelectedDataNode.Accessor is SBM_SelectChrDataTable root)
                {
                    Console.WriteLine("Generating");
                    foreach (var v in root.MenuModel.Children[1].Children)
                    {
                        Console.WriteLine(v.TX + " " + v.TY + " " + v.TZ);
                    }
                }
            };
            MenuItems.Add(CreateGeckoCode);*/

        }
    }
}
