﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Controller that controls the placing of buildings. 
/// </summary>
public class BuildingPlaceController : Controller
{

    #region Fields

    public int placeDistance = 500;
    public Vector3 gridSize;
    public Color canPlaceColor;
    public Color cantPlaceColor;

    /// <summary>
    /// The transform for the preview building.
    /// </summary>
    private Transform _selectedBlueprint;
    private Transform _buildingPreview;
    private Vector3 _placePosition = Vector3.zero;
    private PlaceType _placeType = PlaceType.Single;
    private bool _canPlace = false;
    /// <summary>
    /// Reference to the material color changer that will be used to change the color of all objects 
    /// associated with the preview building.
    /// </summary>
    private MaterialColorChanger _matColorChanger;

    #endregion

    #region Properties

    /// <summary>
    /// The current building preview that will be displayed when placing a building
    /// </summary>
    public Transform Blueprint
    {
        get { return _selectedBlueprint; }
        set
        {
            _selectedBlueprint = value;
        }
    }

    public Transform BuildingPreview
    {
        get { return _buildingPreview; }
        set
        {
            _canPlace = false;
            _buildingPreview.gameObject.SetActive(false);
            _buildingPreview.transform.rotation = Quaternion.identity;
            _buildingPreview.position = Vector3.zero;
            _buildingPreview = value;
            _matColorChanger = value.GetComponent<MaterialColorChanger>();
            _matColorChanger.ChangeColor(canPlaceColor);
        }
    }

    #endregion

    #region State

    public enum PlaceType
    {
        Single,
        Drag
    }
    #endregion


    #region Logic

    public override void Awake () {
        base.Awake();
        _selectedBlueprint = BuildingList.Instance.BuildingBlueprintPrefabs[0];
        _buildingPreview = BuildingList.Instance.BuildingPreviewPrefabs[0];        
	}

    public void OnDisable()
    {
        BuildingPreview.gameObject.SetActive(false);
    }


    public void TriggerStay(Collider other)
    {
        if (!other.tag.Equals("Ground"))
            _canPlace = false;
    }
	
	void Update () {
            // If the mouse is over an UI element, don't show the place transform
            if (UICamera.hoveredObject)
            {
                BuildingPreview.gameObject.SetActive(false);
            }
            else
            {
                // Check the can place Status and change the material colour accordingly
                if (_canPlace)
                    _matColorChanger.ChangeColor(canPlaceColor);
                else
                    _matColorChanger.ChangeColor(cantPlaceColor);
                RaycastHit hit;
                // If the mouse is over the terrain
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, placeDistance, 1 << 9) && hit.collider.tag.Equals("Ground"))
                {
                    BuildingPreview.gameObject.SetActive(true);
                    BuildingPreview.position = new Vector3(Mathf.Round(hit.point.x / gridSize.x) * gridSize.x,
hit.point.y,
Mathf.Round(hit.point.z / gridSize.z) * gridSize.z);

                    if (Input.GetMouseButtonDown(1))
                    {
                        BuildingPreview.Rotate(Vector3.up, 90f);
                    }
                    if (Input.GetMouseButtonDown(0) && _canPlace)
                    {
                        _placePosition = new Vector3(Mathf.Round(hit.point.x / gridSize.x) * gridSize.x,
hit.point.y,
Mathf.Round(hit.point.z / gridSize.z) * gridSize.z);
                        switch (_placeType)
                        {
                            case PlaceType.Single:
                                Transform trans = ((Transform)Instantiate(Blueprint, _placePosition, BuildingPreview.rotation));
                                trans.GetComponent<BuildingInfo>().factionFlags = FactionFlags.one;
                                trans.gameObject.SetActive(true);
                                trans.parent = hit.collider.transform.parent;                                
                                break;
                        }
                    }
                }
                else
                {
                    // If not don't display the preview
                    BuildingPreview.gameObject.SetActive(false);
                }
            }
        // Once everything has been moved we set the placeable trigger to true
        // A OnTriggerStay will be called on the associated BuildingPreview.
        // If a trigger exists then this will be false in the next frame
        // and so on until no triggers exist.
            _canPlace = true;
    }



    #endregion

}
