using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.KAR
{
    public class KAR_VehicleAttributes : IHSDNode
    {
        [FieldData(typeof(int))] public int KirbySitHeight { get; set; }
        [FieldData(typeof(int))] public int Unk1 { get; set; }
        [FieldData(typeof(float))] public float ModelScale { get; set; }
        [FieldData(typeof(float))] public float BaseOffense { get; set; }
        [FieldData(typeof(float))] public float StartCameraDistance { get; set; }
        [FieldData(typeof(float))] public float Unk2 { get; set; }
        [FieldData(typeof(float))] public float LengthOfStarShadow { get; set; }
    }
}
