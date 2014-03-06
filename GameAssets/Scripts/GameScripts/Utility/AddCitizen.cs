using UnityEngine;
using System.Collections;

/// <summary>
/// Utility script to force add a citizen to a CityManager
/// </summary>
public class AddCitizen : MonoBehaviour {

    public CityManager cityManager;

	// Use this for initialization
    void Start()
    {
        cityManager.AddCitizen(gameObject.GetComponent<Mob>());
        Destroy(this);
    }
	
}
