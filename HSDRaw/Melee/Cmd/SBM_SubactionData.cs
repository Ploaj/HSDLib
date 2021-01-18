namespace HSDRaw.Melee.Cmd
{
    public class SBM_FighterSubactionData : HSDAccessor
    {
        protected override int Trim()
        {
            _s.CanBeBuffer = false;
            _s.Align = false;
            return base.Trim();
        }
    }

    public class SBM_ItemSubactionData : SBM_FighterSubactionData
    {

    }

    public class SBM_ColorSubactionData : SBM_FighterSubactionData
    {

    }
}
