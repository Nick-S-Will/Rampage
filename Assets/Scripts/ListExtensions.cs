using System.Collections.Generic;
using System;
using System.Linq;

public static class ListExtensions
{
    private static readonly Random random = new();

    public static IList<T> Shuffle<T>(this IList<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        T[] array = source.ToArray();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }

        return array;
    }
}