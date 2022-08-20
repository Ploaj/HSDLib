using HSDRaw.Common.Animation;
using HSDRaw.Tools;
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
    public partial class DockableTrackEditor : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public DockableTrackEditor()
        {
            InitializeComponent();

            Text = "Animation Editor";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aobj"></param>
        public void SetKeys(GraphEditor.AnimType type, IEnumerable<FOBJ_Player> players)
        {
            graphEditor1.ClearTracks();
            graphEditor1.LoadTracks(type, players);
        }
    }
}
