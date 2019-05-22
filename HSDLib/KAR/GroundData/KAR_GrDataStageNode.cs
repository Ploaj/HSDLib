using System;

namespace HSDLib.KAR
{
    public class KAR_GrDataStageNode : IHSDNode
    {
        public uint Unk1 { get; set; }
        
        public float MachineAccel { get; set; }
        
        public float StageScale { get; set; }
        
        public float UnkGravity { get; set; }
        
        public float GravityX { get; set; }
        
        public float GravityY { get; set; }
        
        public float GravityZ { get; set; }
        
        public uint FogFlags { get; set; }
        
        public float UnkItemRestituion { get; set; }
        
        public float UnkF1 { get; set; }
        
        public float UnkF2 { get; set; }
        
        public float UnkF3 { get; set; }
        
        public float UnkF4 { get; set; }
        
        public float UnkF5 { get; set; }
        
        public float UnkF6 { get; set; }
        
        public float UnkF7 { get; set; }
        
        public float CoRWall { get; set; }
        
        public float CoRBreakableObjects { get; set; }
        
        public float CoRMovingDisks { get; set; }
        
        public float CoRUnk1 { get; set; }
        
        public float CoRUnk2 { get; set; }
        
        public float CoRUnk3 { get; set; }
        
        public float CoRUnk4 { get; set; }
        
        public float CoRUnk5 { get; set; }
        
        public float MinimapScale { get; set; }
        
        public float MinimapPlayerX { get; set; }
        
        public float MinimapPlayerY { get; set; }
        
        public float MinimapPlayerZ { get; set; }
        
        public uint Unused1 { get; set; }
        
        public uint Unused2 { get; set; }
        
        public KAR_StageNodeFloats UnkFloats1 { get; set; }
        
        public KAR_StageNodeFloats UnkFloats2 { get; set; }
        
        public uint Flags { get; set; }
        
        public float UnusedAccelerationBoostPadH { get; set; }
        
        public float UnusedAccelerationBoostPadH2 { get; set; }
        
        public float UnusedAccelerationTimeL { get; set; }
        
        public float AccelerationBoostPadL { get; set; }
        
        public float AccelerationBoostPadL2 { get; set; }
        
        public float AccelerationTimeL { get; set; }
        
        public float AccelerationBoostGateH { get; set; }
        
        public float AccelerationBoostGateH2 { get; set; }
        
        public float AccelerationTimeBoostGateH { get; set; }
        
        public float AccelerationBoostGateL { get; set; }
        
        public float AccelerationBoostGateL2 { get; set; }
        
        public float AccelerationTimeBoostGateL { get; set; }
        
        public float AccelerationBoostRing { get; set; }
        
        public float AccelerationBoostRing2 { get; set; }
        
        public float AccelerationTimeBoostRing { get; set; }
        
        public float UnkUnused1 { get; set; }
        
        public float UnkUnused2 { get; set; }
        
        public float UnkUnused3 { get; set; }
        
        public float OoBMinXArea { get; set; }
        
        public float OoBMinYArea { get; set; }
        
        public float OoBMinZArea { get; set; }
        
        public float OoBMaxXArea { get; set; }
        
        public float OoBMaxYArea { get; set; }
        
        public float OoBMaxZArea { get; set; }

        [System.ComponentModel.Browsable(false)]
        public uint PointerToPointer { get; set; }

        public override void Open(HSDReader Reader)
        {
            base.Open(Reader);
            Console.WriteLine("TODO: Stage Node Pointer " + PointerToPointer.ToString("X"));
        }
    }

    public class KAR_StageNodeFloats : IHSDNode
    {
        public float Float1 { get; set; }
        
        public float Float2 { get; set; }
        
        public float Float3 { get; set; }
        
        public float Float4 { get; set; }
        
        public float Float5 { get; set; }
        
        public float Float6 { get; set; }
    }
}
