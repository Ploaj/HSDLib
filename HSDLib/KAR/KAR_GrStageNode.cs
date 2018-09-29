using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDLib.KAR
{
    public class KAR_GrStageNode : IHSDNode
    {
        [FieldData(typeof(uint))]
        public uint Unk1 { get; set; }

        [FieldData(typeof(float))]
        public float MachineAccel { get; set; }

        [FieldData(typeof(float))]
        public float StageScale { get; set; }

        [FieldData(typeof(float))]
        public float UnkGravity { get; set; }

        [FieldData(typeof(float))]
        public float GravityX { get; set; }

        [FieldData(typeof(float))]
        public float GravityY { get; set; }

        [FieldData(typeof(float))]
        public float GravityZ { get; set; }

        [FieldData(typeof(uint))]
        public uint FogFlags { get; set; }

        [FieldData(typeof(float))]
        public float UnkItemRestituion { get; set; }

        [FieldData(typeof(float))]
        public float UnkF1 { get; set; }

        [FieldData(typeof(float))]
        public float UnkF2 { get; set; }

        [FieldData(typeof(float))]
        public float UnkF3 { get; set; }

        [FieldData(typeof(float))]
        public float UnkF4 { get; set; }

        [FieldData(typeof(float))]
        public float UnkF5 { get; set; }

        [FieldData(typeof(float))]
        public float UnkF6 { get; set; }

        [FieldData(typeof(float))]
        public float UnkF7 { get; set; }

        [FieldData(typeof(float))]
        public float CoRWall { get; set; }

        [FieldData(typeof(float))]
        public float CoRBreakableObjects { get; set; }

        [FieldData(typeof(float))]
        public float CoRMovingDisks { get; set; }

        [FieldData(typeof(float))]
        public float CoRUnk1 { get; set; }

        [FieldData(typeof(float))]
        public float CoRUnk2 { get; set; }

        [FieldData(typeof(float))]
        public float CoRUnk3 { get; set; }

        [FieldData(typeof(float))]
        public float CoRUnk4 { get; set; }

        [FieldData(typeof(float))]
        public float CoRUnk5 { get; set; }

        [FieldData(typeof(float))]
        public float MinimapScale { get; set; }

        [FieldData(typeof(float))]
        public float MinimapPlayerX { get; set; }

        [FieldData(typeof(float))]
        public float MinimapPlayerY { get; set; }

        [FieldData(typeof(float))]
        public float MinimapPlayerZ { get; set; }

        [FieldData(typeof(uint))]
        public uint Unused1 { get; set; }

        [FieldData(typeof(uint))]
        public uint Unused2 { get; set; }

        [FieldData(typeof(KAR_StageNodeFloats))]
        public KAR_StageNodeFloats UnkFloats1 { get; set; }

        [FieldData(typeof(KAR_StageNodeFloats))]
        public KAR_StageNodeFloats UnkFloats2 { get; set; }

        [FieldData(typeof(uint))]
        public uint Flags { get; set; }

        [FieldData(typeof(float))]
        public float UnusedAccelerationBoostPadH { get; set; }

        [FieldData(typeof(float))]
        public float UnusedAccelerationBoostPadH2 { get; set; }

        [FieldData(typeof(float))]
        public float UnusedAccelerationTimeL { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostPadL { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostPadL2 { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationTimeL { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostGateH { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostGateH2 { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationTimeBoostGateH { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostGateL { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostGateL2 { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationTimeBoostGateL { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostRing { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationBoostRing2 { get; set; }

        [FieldData(typeof(float))]
        public float AccelerationTimeBoostRing { get; set; }

        [FieldData(typeof(float))]
        public float UnkUnused1 { get; set; }

        [FieldData(typeof(float))]
        public float UnkUnused2 { get; set; }

        [FieldData(typeof(float))]
        public float UnkUnused3 { get; set; }

        [FieldData(typeof(float))]
        public float OoBMinXArea { get; set; }

        [FieldData(typeof(float))]
        public float OoBMinYArea { get; set; }

        [FieldData(typeof(float))]
        public float OoBMinZArea { get; set; }

        [FieldData(typeof(float))]
        public float OoBMaxXArea { get; set; }

        [FieldData(typeof(float))]
        public float OoBMaxYArea { get; set; }

        [FieldData(typeof(float))]
        public float OoBMaxZArea { get; set; }

        [FieldData(typeof(uint), Viewable: false)]
        public uint PointerToPointer { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            Console.WriteLine("TODO: Stage Node Pointer " + PointerToPointer.ToString("X"));
        }
    }

    public class KAR_StageNodeFloats : IHSDNode
    {
        [FieldData(typeof(float))]
        public float Float1 { get; set; }

        [FieldData(typeof(float))]
        public float Float2 { get; set; }

        [FieldData(typeof(float))]
        public float Float3 { get; set; }

        [FieldData(typeof(float))]
        public float Float4 { get; set; }

        [FieldData(typeof(float))]
        public float Float5 { get; set; }

        [FieldData(typeof(float))]
        public float Float6 { get; set; }
    }
}
