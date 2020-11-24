using HSDRaw.MEX;

namespace HSDRawViewer.GUI.MEX
{
    public interface IMEXControl
    {
        string GetControlName();
        void LoadData(MEX_Data data);
        void SaveData(MEX_Data data);
        void ResetDataBindings();
        void CheckEnable(MexDataEditor editor);
    }
}
