namespace HSDLib.KAR
{
    public class KAR_GrData : IHSDNode
    {
        public uint Unknown1 { get; set; }

        public KAR_GrDataStageNode StageNode { get; set; }

        public uint Unknown2 { get; set; }

        public uint ModelNode { get; set; }

        public uint MotionNode { get; set; }

        public KAR_GrLightGroup LightNode { get; set; }

        public KAR_GrCollisionNode CollisionNode { get; set; }

        public KAR_GrSplineNode SplineNode { get; set; }

        public KAR_GrPositionNode PositionNode { get; set; }

        public int Node10 { get; set; }// node 10

        public int EnemyNode { get; set; }// enemy node

        public int ItemNode { get; set; }// item node

        public int EventNode { get; set; }// event node

        public KAR_GrFogNode FogNode { get; set; }

        public int RailCollisionNode { get; set; }// rail collision?

        public int SoundEffectNode { get; set; }// sound effect node

        public KAR_GrYakumonoNode YakumonoNode { get; set; }

        public int ReplayNode { get; set; }// unknown replay node?

        public KAR_GrPartitionNode PartitionNode { get; set; }// partition node collision tree

        public int Node20 { get; set; }// node 20 respawns?

        public int StadiumNode { get; set; }// unknown

        //public int UnknownNode1 { get; set; }// unknown

        //public int UnknownNode2 { get; set; }// unknown

        //public int UnknownNode3 { get; set; }// unknown
    }
}
