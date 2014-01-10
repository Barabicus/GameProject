using UnityEngine;
using System.Collections;

public abstract class GameEntity : MonoBehaviour
{

    #region Fields
    #endregion

    #region Properties

    #endregion

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {

    }

    #region Triggers

    protected virtual void OnTriggerLOSEnter(TriggerData data)
    {
    }

    protected virtual void OnTriggerLOSExit(TriggerData data)
    {
    }

    protected virtual void OnChaseLOSEnter(TriggerData data)
    {
    }

    protected virtual void OnChaseLOSExit(TriggerData data)
    {
    }

    #endregion

}
