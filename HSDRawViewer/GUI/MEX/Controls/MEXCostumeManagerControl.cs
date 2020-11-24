using System;
using System.Windows.Forms;
using HSDRaw.MEX;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXCostumeManagerControl : UserControl, IMEXControl
    {
        /// <summary>
        /// 
        /// </summary>
        public MEXCostumeManagerControl()
        {
            InitializeComponent();

            Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Costumes";
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void CheckEnable(MexDataEditor editor)
        {
        }
    }
}
