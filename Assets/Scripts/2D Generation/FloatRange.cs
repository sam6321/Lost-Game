using System;

// Serializable so it will show up in the inspector.
[Serializable]
public class FloatRange
{
    public float m_Min;       // The minimum value in this range.
    public float m_Max;       // The maximum value in this range.


    // Constructor to set the values.
    public FloatRange(float min, float max)
    {
        m_Min = min;
        m_Max = max;
    }


    // Get a random value from the range.
    public float Random
    {
        get { return UnityEngine.Random.Range(m_Min, m_Max); }
    }
}