using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : Building {

    public int maxResidents;

    private List<Mob> _currentResidents;
    private CityManager _belongingCity;

    #region Properties
    public int CurrentResidentsCount { get { return _currentResidents.Count; } }
    #endregion

    void Start()
    {
        base.Start();
        _currentResidents = new List<Mob>();
    }

	void Update () {
	
	}
}
