using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WolvenKit.Converters;

/// <summary>
/// Legacy type converter (previously IBindingTypeConverter for ReactiveUI). Kept for the one call site in ShowChecklistDialog; functionality preserved.
/// </summary>
public class CollectionToCollectionTypeConverter
{
    public bool TryConvert(object from, Type toType, object conversionHint, out object result)
    {
        if (from == null)
        {
            result = null;
            return true;
        }

        if (toType == typeof(ObservableCollection<object>) && from is IEnumerable<object> enumerable)
        {
            result = new ObservableCollection<object>(enumerable);
            return true;
        }

        if (toType == typeof(List<object>) && from is IEnumerable<object> enumerableList)
        {
            result = enumerableList.ToList();
            return true;
        }

        result = from;
        return false;
    }
}