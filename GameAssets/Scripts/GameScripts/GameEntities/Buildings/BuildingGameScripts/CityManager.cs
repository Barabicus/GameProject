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

    public ParticleSystem[] spawnParticles;

    #endregion

    #region Properties

    #endregion

    #region Initilization
    void Start()
    {
        _buildings = new List<Building>();
        _spawnPoint = transform.FindChild("_SpawnPoint");
        if (_spawnPoint == null)
        {
            Debug.LogWarning("City Manager: " + gameObject.name + " does not have a spawn point!");
        }
        // Initialize resource object with all the ResourceType values starting with an amount of 0
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
        {
            _cachedResourceNumbers.Add(t, 0);
        }
    }
    #endregion

    #region Utility


    #endregion

    #region Logic
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
            PlayerManager.Instance.SpawnMonster(Random.Range(0, 3), _spawnPoint, spawnParticles);
        }
    }

}
