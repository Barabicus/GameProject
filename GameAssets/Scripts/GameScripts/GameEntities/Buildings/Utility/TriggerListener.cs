using UnityEngine;
using System.Collections;

public class TriggerListener : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        StateController.Instance.GetController<BuildingPlaceController>().OtherTriggerEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        StateController.Instance.GetController<BuildingPlaceController>().OtherTriggerExit(other);
    }
}
