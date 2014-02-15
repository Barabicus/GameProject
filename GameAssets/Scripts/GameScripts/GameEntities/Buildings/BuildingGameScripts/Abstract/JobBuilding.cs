using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class JobBuilding : Building {

    private List<Mob> _workers;
    private List<ActiveEntity> _cachedActiveEntity;
    public int maxWorkers = 0;

    public List<Mob> Workers
    {
        get { return _workers; }
    }
    public List<ActiveEntity> WorkerActiveEntity
    {
        get { return _cachedActiveEntity; }
    }

    protected override void Awake()
    {
        base.Awake();
        _workers = new List<Mob>();
        _cachedActiveEntity = new List<ActiveEntity>();
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
        WorkerActiveEntity.Add(m.ActionEntity);
        return true;
    }

    protected bool RemoveWorker(Mob m)
    {
        if (_workers.Contains(m))
        {
            _workers.Remove(m);
            _cachedActiveEntity.Remove(m.ActionEntity);
            return true;
        }
        return false;
    }
	
}
