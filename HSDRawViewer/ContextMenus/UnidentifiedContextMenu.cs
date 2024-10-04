using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using System;
using System.Windows.Forms;
using HSDRawViewer.GUI.Dialog;
using System.Text;

namespace HSDRawViewer.ContextMenus
{
    public class UnidentifiedContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSDAccessor) };

        public UnidentifiedContextMenu() : base()
        {
            ToolStripMenuItem OpenAs = new ToolStripMenuItem("Open As");
            OpenAs.Click += (sender, args) =>
            {
                using (HSDTypeDialog d = new HSDTypeDialog())
                {
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        var type = Activator.CreateInstance(d.HSDAccessorType);
                        MainForm.Instance.SelectNode((HSDAccessor)type);
                    }
                }
            };
            Items.Add(OpenAs);

            ToolStripMenuItem OpenAsJOBJ = new ToolStripMenuItem("Open As JOBJ");
            OpenAsJOBJ.Click += (sender, args) => MainForm.Instance.SelectNode(new HSD_JOBJ());
            Items.Add(OpenAsJOBJ);

            ToolStripMenuItem OpenAsAJ = new ToolStripMenuItem("Open As AnimJoint");
            OpenAsAJ.Click += (sender, args) => MainForm.Instance.SelectNode(new HSD_AnimJoint());
            Items.Add(OpenAsAJ);

            ToolStripMenuItem OpenAsmah = new ToolStripMenuItem("Open As MatAnimJoint");
            OpenAsmah.Click += (sender, args) => MainForm.Instance.SelectNode(new HSD_MatAnimJoint());
            Items.Add(OpenAsmah);

#if DEBUG
            ToolStripMenuItem CopyAsm = new ToolStripMenuItem("Copy ASM to clipboard");
            CopyAsm.Click += (sender, args) =>
            {
                var d = MainForm.SelectedDataNode.Accessor._s;

                StringBuilder str = new StringBuilder();

                str.AppendLine("length: " + d.Length);
                for (int i = 0; i < d.Length; i += 4)
                {
                    str.AppendLine($".long 0x{d.GetInt32(i).ToString("X8")}");
                }

                Clipboard.SetText(str.ToString());
            };
            Items.Add(CopyAsm);
#endif
        }

    }
}
