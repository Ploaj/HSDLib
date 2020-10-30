using HSDRaw.Common;
using HSDRaw.MEX;
using HSDRaw.MEX.Sounds;
using HSDRaw.MEX.Stages;
using System.Linq;
using System.ComponentModel;
using HSDRawViewer.GUI.MEX.Tools;

namespace HSDRawViewer.GUI.MEX
{
    public class MEXStageExternalEntry
    {
        public MEX_StageIDTable IDTable = new MEX_StageIDTable();
        
        [DisplayName("Stage Internal ID"), Description(""), TypeConverter(typeof(StageInternalIDConverter))]
        public int StageInternalID { get => IDTable.StageID; set { IDTable.StageID = value; } }

        public override string ToString()
        {
            return StageInternalID < MEXConverter.stageIDValues.Count ? MEXConverter.stageIDValues[StageInternalID] : "StageID" + StageInternalID;
        }
    }

    public class MEXStageEntry
    {
        public MEX_Stage Stage = new MEX_Stage();
        public MEX_StageReverb Reverb = new MEX_StageReverb();
        public MEX_StageCollision Collision = new MEX_StageCollision();
        
        public MEX_ItemLookup ItemLookup = new MEX_ItemLookup();
        public MEXPlaylistEntry[] Playlist = new MEXPlaylistEntry[0];

        [Browsable(false), Category("0 - General"), DisplayName("Internal ID"), Description("")]
        public int InternalID { get => Stage.StageInternalID; set { Stage.StageInternalID = value; Collision.InternalID = value; } }

        [Category("0 - General"), DisplayName("File Path"), Description("")]
        public string FileName { get => Stage.StageFileName; set => Stage.StageFileName = value; }

        [Category("0 - General"), DisplayName("Unknown Stage Value"), Description("")]
        public int UnknownValue { get => Stage.UnknownValue; set => Stage.UnknownValue = value; }

        [Category("0 - General"), DisplayName("SSM"), Description(""), TypeConverter(typeof(SSMIDConverter))]
        public int SSM { get => Reverb.SSMID; set => Reverb.SSMID = (byte)value; }

        [Category("0 - General"), DisplayName("Reverb"), Description("")]
        public int ReverbValue { get => Reverb.Reverb; set => Reverb.Reverb = (byte)value; }

        [Category("0 - General"), DisplayName("Unknown Sound Data"), Description("")]
        public int Unknown { get => Reverb.Unknown; set => Reverb.Unknown = (byte)value; }
        
        [Category("0 - General"), DisplayName("Song Playlist"), Description("")]
        public MEXPlaylistEntry[] PlaylistEntries { get => Playlist; set => Playlist = value; }


        [Category("1 - Extra"), DisplayName("MEX Items"),Description("MEX Item lookup for Stage"), SerialIgnore()]
        public HSD_UShort[] Items { get => ItemLookup.Entries; set => ItemLookup.Entries = value; }

        //[Category("1 - Extra"), DisplayName("MEX Effects"), Description("MEX Effect lookup for Stage")]
        //public MEXEffectType[] Effects { get => EffectLookup.Entries; set => EffectLookup.Entries = value; }


        [Category("1 - Extra"), DisplayName("Moving Collision Points"), Description("")]
        public MEX_MovingCollisionPoint[] MovingCollisions
        {
            get => Stage.MovingCollisionPoint?.Array;
            set
            {
                if (value == null)
                    return;

                Stage.MovingCollisionPointCount = value.Length;
                if (value == null || value.Length == 0)
                {
                    Stage.MovingCollisionPoint = null;
                }
                else
                {
                    Stage.MovingCollisionPoint = new HSDRaw.HSDArrayAccessor<MEX_MovingCollisionPoint>();
                    Stage.MovingCollisionPoint.Array = value;
                }
            }
        }

        [Category("1 - Extra"), DisplayName("GOBJ Functions"), Description("")]
        public MEX_MapGOBJFunctions[] Functions
        {
            get => Stage.GOBJFunctions?.Array;
            set
            {
                if (value == null || value.Length == 0)
                {
                    Stage.GOBJFunctions = null;
                }
                else
                {
                    Stage.GOBJFunctions = new HSDRaw.HSDArrayAccessor<MEX_MapGOBJFunctions>();
                    Stage.GOBJFunctions.Array = value;
                }
            }
        }

        [Category("1 - Extra"), DisplayName("GOBJ Functions Pointer"), Description(""), TypeConverter(typeof(HexType))]
        public int FunctionsPointer
        {
            get
            {
                return Stage.GOBJFunctionsPointer;
            }
            set
            {
                Stage.GOBJFunctionsPointer = value;
            }
        }

        /*[Category("1 - Extra"), DisplayName("Collisions"), Description("")]
        public MEX_StageCollisionData[] CollisionData
        {
            get => Collision.Data?.Array;
            set
            {
                if (value == null || value.Length == 0)
                {
                    Stage.GOBJFunctions = null;
                }
                else
                {
                    Collision.Data = new HSDRaw.HSDFixedLengthPointerArrayAccessor<MEX_StageCollisionData>();
                    Collision.Data.Array = value;
                }
            }
        }*/

        [Category("2 - Functions"), DisplayName("OnStageInit"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnStageInit { get => Stage.OnStageInit; set => Stage.OnStageInit = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown1"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown1 { get => Stage.OnUnknown1; set => Stage.OnUnknown1 = value; }

        [Category("2 - Functions"), DisplayName("OnStageLoad"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnStageLoad { get => Stage.OnStageLoad; set => Stage.OnStageLoad = value; }

        [Category("2 - Functions"), DisplayName("OnStageGo"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnStageGo { get => Stage.OnStageGo; set => Stage.OnStageGo = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown2"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown2 { get => Stage.OnUnknown2; set => Stage.OnUnknown2 = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown3"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown3 { get => Stage.OnUnknown3; set => Stage.OnUnknown3 = value; }

        [Category("2 - Functions"), DisplayName("OnUnknown4"), Description(""), TypeConverter(typeof(HexType))]
        public uint OnUnknown4 { get => Stage.OnUnknown4; set => Stage.OnUnknown4 = value; }

        public override string ToString()
        {
            return FileName == null ? "" : FileName;
        }

        public MEX_Playlist GetPlaylist()
        {
            return new MEX_Playlist()
            {
                MenuPlayListCount = Playlist.Length,
                MenuPlaylist = new HSDRaw.HSDArrayAccessor<MEX_PlaylistItem>()
                {
                    Array = Playlist.Select(e => new MEX_PlaylistItem() { HPSID = (ushort)e.MusicID, ChanceToPlay = e.PlayChanceValue }).ToArray()
                }
            };
        }
    }
}
