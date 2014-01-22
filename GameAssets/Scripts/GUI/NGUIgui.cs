using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class NGUIgui : MonoBehaviour
{

    #region Fields
    public UIGrid buildingGrid;
    public Transform buildingGridPrefab;
    private StateController _stateCont;
    private TypeState _typeState = TypeState.Mob;
    #endregion

    #region Properties

    #endregion

    #region Enums

    enum TypeState
    {
        Mob,
        Building
    }

    #endregion

    #region InitLogic

    void Start()
    {
        _stateCont = StateController.Instance;
        // Set the select controller to selection initially
        _stateCont.ControllerState = ControllerState.Selection;
        for (int i = 0; i < BuildingList.Instance.buildings.Length; i++)
        {
            Transform t = Instantiate(buildingGridPrefab) as Transform;
            t.parent = buildingGrid.transform;
            t.localScale = new Vector3(1, 1, 1);
            t.localPosition = Vector3.zero;
            t.GetComponent<UILabel>().text = BuildingList.Instance.buildings[i].GetComponent<BuildingInfo>().BuildingName;

            Building _prefabRef = BuildingList.Instance.buildings[i];
            Transform _previewRef = BuildingList.Instance.BuildingMeshPreviewPrefabs[i];
            EventDelegate.Add(t.GetComponent<UIButton>().onClick, () =>
            {
                _stateCont.ControllerState = ControllerState.Building;
                // Set the building prefab
                _stateCont.GetController<BuildingPlaceController>().BuildingPrefab = _prefabRef.transform;
                // Set the building display
                _stateCont.GetController<BuildingPlaceController>().BuildingPreview = _previewRef;

            });


        }


        buildingGrid.Reposition();

    }

    #endregion

    #region EventCallBacks


    #region SelectionNav

    #endregion

    #endregion

    #region DebugLogic

    void OnGUI()
    {

    }

    #endregion

    #region Unity Method Calls
    public void Update()
    {
        if (_typeState == TypeState.Building)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _stateCont.ControllerState = ControllerState.Selection;
            }
        }
    }
    #endregion
}