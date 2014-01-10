using UnityEngine;
using System.Collections;

/// <summary>
/// Controllers for handling how the user interacts with the game.
/// These controllers may be active only at a single time and not constantly.
/// I.E these typically don't affect gameplay in the background.
/// </summary>
public abstract class Controller : MonoBehaviour {

    public virtual void Awake()
    {
        // Add this controller to the stateControllers controller list. Uses the class's type hashCode as key.
        // The controller can then be accessed by doing typeof(CLASS).getHashCode()
        StateController.Instance.controllers.Add(GetType().GetHashCode(), this);

        //Ensure the controller is disabled so everything is in the correct state
        enabled = false;
    }

    public virtual void Start() { }

}
