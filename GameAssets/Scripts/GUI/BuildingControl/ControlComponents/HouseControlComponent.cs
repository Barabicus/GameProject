using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseControlComponent : ControlComponent {

    public UILabel populationLabel;
    public UITable table;
    public Transform labelPrefab;

    private House _house;
    private Dictionary<Mob, GameObject> _tableLabels;

    void Awake()
    {
        _tableLabels = new Dictionary<Mob, GameObject>();
    }
    
    void Start()
    {
        _house = ParentObject.GetComponent<House>();
        CheckForNull(_house);
        _house.ResidentAdded += AddResidentBar;
        _house.ResidentRemoved += ResidentRemoved;
        populationLabel.text = "0 / 0";      
        // Create Labels
        for (int i = 0; i < _house.CurrentResidents.Count; i++)
        {
            AddResidentBar(_house.CurrentResidents[i]);
        }
        table.Reposition();
    }

    void OnEnable()
    {
        table.Reposition();
    }

    void AddResidentBar(Mob mob)
    {
        GameObject c = NGUITools.AddChild(table.gameObject, labelPrefab.gameObject);
        UILabel label = c.transform.FindChild("Name").GetComponent<UILabel>();
        label.text = mob.UnitName;
        _tableLabels.Add(mob, c);
        populationLabel.text = _tableLabels.Count + " / " +_house.maxResidents;
        table.Reposition();
    }

    void ResidentRemoved(Mob mob)
    {
        Destroy(_tableLabels[mob]);
        _tableLabels.Remove(mob);
        populationLabel.text = _tableLabels.Count + " / " + _house.maxResidents;
        table.Reposition();
    }


}
