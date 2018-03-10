using UnityEngine;

// Only allows for one instance of this script active.
public class SingletonGameObject : MonoBehaviour {

    static SingletonGameObject instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
