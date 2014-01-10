using UnityEngine;
using System.Collections;

/// <summary>
/// Sends Message "TriggerLOSEnter" and "TriggerLOSExit" to the top level gameobject 
/// </summary>
public class TriggerLOS : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (TriggerType.isValid(other.gameObject.tag) && other.transform != transform.parent.parent)
            SendMessageUpwards("OnTriggerLOSEnter", new TriggerData(other, other.gameObject.tag.GetHashCode()));
    }

    void OnTriggerExit(Collider other)
    {
        if (TriggerType.isValid(other.gameObject.tag) && other.transform != transform.parent.parent)
            SendMessageUpwards("OnTriggerLOSExit", new TriggerData(other, other.gameObject.tag.GetHashCode()));
    }

}
