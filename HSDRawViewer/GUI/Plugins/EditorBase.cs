using System;
using HSDRaw;

namespace HSDRawViewer.GUI.Plugins
{
    public interface EditorBase 
    {
        Type[] SupportedTypes { get; }

        HSDAccessor GetAccessor();

        void SetAccessor(HSDAccessor a);
    }
}
