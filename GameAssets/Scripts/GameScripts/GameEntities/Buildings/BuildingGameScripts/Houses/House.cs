using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class House : Building
{

    public int maxResidents;
    public Transform spawnPoint;

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

    public bool HasRoom
    {
        get { return _currentResidents.Count < maxResidents; }
    }
    #endregion

    public override void Start()
    {
        base.Start();
        _currentResidents = new List<Mob>();
        foreach (Mob m in tempResidents)
        {
			if(m == null)
				continue;
            AddResident(m);
        }
    }

    public override void PerformAction(PerformActionVariables actionEvent)
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
            mob.FactionFlags = this.FactionFlags;
            mob.EnemyFlags = this.EnemyFlags;
            CityManager.AddCitizen(mob);
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
        if (HasRoom)
        {
            AddResident(PlayerManager.Instance.SpawnMonster(1, spawnPoint));
        }
    }
}
