using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateController : MonoBehaviour
{
    #region Fields
    private ControllerState _contState = ControllerState.None;
    private static StateController _instance;

    public Dictionary<int, Controller> controllers = new Dictionary<int, Controller>();

    public delegate void StateChangedDelegate(ControllerState newState);

    public event StateChangedDelegate StateChanged;
    #endregion

    #region Properties
    public static StateController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject g = new GameObject("_StateController");
                g.AddComponent<StateController>();
            }
            return _instance;
        }
    }

    public ControllerState ControllerState
    {
        get
        {
            return _contState;
        }
        set
        {
            // Disable all the controllers first
            DisableControllers();

            // Enable the appropriate controller
            switch (value)
            {
                case ControllerState.Building:
                    controllers[typeof(BuildingPlaceController).GetHashCode()].enabled = true;
                    break;
                case ControllerState.Selection:
                    controllers[typeof(SelectController).GetHashCode()].enabled = false;
                    break;
            }
            // Set the state
            _contState = value;
            
            // Fire State Changed event
            if (StateChanged != null)
                StateChanged(_contState);

        }
    }

    #endregion

    #region Logic

    void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Trying to create multiple StateControllers!");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void DisableControllers()
    {
        foreach (Controller g in controllers.Values)
        {
            g.enabled = false;
        }
    }

    public T GetController<T>() where T : Controller
    {
        return (T)controllers[typeof(T).GetHashCode()];
    }

    #endregion

}

#region States
public enum ControllerState
{
    None,
    Building,
    Selection,
}
#endregion
