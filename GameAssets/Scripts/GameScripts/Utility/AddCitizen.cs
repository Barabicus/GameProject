using UnityEngine;
using System.Collections;

/// <summary>
/// Utility script to force add a citizen to a CityManager
/// </summary>
public class AddCitizen : MonoBehaviour {

    public CityManager cityManager;
    public Mob[] citizens;

	// Use this for initialization
    void Start()
    {
        foreach (Mob m in citizens)
        {
            cityManager.AddCitizen(m);
        }
        Destroy(this);
    }
	
}
