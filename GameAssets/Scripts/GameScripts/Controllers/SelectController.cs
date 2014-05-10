using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class SelectController : Controller
{

    #region Fields
    private ISelectable _selected;
    #endregion

    #region Properties
    [DefaultValue(FactionFlags.one)]
    public FactionFlags PlayerSelectionFlags
    {
        get;
        set;
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
        #region Selection

        if (Input.GetMouseButtonDown(0))
        {
            if (UICamera.hoveredObject)
                return;

            // Ray cast the target rather than selecting all within the bounds
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << 11))
            {
                if (hit.collider.GetComponent<ActiveEntity>() is ISelectable && hit.collider.GetComponent<ActiveEntity>() is IFactionFlag)
                {
                    ISelectable selectable = hit.collider.GetComponent<ActiveEntity>() as ISelectable;
                    IFactionFlag flags = hit.collider.GetComponent<ActiveEntity>() as IFactionFlag;
                    if ((flags.FactionFlags & PlayerSelectionFlags) == PlayerSelectionFlags)
                    {
                        _selected = selectable;
                        _selected.IsSelected = true;
                    }
                }
            }
        }


        #endregion
    }

    void OnDisable()
    {
        Deselect();
    }


    public void Deselect()
    {
        if (_selected != null)
            _selected.IsSelected = false;
    }


    #endregion
}
