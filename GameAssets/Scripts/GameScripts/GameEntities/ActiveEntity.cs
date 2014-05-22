using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;

public abstract class ActiveEntity : GameEntity
{

    #region Fields
    private HighlightableObject _ho;

    #endregion

    #region Properties

    public HighlightableObject ObjectHighlight
    {
        get { return _ho; }
    }
    [DefaultValue(true)]
    public bool IsHighlightable
    {
        get;
        set;
    }

    #endregion

    public virtual void Start()
    {
        IsHighlightable = true;
        if (this is IFactionFlag)
        {
            IFactionFlag flag = this as IFactionFlag;
            flag.FactionFlags = FactionFlags.one;
        }
    }

    public virtual void Awake()
    {
        gameObject.AddComponent<HighlightableObject>();
        _ho = gameObject.GetComponent<HighlightableObject>();
    }


    public void OnMouseEnter()
    {
        if (IsHighlightable)
            _ho.ConstantOn(Color.red);
    }

    public void OnMouseExit()
    {
        _ho.ConstantOff();
    }

    /// <summary>
    /// When called this method, using the actionEvent parameter will cause the Entity
    /// to trigger a response from the variables passed in. For example passing in an entity here
    /// with flags that are considered to be enemy flags from this entity, might cause this entity
    /// to move to and attack the entity associated with the actionEvent. This method is how ActiveEntities
    /// communicate with one another. Rather than having logic and references all stored and cached within multiple
    /// different entities this is used instead. PerformActionEvent being a struct also means it will be removed from 
    /// the stack when exiting the scope of the method call which has the added benefit of saving the initilization of
    /// otherwise useless objects on the heap.
    /// </summary>
    public virtual void PerformAction(PerformActionVariables actionVariables)
    {
    }


}


public struct PerformActionVariables
{
    public ActiveEntity entity;
    public string tag;

    public float[] floatArgs;
    public int[] intArgs;
    public string[] stringArgs;
    public Vector3[] vector3Args;
    public ResourceType[] resourceTypesArgs;

    public PerformActionVariables(ActiveEntity entity)
    {
        this.entity = entity;
        this.tag = entity.tag;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
        this.resourceTypesArgs = new ResourceType[0];
    }

    public PerformActionVariables(ActiveEntity entity, ResourceType resourceTypeArgs)
    {
        this.entity = entity;
        this.tag = entity.tag;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
        this.resourceTypesArgs = new ResourceType[] { resourceTypeArgs };
    }
    public PerformActionVariables(ActiveEntity entity, ResourceType resourceTypeArgs, int intArgs)
    {
        this.entity = entity;
        this.tag = entity.tag;
        this.floatArgs = new float[0];
        this.intArgs = new int[] { intArgs };
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
        this.resourceTypesArgs = new ResourceType[] { resourceTypeArgs };
    }

    public PerformActionVariables(ActiveEntity entity, ResourceType[] resourceTypeArgs)
    {
        this.entity = entity;
        this.tag = entity.tag;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
        this.resourceTypesArgs = resourceTypeArgs;
    }

    public PerformActionVariables(ActiveEntity entity, ResourceType[] resourceTypeArgs, int[] intArgs)
    {
        this.entity = entity;
        this.tag = entity.tag;
        this.floatArgs = new float[0];
        this.intArgs = intArgs;
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
        this.resourceTypesArgs = resourceTypeArgs;
    }

    public PerformActionVariables(ActiveEntity entity, string tag)
    {
        this.entity = entity;
        this.tag = tag;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
        this.resourceTypesArgs = new ResourceType[0];
    }

    public PerformActionVariables(ActiveEntity entity, string tag, float[] floatArgs, int[] intArgs, string[] stringArgs, Vector3[] vector3Args)
    {
        this.entity = entity;
        this.tag = tag;
        this.floatArgs = floatArgs;
        this.intArgs = intArgs;
        this.stringArgs = stringArgs;
        this.vector3Args = vector3Args;
        this.resourceTypesArgs = new ResourceType[0];
    }

    public PerformActionVariables(ActiveEntity entity, string tag, Vector3[] vector3Args)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = vector3Args;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.resourceTypesArgs = new ResourceType[0];
    }

    public PerformActionVariables(ActiveEntity entity, string tag, int[] intArgs)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = new Vector3[0];
        this.floatArgs = new float[0];
        this.intArgs = intArgs;
        this.stringArgs = new string[0];
        this.resourceTypesArgs = new ResourceType[0];
    }

    public PerformActionVariables(ActiveEntity entity, string tag, float[] floatArgs)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = new Vector3[0];
        this.floatArgs = floatArgs;
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.resourceTypesArgs = new ResourceType[0];
    }

    public PerformActionVariables(ActiveEntity entity, string tag, string[] stringArgs)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = new Vector3[0];
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = stringArgs;
        this.resourceTypesArgs = new ResourceType[0];
    }

}
