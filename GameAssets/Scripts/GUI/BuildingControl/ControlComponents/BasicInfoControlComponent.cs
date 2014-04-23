using UnityEngine;
using System.Collections;

public class BasicInfoControlComponent : ControlComponent {

    public UILabel pop;

    void Update()
    {
        pop.text = BuildingControl.ParentObject.Resource.ToString();
    }

}
