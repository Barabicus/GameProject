using UnityEngine;
using System.Collections;

public class MarketplaceBuilding : JobBuilding {

    public int restockMeatWhenAt = 10;


    public override void Start()
    {
        base.Start();
        BuildingResourceRequestManager.AddRequest(ResourceType.Meat, 10);
    }

    protected override void Tick()
    {
        Debug.Log("MEAT: " + Resource[ResourceType.Meat]);
        base.Tick();
    }

    void WorkerTask(Mob m)
    {

    }

}
