using System;
using System.Collections.Generic;
using UnityEngine;

public static class Subsystem
{
    private static readonly Dictionary<string, object> m_dictionaryRef = new();

    public static void Bind(this object _instance)
    {   
        string _key = _instance.GetType().Name;
        
        if (!m_dictionaryRef.TryAdd(_key, _instance))
        {
            Debug.Log("Key : " + _key + " already exist");
        }
    }
     
    public static T Get<T>() where  T : MonoBehaviour
    {
        string _key = typeof(T).Name;
        return m_dictionaryRef.TryGetValue(_key, out var value) ? (T)value : null;
    }
    public static void Clear() => m_dictionaryRef.Clear();
    public static void UnBind<T>() 
    {
        var _key = typeof(T).Name;
        if(m_dictionaryRef.ContainsKey(_key))
        {
            m_dictionaryRef.Remove(typeof(T).Name);
        }
        else
        {
            throw new ArgumentException("Key : " + _key + " not found can't use remove");
        }
    }
}