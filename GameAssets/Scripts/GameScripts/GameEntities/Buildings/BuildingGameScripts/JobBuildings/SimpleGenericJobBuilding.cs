using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleGenericJobBuilding : JobBuilding
{
    List<BuildingConstructor> blueprints;

    Mob lumberWorker;
    Mob builderWorker;

    protected override void Start()
    {
        base.Start();
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
                AddWorker(actionEvent.entity as Mob);

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

        if (Workers.Count > 0)
            lumberWorker = Workers[0];

        if (Workers.Count > 1)
            builderWorker = Workers[1];


        if (lumberWorker != null && lumberWorker.JobTask != LumberTask)
            lumberWorker.JobTask = LumberTask;

        if (builderWorker != null && builderWorker.JobTask != BuildTask)
            builderWorker.JobTask = BuildTask;
    }

    void BuildTask(Mob mob)
    {
        Debug.Log("Build: " + blueprints.Count);
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
            foreach (Collider c in Physics.OverlapSphere(transform.position, 50f, 1 << 11))
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

