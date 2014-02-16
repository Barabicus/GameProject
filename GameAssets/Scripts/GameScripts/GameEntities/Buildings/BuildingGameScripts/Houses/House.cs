using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House : Building
{

    public int maxResidents;

    public Mob[] tempResidents;

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

    protected override void Start()
    {
        base.Start();
        _currentResidents = new List<Mob>();
        foreach (Mob m in tempResidents)
        {
            AddResident(m);
        }
    }

    public void AddResident(Mob mob)
    {
        if (_currentResidents.Count < maxResidents)
        {
            Debug.Log(mob);
            mob.Killed += RemoveResident;
            mob.CityManager = CityManager;
            _currentResidents.Add(mob);
        }
    }

    public void RemoveResident(Mob mob)
    {
        if (_currentResidents.Contains(mob))
        {
            _currentResidents.Remove(mob);
        }
    }

    protected override void Tick()
    {
        base.Tick();
    }
}
