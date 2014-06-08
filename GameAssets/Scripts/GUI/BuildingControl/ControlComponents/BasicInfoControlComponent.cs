using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BasicInfoControlComponent : ControlComponent {

    public UILabel pop;

    private Action _updateText;

    void Start()
    {
        Type buildingType = BuildingControl.ParentObject.GetType();

        if (buildingType == typeof(House))
        {
            _updateText = () =>
            {
                House h = BuildingControl.ParentObject.GetComponent<House>();
                pop.text = "Curret Residents: " + h.CurrentResidents.Count + " / " + h.maxResidents;
            };
        }
        else if (buildingType == typeof(JobBuilding))
        {
            _updateText = () =>
                {
                    _updateText = () => { pop.text = "This is a job building"; };
                };
        }
        else
        {
            _updateText = () => { pop.text = "Invalid Building"; };
        }
    }

    void Update()
    {
        _updateText();
    }

}
