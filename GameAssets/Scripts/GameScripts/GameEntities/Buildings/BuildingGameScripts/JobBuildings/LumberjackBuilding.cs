using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : JobBuilding
{
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
                        if (Resource.CurrentResources[ResourceType.Wood] > 20000)
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
        if (Resource.CurrentResources[ResourceType.Wood] < 200)
        {
            foreach (Mob m in Workers)
            {
                if (m.JobTask != LumberTask)
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
            Collider[] c = Physics.OverlapSphere(transform.position, 50f, 1 << 11);
            List<Collider> cl = new List<Collider>();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i].tag.Equals("Tree"))
                    cl.Add(c[i]);
            }
            if (cl.Count > 0)
                mob.PerformAction(new PerformActionEvent(cl[UnityEngine.Random.Range(0, cl.Count)].GetComponent<WorldResource>()));
        }

        if (mob.Resource.CurrentResources[ResourceType.Wood] >= 10 && mob.CurrentActivity != ActivityState.Supplying)
        {
            // We have enough resources, time to supply
            mob.CurrentActivity = ActivityState.Supplying;
            mob.SetEntityAndFollow(this);
        }
    }

}
