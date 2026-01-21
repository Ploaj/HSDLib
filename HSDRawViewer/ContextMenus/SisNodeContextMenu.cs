using HSDRaw.Melee;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

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
            ToolStripMenuItem export = new("Export to Text");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is SBM_SISData root)
                {
                    string file = Tools.FileIO.SaveFile("Text (*.txt)|*.txt");

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
