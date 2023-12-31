using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions 
{
    private static System.Random random = new System.Random();

    /// <summary>
    /// Shuffle a list of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}