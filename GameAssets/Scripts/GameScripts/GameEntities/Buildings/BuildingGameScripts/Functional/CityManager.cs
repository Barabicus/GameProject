﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityManager : Building
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
    /// <summary>
    /// List of houses that have free space
    /// </summary>
    private List<House> _freeHouses;

    public ParticleSystem[] spawnParticles;

    #endregion

    #region Properties
    public List<Mob> Citizens
    {
        get { return _citizens; }
    }
    #endregion

    #region Initilization
    protected override void Awake()
    {
        base.Awake();
        _buildings = new List<Building>();
        _citizens = new List<Mob>();
        _freeHouses = new List<House>();
    }

    protected override void Start()
    {
        // Initialize resource object with all the ResourceType values starting with an amount of 0
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
        {
            _cachedResourceNumbers.Add(t, 0);
        }
        base.Start();
    }
    #endregion

    #region Utility


    #endregion

    #region Logic

    public void AddBuilding(Building building)
    {
        _buildings.Add(building);
        building.CityManager = this;
        building.Destroyed += RemoveBuilding;
        if (building.GetType() == typeof(ResourceBuilding))
        {
            AddResourceContainer((ResourceBuilding)building);
        }
    }

    public void RemoveBuilding(Building building)
    {
        if (_buildings.Contains(building))
        {
            _buildings.Remove(building);
        }
    }

    public void AddResourceContainer(ResourceBuilding building)
    {
        building.Resource.ResourceChanged += UpdateCachedResources;
        // Cache all of the buildings current resources
        foreach (ResourceType t in building.Resource.CurrentResources.Keys)
        {
            UpdateCachedResources(t, building.Resource.CurrentResources[t]);
        }
        // Cache this Resource Containers position
        // Being a building type, it should never move.
        _resourcePositions.Add(building.transform.position);
        // Add the resource reference to the city's current resource pool
        _resources.Add(building.Resource);
    }

    private void UpdateCachedResources(ResourceType type, int amount)
    {
        _cachedResourceNumbers[type] = amount;
    }
    #endregion

    void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.I))
        {
        //    Mob m = PlayerManager.Instance.SpawnMonster(Random.Range(0, 3), _spawnPoint, spawnParticles);
            Mob m = PlayerManager.Instance.SpawnMonster(0, _spawnPoint, spawnParticles);
            m.FactionFlags = global::FactionFlags.one;
            m.EnemyFlags = global::FactionFlags.two;
            if (m != null)
            {
                m.CityManager = this;
                _citizens.Add(m);
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            //    Mob m = PlayerManager.Instance.SpawnMonster(Random.Range(0, 3), _spawnPoint, spawnParticles);
            Mob m = PlayerManager.Instance.SpawnMonster(1, _spawnPoint, spawnParticles);
            m.FactionFlags = global::FactionFlags.one;
            m.EnemyFlags = global::FactionFlags.two;
            if (m != null)
            {
                m.CityManager = this;
                _citizens.Add(m);
            }
        }
    }

}