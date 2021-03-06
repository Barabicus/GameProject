﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class ActiveEntity : GameEntity, IDamageable, IFactionFlag
{

    #region Fields

    protected List<ActiveEntity> enemies = new List<ActiveEntity>();
    private bool _isSelected;
    public  string EntityID = "MISSINGNO";


    #endregion

    #region Properties
    public abstract FactionFlags FactionFlags { get; set; }
    public abstract FactionFlags EnemyFlags { get; set; }
    /// <summary>
    /// Returns true if the object has been selected by the player. This is controlled via the SelectController.
    /// </summary>
    public virtual bool isSelected
    {
        get { return _isSelected; }
        set { _isSelected = value; }
    }
    #endregion

    protected virtual void Start()
    {
    }

    public abstract bool Damage(int damage);


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
    public virtual void PerformAction(PerformActionEvent actionEvent) { }


}


public struct PerformActionEvent
{
    public ActiveEntity entity;
    public string tag;

    public float[] floatArgs;
    public int[] intArgs;
    public string[] stringArgs;
    public Vector3[] vector3Args;

    public PerformActionEvent(ActiveEntity entity, string tag)
    {
        this.entity = entity;
        this.tag = tag;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
        this.vector3Args = new Vector3[0];
    }

    public PerformActionEvent(ActiveEntity entity, string tag, float[] floatArgs, int[] intArgs, string[] stringArgs, Vector3[] vector3Args)
    {
        this.entity = entity;
        this.tag = tag;
        this.floatArgs = floatArgs;
        this.intArgs = intArgs;
        this.stringArgs = stringArgs;
        this.vector3Args = vector3Args;
    }

    public PerformActionEvent(ActiveEntity entity, string tag, Vector3[] vector3Args)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = vector3Args;
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
    }

    public PerformActionEvent(ActiveEntity entity, string tag, int[] intArgs)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = new Vector3[0];
        this.floatArgs = new float[0];
        this.intArgs = intArgs;
        this.stringArgs = new string[0];
    }

    public PerformActionEvent(ActiveEntity entity, string tag, float[] floatArgs)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = new Vector3[0];
        this.floatArgs = floatArgs;
        this.intArgs = new int[0];
        this.stringArgs = new string[0];
    }

    public PerformActionEvent(ActiveEntity entity, string tag, string[] stringArgs)
    {
        this.entity = entity;
        this.tag = tag;
        this.vector3Args = new Vector3[0];
        this.floatArgs = new float[0];
        this.intArgs = new int[0];
        this.stringArgs = stringArgs;
    }

}
