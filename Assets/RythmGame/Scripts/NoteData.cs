// ChartData.cs
using System;
using System.Collections.Generic;

[Serializable]
public class NoteData
{
    public float beat;   // When to hit (in beats)
    public int lane;     // Which lane (0–3)
}

[Serializable]
public class ChartData
{
    public float bpm;
    public float offset;
    public List<NoteData> notes;
}
