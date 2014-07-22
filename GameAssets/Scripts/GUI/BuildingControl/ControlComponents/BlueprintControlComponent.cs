using UnityEngine;
using System.Collections;

public class BlueprintControlComponent : ControlComponent {

    public UIProgressBar progressBar;
    BuildingConstructor _bConst;

	// Use this for initialization
    void Start()
    {
        _bConst = GetParentObjectComponent<BuildingConstructor>();
    }
	
	// Update is called once per frame
    void Update()
    {
        progressBar.value = ToPercent(_bConst.CurrentBuildUnits, _bConst.requiredBuildUnits);
    }

    float ToPercent(float number, float max)
    {
        return number / max;
    }
}
