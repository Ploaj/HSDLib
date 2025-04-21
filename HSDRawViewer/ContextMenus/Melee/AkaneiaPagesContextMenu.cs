using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.MEX.Akaneia;
using HSDRawViewer.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSDRawViewer.ContextMenus.Melee
{
    public class AK_StageIconNew
    {
        public int JointIndex;
        public int ExternalID;
        public AkStageType StageType;
        public StageIconFlags Flags;
        public float CollisionWidth;
        public float CollisionHeight;
        public float ScaleX;
        public float ScaleY;
        public string IconTexture;
        public string NameTexture;
        public int EmblemIndex;
        public int PreviewIndex;
    }

    public class AkaneiaPageContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(AK_StagePage) };

        public AkaneiaPageContextMenu() : base()
        {
            ToolStripMenuItem export = new("Export to Folder");
            export.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_StagePage page)
                {
                    //var f = FileIO.OpenFolder();
                    //if (f == null)
                    //    return;

                    //List<AK_StageIconNew> newicons = new List<AK_StageIconNew>();

                    //int index = 1;
                    //foreach (var i in page.Icons.Array)
                    //{
                    //    var icon_path = $"t{index.ToString("D3")}_icon.png";
                    //    var name_path = $"t{index.ToString("D3")}_name.png";

                    //    if (i.IconMatAnim.Child != null)
                    //    {
                    //        i.IconMatAnim.Child.MaterialAnimation.Next.TextureAnimation.ToTOBJs()[0].SaveImagePNG($"{f}\\{icon_path}");
                    //    }
                    //    else
                    //    {
                    //        icon_path = "";
                    //    }
                    //    i.NameTexture.SaveImagePNG($"{f}\\{name_path}");

                    //    newicons.Add(new AK_StageIconNew()
                    //    {
                    //        JointIndex = index++,
                    //        ExternalID = i.ExternalID,
                    //        StageType = i.StageType,
                    //        Flags = i.Flags,
                    //        CollisionWidth = i.Width,
                    //        CollisionHeight = i.Height,
                    //        ScaleX = i.IconAnimJoint.AOBJ != null ? i.IconAnimJoint.AOBJ.FObjDesc.GetDecodedKeys()[0].Value : 1,
                    //        ScaleY = i.IconAnimJoint.AOBJ != null ? i.IconAnimJoint.AOBJ.FObjDesc.Next.GetDecodedKeys()[0].Value : 1,
                    //        IconTexture = icon_path,
                    //        NameTexture = name_path,
                    //        EmblemIndex = 0,
                    //        PreviewIndex = (ushort)i.PreviewModelIndex,
                    //    });
                    //}

                    //// export yaml
                    //using (StreamWriter streamWriter = new StreamWriter(f + "\\icon_data.yml"))
                    //{
                    //    var serializer = new SerializerBuilder()
                    //        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    //        .Build();
                    //    serializer.Serialize(streamWriter, newicons);
                    //}
                }
            };
            Items.Add(export);

            ToolStripMenuItem import = new("Import from Folder");
            import.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_StagePage page)
                {
                    string f = FileIO.OpenFolder();
                    if (f == null)
                        return;

                    // load icon file
                    string iconfile = $"{f}\\icon_data.yml";
                    AK_StageIconNew[] icons = null;
                    using (StreamReader r = new(iconfile))
                    {
                        IDeserializer deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
                        icons = deserializer.Deserialize<AK_StageIconNew[]>(r);
                    }

                    // load position model
                    string position_file = $"{f}\\position_joint.dat";
                    if (File.Exists(position_file))
                        page.PositionJoint = new HSDRawFile(position_file).Roots[0].Data as HSD_JOBJ;

                    // load position animation data
                    string position_anim_file = $"{f}\\position_animjoint.dat";
                    if (File.Exists(position_anim_file))
                        page.PositionAnimEnter = new HSDRawFile(position_anim_file).Roots[0].Data as HSD_AnimJoint;

                    // import icons and name textures
                    Dictionary<string, ushort> iconToIndex = new();
                    Dictionary<string, ushort> nameToIndex = new();
                    List<HSD_TOBJ> icon_tobjs = new();
                    List<HSD_TOBJ> name_tobjs = new();
                    foreach (AK_StageIconNew v in icons)
                    {
                        string icon_file = $"{f}\\{v.IconTexture}";
                        if (File.Exists(icon_file) &&
                            !iconToIndex.ContainsKey(v.IconTexture))
                        {
                            HSD_TOBJ tobj = new();
                            tobj.ImportImage(icon_file, HSDRaw.GX.GXTexFmt.CI8, HSDRaw.GX.GXTlutFmt.RGB565);
                            iconToIndex.Add(v.IconTexture, (ushort)icon_tobjs.Count);
                            icon_tobjs.Add(tobj);
                        }

                        string name_file = $"{f}\\{v.NameTexture}";
                        if (File.Exists(name_file) &&
                            !nameToIndex.ContainsKey(v.NameTexture))
                        {
                            HSD_TOBJ tobj = new();
                            tobj.ImportImage(name_file, HSDRaw.GX.GXTexFmt.I4, HSDRaw.GX.GXTlutFmt.RGB565);
                            nameToIndex.Add(v.NameTexture, (ushort)name_tobjs.Count);
                            name_tobjs.Add(tobj);
                        }
                    }

                    // create anims
                    page.IconTextures = new HSD_MatAnimJoint();
                    page.IconTextures.Child = new HSD_MatAnimJoint();
                    page.IconTextures.Child.MaterialAnimation = new HSD_MatAnim();
                    page.IconTextures.Child.MaterialAnimation.Next = new HSD_MatAnim();
                    page.IconTextures.Child.MaterialAnimation.Next.TextureAnimation = new HSD_TexAnim();
                    page.IconTextures.Child.MaterialAnimation.Next.TextureAnimation.FromTOBJs(icon_tobjs, true);

                    page.NameTextures = new HSD_MatAnimJoint();
                    page.NameTextures.MaterialAnimation = new HSD_MatAnim();
                    page.NameTextures.MaterialAnimation.TextureAnimation = new HSD_TexAnim();
                    page.NameTextures.MaterialAnimation.TextureAnimation.FromTOBJs(name_tobjs, true);


                    // import icon data
                    page.Count = icons.Length;
                    page.Icons = new HSDArrayAccessor<AK_StageIcon>();
                    page.Icons.Array = icons.Select((e, i) => new AK_StageIcon()
                    {
                        JointIndex = (short)e.JointIndex,
                        ExternalID = (short)e.ExternalID,
                        StageType = e.StageType,
                        Flags = e.Flags,
                        Width = e.CollisionWidth,
                        Height = e.CollisionHeight,
                        ScaleX = e.ScaleX,
                        ScaleY = e.ScaleY,
                        IconIndex = (ushort)(iconToIndex.ContainsKey(e.IconTexture) ? iconToIndex[e.IconTexture] : 0),
                        NameIndex = (ushort)(nameToIndex.ContainsKey(e.NameTexture) ? nameToIndex[e.NameTexture] : 0),
                        EmblemIndex = (ushort)e.EmblemIndex,
                        PreviewIndex = (short)e.PreviewIndex,
                    }).ToArray();
                }
            };
            Items.Add(import);
        }
    }

    public class Stage_IDs : HSDAccessor
    {
        public override int TrimmedSize => 0x8;

        public int count { get => _s.GetInt32(0x00); set => _s.SetInt32(0x00, value); }

        public HSDUShortArray array { get => _s.GetReference<HSDUShortArray>(0x04); set => _s.SetReference(0x04, value); }
    }


    public class AkaneiaPagesContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(AK_StagePages) };

        public AkaneiaPagesContextMenu() : base()
        {
            ToolStripMenuItem genPages = new("Generate Random Stage IDs");
            genPages.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is AK_StagePages pages)
                {
                    string f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter, "MxPtSSSRn.dat");
                    if (f != null)
                    {
                        HSDRawFile file = System.IO.File.Exists(f) ? new HSDRaw.HSDRawFile(f) : new HSDRaw.HSDRawFile();

                        ushort[] ids = pages.Array.SelectMany(e => e.Icons.Array.Select(e => (ushort)e.ExternalID)).Distinct().Where(e => e != 0).ToArray();

                        Stage_IDs data = new();
                        data.count = ids.Length;
                        data.array = new HSDUShortArray();
                        data.array.Array = ids;

                        HSDRootNode stage_id_node = file.Roots.Find(e => e.Name.Equals("stage_ids"));

                        if (stage_id_node == null)
                        {
                            stage_id_node = new HSDRaw.HSDRootNode()
                            {
                                Name = "stage_ids"
                            };
                            file.Roots.Add(stage_id_node);
                        }
                        stage_id_node.Data = data;

                        file.Save(f);
                    }
                }
            };
            Items.Add(genPages);
        }
    }
}
