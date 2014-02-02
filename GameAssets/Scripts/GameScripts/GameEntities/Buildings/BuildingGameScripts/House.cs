using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : Building
{

    public int maxResidents;

    private List<Mob> _currentResidents;
    private CityManager _belongingCity;

    #region Properties
    public int CurrentResidentsCount { get { return _currentResidents.Count; } }
    public CityManager CityManager
    {
        get
        {
            return _belongingCity;
        }
        set
        {
            _belongingCity = value;
        }
    }
    #endregion

    void Start()
    {
        base.Start();
        _currentResidents = new List<Mob>();
    }

    void Update()
    {

    }

    public void AddResident(Mob mob)
    {
        Debug.Log(_currentResidents.Count + " : " + maxResidents);
        if (_currentResidents.Count < maxResidents)
        {
            mob.Killed += RemoveResident;
            _currentResidents.Add(mob);
            Debug.Log("Found House: " + mob.name);
        }
    }

    public void RemoveResident(Mob mob)
    {
        if (_currentResidents.Contains(mob))
        {
            _currentResidents.Remove(mob);
        }
    }
}
