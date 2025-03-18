using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core.Utils;

public static class ListExtensions {
    public static T MoveUp<T>(this IList<T> collection, T item) {
        Int32 old = collection.IndexOf(item);
        if (old < 1) {
            return item;
        }
        T temp = collection[old - 1];
        collection[old - 1] = item;
        collection[old] = temp;

        return item;
    }
    public static T MoveDown<T>(this IList<T> collection, T item) {
        Int32 old = collection.IndexOf(item);
        if (old < 0 || old == collection.Count - 1) {
            return item;
        }
        T temp = collection[old + 1];
        collection[old + 1] = item;
        collection[old] = temp;

        return item;
    }
}