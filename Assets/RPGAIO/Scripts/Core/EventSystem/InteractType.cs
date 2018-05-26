namespace LogicSpawn.RPGMaker.Core
{
    public enum InteractType
    {
        //Help:
        //Interaction:      | Interaction Parameter:
        Collide = 0,
        Click = 1,            //| N/A
        NearTo = 2,           //| Distance to GameObject script is on
        GameObject = 3,       // A gameobject colliding with the right name will trigger an event
    }
}