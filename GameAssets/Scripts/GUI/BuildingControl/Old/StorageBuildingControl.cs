using UnityEngine;
using System.Collections;

public class StorageBuildingControl : BuildingControl {

    public UILabel woodLabel;
    public UILabel weightLabel;

    StorageBuilding building;

    // Use this for initialization
    void Start()
    {
        building = ParentObject.GetComponent<StorageBuilding>();
    }

    // Update is called once per frame
    void Update()
    {
        woodLabel.text = building.Resource.CurrentResources[ResourceType.Wood] + "";
        weightLabel.text = building.Resource.CurrentWeight + " / " + building.Resource.maxWeight;
    }
}
