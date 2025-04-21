﻿using HSDRaw;
using HSDRaw.Common;
using HSDRaw.GX;
using HSDRawViewer.GUI.Dialog;
using HSDRawViewer.GUI.Extra;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HSDRawViewer.GUI.Controls.JObjEditor
{
    public partial class DockableMeshList : DockContent
    {
        private readonly ObservableCollection<DObjProxy> dobjList = new();

        public delegate void DObjSelected(DObjProxy[] dobj, IEnumerable<int> indices);
        public DObjSelected SelectDObj;

        public delegate void OnVisibilityChanged();
        public OnVisibilityChanged VisibilityUpdated;

        public delegate void OnListUpdate();
        public OnListUpdate ListUpdated;

        public delegate void OnListOrderUpdate();
        public OnListOrderUpdate ListOrderUpdated;

        private HSD_JOBJ _root;

        public IEnumerable<DObjProxy> EnumerateDObjs
        {
            get
            {
                foreach (DObjProxy v in dobjList)
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

            listDOBJ.DataSource = null;
            dobjList.Clear();
            int ji = 0;
            foreach (HSD_JOBJ j in jobj.TreeList)
            {
                if (j.Dobj != null)
                {
                    int di = 0;
                    foreach (HSD_DOBJ d in j.Dobj.List)
                    {
                        dobjList.Add(new DObjProxy(j, d, ji, di));
                    }
                    di++;
                }
                ji++;
            }
            listDOBJ.DataSource = dobjList;
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
            exportToolStripMenuItem.Enabled = listDOBJ.SelectedItems.Count == 1;
            //buttonMoveDown.Enabled = listDOBJ.SelectedItems.Count == 1;
            //buttonMoveUp.Enabled = listDOBJ.SelectedItems.Count == 1;
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
                List<DObjProxy> toRem = new();
                foreach (DObjProxy dobj in listDOBJ.SelectedItems)
                {
                    List<HSD_DOBJ> dobjs = dobj.ParentJOBJ.Dobj.List;

                    HSD_DOBJ prev = null;
                    foreach (HSD_DOBJ d in dobjs)
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

                foreach (DObjProxy rem in toRem)
                    dobjList.Remove(rem);

                ListUpdated?.Invoke();
                listDOBJ.DataSource = null;
                listDOBJ.DataSource = dobjList;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            listDOBJ.BeginUpdate();

            // get selected indices
            IEnumerable<int> indexes = listDOBJ.SelectedIndices.Cast<int>();

            // process selected indicies in reverse
            foreach (int i in indexes)
            {
                // get object at index
                DObjProxy moveItem = dobjList[i];

                // move dobj down if possible
                if (moveItem.ParentJOBJ != null)
                {
                    // find dobj in jobj list
                    HSD_DOBJ prev = null;
                    HSD_DOBJ prevprev = null;
                    foreach (HSD_DOBJ dobj in moveItem.ParentJOBJ.Dobj?.List)
                    {
                        if (dobj == moveItem.DOBJ)
                        {
                            // cannot move this item up
                            if (prev == null)
                            {
                                goto LoopEnd;
                            }
                            else
                            {
                                // move dobj up in jobj list
                                if (prevprev == null)
                                {
                                    moveItem.ParentJOBJ.Dobj.Next = moveItem.DOBJ.Next;
                                    moveItem.DOBJ.Next = moveItem.ParentJOBJ.Dobj;
                                    moveItem.ParentJOBJ.Dobj = moveItem.DOBJ;
                                }
                                else
                                {
                                    prev.Next = moveItem.DOBJ.Next;
                                    moveItem.DOBJ.Next = prev;
                                    prevprev.Next = moveItem.DOBJ;
                                }
                            }

                            // update list
                            dobjList.Move(i, i - 1);
                            listDOBJ.SetSelected(i, false);
                            listDOBJ.SetSelected(i - 1, true);

                            break;
                        }

                        prevprev = prev;
                        prev = dobj;
                    }
                }
            }

        LoopEnd:

            int[] sel = listDOBJ.SelectedIndices.Cast<int>().ToArray();
            listDOBJ.DataSource = null;
            listDOBJ.DataSource = dobjList;
            foreach (int s in sel)
                listDOBJ.SetSelected(s, true);

            listDOBJ.EndUpdate();

            ListOrderUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            listDOBJ.BeginUpdate();

            // get selected indices
            IEnumerable<int> indexes = listDOBJ.SelectedIndices.Cast<int>().Reverse();

            // process selected indicies in reverse
            foreach (int i in indexes)
            {
                // get object at index
                DObjProxy moveItem = dobjList[i];

                // can't move this item down
                if (moveItem.DOBJ.Next == null)
                    goto LoopEnd;

                // move dobj down if possible
                if (moveItem.ParentJOBJ != null)
                {
                    // find dobj in jobj list
                    HSD_DOBJ prev = null;
                    foreach (HSD_DOBJ dobj in moveItem.ParentJOBJ.Dobj?.List)
                    {
                        if (dobj == moveItem.DOBJ)
                        {
                            // move dobj down in jobj list
                            if (prev == null)
                                moveItem.ParentJOBJ.Dobj = moveItem.DOBJ.Next;
                            else
                                prev.Next = moveItem.DOBJ.Next;
                            HSD_DOBJ newNext = moveItem.DOBJ.Next.Next;
                            moveItem.DOBJ.Next.Next = moveItem.DOBJ;
                            moveItem.DOBJ.Next = newNext;

                            // update list
                            dobjList.Move(i, i + 1);
                            listDOBJ.SetSelected(i, false);
                            listDOBJ.SetSelected(i + 1, true);

                            break;
                        }

                        prev = dobj;
                    }
                }
            }

        LoopEnd:

            int[] sel = listDOBJ.SelectedIndices.Cast<int>().ToArray();
            listDOBJ.DataSource = null;
            listDOBJ.DataSource = dobjList;
            foreach (int s in sel)
                listDOBJ.SetSelected(s, true);

            listDOBJ.EndUpdate();

            ListOrderUpdated?.Invoke();
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
            DummyDOBJSetting setting = new();

            using PropertyDialog d = new("Dummy Object Generator", setting);
            if (d.ShowDialog() == DialogResult.OK)
            {
                HSD_JOBJ parent = _root;
                if (setting.JointIndex < _root.TreeList.Count)
                    parent = _root.TreeList[setting.JointIndex];

                for (int i = 0; i < setting.NumberToGenerate; i++)
                {
                    HSD_DOBJ dobj = new()
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
                            RepeatT = 1,
                            RepeatS = 1,
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
                string f = FileIO.OpenFile("Material (*.mobj)|*.mobj");

                if (f != null)
                {
                    HSDRawFile file = new(f);

                    if (file.Roots.Count > 0)
                    {
                        HSD_MOBJ mobj = new();
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
                string f = Tools.FileIO.SaveFile("Material (*.mobj)|*.mobj");

                if (f != null)
                {
                    HSDRawFile file = new();
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
            using JObjTextureEditorDialog d = new(_root);
            d.ShowDialog();
            ListUpdated?.Invoke();
        }
    }
}
