using HSDRaw;
using System.Windows.Forms;
using System.Collections.Generic;
using HSDRawViewer.Tools;
using System.Globalization;
using System;
using Be.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.Melee
{
    public partial class SubActionPanel : Form
    {
        public byte[] Data
        {
            get => _data;
            internal set
            {
                _data = value;

                HexEditLock = true;

                if (hexbox.ByteProvider != null)
                {
                    hexbox.ByteProvider.DeleteBytes(0, hexbox.ByteProvider.Length);
                    hexbox.ByteProvider.InsertBytes(0, _data);
                }

                ReLoadData();

                HexEditLock = false;

                hexbox.Invalidate();
            }
        }
        private byte[] _data;

        public HSDStruct Reference = null;

        private ComboBox PointerBox;

        private List<SubactionEditor.Action> AllActions;

        private SubactionGroup SubactionGroup = SubactionGroup.Fighter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AllActions"></param>
        public SubActionPanel(List<SubactionEditor.Action> AllActions)
        {
            this.AllActions = AllActions;

            InitializeComponent();
            
            PointerBox = new ComboBox();
            PointerBox.Dock = DockStyle.Fill;
            PointerBox.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (var s in AllActions)
                PointerBox.Items.Add(s);

            PointerBox.SelectedIndexChanged += (sender, args) =>
            {
                Reference = (PointerBox.SelectedItem as SubactionEditor.Action)._struct;
            };

            hexbox.ByteProvider = new DynamicByteProvider(new byte[0]);
            hexbox.ByteProvider.Changed += (sender, args) =>
            {
                if (HexEditLock)
                    return;

                if (hexbox.ByteProvider.Length == Data.Length)
                {
                    var newdata = new byte[Data.Length];

                    for (int i = 0; i < Data.Length; i++)
                        newdata[i] = hexbox.ByteProvider.ReadByte(i);

                    Data = newdata;
                }
                else
                {
                    HexEditLock = true;
                    hexbox.ByteProvider.DeleteBytes(0, hexbox.ByteProvider.Length);
                    hexbox.ByteProvider.InsertBytes(0, Data);
                    HexEditLock = false;
                }

            };
        }

        private bool HexEditLock = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="reference"></param>
        /// <param name="group"></param>
        public void LoadData(byte[] b, HSDStruct reference, SubactionGroup group)
        {
            SubactionGroup = group;

            comboBox1.Items.Clear();
            foreach (var v in SubactionManager.GetGroup(SubactionGroup))
                comboBox1.Items.Add(v.Name);

            Reference = reference;
            PointerBox.SelectedItem = AllActions.Find(e => e._struct == Reference);

            var sa = SubactionManager.GetSubaction(b[0], SubactionGroup);
            comboBox1.SelectedItem = sa.Name;
            
            Data = b;

            CenterToScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReLoadData()
        {
            var sa = SubactionManager.GetSubaction(Data[0], SubactionGroup);

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
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReadjustHeight()
        {
            Height = panel1.Controls.Count * 24 + 140;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
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
                    editor.Updated += Updated;
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
                    editor.Updated += Updated;
                    editor.SetBitSize(p.BitCount);
                    group.Controls.Add(editor);

                    group.Controls.Add(new Label() { Text = "0x", Dock = DockStyle.Left });
                }
                else
                if (p.HasEnums)
                {
                    SAEnumEditor editor = new SAEnumEditor();
                    editor.Updated += Updated;
                    editor.SetEnums(p.Enums);
                    group.Controls.Add(editor);
                }
                else
                {
                    if(p.Signed)
                    {
                        SAIntEditor editor = new SAIntEditor();
                        editor.Updated += Updated;
                        editor.SetBitSize(p.BitCount);
                        group.Controls.Add(editor);
                    }
                    else
                    {
                        SAUIntEditor editor = new SAUIntEditor();
                        editor.Updated += Updated;
                        editor.SetBitSize(p.BitCount);
                        group.Controls.Add(editor);
                    }
                }

                group.Controls.Add(new Label() { Text = p.Name + ":", Dock = DockStyle.Left, Width = 200 });

                panel1.Controls.Add(group);
            }

            ReadjustHeight();

            CompileAction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Updated(object sender, EventArgs args)
        {
            CompileAction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void CompileAction()
        {
            var sa = SubactionManager.GetGroup(SubactionGroup)[comboBox1.SelectedIndex];
            
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
            
            Data = sa.Compile(values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            CreateParamEditor(SubactionManager.GetSubaction(comboBox1.SelectedItem as string, SubactionGroup));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            CompileAction();
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public interface SubactionValueEditor
    {
        event EventHandler Updated;

        void SetBitSize(int bitCount);

        void SetValue(int value);

        long GetValue();
    }

    // Int Editor
    public class SAUIntEditor : NumericUpDown, SubactionValueEditor
    {
        public event EventHandler Updated;

        public SAUIntEditor()
        {
            Dock = DockStyle.Fill;
            ValueChanged += (sender, args) =>
            {
                if (Updated != null)
                    Updated.Invoke(this, EventArgs.Empty);
            };
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

            ValueChanged += (sender, args) =>
            {
                if (Updated != null)
                    Updated.Invoke(this, EventArgs.Empty);
            };
        }

        public event EventHandler Updated;

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

        public event EventHandler Updated;

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
                    if (Updated != null)
                        Updated.Invoke(this, EventArgs.Empty);
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

        public void SetFloatValue(float value)
        {
            FloatValue = value;
        }

        public float GetFloatValue()
        {
            return FloatValue;
        }

        public long GetValue()
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(FloatValue), 0);
        }
    }


    // Enum Editor
    public class SAEnumEditor : ComboBox, SubactionValueEditor
    {
        public event EventHandler Updated;

        public SAEnumEditor()
        {
            Dock = DockStyle.Fill;

            DropDownStyle = ComboBoxStyle.DropDownList;

            SelectedValueChanged += (sender, args) =>
            {
                if (Updated != null)
                    Updated.Invoke(this, EventArgs.Empty);
            };
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

        public event EventHandler Updated;
        
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
                    if (Updated != null)
                        Updated.Invoke(this, EventArgs.Empty);
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
