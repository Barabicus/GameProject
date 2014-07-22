using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class JobBuilding : Building {

    private List<WorkerDetail> _workers;
    private int hourlyWage = 5;
    private bool _isBuildingWorking = true;

    public int maxAmountOfWorkers = 0;

    public bool IsBuildingWorking
    {
        get { return _isBuildingWorking; }
        set
        {
            if (value == false)
            {
                for (int i = _workers.Count - 1; i >= 0; i--)
                {
                    RemoveWorker(_workers[i].Mob);
                }
            }
            _isBuildingWorking = value;
        }
    }

    public List<Mob> Workers
    {
        get
        {
            var workers = from p in _workers select p.Mob;
            return workers.ToList<Mob>();
        }
    }

    /// <summary>
    /// The Current amount of workers allowed to work here. This value
    /// can be changed during gameplay to allow larger or smaller amounts. 
    /// This amount is limited by the maxAmountOfWorkers variable.
    /// </summary>
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
                RemoveWorker(_workers[_workers.Count - 1].Mob);
            _maxWorkers = Mathf.Min(value, maxAmountOfWorkers);
        }
    }

    public List<Mob> UnassignedWorkers
    {
        get
        {
            var workers = from w in _workers where w.Mob.JobTask == null select w.Mob;
            return workers.ToList<Mob>();
        }
    }


    public override void Awake()
    {
        base.Awake();
        _workers = new List<WorkerDetail>();
        _maxWorkers = maxAmountOfWorkers;
    }

    protected override void Tick()
    {
        base.Tick();
        if (!_isBuildingWorking)
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
        if (_workers.Count >= _maxWorkers || Workers.Contains(m) || !IsBuildingWorking)
            return false;
        if (m.JobBuilding != null)
            m.JobBuilding.RemoveWorker(m);
        m.JobBuilding = this;
        _workers.Add(new WorkerDetail(m, Time.time));
        return true;
    }

    public bool RemoveWorker(Mob m)
    {
        if (Workers.Contains(m))
        {
            m.JobBuilding = null;
            m.JobTask = null;
            m.CurrentActivity = ActivityState.None;
            _workers.Remove(_workers.Find(w => w.Mob == m));
            return true;
        }
        return false;
    }
}

struct WorkerDetail
{
    private Mob _worker;
    private float _workCheckInTime;

    public Mob Mob { get { return _worker; } }
    public float CheckInTime { get { return _workCheckInTime; } }


    public WorkerDetail(Mob worker, float checkInTime)
    {
        this._worker = worker;
        this._workCheckInTime = checkInTime;
    }

}
