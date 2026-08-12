namespace HSDRawViewer.GUI.Plugins.GrTool.Nodes
{
    public interface IUndo
    {
        public void Undo(object selected_object);

        public void Redo(object selected_object);

        public void Commit(object selected_object);

        public void ClearHistory();
    }
}
