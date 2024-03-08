using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRawViewer.Rendering;
using HSDRawViewer.Rendering.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    public interface IGrEditor
    {
        public void Update();

        public void Render(Camera cam, LiveJObj model, object selected);
    }

}