using UnityEngine;
using System.Collections;

public class DisableOnStart : MonoBehaviour {

    public bool disable;

    void Awake()
    {
        gameObject.SetActive(!disable);
    }

}
