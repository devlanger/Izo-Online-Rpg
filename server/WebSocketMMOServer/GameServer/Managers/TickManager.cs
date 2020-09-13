using System;

public class TickManager
{
    public TickManager()
    {
        StartTime = new DateTime(2020, 1, 1).Ticks;
    }

    public float Time
    {
        get
        {
            return (float)(DateTime.Now.Ticks - StartTime) / (float)1000;
        }
    }

    public long StartTime;
    public event Action OnTick = delegate { };

    public void Tick()
    {
        OnTick();
    }
}