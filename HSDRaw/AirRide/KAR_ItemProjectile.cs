using System;
using System.Collections.Generic;
using System.Text;

namespace HSDRaw.AirRide
{
    public class KAR_ItemProjectile : HSDAccessor
    {
        public override int TrimmedSize => 0x18;

        public KAR_ItemProjectileCommonAttributes CommonAttributes { get => _s.GetReference<KAR_ItemProjectileCommonAttributes>(0x00); set => _s.SetReference(0x00, value); }

        public HSDAccessor UniqueAttributes { get => _s.GetReference<HSDAccessor>(0x04); set => _s.SetReference(0x04, value); }

        public KAR_ItemModel Model { get => _s.GetReference<KAR_ItemModel>(0x08); set => _s.SetReference(0x08, value); }

        public HSDArrayAccessor<KAR_ItemAnimation> AnimatonBanks { get => _s.GetReference<HSDArrayAccessor<KAR_ItemAnimation>>(0x0C); set => _s.SetReference(0x0C, value); }

        public KAR_ItemCollisions Collisions { get => _s.GetReference<KAR_ItemCollisions>(0x10); set => _s.SetReference(0x10, value); }

        public KAR_ItemProjectileUnknown Unknown { get => _s.GetReference<KAR_ItemProjectileUnknown>(0x14); set => _s.SetReference(0x14, value); }

    }

    public class KAR_ItemProjectileCommonAttributes : HSDAccessor
    {
        public override int TrimmedSize => 0x10;
    }

    public class KAR_ItemProjectileUnknown : HSDAccessor
    {
        public override int TrimmedSize => 0x10;
    }

}
