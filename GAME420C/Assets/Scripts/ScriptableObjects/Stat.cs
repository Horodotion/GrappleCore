using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    public float stat;
    public float baseStat;
    public float minimum = 0;
    public float maximum;

    void OnValidate()
    {
        baseStat = stat;
    }

    public void AddToStat(float i)
    {
        stat = Mathf.Clamp(stat + i, minimum, maximum);
    }

    public void ResetStat()
    {
        stat = baseStat;
    }

    public bool IsAtMinimum(float i)
    {
        return stat + i <= minimum;
    }

    public bool IsAtMaximum(float i)
    {
        return stat + i >= maximum;
    }
}
