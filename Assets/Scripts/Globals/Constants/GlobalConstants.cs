using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: There are global constants in this directory outside this class.
// Either move them all in here or move all these ones outside.
public static class GlobalConstants {

    public enum GameFreezeEvent
    {
        EntityInjured,
        EntityDead,
        Riposte
    }

    public enum Collectibles
    {
        StaminaPill 
    }

    public enum EntityAllegiance
    {
        Friendly,
        Enemy
    }

    public enum GameStateEvents
    {
        EntityDied,
        PlayerDied,
        NewMomentumPoint,
        HardwareDiscovered,
        MomentumLost
    }
}
