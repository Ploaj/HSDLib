using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HSDRaw.Tools;
using HSDRaw.Common.Animation;
using System.Reflection;

namespace HSDRawViewer.GUI
{
    public partial class KeyEditor : UserControl
    {
        private class Key
        {
            public float Value { get; set; }
            public float Slope { get; set; }
            public GXInterpolationType InterpolationType { get; set; }
        }

        private BindingList<Key> KeyFrames = new BindingList<Key>();
        private ContextMenuStrip KeyContextMenu = new ContextMenuStrip();

        public KeyEditor()
        {
            InitializeComponent();

            DoubleBuffered = true;

            var insert = new ToolStripMenuItem("Insert");
            insert.Click += (sender, args) =>
            {
                InsertKey();
            };
            KeyContextMenu.Items.Add(insert);

            var delete = new ToolStripMenuItem("Delete");
            delete.Click += (sender, args) =>
            {
                DeleteKeys();
            };
            KeyContextMenu.Items.Add(delete);

            dataGridView1.MouseDown += (sender, args) =>
            {
                var hti = dataGridView1.HitTest(args.X, args.Y);
                dataGridView1.ClearSelection();
                if(hti != null && hti.RowIndex != -1)
                    dataGridView1.Rows[hti.RowIndex].Selected = true;
            };

            dataGridView1.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Right)
                    KeyContextMenu.Show(dataGridView1, args.Location);
            };

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Value";
            column.Name = "Value";
            dataGridView1.Columns.Add(column);

            DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
            column2.DataPropertyName = "Slope";
            column2.Name = "Slope";
            dataGridView1.Columns.Add(column2);

            DataGridViewComboBoxColumn column3 = new DataGridViewComboBoxColumn();
            column3.DataSource = Enum.GetValues(typeof(GXInterpolationType));
            column3.DataPropertyName = "InterpolationType";
            column3.Name = "Interpolation";
            dataGridView1.Columns.Add(column3);
            
            dataGridView1.AutoSize = true;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CurrentCellDirtyStateChanged += new EventHandler(dataGridView1_CurrentCellDirtyStateChanged);
            dataGridView1.DataSource = KeyFrames;

            // work around to avoid flickering
            typeof(DataGridView).InvokeMember("DoubleBuffered",
   BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
   null, dataGridView1, new object[] { true });

            typeof(Panel).InvokeMember("DoubleBuffered",
   BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
   null, panel1, new object[] { true });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        public void SetKeys(List<FOBJKey> keys)
        {
            KeyFrames.Clear();

            if (keys == null || keys.Count == 0)
                return;

            var fCount = keys[keys.Count - 1].Frame + 1;
            
            dataGridView1.DataSource = null;

            for(int i = 0; i < fCount; i++)
                KeyFrames.Add(new Key());

            foreach(var k in keys)
            {
                KeyFrames[(int)k.Frame].Value = k.Value;
                KeyFrames[(int)k.Frame].Slope = k.Tan;
                KeyFrames[(int)k.Frame].InterpolationType = k.InterpolationType;
            }

            dataGridView1.DataSource = KeyFrames;

            panel1.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<FOBJKey> GetFOBJKeys()
        {
            List<FOBJKey> keys = new List<FOBJKey>();

            int frame = 0;
            foreach(var v in KeyFrames)
            {
                if(v.InterpolationType != GXInterpolationType.HSD_A_OP_NONE)
                {
                    keys.Add(new FOBJKey()
                    {
                        Frame = frame,
                        Value = v.Value,
                        Tan = v.Slope,
                        InterpolationType = v.InterpolationType
                    });
                }
                frame++;
            }

            return keys;
        }

        private static Brush brush = new SolidBrush(Color.Red);
        private static Brush grayBrush = new SolidBrush(Color.FromArgb(127, Color.Gray));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            var valueBounds = grid.GetCellDisplayRectangle(0, e.RowIndex, true);
            var slopeBounds = grid.GetCellDisplayRectangle(1, e.RowIndex, true);

            if (e.RowIndex < KeyFrames.Count && KeyFrames[e.RowIndex] != null)
            {
                if(KeyFrames[e.RowIndex].InterpolationType == GXInterpolationType.HSD_A_OP_NONE)
                {
                    e.Graphics.FillRectangle(brush, headerBounds);
                    e.Graphics.FillRectangle(grayBrush, slopeBounds);
                    e.Graphics.FillRectangle(grayBrush, valueBounds);
                }
                if (KeyFrames[e.RowIndex].InterpolationType == GXInterpolationType.HSD_A_OP_SLP)
                {
                    e.Graphics.FillRectangle(grayBrush, valueBounds);
                }
                if (KeyFrames[e.RowIndex].InterpolationType == GXInterpolationType.HSD_A_OP_SPL ||
                    KeyFrames[e.RowIndex].InterpolationType == GXInterpolationType.HSD_A_OP_LIN ||
                    KeyFrames[e.RowIndex].InterpolationType == GXInterpolationType.HSD_A_OP_CON ||
                    KeyFrames[e.RowIndex].InterpolationType == GXInterpolationType.HSD_A_OP_KEY)
                {
                    e.Graphics.FillRectangle(grayBrush, slopeBounds);
                }
            }

            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.InvalidateRow(e.RowIndex);

            panel1.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (datagridview.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void InsertKey()
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            var i = dataGridView1.SelectedRows[0].Index + 1;
            if (i != -1)
            {
                KeyFrames.Insert(i, new Key());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeleteKeys()
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.RemoveAt(row.Index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Insert && dataGridView1.SelectedRows.Count > 0)
            {
                InsertKey();
            }
        }

        private static Brush backBrush = new SolidBrush(Color.DarkSlateGray);
        private static Brush numBrush = new SolidBrush(Color.AntiqueWhite);
        private static Brush pointBrush = new SolidBrush(Color.Yellow);
        private static Pen linePen = new Pen(Color.AntiqueWhite);
        private static Pen faintLinePen = new Pen(Color.Gray);
        private static Font numFont = new Font(FontFamily.GenericMonospace, 8);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (KeyFrames.Count == 0)
                return;

            e.Graphics.FillRectangle(backBrush, e.ClipRectangle);

            var graphOffsetX = 50;
            var graphOffsetY = 12;
            var graphWidth = panel1.Width - graphOffsetX;
            var graphHeight = panel1.Height - graphOffsetY;

            float ampHi = float.MinValue;
            float ampLw = float.MaxValue;

            var keyCount = (float)KeyFrames.Count;

            var spaces = (int)(keyCount / (graphWidth / TextRenderer.MeasureText(keyCount.ToString(), numFont).Width));
            if (spaces < 1)
                spaces = 1;
            if (spaces % 5 != 0)
                spaces += 5 - (spaces % 5);

            FOBJ_Player a = new FOBJ_Player();
            a.Keys = GetFOBJKeys();

            for (int i = 0; i < KeyFrames.Count; i++)
            {
                var v = a.GetValue(i);

                ampHi = Math.Max(ampHi, v);
                ampLw = Math.Min(ampLw, v);
            }

            var dis = ampHi - ampLw;
            var off = dis * 0.15f;

            if (dis == 0)
                return;

            dis = (ampHi - ampLw) + off * 2;

            e.Graphics.DrawString(ampHi.ToString("0.0000"), numFont, numBrush, new PointF(0, (off / dis) * graphHeight - 4 + graphOffsetY));
            e.Graphics.DrawString(ampLw.ToString("0.0000"), numFont, numBrush, new PointF(0, ((dis - off) / dis) * graphHeight - 4 + graphOffsetY));
            
            e.Graphics.DrawLine(faintLinePen, new PointF(graphOffsetX, (off / dis) * graphHeight + graphOffsetY), new PointF(panel1.Width, (off / dis) * graphHeight + graphOffsetY));
            e.Graphics.DrawLine(faintLinePen, new PointF(graphOffsetX, ((dis - off) / dis) * graphHeight + graphOffsetY), new PointF(panel1.Width, ((dis - off) / dis) * graphHeight + graphOffsetY));


            for (int i = 1; i <= KeyFrames.Count; i++)
            {
                var x1 = graphOffsetX + (int)(((i - 1) / keyCount) * graphWidth);
                var h1 = (int)((a.GetValue(i - 1) + Math.Abs(ampLw) + off) / dis * graphHeight);
                var h2 = (int)((a.GetValue(i) + Math.Abs(ampLw) + off) / dis * graphHeight);
                
                if(i - 1 < KeyFrames.Count)
                {
                    if (KeyFrames[i - 1].InterpolationType == GXInterpolationType.HSD_A_OP_CON)
                        h2 = h1;
                }

                if (((i - 1) % spaces) == 0)
                {
                    e.Graphics.DrawLine(faintLinePen, new Point(x1, graphOffsetY), new Point(x1, panel1.Height));
                    e.Graphics.DrawString((i - 1).ToString(), numFont, numBrush, new PointF(x1 - 4, 0));
                }

                var px1 = graphOffsetX + (int)(((i - 1) / keyCount) * graphWidth);
                var px2 = graphOffsetX + (int)((i / keyCount) * graphWidth);
                var py1 = panel1.Height - h1;
                var py2 = panel1.Height - h2;
                
                e.Graphics.DrawLine(linePen, 
                    new Point(px1, py1),
                    new Point(px2, py2));

                if (i - 1 < KeyFrames.Count)
                {
                    if (KeyFrames[i - 1].InterpolationType != GXInterpolationType.HSD_A_OP_NONE)
                        e.Graphics.FillRectangle(pointBrush, new RectangleF(px1 - 2, py1 - 2, 4, 4));
                }
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        #region Drag and Drop

        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dataGridView1.DoDragDrop(
                          dataGridView1.Rows[rowIndexFromMouseDown],
                          DragDropEffects.Move);
                }
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dataGridView1.HitTest(e.X, e.Y).RowIndex;

            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(
                          new Point(
                            e.X - (dragSize.Width / 2),
                            e.Y - (dragSize.Height / 2)),
                      dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop = dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                var item = KeyFrames[rowIndexFromMouseDown];
                KeyFrames.RemoveAt(rowIndexFromMouseDown);
                KeyFrames.Insert(rowIndexOfItemUnderMouseToDrop, item);
                panel1.Invalidate();
            }
        }

        #endregion

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            InsertKey();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteKeys();
        }

        public class InsertAt
        {
            public int FrameCount { get; set; }
        }

        private void buttonInsertAt_Click(object sender, EventArgs e)
        {
            var ia = new InsertAt();
            ia.FrameCount = KeyFrames.Count;
            using (PropertyDialog d = new PropertyDialog("Track Settings", ia))
            {
                if(d.ShowDialog() == DialogResult.OK)
                {
                    while (KeyFrames.Count <= ia.FrameCount)
                        KeyFrames.Add(new Key());

                    var toRem = KeyFrames.Count - ia.FrameCount;
                    for(int i = 0; i < toRem; i++)
                        KeyFrames.RemoveAt(KeyFrames.Count - 1);
                }
            }
        }
    }
}
