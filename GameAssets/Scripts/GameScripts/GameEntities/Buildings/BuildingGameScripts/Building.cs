using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ShowRim))]
[RequireComponent(typeof(BuildingInfo))]
[RequireComponent(typeof(DynamicGridObstacle))]
public class Building : ActiveEntity
{

    #region Fields
    public int maxHP;

    public bool isInvincible = false;

    protected int currentHP;
    public BuildState buildState = BuildState.Constructed;

    private FactionFlags _factionFlags = FactionFlags.None;
    private FactionFlags _enemyFlags = FactionFlags.None;

    #endregion

    #region Events
    public event Action Destroyed;
    #endregion

    #region Properties
    public override FactionFlags FactionFlags
    {
        get
        {
            return _factionFlags;
        }
        set
        {
            _factionFlags = value;
        }
    }
    public override FactionFlags EnemyFlags
    {
        get
        {
            return _factionFlags;
        }
        set
        {
            _factionFlags = value;
        }
    }
    #endregion

    #region States

    public enum BuildState
    {
        Constructed,
        Destroyed
    }

    #endregion

    #region Initialization

    void Awake()
    {
        base.Awake();
        GetComponent<ShowRim>().enabled = false;
    }

    void Start()
    {
        base.Start();
        SelectableList.AddSelectableEntity(this);
        if (GetComponent<ShowRim>().enabled)
            Debug.LogError(gameObject.ToString() + "'s showrim should be disabled intially");
    }

    #endregion

    #region Logic


    /// <summary>
    /// The method that actually damages the building.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>Returns true if the building was damaged or false if the building could not be damaged</returns>
    public override bool Damage(int damage)
    {
        if (buildState == BuildState.Constructed)
        {
            currentHP = Math.Max(currentHP - damage, 0);
            if (currentHP == 0)
            {
                buildState = BuildState.Destroyed;
                // Trigger Destroyed events
                if (Destroyed != null)
                    Destroyed();
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Update()
    {
        if (buildState == BuildState.Constructed)
        {
            Tick();
        }
    }

    /// <summary>
    /// Building tick. Used to advance building progression if any exists. For example
    /// if a building is producing some sort of resource progrssion will be added via the tick.
    /// </summary>
    public virtual void Tick() {}

    #endregion

}
