using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseControlComponent : ControlComponent {

    public UILabel populationLabel;
    public UITable table;
    public UILabel labelPrefab;

    private House _house;
    private List<UILabel> _tableLabels;

    void Awake()
    {
        _tableLabels = new List<UILabel>();
    }
    
    void Start()
    {
        _house = ParentObject.GetComponent<House>();
        CheckForNull(_house);
      
        // Create Labels
        for (int i = 0; i < _house.maxResidents; i++)
        {
            UILabel l = Instantiate(labelPrefab) as UILabel;
            l.transform.parent = table.transform;
            l.transform.localScale = new Vector3(1, 1, 1);
            _tableLabels.Add(l);
        }
        table.Reposition();
    }

    void Update()
    {
        populationLabel.text = _house.CurrentResidents.Count + " / " + _house.maxResidents;
    //    for (int i = 0; i < _tableLabels.Count; i++)
    //    {
    //        _tableLabels[i].text = (_house.CurrentResidents.Count <= i-1 && _house.CurrentResidents[i] != null) ? _house.CurrentResidents[i].MobName : noMob;
    //    }
    }

}
