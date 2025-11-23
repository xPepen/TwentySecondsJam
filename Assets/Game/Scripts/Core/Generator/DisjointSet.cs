using System.Collections.Generic;

namespace Game.Scripts.Core.Generator
{
    public class DisjointSet<T>
    {
        private Dictionary<T, T> parent = new Dictionary<T, T>();

        public DisjointSet(IEnumerable<T> items)
        {
            foreach (var item in items)
                parent[item] = item;
        }

        public T Find(T item)
        {
            if (!parent.ContainsKey(item))
                parent[item] = item;

            if (!parent[item].Equals(item))
                parent[item] = Find(parent[item]);

            return parent[item];
        }

        public void Union(T a, T b)
        {
            T rootA = Find(a);
            T rootB = Find(b);
            if (!rootA.Equals(rootB))
                parent[rootB] = rootA;
        }
    }
}