using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : JobBuilding
{

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
        if (mob.CurrentActivity == ActivityState.None)
        {
            Collider[] c = Physics.OverlapSphere(transform.position, 50f, 1 << 11);
            List<Collider> cl = new List<Collider>();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i].tag.Equals("Tree"))
                    cl.Add(c[i]);
            }
            if (cl.Count > 0)
                mob.PerformAction(new PerformActionVariables(cl[UnityEngine.Random.Range(0, cl.Count)].GetComponent<WorldResource>()));
        }

        if (mob.Resource.CurrentResources[ResourceType.Wood] >= 10 && mob.CurrentActivity != ActivityState.Supplying)
        {
            // We have enough resources, time to supply
            mob.CurrentActivity = ActivityState.Supplying;
            mob.PerformActionVariables = new PerformActionVariables(mob, ResourceType.Wood, 10);
            mob.SetEntityAndFollow(CityManager.FindStorageBuildings()[0]);
        }
    }

}
