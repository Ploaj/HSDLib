using System;
using System.Collections.Generic;

namespace HSDRawViewer.Tools
{
    internal class ObjectUndoManager
    {
        private class Entry
        {
            public object Original;
            public object Before;
            public object After;

            public Entry(object original, object before, object after)
            {
                Original = original;
                Before = before;
                After = after;
            }
        }

        private readonly Stack<Entry> undoStack = new();
        private readonly Stack<Entry> redoStack = new();

        public void Commit(object o)
        {
            if (o == null)
                return;

            // Capture the state before the modification.
            undoStack.Push(new Entry(
                o,
                o.Copy(),
                null));

            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count == 0)
                return;

            var e = undoStack.Pop();

            // Capture the state after the modification.
            e.After = e.Original.Copy();

            // Restore the state from Commit().
            e.Before.CopyTo(e.Original);

            redoStack.Push(e);
        }

        public void Redo()
        {
            if (redoStack.Count == 0)
                return;

            var e = redoStack.Pop();

            // Restore the state after the modification.
            e.After.CopyTo(e.Original);

            undoStack.Push(e);
        }

        public void ClearHistory()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
    }
}
