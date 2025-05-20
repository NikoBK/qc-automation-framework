using System;
using System.Collections.Generic;

public class TimerTestTile
{
    public bool Occupied = false;
    public DateTime OccupyStart = DateTime.Now;
    public DateTime OccupyEnd = DateTime.MinValue;

    // 2 * 715 ms (calculated on shuttle speed)
    private TimeSpan _offset = new TimeSpan(0, 0, 0, 1, 430);

    // Key: occupation start, value: occupation end
    public Dictionary<DateTime, DateTime> BookedOccupations;

    public TimerTestTile() {
        BookedOccupations = new Dictionary<DateTime, DateTime>();
    }

    public bool IsAvailableAtTime(DateTime targetTime)
    {
        foreach (var time in BookedOccupations)
        {
            if (targetTime > time.Key && targetTime < time.Value) {
                // inbetween existing time
                return false;
            }
            if (targetTime + _offset > time.Key) {
                // Overlapped trajectories
                return false;
            }
        }
        BookedOccupations.Add(targetTime, targetTime + _offset);
        return true;
    }

    public void Tick()
    {
        var currTime = DateTime.Now;
        foreach (var occupation in BookedOccupations) {
            if (occupation.Value < currTime) {
                BookedOccupations.Remove(occupation.Key);
            }
        }
    }

    /* NOTE: old, scrap it maybe
     *   public void OccupyTile()
     *   {
     *       if (Occupied) {
     *           Console.WriteLine("Already occupied!");
     *           return;
}

OccupyStart = DateTime.Now;
OccupyEnd = OccupyStart + _offset;
// Alternative: OccupyEnd = OccupyStart.Add(_offset); //NOTE: I am not sure if this changes the value for OccupyStart on its memory address...
Occupied = true;
}

public bool IsAvailableAtTime(DateTime targetTime)
{
if (targetTime > OccupyEnd || (targetTime + _offset) < OccupyStart) {
    return true;
}
else {
    return false;
}
}*/
}
