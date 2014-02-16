using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class JobBuilding : Building {

    private List<Mob> _workers;
    public int maxWorkers = 0;

    public List<Mob> Workers
    {
        get { return _workers; }
    }


    protected override void Awake()
    {
        base.Awake();
        _workers = new List<Mob>();
    }

    /// <summary>
    /// Add a worker returns true if the worker was added
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    protected bool AddWorker(Mob m)
    {
        // Cant add another worker
        if (_workers.Count >= maxWorkers || _workers.Contains(m))
            return false;
        _workers.Add(m);
        return true;
    }

    protected bool RemoveWorker(Mob m)
    {
        if (_workers.Contains(m))
        {
            _workers.Remove(m);
            return true;
        }
        return false;
    }
	
}
