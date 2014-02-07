using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(BuildingInfo))]
public class Building : ActiveEntity
{

    #region Fields
    public int maxHP;
    public int tickFrequency = 5;

    public bool isInvincible = false;

    protected int currentHP;
    public BuildState buildState = BuildState.Constructed;

    private CityManager _cityManager;
    private FactionFlags _factionFlags = FactionFlags.None;
    private FactionFlags _enemyFlags = FactionFlags.None;
    private float _lastTick;

    #endregion

    #region Events
    public delegate void BuildingEvent(Building building);
    public event BuildingEvent Destroyed;
    #endregion

    #region Properties
    public CityManager CityManager
    {
        get { return _cityManager; }
        set { _cityManager = value; }
    }
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
    }

    void Start()
    {
        base.Start();
        _lastTick = Time.time;
        transform.parent.GetComponent<IslandManager>().cityManager.AddBuilding(this);
        SelectableList.AddSelectableEntity(this);
        // Register with city manager
        PlayerManager.Instance.cityManager.AddBuilding(this);
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
                    Destroyed(this);
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
            if (Time.time - _lastTick >= tickFrequency)
            {
                _lastTick = Time.time;
                Tick();
            }
        }
    }

    /// <summary>
    /// Building tick. Used to advance building progression if any exists. For example
    /// if a building is producing some sort of resource progrssion will be added via the tick.
    /// </summary>
    protected virtual void Tick() {}

    #endregion

}
