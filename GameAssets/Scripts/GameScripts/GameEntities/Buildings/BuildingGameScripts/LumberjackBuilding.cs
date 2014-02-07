using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : Building
{

    public int maxWorkers = 3;

    private List<Mob> _workers;
    private Type[] _acceptedTypes;

    void Start()
    {
        base.Start();
        Debug.Log(FactionFlags);
        _workers = new List<Mob>();
        _acceptedTypes = new Type[1];
        _acceptedTypes[0] = typeof(OgreAI);
    }

    public void AddWorker(Mob m)
    {
        if (_workers.Count >= maxWorkers)
            return;
        if (Contains(m.GetType()))
        {
            if (!_workers.Contains(m))
                _workers.Add(m);
        }
    }

    private bool Contains(Type type)
    {
        foreach (Type t in _acceptedTypes)
        {
            if (t == type)
                return true;
        }
        return false;
    }

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                AddWorker(actionEvent.entity.GetComponent<Mob>());
                break;
        }
    }

    protected override void Tick()
    {
        base.Tick();
        foreach (RaycastHit hit in Physics.SphereCastAll(new Ray(transform.position, Vector3.forward), 10f))
        {
            if (hit.collider.tag.Equals("Tree"))
            {
                ActiveEntity e = hit.collider.GetComponent<ActiveEntity>();
            }
        }
    }


}
