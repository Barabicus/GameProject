using UnityEngine;
using System.Collections;

public class ControlComponent : MonoBehaviour {

    public string tabName;

    public BuildingControl BuildingControl { get; set; }
    public Building ParentObject { get { return BuildingControl.ParentObject; } }

    protected void CheckForNull(Object o)
    {
        if (o == null)
        {
            Debug.LogError("Could not get component is this Control Component attached to the proper building?");
            Destroy(this.gameObject);
        }
    }



}
