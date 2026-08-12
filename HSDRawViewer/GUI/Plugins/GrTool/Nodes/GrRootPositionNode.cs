using HSDRawViewer.GUI.Plugins.GrTool;

namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public class GrRootPositionNode : GrNode
    {
        public GrCategoryPositionList Start { get; }

        public GrCategoryPositionList Enemy { get; }

        public GrCategoryPositionList Gravity { get; }

        public GrCategoryPositionList AirFlow { get; }

        public GrCategoryPositionList Conveyor { get; }

        public GrCategoryPositionList Item { get; }

        public GrCategoryPositionList Event { get; }

        public GrCategoryPositionList Vehicle { get; }

        public GrCategoryPositionList GlobalDead { get; }

        public GrCategoryPositionList LocalDead { get; }

        public GrCategoryPositionList Yakumono { get; }

        public GrCategoryPositionAreaList VehicleAreaList { get; }

        public GrCategoryPositionAreaList ItemAreaList { get; }


        public GrRootPositionNode(GrDataResource res) 
        {
            Text = "Positions";

            Start = new GrCategoryPositionList("Start", res.StartPositions);
            Enemy = new GrCategoryPositionList("Enemy", res.EnemyPositions);
            Gravity = new GrCategoryPositionList("Gravity", res.GravityPositions);
            AirFlow = new GrCategoryPositionList("AirFlow", res.AirFlowPositions);
            Conveyor = new GrCategoryPositionList("Conveyor", res.ConveyorPositions);
            Item = new GrCategoryPositionList("Item", res.ItemPositions);
            Event = new GrCategoryPositionList("Event", res.EventPositions);
            Vehicle = new GrCategoryPositionList("Vehicle", res.VehiclePositions);
            GlobalDead = new GrCategoryPositionList("GlobalDead", res.GlobalDeadPositions);
            LocalDead = new GrCategoryPositionList("LocalDead", res.LocalDeadPositions);
            Yakumono = new GrCategoryPositionList("Yakumono", res.YakumonoPositions);

            ItemAreaList = new GrCategoryPositionAreaList("ItemArea", res.ItemAreaPositions)
            {
                DisplayColor = new OpenTK.Mathematics.Vector3(0f, 1f, 0f)
            };
            VehicleAreaList = new GrCategoryPositionAreaList("VehicleArea", res.VehicleAreaPositions)
            {
                DisplayColor = new OpenTK.Mathematics.Vector3(1f, 0f, 1f)
            };

            Nodes.Add(Start);
            Nodes.Add(Enemy);
            Nodes.Add(Gravity);
            Nodes.Add(AirFlow);
            Nodes.Add(Conveyor);
            Nodes.Add(Item);
            Nodes.Add(Event);
            Nodes.Add(Vehicle);
            Nodes.Add(GlobalDead);
            Nodes.Add(LocalDead);
            Nodes.Add(Yakumono);

            Nodes.Add(ItemAreaList);
            Nodes.Add(VehicleAreaList);
        }
    }
}
