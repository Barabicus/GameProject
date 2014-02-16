using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleGenericJobBuilding : JobBuilding
{
    public Transform point;

    Dictionary<WorldResource, Mob> _resources;
    List<BuildingConstructor> blueprints;

    List<Mob> LumberWorkers = new List<Mob>();
    List<Mob> BuilderWorkers = new List<Mob>();

    protected override void Start()
    {
        base.Start();
        _resources = new Dictionary<WorldResource, Mob>();
        foreach (RaycastHit hit in Physics.SphereCastAll(new Ray(point.position, new Vector3(1, 1, 1)), 50f, 1f))
        {
            if (hit.collider.tag.Equals("Tree"))
            {
                _resources.Add(hit.collider.GetComponent<WorldResource>(), null);
            }

        }
        blueprints = BlueprintList.Instance.Blueprints;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                Mob m = actionEvent.entity as Mob;
                if (!Workers.Contains(m))
                {
                    AddWorker(actionEvent.entity as Mob);
                }

                switch (m.CurrentActivity)
                {
                    case ActivityState.Supplying:
                        Resource.AddResource(ResourceType.Wood, m.Resource.RemoveResource(ResourceType.Wood, 10));
                        if (Resource.CurrentResources[ResourceType.Wood] > 50)
                        {
                            m.JobTask = null;
                            LumberWorkers.Remove(m);
                            m.CurrentActivity = ActivityState.None;
                        }
                        if (m.Resource.CurrentResources[ResourceType.Wood] == 0)
                        {
                            m.CurrentActivity = ActivityState.None;
                        }
                        break;
                    case ActivityState.Retrieving:
                        // Give Resource
                        m.Resource.AddResource(ResourceType.Wood, Resource.RemoveResource(ResourceType.Wood, 10));
                        break;
                }
                break;
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (blueprints.Count > 0)
        if (Resource.CurrentResources[ResourceType.Wood] < 50 && LumberWorkers.Count != 3)
        {
            foreach (Mob m in Workers)
            {
                if (LumberWorkers.Count == 3)
                    break;
                if (LumberWorkers.Contains(m) || BuilderWorkers.Contains(m))
                    continue;
                LumberWorkers.Add(m);
                if (m.JobTask == null)
                {
                    m.JobTask = LumberTask;
                }
            }
        }
        if (blueprints.Count > 0 && BuilderWorkers.Count != 3)
        {
            foreach (Mob m in Workers)
            {
                if (BuilderWorkers.Count == 3)
                    break;
                if (LumberWorkers.Contains(m) || BuilderWorkers.Contains(m))
                    continue;
                BuilderWorkers.Add(m);
                if (m.JobTask == null)
                {
                    m.JobTask = BuildTask;
                }
            }
        }

    }

    void BuildTask(Mob mob)
    {
        if (blueprints.Count > 0)
        {
            if (!blueprints[0].HasBeenSupplied)
            {
                if ((mob.Resource.CurrentResources[ResourceType.Wood] < 10 && mob.CurrentActivity != ActivityState.Retrieving && mob.CurrentActivity != ActivityState.Building) || (mob.CurrentActivity == ActivityState.Building && mob.Resource.CurrentResources[ResourceType.Wood] == 0))
                {
                    mob.CurrentActivity = ActivityState.Retrieving;
                    mob.SetEntityAndFollow(this);
                }
                else if (mob.CurrentActivity == ActivityState.Retrieving && mob.Resource.CurrentResources[ResourceType.Wood] > 10)
                {
                    mob.CurrentActivity = ActivityState.Supplying;
                    mob.SetEntityAndFollow(blueprints[0]);
                }
            }
            else
            {
                if (mob.CurrentActivity != ActivityState.Building)
                {
                    mob.CurrentActivity = ActivityState.Building;
                    mob.SetEntityAndFollow(blueprints[0]);
                }
            }
        }
        else
        {
            mob.JobTask = null;
        }
    }

    void LumberTask(Mob mob)
    {
        if (mob.ActionEntity == null)
        {
            foreach (KeyValuePair<WorldResource, Mob> kvp in _resources)
            {
                if (kvp.Value == null)
                {
                    mob.PerformAction(new PerformActionEvent(kvp.Key));
                    _resources[kvp.Key] = mob;
                    break;
                }
            }
        }
        
        if (mob.Resource.CurrentResources[ResourceType.Wood] >= 10 && mob.CurrentActivity != ActivityState.Supplying)
        {
            // We have enough resources, time to supply
            mob.CurrentActivity = ActivityState.Supplying;
            mob.SetEntityAndFollow(this);
        }
    }

}

