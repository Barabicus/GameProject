﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Resource : MonoBehaviour
{

    #region Events
    public delegate void ResourceChangedDelegate(ResourceType type, int currentAmount);
    public event ResourceChangedDelegate ResourceChanged;
    #endregion

    #region Fields
    /// <summary>
    /// Resource type and it's weight
    /// </summary>
    public static Dictionary<ResourceType, int> ResourceWeight = new Dictionary<ResourceType, int>();
    /// <summary>
    /// The overall maximum amount of resources that this Resource Object can store. 
    /// Each resource has an associated weight to them, for example you can store more wood
    /// Units than stone due to wood having a smaller weight. The higher this number, the greater
    /// amount of resources * their weight can be stored.
    /// </summary>
    public int maxWeight;
    /// <summary>
    /// The current amount of resources that this Resorce is storing. Using the resource type the
    /// current unit value is returned
    /// </summary>
    private Dictionary<ResourceType, int> _currentResources = new Dictionary<ResourceType, int>();
    /// <summary>
    /// The current weight that this resource is holding.
    /// </summary>
    private int _currentWeight = 0;
    #endregion

    #region Properties
    public int CurrentWeight
    {
        get { return _currentWeight; }
    }

    public Dictionary<ResourceType, int> CurrentResources
    {
        get { return _currentResources; }
    }
    #endregion

    #region Initilization
    static Resource()
    {
        ResourceWeight.Add(ResourceType.Wood, 2);
        ResourceWeight.Add(ResourceType.Stone, 5);
    }
    public void Awake()
    {
        // Initialize resource object with all the ResourceType values starting with an amount of 0
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
        {
            _currentResources.Add(t, 0);
        }
    }
    #endregion

    #region Logic

    /// <summary>
    /// Takes a resource type and the amount you want to add in. It stores
    /// all the resources of that type based on how much resources can be held.
    /// It then returns the amount that cannot be held, or 0 if all the resources
    /// were stored.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int AddResource(ResourceType type, int amount)
    {
        _currentWeight += amount * ResourceWeight[type];
        int leftOver = Mathf.Max(_currentWeight - maxWeight, 0);
        _currentWeight -= leftOver;
        // Fire Changed event to notify of resource change and it's new current amount within this Resource object
        if (ResourceChanged != null)
            ResourceChanged(type, _currentResources[type]);
        return leftOver;
    }

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        foreach (KeyValuePair<ResourceType, int> kvp in _currentResources)
        {
            b.AppendLine(kvp.Key + " : " + kvp.Value);
        }
        return b.ToString();
    }

    #endregion



}

public enum ResourceType
{
    Wood,
    Stone
}

