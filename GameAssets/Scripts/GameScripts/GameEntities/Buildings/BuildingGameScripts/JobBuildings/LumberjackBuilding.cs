using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LumberjackBuilding : JobBuilding
{
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
                AddWorker(actionEvent.entity.GetComponent<Mob>());
                break;
        }
    }

    protected override void Tick()
    {
        base.Tick();
    }


}
