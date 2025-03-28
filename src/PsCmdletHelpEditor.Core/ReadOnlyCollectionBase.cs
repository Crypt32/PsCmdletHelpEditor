using System;
using System.Collections;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.Core;

public abstract class ReadOnlyCollectionBase<T> : IReadOnlyList<T> {
    protected List<T> InternalList { get; } = [];

    /// <inheritdoc />
    public Int32 Count => InternalList.Count;
    /// <inheritdoc />
    public T this[Int32 index] => InternalList[index];

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() {
        return InternalList.GetEnumerator();
    }
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}