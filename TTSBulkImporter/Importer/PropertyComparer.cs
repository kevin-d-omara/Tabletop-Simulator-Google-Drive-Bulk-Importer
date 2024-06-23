using System;
using System.Collections.Generic;

namespace TTSBulkImporter.Importer
{
    public class PropertyComparer<TClass, TProperty> : IComparer<TClass>
    {
        public IComparer<TProperty> Comparer { get; }
        public Func<TClass, TProperty> Accessor { get; }

        public PropertyComparer(IComparer<TProperty> comparer, Func<TClass, TProperty> accessor)
        {
            Comparer = comparer;
            Accessor = accessor;
        }

        public int Compare(TClass x, TClass y)
        {
            return Comparer.Compare(Accessor(x), Accessor(y));
        }
    }
}
