using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectController : Controller
{

    #region Fields
    public Texture2D selectTexture;
    public List<ActiveEntity> selectedUnits = new List<ActiveEntity>();
    public float selectDistance = 50;
    public delegate void SelectListDelegate(List<ActiveEntity> list);
    public event SelectListDelegate SelectedListChanged;

    private Vector3 _clickPos = Vector3.zero;
    private Vector3 _dragSize = Vector3.zero;
    private SelectState _selectState = SelectState.Off;
    private DrawState _drawState = DrawState.Off;
    private Texture2D _outlineTexture;
    /// <summary>
    /// The faction flags that the player can select
    /// </summary>
    private FactionFlags _playerSelectionFlags = FactionFlags.one;

    public RTSCamera rtsCamera;

    public Transform pref;


    #endregion

    #region Properties
    public SelectState ControllerSelectType
    {
        get { return _selectState; }
        set { _selectState = value; }
    }
    public FactionFlags PlayerSelectionFlags
    {
        get { return _playerSelectionFlags; }
        set { _playerSelectionFlags = value; }
    }
    #endregion

    #region States

    public enum SelectState
    {
        Off,
        Mob,
        Building
    }

    enum DrawState
    {
        Off,
        Drawing
    }

    #endregion

    #region Logic

    public override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Deselect();
        }
        if (UICamera.hoveredObject)
            return;
        #region Selection
        if (_selectState == SelectState.Building || _selectState == SelectState.Mob)
        {
            #region Move
            if (_selectState == SelectState.Mob && Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
                {
                    if (_selectState == SelectState.Mob)
                    {
                        foreach (ActiveEntity t in selectedUnits)
                        {
                            t.PerformAction(new PerformActionEvent(hit.collider.GetComponent<ActiveEntity>() != null ? hit.collider.GetComponent<ActiveEntity>() : null, hit.collider.tag, new Vector3[]{hit.point}));
                        }
                    }

                }
            }
            #endregion
            if (Input.GetMouseButtonDown(0))
            {
                // If the mouse is not over a UI object
                if (!UICamera.hoveredObject)
                {
                    _clickPos = Input.mousePosition;
                    _drawState = DrawState.Drawing;
                }
            }
            // Being the selection size position away from the click position while the mouse is down
            if (Input.GetMouseButton(0))
            {
                _dragSize = new Vector3(Input.mousePosition.x - _clickPos.x, _clickPos.y - Input.mousePosition.y, 0);

            }
            // When the user lets go, select all applicable entities
            if (Input.GetMouseButtonUp(0))
            {
                Deselect();
                // If the user just clicked
                if (_dragSize == Vector3.zero)
                {
                    // Ray cast the target rather than selecting all within the bounds
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << 11))
                    {
                        // All GameObjects with layer type object should be of type "ActiveEntity"
                        ActiveEntity ae = hit.collider.GetComponent<ActiveEntity>();
                        if ((ae.FactionFlags & _playerSelectionFlags) == _playerSelectionFlags)
                        {
                            if ((_selectState == SelectState.Mob && ae.tag.Equals("Mob")) || (_selectState == SelectState.Building && ae.tag.Equals("Building")))
                            {
                                selectedUnits.Add(ae);
                                ae.isSelected = true;
                            }
                        }
                    }
                }
                else
                {
                    // If the user held, select multiple
                    // Iterate through the list of selectable units and compare them to the rectangle on screen
                    // If their screen position matches up with the rectangle, add them to the selected units list.
                    Rect r = CoordHelper.NonNegativeRect(CoordHelper.ScreenRect(_clickPos.x, _clickPos.y, _dragSize.x, _dragSize.y));
                    foreach (ActiveEntity i in SelectableList.SelectedList)
                    {
                        if (r.Contains(CoordHelper.RectCoords(Camera.main.WorldToScreenPoint(i.gameObject.transform.position))))
                        {
                            if ((_selectState == SelectState.Mob && i.tag.Equals("Mob")) || (_selectState == SelectState.Building && i.tag.Equals("Building")))
                            {
                                selectedUnits.Add(i.gameObject.GetComponent<ActiveEntity>());
                                i.gameObject.GetComponent<ActiveEntity>().isSelected = true;
                            }
                        }
                    }
                }
                _drawState = DrawState.Off;
                // Trigger a list changed event
                if (SelectedListChanged != null)
                    SelectedListChanged(selectedUnits);
            }
        }
        #endregion
    }

    void OnDisable()
    {
      //  Deselect();
    }

    void OnGUI()
    {
        if (_drawState == DrawState.Drawing)
        {
            if (Input.GetMouseButton(0))
            {
                GUI.DrawTexture(CoordHelper.NonNegativeRect(CoordHelper.ScreenRect(_clickPos.x, _clickPos.y, _dragSize.x, _dragSize.y)), selectTexture);
            }
        }
    }

    public void Deselect()
    {
        foreach (ActiveEntity t in selectedUnits)
        {
            t.isSelected = false;
        }
        selectedUnits.Clear();
        if (SelectedListChanged != null)
            SelectedListChanged(selectedUnits);
    }


    #endregion
}
