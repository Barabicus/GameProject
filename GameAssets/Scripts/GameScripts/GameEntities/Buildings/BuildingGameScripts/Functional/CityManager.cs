using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CityManager : Building, ICurrencyContainer
{

    #region Fields
    /// <summary>
    /// Reference to all the resource containers available to this city manager
    /// </summary>
    private List<Resource> _resources = new List<Resource>();
    /// <summary>
    /// Cached positions of the resource containers in game positions. 
    /// </summary>
    private List<Vector3> _resourcePositions = new List<Vector3>();
    /// <summary>
    /// Cached dictionary of the current amount of resources this city has. When resources are added or spent
    /// this is updated to provide quick real time data of the resources available.
    /// </summary>
    private Dictionary<ResourceType, int> _cachedResourceNumbers = new Dictionary<ResourceType, int>();
    private Transform _spawnPoint;
    private List<Building> _buildings;
    private List<Mob> _citizens;
    private List<BuildingResourceRequestManager> _activeRequests;
    private List<BuildingConstructor> _blueprints;
    /// <summary>
    /// Cached list of all the citizens that are currently unemployed.
    /// </summary>
    private List<Mob> _unemployedCitizens;
    /// <summary>
    /// List of houses
    /// </summary>
    private List<House> _houses;
    private float _lastDayTick;

    /// <summary>
    /// Frequency to trigger a day tick
    /// </summary>
    public int dayTickFrequency = 60;
    public event Action DayTickEvent;
    public int StartCurrency = 10000;
    public int tax = 10;

    #endregion

    #region Properties
    public List<Mob> UnemployedCitizens
    {
        get { return _unemployedCitizens; }
    }
    public int Currency
    {
        get;
        set;
    }
    public List<Mob> Citizens
    {
        get { return _citizens; }
    }
    public List<StorageBuilding> StorageBuildings
    {
        get
        {
            List<StorageBuilding> buildings = new List<StorageBuilding>();
            foreach (StorageBuilding b in _buildings.FindAll(m => m is StorageBuilding))
            {
                buildings.Add(b);
            }
            return buildings;
        }
    }
    public List<BuildingResourceRequestManager> ActiveRequests
    {
        get
        {
            List<BuildingResourceRequestManager> _list = new List<BuildingResourceRequestManager>();
            foreach (BuildingResourceRequestManager request in _activeRequests)
            {
                if (request.HasSupplyContract == null)
                    _list.Add(request);
            }
            return _list;
        }
    }
    #endregion

    #region Initilization
    public override void Awake()
    {
        base.Awake();
        _unemployedCitizens = new List<Mob>();
        _buildings = new List<Building>();
        _citizens = new List<Mob>();
        _houses = new List<House>();
        _activeRequests = new List<BuildingResourceRequestManager>();
        _spawnPoint = transform.FindChild("_SpawnPoint");
        _blueprints = new List<BuildingConstructor>();
    }

    public override void Start()
    {
        // Initialize resource object with all the ResourceType values starting with an amount of 0
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
        {
            _cachedResourceNumbers.Add(t, 0);
        }
        Currency = StartCurrency;
        base.Start();
    }
    #endregion

    #region Utility


    #endregion

    #region Logic

    public StorageBuilding ClosestStorageBuilding(Vector3 position)
    {
        StorageBuilding closest = null;
        foreach (StorageBuilding sb in StorageBuildings)
        {
            if (closest == null)
            {
                closest = sb;
                continue;
            }
            if (Vector3.Distance(sb.transform.position, position) < Vector3.Distance(closest.transform.position, position))
                closest = sb;
        }
        if (closest == null)
            Debug.LogError("Closest equal null");
        return closest;
    }

    public StorageBuilding ClosestStorageBuilding(Building building)
    {
        return ClosestStorageBuilding(building.transform.position);
    }

    public StorageBuilding ClosestStorageBuilding(Mob mob)
    {
        return ClosestStorageBuilding(mob.transform.position);
    }

    public void AddResourceOrderRequest(BuildingResourceRequestManager request)
    {
        if (_activeRequests.Contains(request))
            return;
        _activeRequests.Add(request);
    }

    public void RemoveResourceOrderRequest(BuildingResourceRequestManager request)
    {
        _activeRequests.Remove(request);
    }

    public BuildingResourceRequestManager TakeResourceRequest(Building building)
    {
        if (ActiveRequests.Count == 0)
            return null;
        BuildingResourceRequestManager bm = ActiveRequests[0];
        bm.HasSupplyContract = building;
        return bm;
    }

    public void AddBlueprint(BuildingConstructor blueprint)
    {
        _blueprints.Add(blueprint);
    }

    public BuildingConstructor TakeBlueprintContract()
    {
        BuildingConstructor bc = _blueprints.Find(b => b.HasBeenSupplied);
        if (bc != null)
        {
            _blueprints.Remove(bc);
        }
        return bc;
    }

    public void AddBuilding(Building building)
    {
        _buildings.Add(building);
        building.CityManager = this;
        building.Destroyed += RemoveBuilding;
        DayTickEvent += building.DayTick;
    }

    public void RemoveBuilding(Building building)
    {
        if (_buildings.Contains(building))
        {
            _buildings.Remove(building);
            building.Destroyed -= RemoveBuilding;
            DayTickEvent -= building.DayTick;
        }
    }

    public bool AddCitizen(Mob m)
    {
        if (_citizens.Contains(m))
            return false;
        _citizens.Add(m);
        if (!m.HasJobBuilding)
            _unemployedCitizens.Add(m);
        if (m.CityManager != null)
            m.CityManager.RemoveCitizen(m);
        m.CityManager = this;
        return true;
    }

    public bool RemoveCitizen(Mob m)
    {
        if (!_citizens.Contains(m))
            return false;
        m.CityManager = null;
        _unemployedCitizens.Remove(m);
        return _citizens.Remove(m);
    }

    private void UpdateCachedResources(ResourceType type, int amount)
    {
        _cachedResourceNumbers[type] = amount;
    }
    #endregion

    public override void Update()
    {
        base.Update();
        if (Time.time - _lastDayTick > dayTickFrequency)
        {
            _lastDayTick = Time.time;
            if (DayTickEvent != null)
                DayTickEvent();
        }
    }

    protected override void Tick()
    {
        base.Tick();
    }
}