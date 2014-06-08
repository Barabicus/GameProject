using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : JobBuilding
{

    protected override void Tick()
    {
        base.Tick();
        if (IsBuildingWorking)
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

    public void LumberTask(Mob mob)
    {
        if (mob.CurrentActivity == ActivityState.None)
        {
            Collider[] c = Physics.OverlapSphere(transform.position, 50f, 1 << 11);
            List<Collider> cl = new List<Collider>();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i].tag.Equals("Tree"))
                    cl.Add(c[i]);
            }
            WorldResource closest = null;
            if (cl.Count > 0)
            {
                foreach (Collider collider in cl)
                {
                    WorldResource r = collider.GetComponent<WorldResource>();
                    if (closest == null)
                    {
                        closest = r;
                        continue;
                    }
                    if (Vector3.Distance(r.transform.position, transform.position) < Vector3.Distance(closest.transform.position, transform.position))
                        closest = r;
                }

            }
            if (closest != null)
            {
                mob.PerformActionVariables = new PerformActionVariables(mob);
                mob.PerformAction(new PerformActionVariables(closest));
            }
        }

        if (mob.Resource.GetMaxRemainder(ResourceType.Wood) == 0 && mob.CurrentActivity != ActivityState.Supplying)
        {
            // We have enough resources, time to supply
            mob.CurrentActivity = ActivityState.Supplying;
            mob.PerformActionVariables = new PerformActionVariables(mob, ResourceType.Wood, 10);
            mob.SetEntityAndFollow(CityManager.ClosestStorageBuilding(this));
        }
    }

}
