using HSDRawViewer.GUI.Plugins.Melee;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins
{
    public partial class ParticleGenEditorPanel : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private ParticleOpCode OpCode;

        private Control[] Editors;

        /// <summary>
        /// 
        /// </summary>
        public ParticleGenEditorPanel(ParticleOpCode opcode)
        {
            //
            InitializeComponent();

            // add particles
            cbOpCode.Items.AddRange(ParticleManager.GetDescriptors());

            // load op codes
            OpCode = opcode;

            //
            SelectCode(opcode.Code);

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectCode(byte code)
        {
            // select descriptor
            cbOpCode.SelectedIndex = cbOpCode.Items.IndexOf(ParticleManager.GetParticleDescriptor(code));
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateEditor()
        {
            if(cbOpCode.SelectedItem is ParticleManager.ParticleDescriptor des)
            {
                panel1.SuspendLayout();

                // clear controls
                foreach (Control c in panel1.Controls)
                    c.Dispose();

                panel1.Controls.Clear();

                if (des.ParamDesc == null)
                    return;

                var paramdesc = des.ParamDesc.ToCharArray();

                if (des.ParamDesc.Length != des.Params.Length)
                    throw new Exception("Param count mismatch");

                Editors = new Control[paramdesc.Length];

                for (int i = 0; i < paramdesc.Length; i++)
                {
                    var panel = new Panel();
                    panel.Width = 400;
                    panel.Height = 24;
                    panel.Dock = DockStyle.Top;

                    Control editcontrol = null;

                    switch(paramdesc[i])
                    {
                        case 'c':
                            {
                                var editor = new CheckBox() { Dock = DockStyle.Left };
                                editor.Checked = i < OpCode.Params.Length && OpCode.Params[i] is bool ? (bool)OpCode.Params[i] : false;
                                editcontrol = editor;
                            }
                            break;
                        case 'b':
                            {
                                var editor = new SAUIntEditor() { Dock = DockStyle.Left };
                                editor.SetBitSize(8);
                                editor.SetValue(i < OpCode.Params.Length && OpCode.Params[i] is byte ? (byte)OpCode.Params[i] : 0);
                                editcontrol = editor;
                            }
                            break;
                        case 's':
                            {
                                var editor = new SAUIntEditor() { Dock = DockStyle.Left };
                                editor.SetBitSize(16);
                                editor.SetValue(i < OpCode.Params.Length && OpCode.Params[i] is short ? (short)OpCode.Params[i] : 0);
                                editcontrol = editor;
                            }
                            break;
                        case 'f':
                            {
                                var editor = new SAFloatEditor() { Dock = DockStyle.Left };
                                editor.SetFloatValue(i < OpCode.Params.Length && OpCode.Params[i] is float ? (float)OpCode.Params[i] : 0);
                                editcontrol = editor;
                            }
                            break;
                    }

                    if(editcontrol != null)
                    {
                        panel.Controls.Add(editcontrol);
                        editcontrol.SendToBack();
                        Editors[i] = editcontrol;
                    }

                    panel.Controls.Add(new Label() { Text = des.Params[i], Dock = DockStyle.Left });

                    panel1.Controls.Add(panel);
                }

                panel1.ResumeLayout();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            // save code
            if (cbOpCode.SelectedItem is ParticleManager.ParticleDescriptor des)
            {
                OpCode.Code = des.Code;
                
                if (Editors == null)
                    return;

                OpCode.Params = new object[Editors.Length];

                if (des.ParamDesc == null)
                    return;

                var p = des.ParamDesc.ToCharArray();

                for (int i = 0; i < OpCode.Params.Length; i++)
                {
                    switch (p[i])
                    {
                        case 'c':
                            OpCode.Params[i] = ((CheckBox)Editors[i]).Checked;
                            break;
                        case 'b':
                            OpCode.Params[i] = (byte)((SubactionValueEditor)Editors[i]).GetValue();
                            break;
                        case 's':
                            OpCode.Params[i] = (short)((SubactionValueEditor)Editors[i]).GetValue();
                            break;
                        case 'f':
                            OpCode.Params[i] = ((SAFloatEditor)Editors[i]).GetFloatValue();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            Save();
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbOpCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateEditor();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ParticleOpCode
    {
        public byte Code;
        public object[] Params;

        public ParticleOpCode()
        {
            Code = 0xFE;
            Params = new object[0];
        }

        public override string ToString()
        {
            var des = ParticleManager.GetParticleDescriptor(Code);

            if (des == null)
                return $"{Code.ToString("X")} {string.Join(", ", Params)}";
            else
                return $"{des?.Name} {string.Join(", ", Params)}";
        }
    }
}
