using UnityEngine;
using System.Collections;

/// <summary>
/// Controller that controls the placing of buildings. 
/// </summary>
public class BuildingPlaceController : Controller
{

    #region Fields

    public int placeDistance = 500;
    public float rotateSpeed = 5f;

    /// <summary>
    /// The transform for the preview building.
    /// </summary>
    private Transform _selectedBlueprint;
    private Transform _buildingPreview;
    private Vector3 _placePosition = Vector3.zero;
    private PlaceType _placeType = PlaceType.Single;

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
            _buildingPreview.gameObject.SetActive(false);
            _buildingPreview.transform.rotation = Quaternion.identity;
            _buildingPreview.position = Vector3.zero;
            _buildingPreview = value;
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
	
	void Update () {
            // If the mouse is over an UI element, don't show the place transform
            if (UICamera.hoveredObject)
            {
                BuildingPreview.gameObject.SetActive(false);
            }
            else
            {
                RaycastHit hit;
                // If the mouse if over the terrain
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, placeDistance, 1 << 9))
                {

                    BuildingPreview.gameObject.SetActive(true);
                    BuildingPreview.position = hit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        _placePosition = hit.point;
                        switch (_placeType)
                        {
                            case PlaceType.Single:
                               ((Transform)Instantiate(Blueprint, _placePosition, Blueprint.rotation)).gameObject.SetActive(true);
                                break;
                            case PlaceType.Drag:
                                // Instantiate at inital position
                                Transform t  = Instantiate(Blueprint, _placePosition, _selectedBlueprint.rotation) as Transform;
                        //        StartCoroutine(DragPlace(t));
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
    }

    public IEnumerator DragPlace(Transform t)
    {
        Vector2 placePos = Camera.main.WorldToScreenPoint(_placePosition);
        BoxCollider c = t.GetComponent<BoxCollider>();
        float placeDistance = c.size.x * 2;
        while (Input.GetMouseButton(0))
        {
            if (Mathf.Abs(placePos.x - Input.mousePosition.x) > placeDistance)
            {
                //Instantiate(BuildingPreview, newPos, _placeDisplay.rotation);
                //yield break;
            }
            yield return 0;
        }
    }

    #endregion

}
