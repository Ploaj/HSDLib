using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.MaterialAnimation
{
    public class HSD_MatAnimJoint : IHSDTree<HSD_MatAnimJoint>
    {
        [FieldData(typeof(HSD_MatAnimJoint))]
        public override HSD_MatAnimJoint Child { get; set; }

        [FieldData(typeof(HSD_MatAnimJoint))]
        public override HSD_MatAnimJoint Next { get; set; }

        [FieldData(typeof(HSD_MatAnim))]
        public HSD_MatAnim MaterialAnimation { get; set; }
    }
}
