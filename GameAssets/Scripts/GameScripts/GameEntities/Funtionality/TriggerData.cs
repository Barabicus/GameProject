using UnityEngine;
using System.Collections;

public struct TriggerData {

    public ActiveEntity entity;
    public int triggerType;

    public TriggerData(Collider other, int triggerType)
    {
        this.entity = other.GetComponent<ActiveEntity>();
        this.triggerType = triggerType;
    }

}
