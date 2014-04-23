using UnityEngine;
using System.Collections;

public class LumberBuildingControl : BuildingControl{

    public UILabel workersLabel;
    public UILabel woodLabel;

    LumberjackBuilding building;

    // Use this for initialization
    void Start()
    {
        building = ParentObject.GetComponent<LumberjackBuilding>();
    }

    // Update is called once per frame
    void Update()
    {
        workersLabel.text = building.Workers.Count + " / " + building.maxWorkers;
        woodLabel.text = building.Resource.CurrentResources[ResourceType.Wood] + "";
    }
}
