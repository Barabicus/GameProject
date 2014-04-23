using UnityEngine;
using System.Collections;

public class HouseBuildingControl : BuildingControl {

    House house;
    public UILabel residentsNumber;

	// Use this for initialization
    void Start()
    {
        house = ParentObject.GetComponent<House>();
    }
	
	// Update is called once per frame
    void Update()
    {
        residentsNumber.text = house.CurrentResidents.Count + "";
    }
}
