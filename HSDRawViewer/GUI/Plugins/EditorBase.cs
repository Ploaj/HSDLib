using System;

namespace HSDRawViewer.GUI.Plugins
{
    public interface EditorBase
    {
        Type[] SupportedTypes { get; }

        DataNode Node { get; set; }
    }
}
