using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    [SerializeField]
    public List<TKey> keys = new List<TKey>();

    [SerializeField]
    public List<TValue> values = new List<TValue>();

    public void UpdateDictionary()
    {
        Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Key count and value count do not match!");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], values[i]);
        }
    }
}