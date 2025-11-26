// This code is distributed under MIT license. Copyright (c) 2013 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

// https://gist.github.com/gmamaladze/3d60c127025c991a087e

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CasaXpsUtilities.Shared;

/// <summary>
/// An ordered set implementation that maintains the insertion order of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
/// <param name="comparer">The equality comparer to use for the set.</param>
public class OrderedSet<T>(IEqualityComparer<T> comparer) : ICollection<T>
    where T : notnull
{
    private readonly Dictionary<T, LinkedListNode<T>> _mDictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
    private readonly LinkedList<T> _mLinkedList = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedSet{T}" /> class that uses the default equality comparer for the set type.
    /// </summary>
    public OrderedSet() : this(EqualityComparer<T>.Default)
    {
    }

    /// <summary>
    /// Gets the number of elements that are contained in the set.
    /// </summary>
    public int Count
    {
        get { return _mDictionary.Count; }
    }

    public virtual bool IsReadOnly
    {
        get { return false; }
    }

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    /// <summary>
    /// Removes all elements from the set.
    /// </summary>
    public void Clear()
    {
        _mLinkedList.Clear();
        _mDictionary.Clear();
    }

    /// <summary>
    /// Removes the specified element from the set.
    /// </summary>
    /// <param name="item">The element to remove.</param>
    /// <returns><see langword="true"/> if the item was successfully found and removed; otherwise, <see langword="false"/>. This method returns <see langword="false"/> if the item is not found in the set.</returns>
    public bool Remove(T item)
    {
        var found = _mDictionary.TryGetValue(item, out var node);
        if (!found)
        {
            return false;
        }

        _mDictionary.Remove(item);
        _mLinkedList.Remove(node!);

        return true;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the set.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        return _mLinkedList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Determines whether the set contains the specified element.
    /// </summary>
    /// <param name="item">The element to locate in the set.</param>
    public bool Contains(T item)
    {
        return _mDictionary.ContainsKey(item);
    }

    /// <summary>
    /// Copies the elements of the set to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The array to copy the elements to.</param>
    /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _mLinkedList.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Adds the specified element to the set.
    /// </summary>
    /// <param name="item">The element to add to the set.</param>
    public bool Add(T item)
    {
        if (_mDictionary.ContainsKey(item))
        {
            return false;
        }

        var node = _mLinkedList.AddLast(item);
        _mDictionary.Add(item, node);

        return true;
    }
}

/// <summary>
/// An ordered set implementation that maintains the insertion order of elements.
/// </summary>
/// <typeparam name="T">The type of elements in the set.</typeparam>
public class OrderedSetExt<T> : OrderedSet<T>, ISet<T>
    where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedSetExt{T}" /> class that uses the default equality comparer for the set type.
    /// </summary>
    public OrderedSetExt()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedSetExt{T}" /> class that uses the specified equality comparer.
    /// </summary>
    /// <param name="comparer">The equality comparer to use for the set.</param>
    public OrderedSetExt(IEqualityComparer<T> comparer) : base(comparer)
    {
        if (comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderedSetExt{T}"/> class that contains elements copied from the specified
    /// collection and uses the default equality comparer for the element type.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new set.</param>
    public OrderedSetExt(IEnumerable<T> collection) : this(collection, EqualityComparer<T>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OrderedSetExt class that contains elements copied from the specified
    /// collection and uses the specified equality comparer.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new set. Duplicate elements are ignored.</param>
    /// <param name="comparer">The equality comparer to use for comparing elements.</param>
    public OrderedSetExt(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer)
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Modifies the current set so that it contains all elements that are present in both the current set and in the
    /// specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public void UnionWith(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        foreach (var element in other)
        {
            Add(element);
        }
    }

    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public void IntersectWith(IEnumerable<T> other)
    {
        foreach (var element in other)
        {
            if (Contains(element))
            {
                continue;
            }

            Remove(element);
        }
    }

    /// <summary>
    /// Removes all elements in the specified collection from the current set.
    /// </summary>
    /// <param name="other">The collection of items to remove from the set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public void ExceptWith(IEnumerable<T> other)
    {
        foreach (var element in other)
        {
            Remove(element);
        }
    }

    /// <summary>
    /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        foreach (var element in other)
        {
            if (Contains(element))
            {
                Remove(element);
            }
            else
            {
                Add(element);
            }
        }
    }

    /// <summary>
    /// Determines whether a set is a subset of a specified collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current set is a subset of <paramref name="other" />; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        var otherHashset = new HashSet<T>(other);

        return otherHashset.IsSupersetOf(this);
    }

    /// <summary>
    /// Determines whether the current set is a superset of a specified collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current set is a superset of <paramref name="other" />; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return other.All(Contains);
    }

    /// <summary>
    /// Determines whether the current set is a correct superset of a specified collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the <see cref="T:System.Collections.Generic.ISet`1" /> object is a correct superset of <paramref name="other" />; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="other">The collection to compare to the current set. </param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        var otherHashset = new HashSet<T>(other);

        return otherHashset.IsProperSubsetOf(this);
    }

    /// <summary>
    /// Determines whether the current set is a property (strict) subset of a specified collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current set is a correct subset of <paramref name="other" />; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        var otherHashset = new HashSet<T>(other);

        return otherHashset.IsProperSupersetOf(this);
    }

    /// <summary>
    /// Determines whether the current set overlaps with the specified collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current set and <paramref name="other" /> share at least one common element; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public bool Overlaps(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (Count == 0)
        {
            return false;
        }

        return other.Any(Contains);
    }

    /// <summary>
    /// Determines whether the current set and the specified collection contain the same elements.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current set is equal to <paramref name="other" />; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="other" /> is <see langword="null"/>.</exception>
    public bool SetEquals(IEnumerable<T> other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        var otherHashset = new HashSet<T>(other);

        return otherHashset.SetEquals(this);
    }
}
