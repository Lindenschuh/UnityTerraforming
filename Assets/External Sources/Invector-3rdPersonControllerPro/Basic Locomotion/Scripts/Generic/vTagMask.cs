using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System;
/// <summary>
/// TagMask allow you to display the Tags popup menu in the inspector
/// </summary>
[System.Serializable]
public class vTagMask
{
    [SerializeField]
    private int value;
    [SerializeField]
    private  List<string> tags = new List<string>();
  
    public vTagMask()
    {

    }

    public vTagMask(List<string>tags)
    {
        this.tags = tags;
    }

    public vTagMask(params string[] arg)
    {
        this.tags = new List<string>(arg);
    }

    public bool Contains(string tag)
    {
        return tags.Contains(tag);
    }

    public void Add(string tag)
    {
        if (!tags.Contains(tag)) tags.Add(tag);
    }

    public void Remove(string tag)
    {
        if (tags.Contains(tag)) tags.Remove(tag);
    }

    public void Clear()
    {
        tags.Clear();
    }

    public static implicit operator List<string> (vTagMask t)
    {
        return t.tags;
    }

    public static implicit operator string[](vTagMask t)
    {
        return t.tags.ToArray();
    }

    public static implicit operator vTagMask(List<string> l)
    {
        return new vTagMask(l);
    }

    public static implicit operator vTagMask(string[] l)
    {
        return new vTagMask(l);
    }   

    public static vTagMask operator +(vTagMask a,vTagMask b)
    {
        for(int i=0;i<b.tags.Count; i++)
        {
            if (!a.Contains(b.tags[i]))
                a.Add(b.tags[i]);
        }
        return a.tags;
    }

    public static vTagMask operator -(vTagMask a, vTagMask b)
    {
        for (int i = 0; i < b.tags.Count; i++)
        {
            if (a.Contains(b.tags[i]))
                a.Remove(b.tags[i]);
        }
        return a.tags;
    }

    public static vTagMask operator +(vTagMask a, List<string> b)
    {
        for (int i = 0; i < b.Count; i++)
        {
            if (!a.Contains(b[i]))
                a.Add(b[i]);
        }
        return a.tags;
    }

    public static vTagMask operator -(vTagMask a, List<string> b)
    {
        for (int i = 0; i < b.Count; i++)
        {
            if (a.Contains(b[i]))
                a.Remove(b[i]);
        }
        return a.tags;
    }

    public static vTagMask operator +(vTagMask a, string[] b)
    {
        for (int i = 0; i < b.Length; i++)
        {
            if (!a.Contains(b[i]))
                a.Add(b[i]);
        }
        return a.tags;
    }
 
    public static vTagMask operator -(vTagMask a, string[] b)
    {
        for (int i = 0; i < b.Length; i++)
        {
            if (a.Contains(b[i]))
                a.Remove(b[i]);
        }
        return a.tags;
    }
}

