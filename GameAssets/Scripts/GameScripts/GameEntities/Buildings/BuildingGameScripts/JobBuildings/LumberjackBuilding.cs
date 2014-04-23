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
        JobBuilding.LumberTask(this, mob);
    }

}
