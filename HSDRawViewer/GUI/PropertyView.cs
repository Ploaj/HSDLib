﻿using HSDRaw;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;

namespace HSDRawViewer.GUI
{
    public partial class PropertyView : DockContent
    {
        private HSDAccessor accessor
        {
            get
            {
                if (propertyGrid1.SelectedObject is HSDAccessor a)
                    return a;
                return null;
            }
        }

        public PropertyView()
        {
            InitializeComponent();

            Text = "Property View";

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

        public void SetAccessor(HSDAccessor accessor)
        {
            if (accessor == null)
                return;

            propertyGrid1.SelectedObject = accessor;
            panel1.SetBytes(accessor._s.GetData());

            if(this.accessor != accessor)
                offsetBox.Text = "0";
        }

        private void propertyGrid1_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            if(propertyGrid1.SelectedObject is HSDAccessor accessor)
                panel1.SetBytes(accessor._s.GetData());
        }

        private string LastGoodOffset = "0x00000000";
        private uint CurrentOffset = 0;

        private void offsetBox_TextChanged(object sender, System.EventArgs e)
        { 
            // good
            if (Regex.Match(offsetBox.Text, @"^0[xX][0-9a-fA-F]{1,8}$").Success)
            {
                LastGoodOffset = offsetBox.Text;
                CurrentOffset = uint.Parse(offsetBox.Text.ToLower().Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
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
        }

        private bool IsReference(uint offset, int byteSize)
        {
            if (accessor == null)
                return false;

            foreach(var v in accessor._s.References)
            {
                if (offset + byteSize <= v.Key && offset >= v.Key + 4)
                    return true;
            }
            return false;
        }

        private void UpdateValues()
        {
            // disable all buttons
            buttonInt16.Enabled = false;
            buttonInt32.Enabled = false;
            buttonFloat.Enabled = false;

            floatBox.Text = "0";
            ByteBox.Text = "0";
            Int16Box.Text = "0";
            Int32Box.Text = "0";

            if (accessor == null || CurrentOffset < 0)
                return;

            if (CurrentOffset + 1 > accessor._s.Length || IsReference(CurrentOffset, 1))
                return;

            buttonByte.Enabled = true;
            ByteBox.Text = accessor._s.GetByte((int)CurrentOffset).ToString();

            if (CurrentOffset + 2 > accessor._s.Length || IsReference(CurrentOffset, 2))
                return;

            buttonInt16.Enabled = true;
            Int16Box.Text = accessor._s.GetInt16((int)CurrentOffset).ToString();

            if (CurrentOffset + 4 > accessor._s.Length || IsReference(CurrentOffset, 4))
                return;

            buttonInt32.Enabled = true;
            buttonFloat.Enabled = true;
            Int32Box.Text = accessor._s.GetInt32((int)CurrentOffset).ToString();
            floatBox.Text = accessor._s.GetFloat((int)CurrentOffset).ToString();
        }

        private string prevFloat = "0";
        private void floatBox_TextChanged(object sender, System.EventArgs e)
        {
            float f;
            if (float.TryParse(floatBox.Text, out f))
            {
                prevFloat = floatBox.Text;
            }
            else
                floatBox.Text = prevFloat;
        }

        private string prevInt32 = "0";
        private void Int32Box_TextChanged(object sender, System.EventArgs e)
        {
            int f;
            if (int.TryParse(Int32Box.Text, out f))
            {
                prevInt32 = Int32Box.Text;
            }
            else
                Int32Box.Text = prevInt32;
        }

        private string prevInt16 = "0";
        private void Int16Box_TextChanged(object sender, System.EventArgs e)
        {
            short f;
            if (short.TryParse(Int16Box.Text, out f))
            {
                prevInt16 = Int16Box.Text;
            }
            else
                Int16Box.Text = prevInt16;
        }

        private void buttonFloat_Click(object sender, System.EventArgs e)
        {
            if(accessor != null)
            {
                accessor._s.SetFloat((int)CurrentOffset, float.Parse(floatBox.Text));
                SetAccessor(accessor);
                UpdateValues();
            }
        }

        private void buttonInt32_Click(object sender, System.EventArgs e)
        {
            if (accessor != null)
            {
                accessor._s.SetInt32((int)CurrentOffset, int.Parse(Int32Box.Text));
                SetAccessor(accessor);
                UpdateValues();
            }
        }

        private void buttonInt16_Click(object sender, System.EventArgs e)
        {
            if (accessor != null)
            {
                accessor._s.SetInt16((int)CurrentOffset, short.Parse(Int16Box.Text));
                SetAccessor(accessor);
                UpdateValues();
            }
        }

        private string prevByte = "";
        private void ByteBox_TextChanged(object sender, System.EventArgs e)
        {
            byte f;
            if (byte.TryParse(ByteBox.Text, out f))
            {
                prevByte = ByteBox.Text;
            }
            else
                ByteBox.Text = prevByte;
        }

        private void buttonByte_Click(object sender, System.EventArgs e)
        {
            if (accessor != null)
            {
                accessor._s.SetByte((int)CurrentOffset, byte.Parse(ByteBox.Text));
                SetAccessor(accessor);
                UpdateValues();
            }
        }
    }
}