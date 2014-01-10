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

    /// <summary>
    /// Time in milliseconds it takes to destory this building.
    /// This variable is affected directly when a building is destroyed.
    /// Meaning there is no other sub variable such as currentDestroyTime
    /// 2.5 seconds by default
    /// </summary>
    public float destroyTime = 2500;

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
        Destroying,
        Destroyed
    }

    #endregion

    #region Initialization

    void Awake()
    {
        GetComponent<ShowRim>().enabled = false;
    }

    void Start()
    {
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
                buildState = BuildState.Destroying;
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
        else if (buildState == BuildState.Destroying)
        {
            // Advance destroyed progression
            destroyTime = Mathf.Max(destroyTime - Time.deltaTime, 0f);
            if (destroyTime == 0)
            {
                buildState = BuildState.Destroyed;
                if(Destroyed != null)
                Destroyed();
            }
        }
    }

    /// <summary>
    /// Building tick. Used to advance building progression if any exists. For example
    /// if a building is producing some sort of resource progrssion will be added via the tick.
    /// </summary>
    public virtual void Tick() {}

    #endregion

}
