using System.Threading;

public class TimerTestManager
{
    public List<TimerTestTile> Tiles;

    public void Update()
    {
        while(true)
        {
            Thread.Sleep(250);
            foreach (var tile in Tiles) {
                tile.Tick();
            }
        }
    }
}
