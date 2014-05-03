using UnityEngine;
using System.Collections;

public class TriggerListener : MonoBehaviour {

    IEnumerator OnTriggerStay(Collider other)
    {
        StateController.Instance.GetController<BuildingPlaceController>().TriggerStay(other);
        yield return new WaitForEndOfFrame();
    }
}
