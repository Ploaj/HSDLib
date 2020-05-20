using System;

namespace HSDRawViewer.GUI.Plugins
{
    public class SupportedTypesAttribute : Attribute
    {
        public Type[] Types { get; }

        public SupportedTypesAttribute(Type[] types)
        {
            Types = types;
        }
    }

    public interface EditorBase
    {
        WeifenLuo.WinFormsUI.Docking.DockState DefaultDockState { get; }

        DataNode Node { get; set; }
    }
}
