using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : JobBuilding
{

    public Transform point;

    void Start()
    {
        base.Start();
        Debug.Log(FactionFlags);
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
