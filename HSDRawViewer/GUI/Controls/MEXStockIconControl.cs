using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.MEX;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXStockIconControl : UserControl
    {
        public class StockIconNode : ImageArrayItem
        {
            public TOBJProxy[] TOBJS { get; set; } = new TOBJProxy[0];

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public Image ToImage()
            {
                if (TOBJS.Length > 0)
                    return TOBJS[0].TOBJ.ToImage().ToBitmap();
                return null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return " Icon";
            }
        }

        public StockIconNode[] ReservedItems { get; set; }

        public StockIconNode[] Items { get; set; }

        private MEX_Stock StockNode;

        /// <summary>
        /// 
        /// </summary>
        public MEXStockIconControl()
        {
            InitializeComponent();

            arrayMemberEditor1.SelectedObjectChanged += (sender, args) =>
            {
                arrayMemberEditor3.SetArrayFromProperty(arrayMemberEditor1.SelectedObject, "TOBJS");
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockNode"></param>
        public void LoadStockNode(MEX_Stock stockNode)
        {
            StockNode = stockNode;

            List<FOBJKey> keys = stockNode.MatAnimJoint.MaterialAnimation.TextureAnimation.AnimationObject.FObjDesc.GetDecodedKeys();
            HSD_TOBJ[] tobjs = stockNode.MatAnimJoint.MaterialAnimation.TextureAnimation.ToTOBJs();

            ReservedItems = new StockIconNode[stockNode.Reserved];
            Items = new StockIconNode[stockNode.Stride];

            //
            for (int i = 0; i < stockNode.Reserved; i++)
            {
                ReservedItems[i] = new StockIconNode();
                FOBJKey frame = keys.Find(e => e.Frame == i);
                if (frame != null)
                    ReservedItems[i].TOBJS = new TOBJProxy[] { new(tobjs[(int)frame.Value]) };
            }

            //
            for (int i = 0; i < stockNode.Stride; i++)
            {
                Items[i] = new StockIconNode();
                int colorCount = 0;

                while (keys.Find(e => e.Frame == stockNode.Reserved + stockNode.Stride * colorCount + i) != null)
                    colorCount++;

                Items[i].TOBJS = new TOBJProxy[colorCount];

                for (int color = 0; color < colorCount; color++)
                {
                    FOBJKey frame = keys.Find(e => e.Frame == stockNode.Reserved + stockNode.Stride * color + i);

                    if (frame != null)
                        Items[i].TOBJS[color] = new TOBJProxy(tobjs[(int)frame.Value]);
                }
            }

            arrayMemberEditor1.SetArrayFromProperty(this, nameof(Items));
            arrayMemberEditor2.SetArrayFromProperty(this, nameof(ReservedItems));
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveStockNode()
        {
            // set meta data
            StockNode.Reserved = (short)ReservedItems.Length;
            StockNode.Stride = (short)Items.Length;

            List<FOBJKey> keys = new();
            List<HSD_TOBJ> tobjs = new();

            // reserved
            for (int i = 0; i < ReservedItems.Length; i++)
            {
                if (ReservedItems[i].TOBJS.Length > 0)
                {
                    keys.Add(new FOBJKey() { Frame = i, InterpolationType = GXInterpolationType.HSD_A_OP_CON, Value = tobjs.Count });
                    tobjs.Add(ReservedItems[i].TOBJS[0].TOBJ);
                }
            }

            // get fighter stock icons
            for (int i = 0; i < Items.Length; i++)
            {
                for (int j = 0; j < Items[i].TOBJS.Length; j++)
                {
                    keys.Add(new FOBJKey() { Frame = ReservedItems.Length + j * Items.Length + i, InterpolationType = GXInterpolationType.HSD_A_OP_CON, Value = tobjs.Count });
                    tobjs.Add(Items[i].TOBJS[j].TOBJ);
                }
            }

            // order keys
            keys = keys.OrderBy(e => e.Frame).ToList();

            // generate new tex anim
            HSD_TexAnim newTexAnim = new();

            newTexAnim.AnimationObject = new HSD_AOBJ();
            newTexAnim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            newTexAnim.AnimationObject.FObjDesc.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TIMG);
            newTexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
            newTexAnim.AnimationObject.FObjDesc.Next.SetKeys(keys, (byte)TexTrackType.HSD_A_T_TCLT);

            newTexAnim.FromTOBJs(tobjs, false);
            newTexAnim.Optimize();

            StockNode.MatAnimJoint = new HSD_MatAnimJoint();
            StockNode.MatAnimJoint.MaterialAnimation = new HSD_MatAnim();
            StockNode.MatAnimJoint.MaterialAnimation.TextureAnimation = newTexAnim;

            // done
            MessageBox.Show("Stock Icon Symbol has been rebuilt");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveStockNode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void replaceIcon_Click(object sender, EventArgs e)
        {
            string file = HSDRawViewer.Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

            if (file != null)
            {
                if (arrayMemberEditor3.SelectedObject is TOBJProxy proxy)
                {
                    proxy.TOBJ = TOBJExtentions.ImportTObjFromFile(file, HSDRaw.GX.GXTexFmt.CI4, HSDRaw.GX.GXTlutFmt.RGB5A3);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string file = HSDRawViewer.Tools.FileIO.OpenFile(ApplicationSettings.ImageFileFilter);

            if (file != null)
            {
                if (arrayMemberEditor2.SelectedObject is StockIconNode node)
                {
                    node.TOBJS = new TOBJProxy[] { new(TOBJExtentions.ImportTObjFromFile(file, HSDRaw.GX.GXTexFmt.CI4, HSDRaw.GX.GXTlutFmt.RGB5A3)) };
                }
            }
        }
    }
}
