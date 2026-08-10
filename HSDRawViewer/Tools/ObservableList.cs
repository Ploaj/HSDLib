using System;
using System.Collections;
using System.Collections.Generic;

namespace HSDRawViewer.Tools
{
    public class ObservableList<T> : IEnumerable<T>
    {
        private readonly List<T> _items = new List<T>();

        public event Action<T> Added;
        public event Action<T> Removed;

        public void Add(T item)
        {
            _items.Add(item);
            Added?.Invoke(item);
        }

        public bool Remove(T item)
        {
            if (!_items.Remove(item))
                return false;

            Removed?.Invoke(item);
            return true;
        }

        public void Refresh()
        {
            var temp = new List<T>();
            temp.AddRange(_items);
            foreach (var a in temp)
                Remove(a);
            foreach (var a in temp)
                Add(a);
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
