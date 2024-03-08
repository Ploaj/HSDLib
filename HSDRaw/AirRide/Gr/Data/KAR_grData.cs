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

        public KAR_grSubAnimNode SubAnimNode { get => _s.GetReference<KAR_grSubAnimNode>(0x24); set => _s.SetReference(0x24, value); }

        public HSDAccessor EnemyNode { get => _s.GetReference<HSDAccessor>(0x28); set => _s.SetReference(0x28, value); }

        public HSDArrayAccessor<KAR_grItemNode> ItemNode { get => _s.GetReference<HSDArrayAccessor<KAR_grItemNode>>(0x2C); set => _s.SetReference(0x2C, value); }

        // x30 Runtime city event

        public KAR_grFogNode FogNode { get => _s.GetReference<KAR_grFogNode>(0x34); set => _s.SetReference(0x34, value); }

        public KAR_grRailCollNode RailCollNode { get => _s.GetReference<KAR_grRailCollNode>(0x38); set => _s.SetReference(0x38, value); }

        public KAR_grFGMNode FGMNode { get => _s.GetReference<KAR_grFGMNode>(0x3C); set => _s.SetReference(0x3C, value); }

        public HSDAccessor YakumonoNode { get => _s.GetReference<HSDAccessor>(0x40); set => _s.SetReference(0x40, value); }

        public HSDAccessor ReplayNode { get => _s.GetReference<HSDAccessor>(0x44); set => _s.SetReference(0x44, value); }

        public KAR_grCollisionTreeNode PartitionNode { get => _s.GetReference<KAR_grCollisionTreeNode>(0x48); set => _s.SetReference(0x48, value); }

        public KAR_grRespawnNode RespawnNode { get => _s.GetReference<KAR_grRespawnNode>(0x4C); set => _s.SetReference(0x4C, value); }

        public HSDAccessor StadiumNode { get => _s.GetReference<HSDAccessor>(0x50); set => _s.SetReference(0x50, value); }
    }

    public class KAR_grRespawnNode : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        /// <summary>
        /// Indices refer to GlobalDeadPos in <see cref="KAR_grPositionNode"/>
        /// </summary>
        public HSDIntArray Indices { get => _s.GetReference<HSDIntArray>(0x00); set => _s.SetReference(0x00, value); }

        public int Count { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
    }
}
