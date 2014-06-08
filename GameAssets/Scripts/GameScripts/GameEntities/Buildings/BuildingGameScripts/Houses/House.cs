using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class House : Building
{

    public event MobAction ResidentAdded;
    public event MobAction ResidentRemoved;
    public int maxResidents;
    public Transform spawnPoint;

    public Mob[] tempResidents;

    private List<Mob> _currentResidents;

    #region Properties

    public List<Mob> CurrentResidents
    {
        get
        {
            return _currentResidents;
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
            if (m == null)
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
        if (mob == null)
            return;
        if (_currentResidents.Count < maxResidents && !_currentResidents.Contains(mob))
        {
            mob.Killed += RemoveResident;
            mob.FactionFlags = this.FactionFlags;
            mob.EnemyFlags = this.EnemyFlags;
            CityManager.AddCitizen(mob);
            mob.House = this;
            _currentResidents.Add(mob);
        }
        if (ResidentAdded != null)
            ResidentAdded(mob);
    }

    public void RemoveResident(Mob mob)
    {
        if (_currentResidents.Contains(mob))
        {
            _currentResidents.Remove(mob);
        }
        if (ResidentRemoved != null)
            ResidentRemoved(mob);
    }

    protected override void Tick()
    {
        base.Tick();

        if (HasRoom)
        {
            // Find Spawn point
            RaycastHit hit;
            Physics.Raycast(new Ray(spawnPoint.position + new Vector3(0, 100, 0), -Vector3.up), out hit, Mathf.Infinity, 1 << 9);
            AddResident(PlayerManager.Instance.SpawnMonster(1, hit.point + new Vector3(0, 2, 0), spawnPoint.rotation));
        }

    }
}