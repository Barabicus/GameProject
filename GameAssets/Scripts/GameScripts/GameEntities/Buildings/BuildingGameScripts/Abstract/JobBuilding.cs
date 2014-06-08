using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class JobBuilding : Building {

    private List<Mob> _workers;

    public int maxAmountOfWorkers = 0;
    public int workWage = 20;
    public bool IsBuildingWorking = true;

    public List<Mob> Workers
    {
        get { return _workers; }
    }

    int _maxWorkers;
    public int MaxWorkers
    {
        get
        {
            return _maxWorkers;
        }
        set
        {
            if (_workers.Count > value)
                RemoveWorker(_workers[_workers.Count - 1]);
            _maxWorkers = Mathf.Min(value, maxAmountOfWorkers);
        }
    }

    public List<Mob> UnassignedWorkers
    {
        get
        {
            return _workers.FindAll(m => m.JobTask == null);
        }
    }


    public override void Awake()
    {
        base.Awake();
        _workers = new List<Mob>();
        _maxWorkers = maxAmountOfWorkers;
    }

    protected override void Tick()
    {
        base.Tick();
        if (!IsBuildingWorking)
            return;
        foreach (Mob m in CityManager.Citizens)
        {
            if (Workers.Count == _maxWorkers)
                break;
            if (!m.HasJobBuilding)
                AddWorker(m);
        }
    }

    /// <summary>
    /// Add a worker returns true if the worker was added
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public bool AddWorker(Mob m)
    {
        // Cant add another worker
        if (_workers.Count >= _maxWorkers || _workers.Contains(m))
            return false;
        if (m.JobBuilding != null)
            m.JobBuilding.RemoveWorker(m);
        m.JobBuilding = this;
        _workers.Add(m);
        return true;
    }

    public bool RemoveWorker(Mob m)
    {
        if (_workers.Contains(m))
        {
            m.JobBuilding = null;
            m.JobTask = null;
            m.CurrentActivity = ActivityState.None;
            _workers.Remove(m);
            return true;
        }
        return false;
    }


}
