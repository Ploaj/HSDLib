using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Controls
{
    public class MeshListItem
    {
        public bool Visible { get; set; } = true;
    }

    public class MeshList : ListBox
    {

        public event EventHandler ItemVisiblilityChanged;

        public MeshList() : base()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);

            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += Ditem;


            HashSet<int> _selIndex = new HashSet<int>();
            HashSet<int> _newSet = new HashSet<int>();
            bool updating = false;
            SelectedIndexChanged += (sender, args) =>
            {
                if (updating)
                    return;

                var mpos = PointToClient(Cursor.Position);
                var index = IndexFromPoint(mpos);

                if (index != -1 &&
                    Items[index] is MeshListItem checkable &&
                    mpos.X < ItemHeight)
                {
                    // undo selection
                    _newSet.Clear();
                    foreach (int i in SelectedIndices)
                        _newSet.Add(i);

                    if (!_newSet.SetEquals(_selIndex))
                    {
                        updating = true;
                        BeginUpdate();
                        SelectedIndices.Clear();
                        SelectedIndex = -1;
                        foreach (var v in _selIndex)
                            SelectedIndices.Add(v);
                        EndUpdate();
                        updating = false;
                    }

                    // toggle visibility
                    {
                        if (SelectedIndices.Contains(index))
                        {
                            bool state = !checkable.Visible;
                            foreach (var s in SelectedItems)
                                if (s is MeshListItem mesh)
                                    mesh.Visible = state;
                        }
                        else
                        {
                            checkable.Visible = !checkable.Visible;
                        }
                        ItemVisiblilityChanged?.Invoke(this, new EventArgs());
                        Invalidate();
                    }
                    return;
                }

                _selIndex.Clear();
                foreach (int v in SelectedIndices)
                    _selIndex.Add(v);
            };
        }

        public void SetVisibleState(int index, bool visible)
        {
            if (index < Items.Count && index >= 0)
            {
                if (Items[index] is MeshListItem mesh)
                    mesh.Visible = visible;
            }

            ItemVisiblilityChanged?.Invoke(this, new EventArgs());
            Invalidate();
        }

        public void SetAllVisibleState(bool visible)
        {
            foreach (var v in Items)
                if (v is MeshListItem mesh)
                    mesh.Visible = visible;

            ItemVisiblilityChanged?.Invoke(this, new EventArgs());
            Invalidate();
        }

        private void Ditem(object sender, DrawItemEventArgs e)
        {
            using (var textBrush = new SolidBrush(ForeColor))
                try
                {
                    e.DrawBackground();
                    Rectangle rect = e.Bounds;

                    if (((ListBox)sender).Items[e.Index] is MeshListItem mesh)
                    {
                        e.Graphics.DrawImage(mesh.Visible ? Properties.Resources.ts_visible : Properties.Resources.ts_hidden, rect.X, rect.Y, ItemHeight, ItemHeight);
                        rect.X += ItemHeight;
                    }

                    e.Graphics.DrawString($"{e.Index}. {((ListBox)sender).Items[e.Index].ToString()}", Font, textBrush, rect, StringFormat.GenericDefault);

                    e.DrawFocusRectangle();
                }
                catch
                {

                }
        }


        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}
