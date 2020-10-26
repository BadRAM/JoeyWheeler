using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    private static int Value;
    private static float RunTime;

    public static void Increment(int i)
    {
        Value += 1;
    }

    public static int Get()
    {
        return Value;
    }
}
