using UnityEngine;
using System.Collections;

public class GenericjobBuildingControl : BuildingControl {

    public UILabel workersLabel;
    public UILabel woodLabel;

    SimpleGenericJobBuilding building;

	// Use this for initialization
    void Start()
    {
        building = Building.GetComponent<SimpleGenericJobBuilding>();
    }
	
	// Update is called once per frame
    void Update()
    {
        workersLabel.text = building.Workers.Count + " / " + building.maxWorkers;
        woodLabel.text = building.Resource.CurrentResources[ResourceType.Wood] + "";
    }
}
