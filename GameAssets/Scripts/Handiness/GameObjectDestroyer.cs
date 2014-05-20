using UnityEngine;
using System.Collections;

public class GameObjectDestroyer : MonoBehaviour {

    public void DestroyObject()
    {
        Destroy(transform.parent.gameObject);
    }

}
