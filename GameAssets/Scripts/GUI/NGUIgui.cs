using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class NGUIgui : MonoBehaviour
{

    #region Fields
    private StateController _stateCont;
    #endregion

    #region Properties

    #endregion

    #region InitLogic

    void Start()
    {
        try
        {
            _stateCont = StateController.Instance;
            // Set the select controller to selection initially
            _stateCont.ControllerState = ControllerState.Selection;
        }
        catch (Exception e)
        {
            Debug.LogError("Exception");
        }
    }

    #endregion


    #region Unity Method Calls
    public void Update()
    {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _stateCont.ControllerState = ControllerState.Selection;
            }   
    }
    #endregion
}