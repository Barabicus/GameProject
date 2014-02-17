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

        foreach (Collider c in Physics.OverlapSphere(point.position, 50f, 1 << 11))
        {
            if (c.tag.Equals("Tree"))
            {
                _resources.Add(c.GetComponent<WorldResource>(), null);
            }
        }
        blueprints = BlueprintList.Instance.Blueprints;
		Resource.AddResource(ResourceType.Wood, 500);
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
                        if (Resource.CurrentResources[ResourceType.Wood] > 200)
                        {
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
        if (Resource.CurrentResources[ResourceType.Wood] < 200 && LumberWorkers.Count != 3)
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
        foreach (BuildingConstructor bc in blueprints)
        {
            if (!bc.HasBeenSupplied)
            {
                if (mob.Resource.CurrentResources[ResourceType.Wood] == 0 && mob.ActionEntity != this)
                {
                    // Need wood, get wood
                    mob.CurrentActivity = ActivityState.Retrieving;
                    mob.SetEntityAndFollow(this);
                }
                else if (mob.Resource.CurrentResources[ResourceType.Wood] >= 1)
                {
                    // We have enough, supply
                    mob.CurrentActivity = ActivityState.Supplying;
                    mob.SetEntityAndFollow(bc);
                }
                break;
            }
            else
            {
                mob.CurrentActivity = ActivityState.Building;
                mob.SetEntityAndFollow(bc);
                break;
            }
        }
    }

    void LumberTask(Mob mob)
    {
        if (mob.ActionEntity == null)
        {
            foreach (Collider c in Physics.OverlapSphere(point.position, 50f, 1 << 11))
            {
                if (c.tag.Equals("Tree"))
                {
                    mob.PerformAction(new PerformActionEvent(c.GetComponent<WorldResource>()));
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

