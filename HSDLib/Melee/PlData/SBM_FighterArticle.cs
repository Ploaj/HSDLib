using HSDLib.Common;

namespace HSDLib.Melee.PlData
{
    public class SBM_FighterArticles : IHSDNode
    {
        public SBM_FighterArticle Article1 { get; set; }

        public SBM_FighterArticle Article2 { get; set; }

        public SBM_FighterArticle Article3 { get; set; }
    }

    public class SBM_FighterArticle : IHSDNode
    {
        public int Unknown1 { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        public SBM_ArticleModel model { get; set; }

        public int Unknown6 { get; set; }
    }

    public class SBM_ArticleModel : IHSDNode
    {
        public HSD_JOBJ Model { get; set; }

        public int Unknown2 { get; set; }

        public int Unknown3 { get; set; }

        public int Unknown4 { get; set; }
    }
}
