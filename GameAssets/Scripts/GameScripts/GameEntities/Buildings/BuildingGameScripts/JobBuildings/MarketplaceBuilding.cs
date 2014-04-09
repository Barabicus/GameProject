using UnityEngine;
using System.Collections;

public class MarketplaceBuilding : JobBuilding {

    public int restockMeatWhenAt = 10;

    public override void PerformAction(PerformActionVariables actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                Mob m = actionEvent.entity.GetComponent<Mob>();
                switch (m.CurrentActivity)
                {
                    case ActivityState.Supplying:
                        if (m.Resource.CurrentResources[ResourceType.Meat] > 0)
                        {
                            Resource.TransferResources(m.Resource, ResourceType.Meat, 1);
                        }
                        else
                        {
                            m.CurrentActivity = ActivityState.None;
                        }
                        break;
                }
                break;
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (Workers.Count > 0)
            Workers[0].JobTask = WorkerTask;
    }

    void WorkerTask(Mob m)
    {
        if (m.Resource.CurrentResources[ResourceType.Meat] <= restockMeatWhenAt)
        {
            if (m.Resource.CurrentResources[ResourceType.Meat] == 0 && m.CurrentActivity !=  ActivityState.Retrieving)
            {
                m.CurrentActivity = ActivityState.Retrieving;
                m.PerformActionVariables = new PerformActionVariables(m, ResourceType.Meat, 5);
                m.SetEntityAndFollow(CityManager.FindStorageBuildings()[0]);
            }
        }
        else if(m.CurrentActivity != ActivityState.Supplying)
        {
            m.CurrentActivity = ActivityState.Supplying;
            m.SetEntityAndFollow(this);
        }
    }

}
