using UnityEngine;
using System.Collections;

public class RimSelectProperties : MonoBehaviour {

    public Color selectedColor = Color.white;
    public Color nonSelectColor = Color.black;
    [Range(0.0f, 8.0f)]
    public float rimPower = 1.0f;

    private static RimSelectProperties _instance;
    public static RimSelectProperties instance
    {
        get
        {
            if (_instance == null)
            {
                // Create RimProperties with default values
                Debug.Log("RimSelectProperties instance not found creating a new one");
                GameObject p = GameObject.Find("_Properties");
                GameObject prop = new GameObject("RimSelectProperties");
                prop.AddComponent<RimSelectProperties>();
                prop.transform.parent = p.transform;
            }
            return _instance;                
        }
    }

    public void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("Attempting to re-assign RimSelectProperties instance!\nDestroying object");
            Destroy(gameObject);
        }else
        _instance = this;
    }

}
