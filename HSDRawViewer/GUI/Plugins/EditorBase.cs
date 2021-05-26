using System;

namespace HSDRawViewer.GUI.Plugins
{
    public class SupportedTypes : Attribute
    {
        public Type[] Types { get; }

        public SupportedTypes(Type[] types)
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
