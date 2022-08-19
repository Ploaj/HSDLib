using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Rendering
{
    public class RenderObject
    {
        protected bool Initialized { get; set; }

        public void Initialize()
        {
            Initialized = true;
        }
    }
}
