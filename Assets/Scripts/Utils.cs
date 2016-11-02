using System;
using System.Collections.Generic;


public static class Utils
{
    public static T MaxBy<T, U>(this IEnumerable<T> list, Func<T, U> map) where U : IComparable<U>
    {
        bool empty = true;
        U bestValue = default(U);
        T bestKey = default(T);
        foreach (T key in list)
        {
            U value = map(key);
            if (empty || value.CompareTo(bestValue) > 0)
            {
                bestValue = value;
                bestKey = key;
                empty = false;
            }
        }
        return bestKey;
    }
    public static T MinBy<T, U>(this IEnumerable<T> list, Func<T, U> map) where U : IComparable<U>
    {
        bool empty = true;
        U bestValue = default(U);
        T bestKey = default(T);
        foreach (T key in list)
        {
            U value = map(key);
            if (empty || value.CompareTo(bestValue) < 0)
            {
                bestValue = value;
                bestKey = key;
                empty = false;
            }
        }
        return bestKey;
    }
    public static bool special(this Good good)
    {
        return good == Good.habitability || good == Good.mineralAbundance || good == Good.gasAbundance;
    }
}

/*public class Cache<T>
{
    private T? cache;
    private Func<T> func;

    public Cache(Func<T> _func)
    {
        func = _func;
    }

    public static implicit operator T(Cache<T> cache)
    {
        if (!cache.cache.HasValue)
        {
            cache.cache = cache.func();
        }
        return cache.cache.Value;
    }

    public void clear() { cache = null; }
}*/