using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using HSDRaw.MEX;
using HSDRaw;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXEffectControl : UserControl, IMEXControl
    {
        /// <summary>
        /// 
        /// </summary>
        public MEX_Data MexData
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e._data;
                return null;
            }
        }

        public MexDataEditor MexDataEditor
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e;
                return null;
            }
        }
        
        public MEXEffectEntry[] Effects { get; set; }

        public MEX_EffectTypeLookup[] MEX_Effects { get; set; }

        public MEXEffectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Effects";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            bool updated = false;

            // legacy update
            if(data.EffectTable._s.Length != new MEX_EffectData().TrimmedSize)
            {
                var strings = data.EffectTable._s;
                data.EffectTable = new MEX_EffectData();
                var length = strings.Length;
                data.EffectTable.EffectFiles = new HSDArrayAccessor<MEX_EffectFiles>() { _s = strings };
                data.EffectTable.RuntimeUnk1 = new HSDAccessor() { _s = new HSDStruct(0x60) };
                data.EffectTable.RuntimeUnk3 = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                data.EffectTable.RuntimeTexGrNum = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                data.EffectTable.RuntimeTexGrData = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                data.EffectTable.RuntimeUnk4 = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                data.EffectTable.RuntimePtclLast = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                data.EffectTable.RuntimePtclData = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                data.EffectTable.RuntimeLookup = new HSDAccessor() { _s = new HSDStruct(4 * length) };
                updated = true;
            }

            if(updated)
                MessageBox.Show("Effect Node Updated");

            Effects = new MEXEffectEntry[data.EffectTable.EffectFiles.Length];
            for (int i = 0; i < Effects.Length; i++)
            {
                Effects[i] = new MEXEffectEntry()
                {
                    FileName = data.EffectTable.EffectFiles[i].FileName,
                    Symbol = data.EffectTable.EffectFiles[i].Symbol,
                };
            }
            effectEditor.SetArrayFromProperty(this, "Effects");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            data.MetaData.NumOfEffects = Effects.Length;
            data.EffectTable = new MEX_EffectData();
            data.EffectTable.EffectFiles = new HSDArrayAccessor<MEX_EffectFiles>();
            foreach (var v in Effects)
            {
                data.EffectTable.EffectFiles.Add(new MEX_EffectFiles()
                {
                    FileName = v.FileName,
                    Symbol = v.Symbol
                });
            }

            data.EffectTable.RuntimeUnk1 = new HSDAccessor() { _s = new HSDStruct( 0x60 )};
            data.EffectTable.RuntimeUnk3 = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
            data.EffectTable.RuntimeTexGrNum = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
            data.EffectTable.RuntimeTexGrData = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
            data.EffectTable.RuntimeUnk4 = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
            data.EffectTable.RuntimePtclLast = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
            data.EffectTable.RuntimePtclData = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
            data.EffectTable.RuntimeLookup = new HSDAccessor() { _s = new HSDStruct(4 * Effects.Length) };
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {
            effectEditor.ResetBindings();
        }

        /// <summary>
        /// Adds a new MEX effect file to table
        /// </summary>
        /// <param name="item"></param>
        /// <returns>added mex file id</returns>
        public int AddMEXEffectFile(MEXEffectEntry item)
        {
            return effectEditor.AddItem(item);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public bool SafeRemoveEffectFile(int index)
        {
            if (!MexDataEditor.FighterControl.EffectInUse(index))
            {
                effectEditor.RemoveAt(index);
                return true;
            }

            return false;
        }


        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void effectEditor_ArrayUpdated(object sender, EventArgs e)
        {
            MEXConverter.effectValues.Clear();
            MEXConverter.effectValues.AddRange(Effects.Select(ef => ef.FileName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void effectEditor_OnItemRemove(RemovedItemEventArgs e)
        {
            var index = e.Index;

            foreach (var v in MexDataEditor.FighterControl.FighterEntries)
            {
                if (v.EffectIndex == index)
                    v.EffectIndex = 0;

                if (v.EffectIndex > index)
                    v.EffectIndex--;

                if (v.KirbyEffectID == index)
                    v.KirbyEffectID = 0;

                if (v.KirbyEffectID > index)
                    v.KirbyEffectID--;
            }
        }

        #endregion

    }
}
