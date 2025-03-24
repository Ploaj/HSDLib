using HSDRaw.Common.Animation;
using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSDRaw.Melee;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HSDRawViewer.ContextMenus
{
    public class SisNodeContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(SBM_SISData) };

        public class SisJson
        {
            public string Text
            {
                get => TextCode;
                set => TextCode = value;
            }

            [JsonIgnore]
            private string TextCode;
        }

        public SisNodeContextMenu() : base()
        {
            ToolStripMenuItem export = new ToolStripMenuItem("Export to Text");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_SISData root)
                {
                    var file = Tools.FileIO.SaveFile("Text (*.txt)|*.txt");

                    if (file != null)
                    {
                        File.WriteAllText(file, JsonSerializer.Serialize(root.SISData.Select(e => new SisJson() { Text = e.TextCode }),
                            new JsonSerializerOptions()
                            {
                                WriteIndented = true
                            }));
                    }
                }
            };
            Items.Add(export);


        }
    }
}
