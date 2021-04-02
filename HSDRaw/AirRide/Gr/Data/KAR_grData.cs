namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grData : HSDAccessor
    {
        // The size of this node is inconsistant... but 0x54 is the smallest GrJump1.dat
        public override int TrimmedSize => 0x54;

        public int Unknown1 { get => _s.GetInt32(0x0); set => _s.SetInt32(0x0, value); }

        public KAR_grStageNode StageNode { get => _s.GetReference<KAR_grStageNode>(0x4); set => _s.SetReference(0x4, value); }

        public int Unknown2 { get => _s.GetInt32(0x8); set => _s.SetInt32(0x8, value); }

        // x0C Model Runtime Pointer

        // x10 Model Animation Pointer
        
        public KAR_grLightGroup LightNode { get => _s.GetReference<KAR_grLightGroup>(0x14); set => _s.SetReference(0x14, value); }
        
        public KAR_grCollisionNode CollisionNode { get => _s.GetReference<KAR_grCollisionNode>(0x18); set => _s.SetReference(0x18, value); }
        
        public KAR_grSplineNode SplineNode { get => _s.GetReference<KAR_grSplineNode>(0x1c); set => _s.SetReference(0x1c, value); }

        public KAR_grPositionNode PositionNode { get => _s.GetReference<KAR_grPositionNode>(0x20); set => _s.SetReference(0x20, value); }

        //public int Node10 { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }// node 10
        //public int EnemyNode { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }// enemy node
        //public int ItemNode { get => _s.GetInt32(0x2c); set => _s.SetInt32(0x2c, value); }// item node

        // x30 Runtime city event

        //public KAR_GrFogNode FogNode { get => _s.GetReference<KAR_GrFogNode>(0x34); set => _s.SetReference(0x34, value); }
        //public int RailCollisionNode { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }// rail collision?
        //public int SoundEffectNode { get => _s.GetInt32(0x3c); set => _s.SetInt32(0x3c, value); }// sound effect node
        //public KAR_GrYakumonoNode YakumonoNode { get => _s.GetReference<KAR_GrYakumonoNode>(0x40); set => _s.SetReference(0x40, value); }
        //public int ReplayNode { get => _s.GetInt32(0x44); set => _s.SetInt32(0x44, value); }// unknown replay node?

        public KAR_grCollisionTreeNode PartitionNode { get => _s.GetReference<KAR_grCollisionTreeNode>(0x48); set => _s.SetReference(0x48, value); }

        //public int Node20 { get => _s.GetInt32(0x4c); set => _s.SetInt32(0x4c, value); }// node 20 respawns?
        //public int StadiumNode { get => _s.GetInt32(0x50); set => _s.SetInt32(0x50, value); }// unknown
    }
}
