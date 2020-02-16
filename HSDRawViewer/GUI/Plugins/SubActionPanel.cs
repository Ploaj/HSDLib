using HSDRaw;
using System.Windows.Forms;
using System.Collections.Generic;
using HSDRawViewer.Tools;
using HSDRaw.Tools.Melee;
using System.Globalization;
using System;

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

            foreach(var v in SubactionManager.Subactions)
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
            
            var sa = SubactionManager.GetSubaction(Data[0]);

            comboBox1.SelectedItem = sa.Name;

            var paramd = sa.GetParameters(Data);

            for (int i = 0; i < sa.Parameters.Length; i++)
            {
                var p = sa.Parameters[i];

                if (p.Name.Contains("None"))
                    continue;

                if (p.IsPointer)
                    continue;

                var value = paramd[i];

                (panel1.Controls[sa.Parameters.Length - 1 - i].Controls[0] as SubactionValueEditor).SetValue(value);
            }
            
            CenterToScreen();
        }

        private void ReadjustHeight()
        {
            Height = panel1.Controls.Count * 24 + 120;
        }

        private void CreateParamEditor(Subaction action)
        {
            if (action == null)
                return;
            
            panel1.Controls.Clear();
            
            for(int i = action.Parameters.Length - 1; i >= 0; i--)
            {
                var p = action.Parameters[i];

                if (p.Name == "None")
                    continue;

                Panel group = new Panel();
                group.Dock = DockStyle.Top;
                group.Height = 24;

                if (p.IsFloat)
                {
                    SAFloatEditor editor = new SAFloatEditor();
                    group.Controls.Add(editor);
                }
                else
                if (p.IsPointer)
                {
                    if (Reference == null)
                        PointerBox.SelectedIndex = 0;
                    group.Controls.Add(PointerBox);
                }
                else
                if(p.Hex)
                {
                    SAHexEditor editor = new SAHexEditor();
                    editor.SetBitSize(p.BitCount);
                    group.Controls.Add(editor);

                    group.Controls.Add(new Label() { Text = "0x", Dock = DockStyle.Left });
                }
                else
                if (p.HasEnums)
                {
                    SAEnumEditor editor = new SAEnumEditor();
                    editor.SetEnums(p.Enums);
                    group.Controls.Add(editor);
                }
                else
                {
                    if(p.Signed)
                    {
                        SAIntEditor editor = new SAIntEditor();
                        editor.SetBitSize(p.BitCount);
                        group.Controls.Add(editor);
                    }
                    else
                    {
                        SAUIntEditor editor = new SAUIntEditor();
                        editor.SetBitSize(p.BitCount);
                        group.Controls.Add(editor);
                    }
                }

                group.Controls.Add(new Label() { Text = p.Name + ":", Dock = DockStyle.Left, Width = 200 });

                panel1.Controls.Add(group);
            }

            ReadjustHeight();
        }

        public byte[] CompileAction()
        {
            var sa = SubactionManager.Subactions[comboBox1.SelectedIndex];
            
            int[] values = new int[sa.Parameters.Length];
            for(int i = 0; i < sa.Parameters.Length; i++)
            {
                var bm = sa.Parameters[i];

                if (bm.Name.Contains("None") || bm.IsPointer)
                {
                    continue;
                }
                values[i] = (int)(panel1.Controls[sa.Parameters.Length - 1 - i].Controls[0] as SubactionValueEditor).GetValue();
                
            }
            
            return sa.Compile(values);
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            CreateParamEditor(SubactionManager.GetSubaction(comboBox1.SelectedItem as string));
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            Data = CompileAction();
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public interface SubactionValueEditor
    {
        void SetBitSize(int bitCount);

        void SetValue(int value);

        long GetValue();
    }

    // Int Editor
    public class SAUIntEditor : NumericUpDown, SubactionValueEditor
    {
        public SAUIntEditor()
        {
            Dock = DockStyle.Fill;
        }

        public void SetBitSize(int bitCount)
        {
            Maximum = ((1L << bitCount) - 1L);
            Minimum = 0;
        }
        
        public void SetValue(int value)
        {
            Value = value;
        }

        public long GetValue()
        {
            return (long)Value;
        }
    }


    // UInt Editor
    public class SAIntEditor : NumericUpDown, SubactionValueEditor
    {
        public SAIntEditor()
        {
            Dock = DockStyle.Fill;
        }

        public void SetBitSize(int bitCount)
        {
            var mask = (1L << (bitCount - 1)) - 1L;
            Maximum = mask;
            Minimum = -mask;
        }

        public void SetValue(int value)
        {
            Value = value;
        }

        public long GetValue()
        {
            return (long)Value;
        }
    }

    // Int Editor
    public class SAFloatEditor : TextBox, SubactionValueEditor
    {
        private float FloatValue = 0;

        public SAFloatEditor()
        {
            Dock = DockStyle.Fill;
            TextChanged += (sender, args) =>
            {
                // Filter text
                float val;
                if (float.TryParse(Text, out val))
                {
                    FloatValue = val;
                }
                else
                {
                    Text = FloatValue.ToString();
                }
            };
        }

        public void SetBitSize(int bitCount)
        {
            //Maximum = ((1L << bitCount) - 1L);
            //Minimum = 0;
            if (bitCount != 32)
                throw new Exception("Float Parameter is larger than 32 bits");
        }

        public void SetValue(int value)
        {
            Text = BitConverter.ToSingle(BitConverter.GetBytes(value), 0).ToString();
        }

        public long GetValue()
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(FloatValue), 0);
        }
    }


    // Enum Editor
    public class SAEnumEditor : ComboBox, SubactionValueEditor
    {
        public SAEnumEditor()
        {
            Dock = DockStyle.Fill;

            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public long GetValue()
        {
            return SelectedIndex;
        }

        public void SetBitSize(int bitCount)
        {
            // not needed
        }

        public void SetEnums(string[] enums)
        {
            Items.AddRange(enums);
        }

        public void SetValue(int value)
        {
            if (value >= Items.Count)
                value = Items.Count - 1;
            
            SelectedIndex = value;
        }
    }

    // Float Editor

    // Hex Editor
    public class SAHexEditor : TextBox, SubactionValueEditor
    {
        private uint IntValue = 0;
        private long MaxValue = 0;

        public SAHexEditor()
        {
            Dock = DockStyle.Fill;
            TextChanged += (sender, args) =>
            {
                // Filter text
                int val;
                if(int.TryParse(Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out val) && val <= MaxValue)
                {
                    IntValue = (uint)val;
                }
                else
                {
                    Text = IntValue.ToString("X");
                }
            };
        }

        public long GetValue()
        {
            return IntValue;
        }

        public void SetBitSize(int bitCount)
        {
            MaxValue = ((1L << bitCount) - 1L);
        }

        public void SetValue(int value)
        {
            Text = value.ToString("X");
        }
    }

}
