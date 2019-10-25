using HSDRaw;
using HSDRaw.Tools.Melee;
using System.Windows.Forms;
using System.Collections.Generic;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class SubActionPanel : Form
    {
        public byte[] Data { get; internal set; }

        public HSDStruct Reference = null;

        private ComboBox PointerBox;

        private List<SubactionEditor.Action> AllActions;

        public SubActionPanel(List<SubactionEditor.Action> AllActions)
        {
            this.AllActions = AllActions;

            InitializeComponent();

            CenterToScreen();

            foreach(var v in ActionCommon.SubActions)
            {
                comboBox1.Items.Add(v.Name);
            }
            
            PointerBox = new ComboBox();
            PointerBox.Dock = DockStyle.Fill;
            PointerBox.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (var s in AllActions)
                PointerBox.Items.Add(s);

            PointerBox.SelectedIndexChanged += (sender, args) =>
            {
                Reference = (PointerBox.SelectedItem as SubactionEditor.Action)._struct;
            };
        }

        public void LoadData(byte[] b, HSDStruct reference)
        {
            Data = b;
            Reference = reference;

            PointerBox.SelectedItem = AllActions.Find(e => e._struct == Reference);

            Bitreader r = new Bitreader(Data);

            var sa = ActionCommon.GetMeleeCMDAction((byte)r.Read(6));

            comboBox1.SelectedItem = sa.Name;

            for (int i = 0; i < sa.BMap.Count; i++)
            {
                var p = sa.BMap[i];

                if (p.Name.Contains("None"))
                    continue;

                var value = r.Read(p.Count);

                if (p.Name.Contains("Pointer"))
                    continue;

                (panel1.Controls[sa.BMap.Count - 1 - i].Controls[0] as NumericUpDown).Value = value;
            }
        }

        private void CreateParamEditor(MeleeCMDAction action)
        {
            if (action == null)
                return;
            
            panel1.Controls.Clear();
            
            for(int i = action.BMap.Count - 1; i >= 0; i--)
            {
                var p = action.BMap[i];

                if (p.Name == "None")
                    continue;

                Panel group = new Panel();
                group.Dock = DockStyle.Top;
                group.Height = 24;

                NumericUpDown nud = new NumericUpDown();
                nud.Maximum = ((1L << p.Count) - 1);
                nud.Minimum = 0;
                nud.Dock = DockStyle.Fill;

                if (p.Name == "Pointer")
                {
                    if (Reference == null)
                        PointerBox.SelectedIndex = 0;
                    group.Controls.Add(PointerBox);
                }
                else
                    group.Controls.Add(nud);
                group.Controls.Add(new Label() { Text = p.Name + ":", Dock = DockStyle.Left, Width = 200 });

                panel1.Controls.Add(group);
            }
            
        }

        public byte[] CompileAction()
        {
            BitWriter w = new BitWriter();

            var sa = ActionCommon.SubActions[comboBox1.SelectedIndex];

            w.Write(sa.Command, 6);
            for(int i = 0; i < sa.BMap.Count; i++)
            {
                var bm = sa.BMap[i];

                if (bm.Name.Contains("None") || bm.Name.Contains("Pointer"))
                {
                    w.Write(0, bm.Count);
                    continue;
                }
                
                var value = (int)(panel1.Controls[sa.BMap.Count - 1 - i].Controls[0] as NumericUpDown).Value;

                w.Write(value, bm.Count);
            }

            if (sa.BMap.Count == 0)
                w.Write(0, 26);

            return w.Bytes.ToArray();
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            CreateParamEditor(ActionCommon.GetMeleeCMDAction(comboBox1.SelectedItem as string));
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            Data = CompileAction();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
