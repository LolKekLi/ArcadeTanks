using System.Collections.Generic;
using Project;

public class LevelData
{
    public List<ITank> Tanks
    {
        get;
        set;
    }

    public TankController TankController
    {
        get;
        set;
    }

    public LevelData()
    {
        Tanks = new List<ITank>();
    }
}