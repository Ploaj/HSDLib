using System.ComponentModel;

namespace HSDRaw.AirRide.Gr.Data
{
    public class KAR_grEventParamNode : HSDAccessor
    {
        public override int TrimmedSize => 0x08;

        public KAR_grEventConfig EventConfig { get => _s.GetReference<KAR_grEventConfig>(0x00); set => _s.SetReference(0x00, value); }

        public HSDArrayAccessor<KAR_grEventParam> EventParams { get => _s.GetReference<HSDArrayAccessor<KAR_grEventParam>>(0x04); set => _s.SetReference(0x04, value); }
    }

    public class KAR_grEventParam : HSDAccessor
    {
        public override int TrimmedSize => 0x14;

        [Description("Index of BGM to use during the event.")]
        public int BgmIndex { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [Description("Index of sky (found in GrData file) to use for this event.")]
        public int SkyIndex { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [Description("Start position index (found in GrData->PositionNode->Eventpos) to use for this event.")]
        public int PositionStart { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [Description("Number of positions following the PositionStart to use.")]
        public int PositionNum { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        [Description("Unique param data for the event.")]
        public HSDAccessor Param { get => _s.GetReference<HSDAccessor>(0x10); set => _s.SetReference(0x10, value); }
    }

    public class KAR_grEventConfig : HSDAccessor
    {
        public override int TrimmedSize => 0x34;

        [Description("Minimum delay (in frames) before an event can start.")]
        public int DelayMin { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [Description("Maximum delay (in frames) before an event can start.")]
        public int DelayMax { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [Description("Chance event can occur after the delay. The delay will restart.")]
        public int OccurChance { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }

        [Description("")]
        public int SkipChance { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }

        [Description("")]
        public int x10 { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }

        [Description("Minimum delay (in frames) that must elapse before the first events starts.")]
        public int MinTime { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }

        [Description("Maximum number of event history entries. This is hardcoded at a max of 16, so don't exceed that.")]
        public int PrevKindMax { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }

        [Description("Time (in frames) to fade out the music.")]
        public int MusicFadeoutFrames { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }

        [Description("Start up delay (in frames) when an event is starting.")]
        public int StartingDelay { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }

        [Description("Closing delay (in frames) when an event is ending.")]
        public int CleanupDelay { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }

        [Description("Time (in frames) to display event HUD text.")]
        public int HudDisplayFrames { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }

        [Description("The weighted chances for each event to appear based on the chosen stadium kind.")]
        public HSDArrayAccessor<KAR_grEventChance> StadiumEventChances { get => _s.GetReference<HSDArrayAccessor<KAR_grEventChance>>(0x2C); set => _s.SetReference(0x2C, value); }

        [Description("Per event params to control various general aspects of the event.")]
        public HSDArrayAccessor<KAR_grEventGeneralConfig> EventConfigs { get => _s.GetReference<HSDArrayAccessor<KAR_grEventGeneralConfig>>(0x30); set => _s.SetReference(0x30, value); }

    }

    public class KAR_grEventChance : HSDAccessor
    {
        public override int TrimmedSize => 0x40;

        public int DynaBlade        { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }
        public int Tac              { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }
        public int Meteor           { get => _s.GetInt32(0x08); set => _s.SetInt32(0x08, value); }
        public int Pillar           { get => _s.GetInt32(0x0C); set => _s.SetInt32(0x0C, value); }
        public int RunAmok          { get => _s.GetInt32(0x10); set => _s.SetInt32(0x10, value); }
        public int RestorationArea  { get => _s.GetInt32(0x14); set => _s.SetInt32(0x14, value); }
        public int RailFire         { get => _s.GetInt32(0x18); set => _s.SetInt32(0x18, value); }
        public int SameItem         { get => _s.GetInt32(0x1C); set => _s.SetInt32(0x1C, value); }
        public int Lighthouse       { get => _s.GetInt32(0x20); set => _s.SetInt32(0x20, value); }
        public int SecretChamber    { get => _s.GetInt32(0x24); set => _s.SetInt32(0x24, value); }
        public int Prediction       { get => _s.GetInt32(0x28); set => _s.SetInt32(0x28, value); }
        public int MachineFormation { get => _s.GetInt32(0x2C); set => _s.SetInt32(0x2C, value); }
        public int Ufo              { get => _s.GetInt32(0x30); set => _s.SetInt32(0x30, value); }
        public int Bounce           { get => _s.GetInt32(0x34); set => _s.SetInt32(0x34, value); }
        public int Fog              { get => _s.GetInt32(0x38); set => _s.SetInt32(0x38, value); }
        public int FakePowerups     { get => _s.GetInt32(0x3C); set => _s.SetInt32(0x3C, value); }
    }

    public class KAR_grEventGeneralConfig : HSDAccessor
    {
        public override int TrimmedSize => 0x0C;

        [Description("Used to help ensure diversity (can be 0 or 1).")]
        public int Category { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        [Description("Event duration (in frames).")]
        public int Duration { get => _s.GetInt32(0x04); set => _s.SetInt32(0x04, value); }

        [Description("Whether this event can only happen once per trial.")]
        public byte OneShot { get => _s.GetByte(0x08); set => _s.SetByte(0x08, value); }

        [Description("Whether this event plays siren + fades music + changes sky.")]
        public byte UseSiren { get => _s.GetByte(0x09); set => _s.SetByte(0x09, value); }

        public byte x0A { get => _s.GetByte(0x0A); set => _s.SetByte(0x0A, value); }

        public byte x0B { get => _s.GetByte(0x0B); set => _s.SetByte(0x0B, value); }
    }
}
