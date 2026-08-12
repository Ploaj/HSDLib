using System;
using System.Collections.Generic;

namespace HSDRawViewer.Tools
{
    internal class ObjectUndoManager
    {
        private class Entry
        {
            public object Original;
            public object Backup;

            public Entry(object original, object backup)
            {
                Original = original;
                Backup = backup;
            }
        }

        private Stack<Entry> undoStack = new Stack<Entry>();
        private Stack<Entry> redoStack = new Stack<Entry>();

        public void Undo()
        {
            if (undoStack.Count == 0)
                return;

            var e = undoStack.Pop();

            redoStack.Push(e);

            e.Backup.CopyTo(e.Original);
        }

        public void Commit(object o)
        {
            undoStack.Push(new Entry(o, o.Copy()));

            redoStack.Clear();
        }

        public void Redo()
        {
            if (redoStack.Count == 0)
                return;

            var e = redoStack.Pop();

            undoStack.Push(e);

            e.Backup.CopyTo(e.Original);
        }

        public void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
    }
}
