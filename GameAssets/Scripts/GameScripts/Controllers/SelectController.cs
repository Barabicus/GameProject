using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

public class SelectController : Controller
{

    #region Fields
    public delegate void SelectListDelegate(List<ActiveEntity> list);
    public event SelectListDelegate SelectedListChanged;

    private ISelectable _selected;

    public RTSCamera rtsCamera;
    #endregion

    #region Properties
    [DefaultValue(FactionFlags.one)]
    public FactionFlags PlayerSelectionFlags
    {
        get;
        set;
    }
    #endregion

    #region States


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


                    // Ray cast the target rather than selecting all within the bounds
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, 1 << 11))
                    {
                        if (hit.collider.GetComponent<ActiveEntity>() is ISelectable && hit.collider.GetComponent<ActiveEntity>() is IFactionFlag)
                        {
                            ISelectable selectable = hit.collider.GetComponent<ActiveEntity>() as ISelectable;
                            IFactionFlag flags = hit.collider.GetComponent<ActiveEntity>() as IFactionFlag;
                            if ((ae.FactionFlags & _playerSelectionFlags) == _playerSelectionFlags)
                            {
                                selectedUnits.Add(ae);
                                ae.IsSelected = true;
                            }
                        }
                    }
            
        
        #endregion
    }

    void OnDisable()
    {
      //  Deselect();
    }


    public void Deselect()
    {
    }


    #endregion
}
