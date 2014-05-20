﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class BuildingControl : MonoBehaviour
{

    public UIPanel panel;
    public UILabel BuildingLabel;
    public Transform ContentArea;
    public UIGrid TabLine;

    List<Transform> _tabs = new List<Transform>();

    public Transform TabPrefab;

    #region Control Prefabs

    public ControlComponent ResourceComponent;
    public ControlComponent BasicInfoComponent;
    public ControlComponent HouseComponent;
    public ControlComponent JobBuildingComponent;
    public ControlComponent ProgressComponent;

    #endregion

    GameObject _prefabInstance;

    public Building ParentObject { get; set; }


    void Start()
    {
        BuildingLabel.text = ParentObject.GetComponent<BuildingInfo>().BuildingName;
    }

    public void AddTab(ControlType cType)
    {
        ControlComponent controlComp = null;
        switch (cType)
        {
            case ControlType.BasicInfo:
                controlComp = Instantiate(BasicInfoComponent) as ControlComponent;
                break;
            case ControlType.Resource:
                controlComp = Instantiate(ResourceComponent) as ControlComponent;
                break;
            case ControlType.House:
                controlComp = Instantiate(HouseComponent) as ControlComponent;
                break;
            case ControlType.JobBuilding:
                controlComp = Instantiate(JobBuildingComponent) as ControlComponent;
                break;
            case ControlType.Blueprint:
                controlComp = Instantiate(ProgressComponent) as ControlComponent;
                break;
        }

        controlComp.BuildingControl = this;
        controlComp.transform.parent = ContentArea.transform;
        controlComp.transform.localScale = new Vector3(1, 1, 1);
        controlComp.transform.localPosition = Vector3.zero;

        // Create Tab
        Transform tab = Instantiate(TabPrefab) as Transform;
        tab.Find("TabText").GetComponent<UILabel>().text = controlComp.tabName;
        tab.parent = TabLine.transform;
        tab.localScale = new Vector3(1, 1, 1);
        tab.localPosition = Vector3.zero;
        TabLine.Reposition();
        _tabs.Add(tab);
        tab.GetComponent<UIToggledObjects>().activate.Add(controlComp.gameObject);

        if (_tabs.Count == 0)
            tab.GetComponent<UIToggle>().value = true;


    }

    public void CloseInstance()
    {
        BuildControlsGUIManager.Instance.CurrentControlBox = null;
    }

    // TODO find out the cause of the panel not being updates properly and substitute for the proper change
    /// <summary>
    /// Dirty Workaround
    /// </summary>
    void LateUpdate()
    {
        panel.Invalidate(true);
    }

}

public enum ControlType
{
    BasicInfo,
    Resource,
    House,
    JobBuilding,
    Blueprint
}