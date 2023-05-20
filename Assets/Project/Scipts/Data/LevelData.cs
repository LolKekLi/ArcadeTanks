using System.Collections.Generic;

public class LevelData
{
    public List<ITank> Tanks
    {
        get;
        set;
    }

    public LevelData()
    {
        Tanks = new List<ITank>();
    }
}