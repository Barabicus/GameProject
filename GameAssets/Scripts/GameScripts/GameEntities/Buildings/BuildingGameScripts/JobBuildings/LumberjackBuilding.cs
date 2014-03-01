using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : JobBuilding
{
    List<Mob> LumberWorkers = new List<Mob>();

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                Mob m = actionEvent.entity as Mob;
                AddWorker(m);

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
                if (LumberWorkers.Contains(m))
                    continue;
                LumberWorkers.Add(m);
                if (m.JobTask == null)
                {
                    m.JobTask = LumberTask;
                }
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
