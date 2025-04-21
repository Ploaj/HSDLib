﻿using Be.Windows.Forms;
using HSDRaw;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI
{
    public partial class PropertyView : DockContent
    {
        public class PropertyPoke
        {
            public HSDAccessor accessor;
            public uint CurrentOffset = 0;

            [Category("Types")]
            public float Float
            {
                get
                {
                    if (accessor != null && CurrentOffset + 4 <= accessor._s.Length)
                        return accessor._s.GetFloat((int)CurrentOffset);
                    else
                        return 0;
                }
                set
                {
                    if (accessor != null && CurrentOffset + 4 <= accessor._s.Length)
                        accessor._s.SetFloat((int)CurrentOffset, value);
                }
            }

            [Category("Types")]
            public int Int
            {
                get
                {
                    if (accessor != null && CurrentOffset + 4 <= accessor._s.Length)
                        return accessor._s.GetInt32((int)CurrentOffset);
                    else
                        return 0;
                }
                set
                {
                    if (accessor != null && CurrentOffset + 4 <= accessor._s.Length)
                        accessor._s.SetInt32((int)CurrentOffset, value);
                }
            }

            [Category("Types")]
            public uint UInt
            {
                get => (uint)Int;
                set => Int = (int)value;
            }

            [Category("Types")]
            public short Short
            {
                get
                {
                    if (accessor != null && CurrentOffset + 4 <= accessor._s.Length)
                        return accessor._s.GetInt16((int)CurrentOffset);
                    else
                        return 0;
                }
                set
                {
                    if (accessor != null && CurrentOffset + 4 <= accessor._s.Length)
                        accessor._s.SetInt16((int)CurrentOffset, value);
                }
            }

            [Category("Types")]
            public ushort UShort
            {
                get => (ushort)Short;
                set => Short = (short)value;
            }

            [Category("Types")]
            public sbyte SByte
            {
                get => (sbyte)Byte;
                set => Byte = (byte)value;
            }

            [Category("Types")]
            public byte Byte
            {
                get
                {
                    if (accessor != null && CurrentOffset + 1 <= accessor._s.Length)
                        return accessor._s.GetByte((int)CurrentOffset);
                    else
                        return 0;
                }
                set
                {
                    if (accessor != null && CurrentOffset + 1 <= accessor._s.Length)
                        accessor._s.SetByte((int)CurrentOffset, value);
                }
            }
        }

        private readonly PropertyPoke Poker = new();

        private HSDAccessor accessor
        {
            get
            {
                return Poker.accessor;
            }
        }
        private DataNode Node;

        public PropertyView()
        {
            InitializeComponent();

            Text = "Property View";

            propertyGrid2.SelectedObject = Poker;

            structSize.LostFocus += structSize_TextChanged;

            hexbox.SelectionStartChanged += (sender, args) =>
            {
                selectedIndex = (int)hexbox.SelectionStart;
                offsetBox.Text = "0x" + hexbox.SelectionStart.ToString("X8");
            };

            hexbox.SelectionLengthChanged += (sender, args) =>
            {
                selectedLength = (int)hexbox.SelectionLength;
            };

            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                    MainForm.Instance.TryClose(this);
                }
            };

            offsetBox.Text = LastGoodOffset;
        }

        public void SetNode(DataNode node)
        {
            HSDAccessor accessor = node.Accessor;

            if (accessor == null)
                return;

            Node = node;

            Poker.accessor = accessor;
            propertyGrid1.SelectedObject = accessor;
            SetBytes();

            structSize.Text = "0x" + accessor._s.Length.ToString("X");

            if (this.accessor != accessor)
                offsetBox.Text = "0";
        }

        private int selectedLength = 0;
        private int selectedIndex = 0;

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (accessor != null)
            {
                Node?.NotifyChange();
                SetBytes();
            }
        }

        private void propertyGrid2_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (accessor != null)
            {
                Node?.NotifyChange();
                SetBytes();
            }
        }

        private void SetBytes()
        {
            hexbox.ByteProvider = new DynamicByteProvider(accessor._s.GetData());
            bool editinglock = false;
            hexbox.ByteProvider.Changed += (sender, args) =>
            {
                if (editinglock)
                    return;

                if (hexbox.ByteProvider.Length != accessor._s.Length)
                {
                    editinglock = true;
                    hexbox.ByteProvider.DeleteBytes(0, hexbox.ByteProvider.Length);
                    hexbox.ByteProvider.InsertBytes(0, accessor._s.GetData());
                    editinglock = false;
                }

                if (hexbox.ByteProvider.Length == accessor._s.Length)
                {
                    for (int i = 0; i < hexbox.ByteProvider.Length; i++)
                    {
                        if (hexbox.ByteProvider.ReadByte(i) != accessor._s.GetByte(i))
                            accessor._s.SetByte(i, hexbox.ByteProvider.ReadByte(i));
                    }
                }
                else
                if (hexbox.ByteProvider.Length > accessor._s.Length)
                {
                    int added = (int)hexbox.ByteProvider.Length - accessor._s.Length;

                    accessor._s.Resize((int)hexbox.ByteProvider.Length);

                    // shift references
                    Dictionary<int, HSDStruct> newref = new();
                    foreach (KeyValuePair<int, HSDStruct> r in accessor._s.References)
                    {
                        if (r.Key >= selectedIndex)
                            newref.Add(r.Key + added, r.Value);
                        else
                            newref.Add(r.Key, r.Value);
                    }
                    accessor._s.References.Clear();
                    foreach (KeyValuePair<int, HSDStruct> r in newref)
                        accessor._s.References.Add(r.Key, r.Value);

                    // shift data
                    for (int i = accessor._s.Length - 1; i > selectedIndex + added; i--)
                        accessor._s.SetByte(i, accessor._s.GetByte(i - 1));

                }
                else
                if (hexbox.ByteProvider.Length < accessor._s.Length)
                {
                    int shiftStart = selectedIndex + selectedLength;

                    // shift references
                    Dictionary<int, HSDStruct> newref = new();
                    foreach (KeyValuePair<int, HSDStruct> r in accessor._s.References)
                    {
                        if (r.Key >= selectedIndex)
                            newref.Add(r.Key - selectedLength, r.Value);
                        else
                            newref.Add(r.Key, r.Value);
                    }
                    accessor._s.References.Clear();
                    foreach (KeyValuePair<int, HSDStruct> r in newref)
                        accessor._s.References.Add(r.Key, r.Value);

                    // shift data
                    for (int i = shiftStart; i < accessor._s.Length; i++)
                        accessor._s.SetByte(selectedIndex + (i - shiftStart), accessor._s.GetByte(i));

                    accessor._s.Resize((int)hexbox.ByteProvider.Length);
                }

                Node?.NotifyChange();
                UpdateValues();
            };
            UpdateValues();
        }

        private string LastGoodOffset = "0x00000000";

        private void offsetBox_TextChanged(object sender, System.EventArgs e)
        {
            // good
            if (Regex.Match(offsetBox.Text, @"^0[xX][0-9a-fA-F]{1,8}$").Success)
            {
                LastGoodOffset = offsetBox.Text;
                Poker.CurrentOffset = uint.Parse(offsetBox.Text.ToLower().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                UpdateValues();
                return;
            }

            // missing header
            if (Regex.Match(offsetBox.Text, @"^[0-9a-fA-F]{1,8}$").Success)
            {
                offsetBox.Text = "0x" + offsetBox.Text;
                return;
            }

            // bad
            offsetBox.Text = LastGoodOffset;
            UpdateValues();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void structSize_TextChanged(object sender, System.EventArgs e)
        {
            // good
            if (Regex.Match(structSize.Text, @"^0[xX][0-9a-fA-F]{1,8}$").Success)
            {
                ResizeStruct();
                return;
            }

            // missing header
            if (Regex.Match(structSize.Text, @"^[0-9a-fA-F]{1,8}$").Success)
            {
                structSize.Text = "0x" + structSize.Text;
                ResizeStruct();
                return;
            }

            // bad
            structSize.Text = "0x" + accessor._s.Length.ToString("X");
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResizeStruct()
        {
            if (int.TryParse(structSize.Text.Replace("0x", ""),
                System.Globalization.NumberStyles.HexNumber,
                CultureInfo.InvariantCulture,
                out int value))
            {
                if (value < accessor._s.Length)
                {
                    // check if you want to resize
                    if (MessageBox.Show("Are you sure you want to truncate this struct?", "Resize Struct", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        accessor._s.Resize(value);
                        SetBytes();
                    }
                    structSize.Text = "0x" + accessor._s.Length.ToString("X");
                }
                else
                {
                    accessor._s.Resize(value);
                    SetBytes();
                }
            }

        }

        private bool IsReference(uint offset, int byteSize)
        {
            if (accessor == null)
                return false;

            foreach (KeyValuePair<int, HSDStruct> v in accessor._s.References)
            {
                if (offset + byteSize <= v.Key && offset >= v.Key + 4)
                    return true;
            }
            return false;
        }

        public void UpdateValues()
        {
            propertyGrid2.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void offsetBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.OemPeriod)
                {
                    SetPointer();
                }
                if (e.KeyCode == Keys.OemMinus)
                {
                    RemovePointer();
                }
            }
        }

        private void RemovePointer()
        {
            accessor._s.SetReference((int)Poker.CurrentOffset, null);

            Node.Collapse();
        }

        private void SetPointer()
        {
            string f = Tools.FileIO.OpenFile("All Files |*.*");

            if (f != null)
            {
                if (Poker.CurrentOffset >= accessor._s.Length)
                    accessor._s.Resize((int)Poker.CurrentOffset + 4);

                if (f.ToLower().EndsWith(".dat"))
                {
                    HSDRawFile datFile = new(f);

                    accessor._s.SetReferenceStruct((int)Poker.CurrentOffset, datFile.Roots[0].Data._s);
                }
                else
                    accessor._s.SetReferenceStruct((int)Poker.CurrentOffset, new HSDStruct(System.IO.File.ReadAllBytes(f)));

                Node.Collapse();
            }
        }

        private void buttonSetPointer_Click(object sender, System.EventArgs e)
        {
            SetPointer();
        }

        private void buttonRemovePointer_Click(object sender, System.EventArgs e)
        {
            RemovePointer();
        }

        private void structSize_TextChanged(object sender, TypeValidationEventArgs e)
        {

        }
    }
}
