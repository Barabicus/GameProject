using UnityEngine;
using System.Collections;

/// <summary>
/// Sends Message "ChaseLOSEnter" and "ChaseLOSExit" to the top level gameobject 
/// </summary>
public class ChaseLOS : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (TriggerType.isValid(other.gameObject.tag) && other.transform != transform.parent.parent)
            SendMessageUpwards("OnChaseLOSEnter", new TriggerData(other, other.gameObject.tag.GetHashCode()));
    }

    void OnTriggerExit(Collider other)
    {
        if (TriggerType.isValid(other.gameObject.tag) && other.transform != transform.parent.parent)
            SendMessageUpwards("OnChaseLOSExit", new TriggerData(other, other.gameObject.tag.GetHashCode()));
    }
}
