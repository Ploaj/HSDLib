using System;
using System.Windows.Forms;
using HSDRaw;
using HSDRaw.Tools.Melee;
using System.Collections.Generic;
using HSDRaw.Melee.Pl;

namespace HSDRawViewer.GUI
{
    public partial class SubactionEditor : UserControl
    {
        private DataNode SubactionNode { get; set; }

        private List<string> Scripts = new List<string>();

        private SBM_SubActionTable Subactions
        {
            get
            {
                if (SubactionNode == null)
                    return null;
                return SubactionNode.Accessor as SBM_SubActionTable;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SubactionEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        public void SetSubactionAccessor(DataNode subactionNode)
        {
            SubactionNode = subactionNode;

            Decompile();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Decompile()
        {
            comboBox1.Items.Clear();

            ActionDecompiler decompiler = new ActionDecompiler();

            var sa = Subactions.Subactions;

            for (int i = 0; i < sa.Length; i++)
            {
                comboBox1.Items.Add(sa[i].Name == null ? "Subaction_" + i.ToString("X3") : sa[i].Name);

                Scripts.Add(decompiler.Decompile("Func_" + i.ToString("X3"), sa[i].SubAction));
                
                /* g.flags = subaction.Flags;
                g.off = subaction.AnimationOffset;
                g.size = subaction.AnimationSize; */

                //Console.WriteLine(i + " " + subaction.Name + " " + subaction.SubAction._s.References.Count);
            }

            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Compile()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = Scripts[comboBox1.SelectedIndex];
        }
    }
}
