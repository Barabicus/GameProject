using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class House : Building
{

    public int maxResidents;

    public Mob[] tempResidents;

    private List<Mob> _currentResidents;

    #region Properties
    /// <summary>
    /// Returns a cloned list of the current residents of this house. 
    /// </summary>
    public ReadOnlyCollection<Mob> CurrentResidents
    {
        get
        {
            return _currentResidents.AsReadOnly();
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

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                AddResident(actionEvent.entity.GetComponent<Mob>());
                break;
        }
    }

    public void AddResident(Mob mob)
    {
        if (_currentResidents.Count < maxResidents && !_currentResidents.Contains(mob))
        {
            mob.Killed += RemoveResident;
            mob.CityManager = CityManager;
            mob.House = this;
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
