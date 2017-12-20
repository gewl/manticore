using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventEmitter : MonoBehaviour {

    public delegate void EntityAlteredHandler (GlobalConstants.EntityTypes entityType);
    public static EntityAlteredHandler OnEntityDiedHandler;  

}
