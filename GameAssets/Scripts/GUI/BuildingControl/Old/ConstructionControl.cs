using UnityEngine;
using System.Collections;

public class ConstructionControl : BuildingControl {

    BuildingConstructor buildingConstructor;
    public UIProgressBar progressBar;

	// Use this for initialization
    void Start()
    {
        buildingConstructor = ParentObject.GetComponent<BuildingConstructor>();
    }
	
	// Update is called once per frame
    void Update()
    {
        progressBar.value = (float)buildingConstructor.CurrentBuildUnits / (float)buildingConstructor.requiredBuildUnits;
    }
}
