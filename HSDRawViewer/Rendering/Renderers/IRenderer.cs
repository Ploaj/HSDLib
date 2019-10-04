using HSDRaw;
using System;
using System.Windows.Forms;

namespace HSDRawViewer.Rendering.Renderers
{
    public interface IRenderer
    {
        Type[] SupportedTypes { get; }

        ToolStrip ToolStrip { get; }

        void Clear();
        
        void Render(HSDAccessor a, int windowWidth, int windowHeight);
    }
}
