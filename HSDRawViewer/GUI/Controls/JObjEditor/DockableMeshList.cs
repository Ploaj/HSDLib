using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.Converters;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.GUI.Extra;
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
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class DockableMeshList : DockContent
    {
        private BindingList<DObjProxy> dobjList = new BindingList<DObjProxy>();

        public delegate void DObjSelected(DObjProxy[] dobj, IEnumerable<int> indices);
        public DObjSelected SelectDObj;

        public delegate void OnVisibilityChanged();
        public OnVisibilityChanged VisibilityUpdated;

        public delegate void OnListUpdate();
        public OnListUpdate ListUpdated;

        private HSD_JOBJ _root;

        public IEnumerable<DObjProxy> EnumerateDObjs
        {
            get
            {
                foreach (var v in dobjList)
                    yield return v;
            }
        }

        public IEnumerable<int> SelectedIndices
        {
            get
            {
                return listDOBJ.SelectedIndices.Cast<int>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DockableMeshList()
        {
            InitializeComponent();

            Text = "Objects";

            listDOBJ.DataSource = dobjList;

            listDOBJ.SelectionMode = SelectionMode.MultiExtended;

            // prevent user closing
            CloseButtonVisible = false;
            FormClosing += (sender, args) =>
            {
                if (args.CloseReason == CloseReason.UserClosing)
                {
                    args.Cancel = true;
                }
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        public void SetJObj(HSD_JOBJ jobj)
        {
            _root = jobj;

            dobjList.Clear();
            int ji = 0;
            foreach (var j in jobj.BreathFirstList)
            {
                if (j.Dobj != null)
                {
                    int di = 0;
                    foreach (var d in j.Dobj.List)
                    {
                        dobjList.Add(new DObjProxy(j, d, ji, di));
                    }
                    di++;
                }
                ji++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listDOBJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            // invoke callback
            SelectDObj?.Invoke(listDOBJ.SelectedItems.Cast<DObjProxy>().ToArray(), listDOBJ.SelectedIndices.Cast<int>());

            // can only move one at a time
            editToolStripMenuItem.Enabled = listDOBJ.SelectedItems.Count == 1;
            exportToolStripMenuItem.Enabled = listDOBJ.SelectedItems.Count == 1;
            buttonMoveDown.Enabled = listDOBJ.SelectedItems.Count == 1;
            buttonMoveUp.Enabled = listDOBJ.SelectedItems.Count == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listDOBJ_ItemVisiblilityChanged(object sender, EventArgs e)
        {
            VisibilityUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            listDOBJ.SetAllVisibleState(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHidePoly_Click(object sender, EventArgs e)
        {
            listDOBJ.SetAllVisibleState(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDOBJDelete_Click(object sender, EventArgs e)
        {
            if (listDOBJ.SelectedItems.Count > 0 && MessageBox.Show("Are you sure?\nThis cannot be undone", "Delete Object?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                List<DObjProxy> toRem = new List<DObjProxy>();
                foreach (DObjProxy dobj in listDOBJ.SelectedItems)
                {
                    var dobjs = dobj.ParentJOBJ.Dobj.List;

                    HSD_DOBJ prev = null;
                    foreach (var d in dobjs)
                    {
                        if (d == dobj.DOBJ)
                        {
                            if (prev == null)
                                dobj.ParentJOBJ.Dobj = d.Next;
                            else
                                prev.Next = d.Next;
                            break;
                        }
                        prev = d;
                    }

                    toRem.Add(dobj);
                }

                foreach (var rem in toRem)
                    dobjList.Remove(rem);

                ListUpdated?.Invoke();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            if (listDOBJ.SelectedIndices.Count == 1 && listDOBJ.SelectedItem is DObjProxy con)
            {
                var newIndex = listDOBJ.SelectedIndex - 1;

                HSD_DOBJ prev = null;
                HSD_DOBJ prevprev = null;
                foreach (var dobj in con.ParentJOBJ.Dobj?.List)
                {
                    if (dobj._s == con.DOBJ._s)
                    {
                        if (prev == null)
                            break;

                        // totally not confusing
                        if (prevprev == null)
                            con.ParentJOBJ.Dobj = con.DOBJ;
                        else
                            prevprev.Next = con.DOBJ;
                        prev.Next = con.DOBJ.Next;
                        con.DOBJ.Next = prev;

                        dobjList.Remove(con);
                        dobjList.Insert(newIndex, con);

                        listDOBJ.SelectedIndices.Clear();
                        listDOBJ.SelectedIndex = -1;
                        listDOBJ.SelectedIndex = newIndex;

                        break;
                    }
                    prevprev = prev;
                    prev = dobj;
                }

                ListUpdated?.Invoke();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            if (listDOBJ.SelectedIndices.Count == 1 && listDOBJ.SelectedItem is DObjProxy con)
            {
                if (con.DOBJ.Next == null)
                    return;

                var newIndex = listDOBJ.SelectedIndex + 1;

                HSD_DOBJ prev = null;
                foreach (var dobj in con.ParentJOBJ.Dobj?.List)
                {
                    if (dobj._s == con.DOBJ._s)
                    {
                        // totally not confusing
                        if (prev == null)
                            con.ParentJOBJ.Dobj = con.DOBJ.Next;
                        else
                            prev.Next = con.DOBJ.Next;
                        var newNext = con.DOBJ.Next.Next;
                        con.DOBJ.Next.Next = con.DOBJ;
                        con.DOBJ.Next = newNext;

                        dobjList.Remove(con);
                        dobjList.Insert(newIndex, con);

                        listDOBJ.SelectedIndices.Clear();
                        listDOBJ.SelectedIndex = -1;
                        listDOBJ.SelectedIndex = newIndex;

                        break;
                    }
                    prev = dobj;
                }

                ListUpdated?.Invoke();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DummyDOBJSetting
        {
            [DisplayName("Number to Generate"), Description("")]
            public int NumberToGenerate { get; set; } = 1;

            [DisplayName("Add Dummy Texture"), Description("")]
            public bool AddDummyTexture { get; set; } = false;

            [DisplayName("Joint Index"), Description("")]
            public int JointIndex { get; set; } = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDummyDOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var setting = new DummyDOBJSetting();

            using (PropertyDialog d = new PropertyDialog("Dummy Object Generator", setting))
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    HSD_JOBJ parent = _root;
                    if (setting.JointIndex < _root.BreathFirstList.Count)
                        parent = _root.BreathFirstList[setting.JointIndex];

                    for (int i = 0; i < setting.NumberToGenerate; i++)
                    {
                        var dobj = new HSD_DOBJ()
                        {
                            Mobj = new HSD_MOBJ()
                            {
                                RenderFlags = RENDER_MODE.CONSTANT,
                                Material = new HSD_Material()
                                {
                                    DiffuseColor = Color.White,
                                    SpecularColor = Color.White,
                                    AmbientColor = Color.White,
                                    Shininess = 50
                                }
                            }
                        };

                        if (setting.AddDummyTexture)
                        {
                            dobj.Mobj.RenderFlags |= RENDER_MODE.TEX0;
                            dobj.Mobj.Textures = new HSD_TOBJ()
                            {
                                MagFilter = GXTexFilter.GX_LINEAR,
                                Flags = TOBJ_FLAGS.COORD_UV | TOBJ_FLAGS.LIGHTMAP_DIFFUSE | TOBJ_FLAGS.COLORMAP_MODULATE | TOBJ_FLAGS.ALPHAMAP_MODULATE,
                                HScale = 1,
                                WScale = 1,
                                WrapS = GXWrapMode.CLAMP,
                                WrapT = GXWrapMode.CLAMP,
                                SX = 1,
                                SY = 1,
                                SZ = 1,
                                GXTexGenSrc = GXTexGenSrc.GX_TG_TEX0,
                                Blending = 1,
                                ImageData = new HSD_Image()
                                {
                                    Format = GXTexFmt.I4,
                                    Width = 8, 
                                    Height = 8,
                                    ImageData = new byte[32]
                                }
                            };
                            dobj.Mobj.Textures.Optimize();
                        }

                        if (parent.Dobj == null)
                            parent.Dobj = dobj;
                        else
                            parent.Dobj.Add(dobj);
                    }
                    SetJObj(_root);
                    ListUpdated();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearSelectedPOBJsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?\nThis will clear all polygons in the model\n and cannot be undone", "Clear PObjs", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DObjProxy v in listDOBJ.SelectedItems)
                {
                    v.DOBJ.Pobj = null;
                }
                SetJObj(_root);
                ListUpdated();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listDOBJ.SelectedIndices.Count > 0)
            {
                var f = FileIO.OpenFile("Material (*.mobj)|*.mobj");

                if (f != null)
                {
                    HSDRawFile file = new HSDRawFile(f);

                    if (file.Roots.Count > 0)
                    {
                        var mobj = new HSD_MOBJ();
                        if (file.Roots[0].Data._s.Length >= mobj.TrimmedSize)
                        {
                            mobj._s = file.Roots[0].Data._s;

                            foreach (DObjProxy con in listDOBJ.SelectedItems)
                            {
                                if (con != null)
                                    con.DOBJ.Mobj = HSDAccessor.DeepClone<HSD_MOBJ>(mobj);
                            }
                        }
                    }

                    ListUpdated();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listDOBJ.SelectedIndices.Count == 1 && listDOBJ.SelectedItem is DObjProxy con)
            {
                var f = Tools.FileIO.SaveFile("Material (*.mobj)|*.mobj");

                if (f != null)
                {
                    HSDRawFile file = new HSDRawFile();
                    file.Roots.Add(new HSDRootNode() { Name = "mobj", Data = con.DOBJ.Mobj });
                    file.Save(f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void massTextureEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (JObjTextureEditorDialog d = new JObjTextureEditorDialog(_root))
            {
                d.ShowDialog();
                ListUpdated?.Invoke();
            }
        }
    }
}
