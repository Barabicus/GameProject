using UnityEngine;
using System.Collections;

public class JobBuildingControlComponent : ControlComponent {

    public UILabel amountLabel;
    public UILabel unassignedAmount;
    JobBuilding _building;


	// Use this for initialization
    void Start()
    {
        _building = ParentObject.GetComponent<JobBuilding>();
        CheckForNull(_building);
    }
	
	// Update is called once per frame
    void Update()
    {
        amountLabel.text = _building.Workers.Count + " / " + _building.maxWorkers;
        unassignedAmount.text = _building.UnassignedWorkers.Count + " / " + _building.maxWorkers;
    }
}
