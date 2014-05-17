using UnityEngine;
using System.Collections;

public class JobBuildingControlComponent : ControlComponent {

    public UILabel amountLabel;
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
        amountLabel.text = _building.Workers.Count.ToString() + " / " + _building.MaxWorkers;
    }

    public void IncreaseJob()
    {
        _building.MaxWorkers += 1;
    }

    public void DecreaseJobs()
    {
        _building.MaxWorkers = Mathf.Max(0, _building.MaxWorkers - 1);
    }
}
