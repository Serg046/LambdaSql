using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SqlSelectBuilder
{
    public class ImmutableList<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _items;

        private ImmutableList(IEnumerable<T> items)
        {
            _items = items;
        }

        public static ImmutableList<T> Empty => new ImmutableList<T>(Enumerable.Empty<T>());

        public ImmutableList<T> Add(T item)
        {
            var list = new List<T>(_items);
            list.Add(item);
            return new ImmutableList<T>(list);
        }

        public ImmutableList<T> AddRange(IEnumerable<T> items)
        {
            var list = new List<T>(_items);
            list.AddRange(items);
            return new ImmutableList<T>(list);
        }

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
