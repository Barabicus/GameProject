using UnityEngine;
using System.Collections;

public class ControlComponent : MonoBehaviour {

    public string tabName;

    public BuildingControl BuildingControl { get; set; }
    public Building ParentObject { get { return BuildingControl.ParentObject; } }

    protected T GetParentObjectComponent<T>() where T : Component
    {
        T comp = ParentObject.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogError("Could not get component is this Control Component attached to the proper building?");
            Destroy(this.gameObject);
            return null;
        }
        return comp;
    }



}
