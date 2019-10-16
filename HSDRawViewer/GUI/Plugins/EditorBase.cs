using System;

namespace HSDRawViewer.GUI.Plugins
{
    public interface EditorBase
    {
        WeifenLuo.WinFormsUI.Docking.DockState DefaultDockState { get; }

        Type[] SupportedTypes { get; }

        DataNode Node { get; set; }
    }
}
