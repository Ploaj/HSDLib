using HSDRaw.Common;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grPositionNode : HSDAccessor
    {
        public override int TrimmedSize => 0x38;

        public HSD_JOBJ PositionJoint { get => _s.GetReference<HSD_JOBJ>(0x00); set => _s.SetReference(0x00, value); }

        public KAR_grPositionList Startpos { get => _s.GetReference<KAR_grPositionList>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_grPositionList Enemypos { get => _s.GetReference<KAR_grPositionList>(0x08); set => _s.SetReference(0x08, value); }

        public KAR_grPositionList Gravitypos { get => _s.GetReference<KAR_grPositionList>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_grPositionList Airflowpos { get => _s.GetReference<KAR_grPositionList>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_grPositionList Conveyorpos { get => _s.GetReference<KAR_grPositionList>(0x14); set => _s.SetReference(0x14, value); }

        public KAR_grPositionList ItemPos { get => _s.GetReference<KAR_grPositionList>(0x18); set => _s.SetReference(0x18, value); }

        public KAR_grPositionList Eventpos { get => _s.GetReference<KAR_grPositionList>(0x1C); set => _s.SetReference(0x1C, value); }

        public KAR_grPositionList Vehiclepos { get => _s.GetReference<KAR_grPositionList>(0x20); set => _s.SetReference(0x20, value); }

        public KAR_grPositionList GlobalDeadPos { get => _s.GetReference<KAR_grPositionList>(0x24); set => _s.SetReference(0x24, value); }

        public KAR_grPositionList LocalDeadPos { get => _s.GetReference<KAR_grPositionList>(0x28); set => _s.SetReference(0x28, value); }

        public KAR_grPositionList Yakumonopos { get => _s.GetReference<KAR_grPositionList>(0x2C); set => _s.SetReference(0x2C, value); }

        public KAR_grPositionList ItemAreaPos { get => _s.GetReference<KAR_grPositionList>(0x30); set => _s.SetReference(0x30, value); }

        public KAR_grPositionList VehicleAreapos { get => _s.GetReference<KAR_grPositionList>(0x34); set => _s.SetReference(0x34, value); }

    }

}
