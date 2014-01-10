using UnityEngine;
using System.Collections;
using System;

public class DebugGUI : MonoBehaviour
{

    #region Fields
    private StateController stateCont;
    private bool buildPlace = false;
    private bool drawGameObjectScreenPos = false;
    private TypeState _typeState = TypeState.Mob;

    public Texture2D highLightTexture;

    #endregion

    #region Enums

    enum TypeState
    {
        Mob,
        Building
    }

    #endregion

    void Start()
    {
        stateCont = StateController.Instance;
        stateCont.GetController<SelectController>().ControllerSelectType = SelectController.SelectState.Mob;
    }

    void OnGUI()
    {
        int yPos = 10;
        int yInc = 21;
        // Make a background box
        GUI.Box(new Rect(10, 10, 180, 125), "Selection Menu");

        GUI.Label(new Rect(20, yPos += yInc, 120, 20), "State: " + _typeState);
        GUI.Label(new Rect(20, yPos += yInc, 120, 20), "ContState: " + stateCont.ControllerState);

        if (GUI.Button(new Rect(20, yPos += yInc, 80, 20), "Mob"))
        {
            _typeState = TypeState.Mob;
            stateCont.GetController<SelectController>().ControllerSelectType = SelectController.SelectState.Mob;
            stateCont.ControllerState = ControllerState.None;
        }

        if (GUI.Button(new Rect(20, yPos += yInc, 80, 20), "Building"))
        {
            _typeState = TypeState.Building;
            stateCont.GetController<SelectController>().ControllerSelectType = SelectController.SelectState.Building;
            stateCont.ControllerState = ControllerState.None;
        }

        drawGameObjectScreenPos = GUI.Toggle(new Rect(20, yPos += yInc, 150, 20), drawGameObjectScreenPos, "Draw Screen Pos");

        if (drawGameObjectScreenPos)
        {
            foreach (GameEntity i in SelectableList.SelectedList)
            {
                GUI.DrawTexture(CoordHelper.TransformToScreenRect(i.gameObject, 25, 25), highLightTexture);
            }
        }


        if (_typeState == TypeState.Building)
        {

            // BUILDING
            GUI.BeginGroup(new Rect(Screen.width - 150, 10, 125, (BuildingList.Instance.buildings.Length * 50) + 10));

            GUI.Box(new Rect(0, 0, 125, (BuildingList.Instance.buildings.Length * 50) + 10), "Building");

            int ypos = 10;
            yInc = 22;
            foreach (Building b in BuildingList.Instance.buildings)
            {
                if (GUI.Button(new Rect(5, ypos += yInc, 115, 20), b.GetComponent<BuildingInfo>().BuildingName))
                {
                    stateCont.GetController<BuildingPlaceController>().BuildingPrefab = b.transform;
                }
            }

            GUI.EndGroup();

            GUI.BeginGroup(new Rect(250, 10, 200, 55));

            int xCPos = 5;
            int xCPosIncr = 50;

            GUI.Box(new Rect(0, 0, 200, 55), "Building Commands");
            if (GUI.Button(new Rect(xCPos, 20, 50, 25), "Select"))
            {
                stateCont.ControllerState = ControllerState.Selection;
            }
            if (GUI.Button(new Rect(xCPos += xCPosIncr, 20, 50, 25), "Place"))
            {
                stateCont.ControllerState = ControllerState.Building;
            }

            GUI.EndGroup();


        }
        else if (_typeState == TypeState.Mob)
        {

            GUI.BeginGroup(new Rect(250, 10, 200, 55));

            int xCPos = 5;
            int xCPosIncr = 50;

            GUI.Box(new Rect(0, 0, 200, 55), "Unit Commands");
            if (GUI.Button(new Rect(xCPos, 20, 50, 25), "Select"))
            {
                stateCont.ControllerState = ControllerState.Selection;
            }
            if (GUI.Button(new Rect(xCPos += xCPosIncr, 20, 50, 25), "Add Wood"))
            {
            }

            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.U)
            {
            /*    foreach (Transform t in stateCont.GetController<SelectController>().selectedUnits)
                {
                    t.GetComponent<Resource>().AddResource(ResourceType.Wood, 2);
                }
             * */
            }

            GUI.EndGroup();

            GUI.BeginGroup(new Rect(Screen.width - 130, 10, 120, 125));

            GUI.Box(new Rect(0, 0, 120, 125), "Unit Select");
            GUI.Label(new Rect(5, 20, 120, 20), "Rect: " +  stateCont.GetController<SelectController>().selectedUnits.Count);

            GUI.EndGroup();
        }


    }




}
