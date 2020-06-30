using HSDRaw;
using HSDRaw.Common;
using HSDRaw.MEX;
using HSDRaw.MEX.Menus;
using HSDRaw.MEX.Sounds;
using HSDRaw.MEX.Stages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HSDRawViewer.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class MxDtSettings
    {
        public bool IncludeMoveLogic { get; set; } = false;

        public bool IncludeItemStates { get; set; } = false;

        public bool IncludeMapGOBJs { get; set; } = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MxDtGenerator
    {
        /// <summary>
        /// Offsets in dol to data tables
        /// </summary>
        private static Dictionary<string, Tuple<int, int>> dolOffset = new Dictionary<string, Tuple<int, int>>()
        {

        // Menu

            { "CSSIconData", new Tuple<int, int>(0x3EDA48, 0x398) },
            { "SSSIconData", new Tuple<int, int>(0x3ED6D0, 0x3C0) },
            
	    // Fighter Data

            { "DefineIDs", new Tuple<int, int>(0x3B9DE0, 0x64) },
            { "CostumeIDs", new Tuple<int, int>(0x3D21A0, 0x68) },
            { "AnimCount", new Tuple<int, int>(0x3BDFC8, 0x108) },
            { "EffectIDs", new Tuple<int, int>(0x3BF6FC, 0x21) },
            { "ResultScale", new Tuple<int, int>(0x3D4058, 0x68) },
            { "SSMFileIDs", new Tuple<int, int>(0x3B83C0, 0x210) },
            { "RstRuntime", new Tuple<int, int>(0x3BF5FC, 0x100) },

	    // Fighter Functions

            { "OnLoad", new Tuple<int, int>(0x3BE154, 0x84) },
            { "OnDeath", new Tuple<int, int>(0x3BE1D8, 0x84) },
            { "OnUnknown", new Tuple<int, int>(0x3BE25C, 0x84) },
            { "MoveLogic", new Tuple<int, int>(0x3BE2E0, 0x84) },
            { "SpecialN", new Tuple<int, int>(0x3BE67C, 0x84) },
            { "SpecialNAir", new Tuple<int, int>(0x3BE5F8, 0x84) },
            { "SpecialS", new Tuple<int, int>(0x3BE3E8, 0x84) },
            { "SpecialSAir", new Tuple<int, int>(0x3BE574, 0x84) },
            { "SpecialHi", new Tuple<int, int>(0x3BE784, 0x84) },
            { "SpecialHiAir", new Tuple<int, int>(0x3BE46C, 0x84) },
            { "SpecialLw", new Tuple<int, int>(0x3BE700, 0x84) },
            { "SpecialLwAir", new Tuple<int, int>(0x3BE4F0, 0x84) },
            { "OnAbsorb", new Tuple<int, int>(0x3BE808, 0x84) },
            { "onItemPickup", new Tuple<int, int>(0x3BE88C, 0x84) },
            { "onMakeItemInvisible", new Tuple<int, int>(0x3BE910, 0x84) },
            { "onMakeItemVisible", new Tuple<int, int>(0x3BE994, 0x84) },
            { "onItemDrop", new Tuple<int, int>(0x3BEA18, 0x84) },
            { "onItemCatch", new Tuple<int, int>(0x3BEA9C, 0x84) },
            { "onUnknownItemRelated", new Tuple<int, int>(0x3BEB20, 0x84) },
            { "onUnknownCharacterModelFlags1", new Tuple<int, int>(0x3BEBA4, 0x84) },
            { "onUnknownCharacterModelFlags2", new Tuple<int, int>(0x3BEC28, 0x84) },
            { "onHit", new Tuple<int, int>(0x3BECAC, 0x84) },
            { "onUnknownEyeTextureRelated", new Tuple<int, int>(0x3BED30, 0x84) },
            { "onFrame", new Tuple<int, int>(0x3BEDB4, 0x84) },
            { "onActionStateChange", new Tuple<int, int>(0x3BEE38, 0x84) },
            { "onRespawn", new Tuple<int, int>(0x3BEEBC, 0x84) },
            { "onModelRender", new Tuple<int, int>(0x3BF0CC, 0x84) },
            { "onShadowRender", new Tuple<int, int>(0x3BF150, 0x84) },
            { "onUnknownMultijump", new Tuple<int, int>(0x3BF1D4, 0x84) },
            { "onActionStateChangeWhileEyeTextureIsChanged", new Tuple<int, int>(0x3BF258, 0x84) },
            { "onTwoEntryTable", new Tuple<int, int>(0x3BF5F4, 0x108) },

            // SSM

            { "SSM_BufferSizes", new Tuple<int, int>(0x3B94E4, 0x1C0) },
            { "SSM_LookupTable", new Tuple<int, int>(0x3B85D0, 0xE0) },

            // Item

            { "CommonItems", new Tuple<int, int>(0x3EE4C4, 0xA14) },
            { "FighterItems", new Tuple<int, int>(0x3F0100, 0x1BA8) },
            { "Pokemon", new Tuple<int, int>(0x3EF3CC, 0xB04) },
            { "StageItems", new Tuple<int, int>(0x3F0100, 0x6CC) },

            // kirby 

	        { "KirbyEffectIDs", new Tuple<int, int>(0x3C846C, 0x21) },

            { "KirbyAbility", new Tuple<int, int>(0x3C6CC8, 0x108) },
            { "KirbySpecialN", new Tuple<int, int>(0x3C6DD0, 0x84) },
            { "KirbySpecialNAir", new Tuple<int, int>(0x3C6E54, 0x84) },
            
	        // Stage Data

	        { "StageIDTable", new Tuple<int, int>(0x3E6960, 0xD68) },
            { "ReverbTable", new Tuple<int, int>(0x3B86B0, 0xD4) }, // is actually larger, but entries are unused
	        { "CollisionTable", new Tuple<int, int>(0x3BC248, 0x238) },


        };

        // these are shift jis so imma just manually enter them
        //private static readonly uint CharNameOffset = 0x3D1FDC;

        private static readonly uint CostumePointerOffset = 0x3BDEC0;
        private static readonly uint CostumeStringOffset = 0x3BF360;
        private static readonly uint ftDemoStringOffset = 0x3BF468;
        private static readonly uint CharAnimStringOffset = 0x3BF3E4;

        private static readonly uint CharStringOffset = 0x3BEF40;
        
        private static readonly uint SSMStringOffset = 0x3B8CFC;

        private static readonly uint MusicStringOffset = 0x3B9314;

        private static readonly uint EffectStringOffset = 0x3BD25C;

        private static readonly uint KirbyCapOffset = 0x3C79D0;
        private static readonly uint KirbyCostumeOffset = 0x3C83E8;

        private static readonly uint StageFunctionOffset = 0x3DCEDC;

        /// <summary>
        /// 
        /// </summary>
        public static void GenerateMxDt()
        {
            var dol = FileIO.OpenFile("DOL (*.dol)|*.dol");
            if (dol == null)
                return;

            var mxdt = FileIO.SaveFile("DAT (*.dat)|*.dat", "MxDt.dat");
            if (mxdt == null)
                return;

            var data = GenerateMexData(dol, new MxDtSettings());

            HSDRawFile f = new HSDRawFile();
            f.Roots.Add(new HSDRootNode()
            {
                Name = "mexData",
                Data = data
            });
            f.Save(mxdt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dolFile"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static MEX_Data GenerateMexData(string dolFile, MxDtSettings settings)
        {
            DOLScrubber dol = new DOLScrubber(dolFile);

            MEX_Data data = new MEX_Data();

            // generate meta data
            data.MetaData = new MEX_Meta()
            {
                NumOfInternalIDs = 33,
                NumOfExternalIDs = 33,
                NumOfCSSIcons = 26,
                NumOfSSSIcons = 30,
                NumOfSSMs = 55,
                NumOfMusic = 98,
                NumOfEffects = 51,
            };

            data.MetaData._s.SetInt16(0x00, 1); // Major

            if (settings.IncludeMoveLogic)
                data.MetaData.Flags |= MexFlags.ContainMoveLogic;
            if (settings.IncludeItemStates)
                data.MetaData.Flags |= MexFlags.ContainItemState;
            if (settings.IncludeMapGOBJs)
                data.MetaData.Flags |= MexFlags.ContainMapGOBJs;


            // generate menu table
            data.MenuTable = new MEX_MenuTable();
            data.MenuTable.Parameters = new MEX_MenuParameters()
            {
                CSSHandScale = 1
            };
            ExtractData(dol, data.MenuTable);


            // generate fighter table
            data.FighterData = new MEX_FighterData();
            ExtractData(dol, data.FighterData);

            // costume strings and runtime setup
            data.FighterData.CostumePointers = new HSDArrayAccessor<MEX_CostumeRuntimePointers>() { _s = dol.GetStruct(new Tuple<int, int>((int)CostumePointerOffset, 0x108))};
            data.FighterData.CostumeFileSymbols = new HSDArrayAccessor<MEX_CostumeFileSymbolTable>();
            for (int i = 0; i < data.FighterData.CostumePointers.Length; i++)
            {
                data.FighterData.CostumePointers._s.SetReferenceStruct(i * 8, new HSDStruct(0x18)); // is this constant 0x18?

                if(data.FighterData.CostumePointers[i].CostumeCount > 0)
                {
                    var addr = DOLScrubber.RAMToDOL(dol.ReadValueAt(CostumeStringOffset + (uint)i * 4));

                    MEX_CostumeFileSymbolTable costume = new MEX_CostumeFileSymbolTable()
                    {
                        CostumeSymbols = new HSDArrayAccessor<MEX_CostumeFileSymbol>()
                        {
                            _s = new HSDFixedLengthPointerArrayAccessor<HSD_String>()
                            {
                                Array = dol.ReadStringTable(addr, data.FighterData.CostumePointers[i].CostumeCount * 3)
                            }._s
                        }
                    };

                    data.FighterData.CostumeFileSymbols.Set(i, costume);
                }
            }

            // anim file strings
            data.FighterData.AnimFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>() { Array = dol.ReadStringTable(CharAnimStringOffset, data.MetaData.NumOfInternalIDs) };
            
            // ftDemo strings
            data.FighterData.FtDemo_SymbolNames = new HSDFixedLengthPointerArrayAccessor<MEX_FtDemoSymbolNames>();
            for (uint i = 0; i < 27; i++)
            {
                var addr = DOLScrubber.RAMToDOL(dol.ReadValueAt(ftDemoStringOffset + i * 4));

                data.FighterData.FtDemo_SymbolNames.Add(
                    new MEX_FtDemoSymbolNames()
                    {
                        _s = new HSDFixedLengthPointerArrayAccessor<HSD_String>()
                        {
                            Array = dol.ReadStringTable(addr, 4)
                        }._s
                    }
                    );
            }

            // result anim symbols
            data.FighterData.ResultAnimFiles = new HSDFixedLengthPointerArrayAccessor<HSD_String>();
            for (uint i = 0; i < RestAnimFiles.Length; i ++)
                data.FighterData.ResultAnimFiles.Add(new HSD_String() { Value = RestAnimFiles[i] });

            // character file strings
            data.FighterData._s.SetReferenceStruct(0x40, new HSDStruct(0x108));
            data.FighterData.CharFiles = new HSDArrayAccessor<MEX_CharFileStrings>()
            {
                _s = new HSDFixedLengthPointerArrayAccessor<HSD_String>()
                {
                    Array = dol.ReadStringTable(CharStringOffset, data.MetaData.NumOfInternalIDs * 2)
                }._s
            };

            // blank mex data
            data.FighterData.FighterItemLookup = new HSDArrayAccessor<MEX_ItemLookup>() { Array = new MEX_ItemLookup[data.MetaData.NumOfInternalIDs] };
            data.FighterData.FighterEffectLookup = new HSDArrayAccessor<MEX_EffectTypeLookup>() { Array = new MEX_EffectTypeLookup[data.MetaData.NumOfInternalIDs] };

            // hard coded fighter table
            data.FighterData.NameText = new HSDFixedLengthPointerArrayAccessor<HSD_String>() { Array = CharText.Select(e => new HSD_String() { Value = e }).ToArray() };
            data.FighterData.TargetTestStageLookups = new HSDArrayAccessor<HSD_UShort>() { Array = TargetTestIDs.Select(e => new HSD_UShort() { Value = e }).ToArray() };
            data.FighterData.InsigniaIDs = new HSDArrayAccessor<HSD_Byte>() { Array = InsigniaIDs.Select(e => new HSD_Byte() { Value = e }).ToArray() };
            data.FighterData.AnnouncerCalls = new HSDArrayAccessor<HSD_Int>() { Array = AnnouncerCalls.Select(e => new HSD_Int() { Value = (int)e }).ToArray() };
            data.FighterData.VictoryThemeIDs = new HSDArrayAccessor<HSD_Int>() { Array = VictoryThemeIDs.Select(e => new HSD_Int() { Value = (int)e }).ToArray() };
            data.FighterData.WallJump = new HSDArrayAccessor<HSD_Byte>() { _s = new HSDStruct(WallJump) };


            // generate figther functions
            data.FighterFunctions = new MEX_FighterFunctionTable();
            ExtractData(dol, data.FighterFunctions);
            data.FighterFunctions.enterFloat = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            data.FighterFunctions.enterSpecialDoubleJump = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            data.FighterFunctions.enterTether = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            data.FighterFunctions.onLand = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            data.FighterFunctions.onSmashDown = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            data.FighterFunctions.onSmashForward = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            data.FighterFunctions.onSmashUp = new HSDArrayAccessor<HSD_UInt>() { _s = new HSDStruct(0x84) };
            
            // Optional Move Logic
            if (settings.IncludeMoveLogic)
            {
                var moveLogicStruct = data.FighterFunctions._s.GetReference<HSDAccessor>(0x0C);
                var movelogicStride = 0x20;
                for (int i = 0; i < data.MetaData.NumOfInternalIDs; i++)
                {
                    // get move logic pointer
                    var off = (uint)moveLogicStruct._s.GetInt32(i * 4);

                    // null pointer skips
                    if (off == 0 || MoveLogicEntries[i] == 0)
                        continue;

                    // convert ram offset to dol
                    off = DOLScrubber.RAMToDOL(off);

                    // set the pointer to data
                    moveLogicStruct._s.SetReferenceStruct(i * 4, new HSDStruct(dol.GetSection(off, MoveLogicEntries[i] * movelogicStride)));
                }
            }


            // generate ssm table
            data.SSMTable = new MEX_SSMTable();
            ExtractData(dol, data.SSMTable);
            data.SSMTable.SSM_SSMFiles = new HSDNullPointerArrayAccessor<HSD_String>() { Array = dol.ReadStringTable(SSMStringOffset, data.MetaData.NumOfSSMs) };
            data.SSMTable.SSM_Runtime = new HSDAccessor() { _s = new HSDStruct(0x18) };
            data.SSMTable.SSM_Runtime._s.SetReferenceStruct(0x00, new HSDStruct(0x180));
            data.SSMTable.SSM_Runtime._s.SetReferenceStruct(0x04, new HSDStruct(0xDC));
            data.SSMTable.SSM_Runtime._s.SetReferenceStruct(0x08, new HSDStruct(0xDC));
            data.SSMTable.SSM_Runtime._s.SetReferenceStruct(0x0C, new HSDStruct(0xDC));
            data.SSMTable.SSM_Runtime._s.SetReferenceStruct(0x10, new HSDStruct(0xDC));
            data.SSMTable.SSM_Runtime._s.SetReferenceStruct(0x14, new HSDStruct(0xDC));


            // generate music table
            data.MusicTable = new MEX_BGMStruct();
            data.MusicTable.BackgroundMusicStrings = new HSDFixedLengthPointerArrayAccessor<HSD_String>() { Array = dol.ReadStringTable(MusicStringOffset, data.MetaData.NumOfMusic) };
            data.MusicTable.MenuPlaylist = new HSDArrayAccessor<MEX_PlaylistItem>() { Array = new MEX_PlaylistItem[] { new MEX_PlaylistItem() { ChanceToPlay = 100, HPSID = 52 } } };
            data.MusicTable.MenuPlayListCount = 1;


            // generate effect table
            data.EffectTable = new MEX_EffectData();
            data.EffectTable.EffectFiles = new HSDArrayAccessor<MEX_EffectFiles>()
            {
                _s = new HSDFixedLengthPointerArrayAccessor<HSD_String>()
                {
                    Array = dol.ReadStringTable(EffectStringOffset, data.MetaData.NumOfEffects * 3)
                }._s
            };
            data.EffectTable.EffectRuntime = new HSDAccessor() { _s = new HSDStruct(0x60) };
            data.EffectTable.RuntimeArray1 = new HSDAccessor() { _s = new HSDStruct(0xCC) };
            data.EffectTable.RuntimeArray2 = new HSDAccessor() { _s = new HSDStruct(0xCC) };
            data.EffectTable.RuntimeArray3 = new HSDAccessor() { _s = new HSDStruct(0xCC) };
            data.EffectTable.RuntimeArray4 = new HSDAccessor() { _s = new HSDStruct(0xCC) };
            data.EffectTable.RuntimeArray5 = new HSDAccessor() { _s = new HSDStruct(0xCC) };
            data.EffectTable.RuntimeArray6 = new HSDAccessor() { _s = new HSDStruct(0xCC) };


            // generate item table
            data.ItemTable = new MEX_ItemTables();
            ExtractData(dol, data.ItemTable);
            data.ItemTable._s.SetReferenceStruct(0x14, new HSDStruct(4));

            // Optional Item States
            if (settings.IncludeItemStates)
            {
                var itemStride = 0x10;
                for (int i = 0; i < data.ItemTable.CommonItems.Length; i++)
                {
                    var off = (uint)data.ItemTable.CommonItems._s.GetInt32(i * 0x3C);
                    if (off == 0 || CommonItemStates[i] == 0)
                        continue;
                    off = DOLScrubber.RAMToDOL(off);
                    data.ItemTable.CommonItems._s.SetReferenceStruct(i * 0x3C, new HSDStruct(dol.GetSection(off, CommonItemStates[i] * itemStride)));
                }
                for (int i = 0; i < data.ItemTable.FighterItems.Length; i++)
                {
                    var off = (uint)data.ItemTable.FighterItems._s.GetInt32(i * 0x3C);
                    if (off == 0 || FighterItemStates[i] == 0)
                        continue;
                    off = DOLScrubber.RAMToDOL(off);
                    data.ItemTable.FighterItems._s.SetReferenceStruct(i * 0x3C, new HSDStruct(dol.GetSection(off, FighterItemStates[i] * itemStride)));
                }
                for (int i = 0; i < data.ItemTable.Pokemon.Length; i++)
                {
                    var off = (uint)data.ItemTable.Pokemon._s.GetInt32(i * 0x3C);
                    if (off == 0 || PokemonItemStates[i] == 0)
                        continue;
                    off = DOLScrubber.RAMToDOL(off);
                    data.ItemTable.Pokemon._s.SetReferenceStruct(i * 0x3C, new HSDStruct(dol.GetSection(off, PokemonItemStates[i] * itemStride)));
                }
                for (int i = 0; i < data.ItemTable.StageItems.Length; i++)
                {
                    var off = (uint)data.ItemTable.StageItems._s.GetInt32(i * 0x3C);
                    if (off == 0 || StageItemStates[i] == 0)
                        continue;
                    off = DOLScrubber.RAMToDOL(off);
                    data.ItemTable.StageItems._s.SetReferenceStruct(i * 0x3C, new HSDStruct(dol.GetSection(off, StageItemStates[i] * itemStride)));
                }
            }


            // generate kirby data table
            data.KirbyData = new MEX_KirbyTable();
            ExtractData(dol, data.KirbyData);
            data.KirbyData.CapFiles = new HSDArrayAccessor<MEX_KirbyCapFiles>()
            {
                _s = new HSDFixedLengthPointerArrayAccessor<HSD_String>()
                {
                    Array = dol.ReadStringTable(KirbyCapOffset, data.MetaData.NumOfInternalIDs * 2)
                }._s
            };
            data.KirbyData.KirbyCostumes = new HSDFixedLengthPointerArrayAccessor<MEX_KirbyCostume>();
            for (uint i = 0; i < data.MetaData.NumOfInternalIDs; i++)
            {
                var ramaddr = DOLScrubber.RAMToDOL(dol.ReadValueAt(KirbyCostumeOffset + i * 4));

                HSDStruct costumeStruct = null;

                if (ramaddr != 0)
                    costumeStruct = new HSDFixedLengthPointerArrayAccessor<HSD_String>() { Array = dol.ReadStringTable(ramaddr, 6 * 3) }._s;

                data.KirbyData.KirbyCostumes.Add(new MEX_KirbyCostume() { _s = costumeStruct });
            }
            data.KirbyData.CapFileRuntime = new HSDAccessor() { _s = new HSDStruct(0x100) };
            data.KirbyData.CostumeRuntime = new HSDAccessor() { _s = new HSDStruct(0xB8) }; // TODO: check size
            data.KirbyData.CostumeRuntime._s.SetReferenceStruct(0x0C, new HSDStruct(0x30));
            data.KirbyData.CostumeRuntime._s.SetReferenceStruct(0x3C, new HSDStruct(0x30));
            data.KirbyData.CostumeRuntime._s.SetReferenceStruct(0x40, new HSDStruct(0x30));
            data.KirbyData.CostumeRuntime._s.SetReferenceStruct(0x58, new HSDStruct(0x30));
            data.KirbyData.CostumeRuntime._s.SetReferenceStruct(0x60, new HSDStruct(0x30));


            // generate kirby function table
            data.KirbyFunctions = new MEX_KirbyFunctionTable();
            ExtractData(dol, data.KirbyFunctions);
            var abtb = new HSDArrayAccessor<HSD_UInt>() { _s = dol.GetStruct(dolOffset["KirbyAbility"]) };
            data.KirbyFunctions.OnAbilityGain = new HSDArrayAccessor<HSD_UInt>() { Array = abtb.Array.Where((e, i) => i % 2 == 0).ToArray() };
            data.KirbyFunctions.OnAbilityLose = new HSDArrayAccessor<HSD_UInt>() { Array = abtb.Array.Where((e, i) => i % 2 == 1).ToArray() };
            data.KirbyFunctions.KirbyOnHit = new HSDArrayAccessor<HSD_UInt>() { Array = new HSD_UInt[data.MetaData.NumOfInternalIDs] };
            data.KirbyFunctions.KirbyOnItemInit = new HSDArrayAccessor<HSD_UInt>() { Array = new HSD_UInt[data.MetaData.NumOfInternalIDs] };

            // generate stage data
            data.StageData = new MEX_StageData();
            ExtractData(dol, data.StageData);
            var stageCount = data.StageData.CollisionTable.Length;
            data.StageData.StageEffectLookup = new HSDArrayAccessor<MEX_EffectTypeLookup>() { Array = new MEX_EffectTypeLookup[stageCount] };
            data.StageData.StageItemLookup = new HSDArrayAccessor<MEX_ItemLookup>() { Array = new MEX_ItemLookup[stageCount] };
            data.StageData.StagePlaylists = new HSDArrayAccessor<MEX_Playlist>() { Array = new MEX_Playlist[stageCount] };

            // generate stage functions
            data.StageFunctions = new HSDFixedLengthPointerArrayAccessor<MEX_Stage>();

            for(int i = 0; i < 71; i++)
            {
                if (dol.ReadValueAt(StageFunctionOffset + (uint)i * 4) == 0)
                {
                    data.StageFunctions.Set(i, new MEX_Stage());
                    continue;
                }

                var off = DOLScrubber.RAMToDOL(dol.ReadValueAt(StageFunctionOffset + (uint)i * 4));

                var stage = new MEX_Stage() { _s = new HSDStruct(dol.GetSection(off, 0x34)) };

                // gobj functions at 0x04 stride 0x14 unknown number of entries
                
                if(stage._s.GetInt32(0x08) != 0)
                    stage.StageFileName = dol.ReadStringAt((uint)stage._s.GetInt32(0x08)).Value;
                
                if (stage.MovingCollisionPointCount > 0 && stage._s.GetInt32(0x2C) != 0)
                {
                    var coloff = DOLScrubber.RAMToDOL((uint)stage._s.GetInt32(0x2C));

                    stage._s.SetReferenceStruct(0x2C, new HSDStruct(dol.GetSection(coloff, 6 * stage.MovingCollisionPointCount)));
                }

                data.StageFunctions.Set(i, stage);
            }


            // Optional Map GOBJs
            if (settings.IncludeMapGOBJs)
            {
                var mapGOBJStride = 20;
                var stages = data.StageFunctions.Array;
                for(int i = 0; i < stages.Length; i++)
                {
                    var off = (uint)stages[i]._s.GetInt32(0x04);

                    if (off == 0)
                        continue;

                    off = DOLScrubber.RAMToDOL(off);

                    stages[i]._s.SetReferenceStruct(0x04, new HSDStruct(dol.GetSection(off, MapGOBJEntries[i] * mapGOBJStride)));
                }
                stages = data.StageFunctions.Array = stages;
            }


            dol.Dispose();

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ExtractData(DOLScrubber dol, HSDAccessor acc)
        {
            foreach(var p in acc.GetType().GetProperties())
            {
                if (dolOffset.ContainsKey(p.Name))
                {
                    var i = Activator.CreateInstance(p.PropertyType);
                    ((HSDAccessor)i)._s = dol.GetStruct(dolOffset[p.Name]);
                    p.SetValue(acc, i);
                }
            }
        }

        // Hardcoded tables

        public static readonly string[] CharText =
      {
            "C. Falcon",
            "DK",
            "Fox",
            "GaW",
            "Kirby",
            "Bowser",
            "Link",
            "Luigi",
            "Mario",
            "Marth",
            "Mewtwo",
            "Ness",
            "Peach",
            "Pikachu",
            "Ice Climbers",
            "Jigglypuff",
            "Samus",
            "Yoshi",
            "Zelda",
            "Sheik",
            "Falco",
            "Young Link",
            "Dr. Mario",
            "Roy",
            "Pichu",
            "Ganondorf",
            "Master Hand",
            "Wireframe Male",
            "Wireframe Female",
            "Giga Bowser",
            "Crazy Hand",
            "Sandbag",
            "Popo",
        };

        public static readonly string[] RestAnimFiles = new string[] { "GmRstMCa.dat", "GmRstMDk.dat", "GmRstMFx.dat", "GmRstMGw.dat", "GmRstMKb.dat", "GmRstMKp.dat", "GmRstMLk.dat", "GmRstMLg.dat", "GmRstMMr.dat", "GmRstMMs.dat", "GmRstMMt.dat", "GmRstMNs.dat", "GmRstMPe.dat", "GmRstMPk.dat", "GmRstMPn.dat", "GmRstMPr.dat", "GmRstMSs.dat", "GmRstMYs.dat", "GmRstMZd.dat", "GmRstMSk.dat", "GmRstMFc.dat", "GmRstMCl.dat", "GmRstMDr.dat", "GmRstMFe.dat", "GmRstMPc.dat", "GmRstMGn.dat", "GmRstMMr.dat", "GmRstMMh.dat", "GmRstMBo.dat", "GmRstMGl.dat", "GmRstMGk.dat", "GmRstMCh.dat", "GmRstMSb.dat", };

        public static readonly ushort[] TargetTestIDs = new ushort[] { 0x22, 0x24, 0x27, 0x38, 0x29, 0x2A, 0x2B, 0x2C, 0x21, 0x2D, 0x2E, 0x2F, 0x30, 0x32, 0x28, 0x33, 0x34, 0x36, 0x37, 0x35, 0x36, 0x23, 0x25, 0x39, 0x31, 0x3A };

        public static readonly byte[] InsigniaIDs = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x05, 0x06, 0x0D, 0x06, 0x06, 0x07, 0x09, 0x08, 0x06, 0x09, 0x04, 0x09, 0x0A, 0x0C, 0x0D, 0x0D, 0x02, 0x0D, 0x06, 0x07, 0x09, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04 };
        
        public static readonly uint[] VictoryThemeIDs = new uint[] { 0x11, 0x0D, 0x10, 0x0F, 0x14, 0x16, 0x15, 0x16, 0x16, 0x0E, 0x18, 0x17, 0x16, 0x18, 0x13, 0x18, 0x19, 0x1D, 0x15, 0x15, 0x10, 0x15, 0x16, 0x0E, 0x18, 0x15, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00 };

        public static readonly byte[] WallJump = new byte[] { 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static readonly uint[] AnnouncerCalls = new uint[] { 0x07C830, 0x07C831, 0x07C835, 0x07C83A, 0x07C83F, 0x07C840, 0x07C842, 0x07C844, 0x07C845, 0x07C846, 0x07C848, 0x07C84A, 0x07C84B, 0x07C84D, 0x07C83B, 0x07C83D, 0x07C84E, 0x07C84F, 0x07C851, 0x07C850, 0x07C834, 0x07C843, 0x07C832, 0x07C83C, 0x07C84C, 0x07C836, 0x07C849, 0x07C833, 0x07C833, 0x07C838, 0x07C849, 0x07C848, 0x07C83B };

        public static readonly byte[] MoveLogicEntries = new byte[] {
                              12,35,23,46,203,23,21,24,36,26,
                              16,26,26,18,28,32,20,20,32,18,
                              22,10,35,26,40,23,32,50,49,0,
                              0,24,1};

        public static readonly byte[] MapGOBJEntries = new byte[] {
            0, 4, 21, 8, 12, 8, 11, 3, 12, 6,
            4, 2, 11, 8, 22, 19, 12, 28, 40, 41,
            8, 8, 11, 0, 4, 16, 0, 9, 6, 6,
            4, 4, 8, 4, 38, 4, 8, 11, 2, 3,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 4, 6, 14, 3, 3,
            3 };

        public static readonly byte[] CommonItemStates = new byte[] { 7, 10, 8, 8, 10, 10, 14, 12, 6, 6, 1, 6, 6, 14, 10, 8, 6, 6, 4, 8, 8, 6, 6, 6, 10, 8, 2, 2, 6, 6, 6, 6, 6, 6, 8, 1, 1, 2, 10, 1, 4, 6, 6 };
        public static readonly byte[] FighterItemStates = new byte[] { 12, 18, 10, 12, 2, 1, 8, 1, 1, 1, 3, 2, 2, 3, 3, 8, 8, 4, 4, 10, 10, 6, 6, 1, 1, 3, 1, 3, 3, 3, 3, 12, 12, 10, 10, 1, 6, 1, 3, 3, 2, 2, 1, 3, 3, 1, 2, 1, 2, 1, 4, 10, 4, 10, 6, 2, 6, 1, 2, 4, 3, 2, 1, 4, 1, 2, 1, 1, 1, 18, 4, 4, 1, 1, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 1, 2, 2, 1, 8, 1, 4, 2, 1, 2, 2, 12, 12, 6, 6, 10, 10, 18, 3, 1, 2, 1, 2, 1, 10, 6, 1, 1, 2, 1, 3, 3, 6, 20 };
        public static readonly byte[] PokemonItemStates = new byte[] { 6, 3, 3, 4, 3, 4, 3, 3, 3, 3, 4, 3, 2, 2, 2, 6, 8, 6, 6, 3, 6, 8, 3, 3, 3, 8, 2, 3, 4, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 6, 1, 0 };
        public static readonly byte[] StageItemStates = new byte[] { 12, 1, 6, 12, 8, 22, 0, 0, 12, 12, 6, 12, 14, 14, 0, 0, 0, 8, 8, 0, 0, 0, 10, 0, 0, 12, 8, 2, 6 };
    }

    /// <summary>
    /// 
    /// </summary>
    public class DOLScrubber : IDisposable
    {
        private BinaryReaderExt r;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dolFile"></param>
        public DOLScrubber(string dolFile)
        {
            r = new BinaryReaderExt(new FileStream(dolFile, FileMode.Open));
            r.BigEndian = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dolAddr"></param>
        /// <returns></returns>
        public uint ReadValueAt(uint dolAddr)
        {
            r.Position = dolAddr;
            return r.ReadUInt32();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc"></param>
        public HSDStruct GetStruct(Tuple<int, int> loc)
        {
            r.BaseStream.Position = loc.Item1;
            HSDStruct s = new HSDStruct(r.ReadBytes(loc.Item2));
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ramAddr"></param>
        /// <returns></returns>
        public static uint DOLToRAM(uint dolAddr)
        {
            if (dolAddr == 0)
                return 0;

            else if (dolAddr + 0x80003000 <= 0x804D0000)
                return dolAddr + 0x80003000;
            else
                return dolAddr + 0x800a4fe0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ramAddr"></param>
        /// <returns></returns>
        public static uint RAMToDOL(uint ramAddr)
        {
            if (ramAddr == 0)
                return 0;

            else if (ramAddr <= 0x804D0000)
                return ramAddr - 0x80003000;
            else
                return ramAddr - 0x800a4fe0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] GetSection(uint doloffset, int size)
        {
            return r.GetSection(doloffset, size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public HSD_String[] ReadStringTable(uint position, int length, bool shiftJis = false)
        {
            HSD_String[] s = new HSD_String[length];

            for(int i = 0;i < length; i++)
            {
                r.Position = position + (uint)i * 4;
                s[i] = ReadStringAt(r.ReadUInt32(), shiftJis);
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ramOffset"></param>
        public HSD_String ReadStringAt(uint ramOffset, bool shiftJis = false)
        {
            if (ramOffset == 0 || ramOffset == 0xFFFFFFFF)
                return null;

            var off = RAMToDOL(ramOffset);

            r.BaseStream.Position = off;

            var str = "";

            if (shiftJis)
            {
                byte[] c = r.ReadBytes(2);
                while (c.Length >= 2 && !(c[0] == 0 && c[1] == 0))
                {
                    str += System.Text.Encoding.GetEncoding(932).GetString(c);
                    c = r.ReadBytes(2);
                }
            }
            else
            {
                byte c = r.ReadByte();
                while (c != 0)
                {
                    str += (char)c;
                    c = r.ReadByte();
                }
            }

            return new HSD_String() { Value = str };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            r.Close();
            r.Dispose();
        }
    }
}
