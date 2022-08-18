using System;
using WeifenLuo.WinFormsUI.Docking;

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

    public class EditorBase : DockContent
    {
        public virtual DockState DefaultDockState { get; } = DockState.Document;

        public virtual DataNode Node { get; set; }
    }

    public class SaveableEditorBase : EditorBase
    {
        public virtual void OnDatFileSave()
        {

        }
    }
}
