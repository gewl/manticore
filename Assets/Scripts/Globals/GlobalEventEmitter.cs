using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventEmitter : MonoBehaviour {

    // TODO: This is ugly rn. I'm making it for momentum broadcasting on death, hence 'quantity.'
    // It looks generic but I can't imagine this would ever be reused as-is. Move to UnityAction<T>?
    public delegate void EntityAlteredDelegate(string EntityID, int quantity);
    public static EntityAlteredDelegate OnEntityDied;
}
