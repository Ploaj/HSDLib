using System;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using HSDRaw.MEX;
using HSDRaw;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using HSDRawViewer.Tools;
using HSDRawViewer.GUI.MEX.Tools;

namespace HSDRawViewer.GUI.MEX.Controls
{
    public partial class MEXItemControl : UserControl, IMEXControl
    {
        /// <summary>
        /// 
        /// </summary>
        public MEX_Data MexData
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e._data;
                return null;
            }
        }

        public MexDataEditor MexDataEditor
        {
            get
            {
                var c = Parent;
                while (c != null && !(c is MexDataEditor)) c = c.Parent;
                if (c is MexDataEditor e) return e;
                return null;
            }
        }

        public int MEXItemOffset { get; internal set; }

        public MEX_Item[] ItemCommon { get; set; }
        public MEX_Item[] ItemFighter { get; set; }
        public MEX_Item[] ItemPokemon { get; set; }
        public MEX_Item[] ItemStage { get; set; }
        public MEX_Item[] ItemMEX { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MEXItemControl()
        {
            InitializeComponent();

            commonItemEditor.DisableAllControls();
            fighterItemEditor.DisableAllControls();
            pokemonItemEditor.DisableAllControls();
            stageItemEditor.DisableAllControls();

            mexItemEditor.OnItemRemove += (args) =>
            {
                var index = MEXItemOffset + args.Index;

                foreach (var v in MexDataEditor.FighterControl.FighterEntries)
                {
                    foreach (var s in v.MEXItems)
                    {
                        if (s.Value == index) s.Value = 0;
                        if (s.Value > index) s.Value -= 1;
                    }
                }

                foreach (var stage in MexDataEditor.StageControl.StageEntries)
                {
                    var items = stage.Items;
                    foreach (var f in items)
                    {
                        if (f.Value == index) f.Value -= 1;
                        if (f.Value > index) f.Value -= 1;
                    }
                    stage.Items = items;
                }
            };
            
            commonItemEditor.TextOverrides.AddRange(DefaultItemNames.CommonItemNames);
            fighterItemEditor.TextOverrides.AddRange(DefaultItemNames.FighterItemNames);
            pokemonItemEditor.TextOverrides.AddRange(DefaultItemNames.PokemonItemNames);
            stageItemEditor.TextOverrides.AddRange(DefaultItemNames.StageItemNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetControlName()
        {
            return "Items";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void LoadData(MEX_Data data)
        {
            MEXItemOffset = 0;

            ItemCommon = data.ItemTable.CommonItems.Array;
            commonItemEditor.SetArrayFromProperty(this, "ItemCommon");
            MEXItemOffset += ItemCommon.Length;

            ItemFighter = data.ItemTable.FighterItems.Array;
            fighterItemEditor.SetArrayFromProperty(this, "ItemFighter");
            fighterItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemFighter.Length;

            ItemPokemon = data.ItemTable.Pokemon.Array;
            pokemonItemEditor.SetArrayFromProperty(this, "ItemPokemon");
            pokemonItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemPokemon.Length;

            ItemStage = data.ItemTable.StageItems.Array;
            stageItemEditor.SetArrayFromProperty(this, "ItemStage");
            stageItemEditor.ItemIndexOffset = MEXItemOffset;
            MEXItemOffset += ItemStage.Length;

            ItemMEX = data.ItemTable.MEXItems.Array;
            mexItemEditor.SetArrayFromProperty(this, "ItemMEX");
            mexItemEditor.ItemIndexOffset = MEXItemOffset;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SaveData(MEX_Data data)
        {
            data.ItemTable.CommonItems.Array = ItemCommon;
            data.ItemTable.FighterItems.Array = ItemFighter;
            data.ItemTable.Pokemon.Array = ItemPokemon;
            data.ItemTable.StageItems.Array = ItemStage;
            data.ItemTable.MEXItems.Array = ItemMEX;
            data.ItemTable._s.GetCreateReference<HSDAccessor>(0x14)._s.Resize(Math.Max(4, ItemMEX.Length * 4));
        }


        /// <summary>
        /// 
        /// </summary>
        public void ResetDataBindings()
        {
            mexItemEditor.ResetBindings();
        }

        /// <summary>
        /// Adds a new MEX item to table
        /// </summary>
        /// <param name="item"></param>
        /// <returns>added mex item id</returns>
        public int AddMEXItem(string yaml)
        {
            return AddMEXItem(Deserialize(yaml));
        }

        /// <summary>
        /// Adds a new MEX item to table
        /// </summary>
        /// <param name="item"></param>
        /// <returns>added mex item id</returns>
        public int AddMEXItem(MEX_Item item)
        {
            return mexItemEditor.AddItem(item);
        }

        /// <summary>
        /// Only removes item if there are no dependencies
        /// </summary>
        /// <returns></returns>
        public bool SaveRemoveMexItem(int index)
        {
            // only remove if index is in range of mex item
            if (index < MEXItemOffset)
                return false;

            // check if used
            var fighterUsing = MexDataEditor.FighterControl.ItemInUse(index);
            var stageUsing = MexDataEditor.StageControl.StageEntries.Any(e => e.Items.Any(r => r.Value == index));

            if (fighterUsing || stageUsing)
                return false;

            // remove item
            mexItemEditor.RemoveAt(index - MEXItemOffset);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MEX_Item Deserialize(string data)
        {
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .Build();

            return deserializer.Deserialize<MEX_Item>(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        public void Serialize(string filepath, MEX_Item item)
        {
            var builder = new SerializerBuilder()
            .WithTypeInspector(inspector => new MEXTypeInspector(inspector))
            .WithNamingConvention(CamelCaseNamingConvention.Instance);

            using (StreamWriter writer = File.CreateText(filepath))
            {
                builder.Build().Serialize(writer, item);
            }
        }

        #region Events
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mexItemCloneButton_Click(object sender, EventArgs e)
        {
            if (itemTabs.SelectedIndex == 0)
                mexItemEditor.AddItem(commonItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 1)
                mexItemEditor.AddItem(fighterItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 2)
                mexItemEditor.AddItem(pokemonItemEditor.SelectedObject);
            if (itemTabs.SelectedIndex == 3)
                mexItemEditor.AddItem(stageItemEditor.SelectedObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MEX_Item GetItem(int index)
        {
            var item = commonItemEditor.GetItemAt(index);

            if (item == null)
                item = fighterItemEditor.GetItemAt(index);

            if (item == null)
                item = pokemonItemEditor.GetItemAt(index);

            if (item == null)
                item = stageItemEditor.GetItemAt(index);

            if (item == null)
                item = mexItemEditor.GetItemAt(index);

            if (item != null)
                return (MEX_Item)item;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemExportButton_Click(object sender, EventArgs e)
        {
            var f = FileIO.SaveFile("Item (*.yaml)|*.yaml");
            if (f != null)
            {
                if (itemTabs.SelectedIndex == 0 && commonItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, commonItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 1 && fighterItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, fighterItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 2 && pokemonItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, pokemonItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 3 && stageItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, stageItemEditor.SelectedObject as MEX_Item);
                if (itemTabs.SelectedIndex == 4 && mexItemEditor.SelectedObject is MEX_Item)
                    Serialize(f, mexItemEditor.SelectedObject as MEX_Item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpyLogicToClipButton_Click(object sender, EventArgs e)
        {
            MEX_Item item = null;
            if (itemTabs.SelectedIndex == 0 && commonItemEditor.SelectedObject is MEX_Item it)
                item = it;
            if (itemTabs.SelectedIndex == 1 && fighterItemEditor.SelectedObject is MEX_Item it2)
                item = it2;
            if (itemTabs.SelectedIndex == 2 && pokemonItemEditor.SelectedObject is MEX_Item it3)
                item = it3;
            if (itemTabs.SelectedIndex == 3 && stageItemEditor.SelectedObject is MEX_Item it4)
                item = it4;
            if (itemTabs.SelectedIndex == 4 && mexItemEditor.SelectedObject is MEX_Item it5)
                item = it5;

            if (item == null)
                return;

            if (item.ItemStates == null)
            {
                MessageBox.Show("This MxDt file does not contains item states", "Nothing to copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            StringBuilder table = new StringBuilder();
            int index = 0;
            foreach (var m in item.ItemStates)
            {
                table.AppendLine("\t// State " + index++);

                table.AppendLine(string.Format(
                    "\t{{" +
                    "\n\t\t0x{0, -10}// AnimID" +
                    "\n\t\t0x{1, -10}// AnimationCallback" +
                    "\n\t\t0x{2, -10}// PhysicsCallback" +
                    "\n\t\t0x{3, -10}// CollisionCallback" +
                    "\n\t}},",
            m.AnimID.ToString("X") + ",",
            m.AnimationCallback.ToString("X") + ",",
            m.PhysicsCallback.ToString("X") + ",",
            m.CollisionCallback.ToString("X") + ","
            ));
            }

            Clipboard.SetText(
                @"__attribute__((used))
static struct ItemState item_states[] = {
" + table.ToString() + @"}; ");

            MessageBox.Show("Item State Table Copied to Clipboard");
        }

        #endregion

    }
}
