using UnityEngine;
using System.Collections;

/// <summary>
/// Controller that controls the placing of buildings. 
/// </summary>
public class BuildingPlaceController : Controller
{

    #region Fields

    public Transform placePrefab;
    public int placeDistance = 500;
    public float rotateSpeed = 5f;

    /// <summary>
    /// The transform for the preview building.
    /// </summary>
    private Transform _placeDisplay;

    /// <summary>
    /// The prefab of the building we are going to create.
    /// </summary>
    private Transform _buildingPlacePrefab;

    private Vector3 _placePosition = Vector3.zero;
    private PlaceState _placeState = PlaceState.Preview;
    private PlaceType _placeType = PlaceType.Single;

    #endregion

    #region Properties

    public PlaceState BuildingPlaceState
    {
        get { return _placeState; }
        set
        {
            _placeState = value;
        }
    }

    /// <summary>
    /// The transform that we will use to instantiate new instances of this building
    /// A gameobject with a building component should only be passed in here.
    /// </summary>
    public Transform BuildingPrefab
    {
        get { return _buildingPlacePrefab; }
        set
        {
            _buildingPlacePrefab = value;
            _placeType = value.GetComponent<BuildingInfo>().placeType;
        }
    }

    /// <summary>
    /// The current building preview that will be displayed when placing a building
    /// </summary>
    public Transform BuildingPreview
    {
        get { return _placeDisplay; }
        set
        {
            _placeDisplay.gameObject.SetActive(false);
            _placeDisplay.transform.rotation = Quaternion.identity;
            _placeDisplay.position = Vector3.zero;
            _placeDisplay = value;
        }
    }

    #endregion

    #region State
    public enum PlaceState
    {
        Preview,
        Placing
    }

    public enum PlaceType
    {
        Single,
        Drag
    }
    #endregion


    #region Logic

    public override void Awake () {
        base.Awake();
        _placeDisplay = BuildingList.Instance.BuildingMeshPreviewPrefabs[0];
	}

    public override void Start()
    {
        base.Start();
        // Set the building prefab to the first in the building list
     //   _buildingPlacePrefab = BuildingList.Instance.buildings[0].transform;

    } 

    public void OnDisable()
    {
        _placeDisplay.gameObject.SetActive(false);
    }
	
	void Update () {
        if (_placeState == PlaceState.Preview)
        {

            // If the mouse is over an UI element, don't show the place transform
            if (UICamera.hoveredObject)
            {
                _placeDisplay.gameObject.SetActive(false);
            }
            else
            {
                RaycastHit hit;
                // If the mouse if over the terrain
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, placeDistance, 1 << 9))
                {

                    _placeDisplay.gameObject.SetActive(true);
                    _placeDisplay.position = hit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        _placePosition = hit.point;
                        switch (_placeType)
                        {
                            case PlaceType.Single:
                                Instantiate(BuildingPreview, _placePosition, _placeDisplay.rotation);
                                break;
                            case PlaceType.Drag:
                                // Instantiate at inital position
                                Transform t  = Instantiate(BuildingPreview, _placePosition, _placeDisplay.rotation) as Transform;
                        //        StartCoroutine(DragPlace(t));
                                break;
                        }
                    }
                }
                else
                {
                    // If not don't display the preview
                    _placeDisplay.gameObject.SetActive(false);
                }
            }
            
        }
        else if (_placeState == PlaceState.Placing)
        {
            if (!UICamera.hoveredObject)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    _placeState = PlaceState.Preview;
                    return;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    // Instantiate the building prefab and attatch the building constructor script
                    Instantiate(BuildingPreview, _placePosition, _placeDisplay.rotation);                   
                    _placeState = PlaceState.Preview;
                    return;
                }
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
