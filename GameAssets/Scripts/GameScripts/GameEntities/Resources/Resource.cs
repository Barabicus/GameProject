using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Resource : MonoBehaviour
{

    #region Events
    public delegate void ResourceDelegate(ResourceType type, int currentAmount);
    public event ResourceDelegate ResourceChanged;
    public event ResourceDelegate ResourceAdded;
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
    public int _currentWeight = 0;
    #endregion

    #region Properties
    public int this[ResourceType rt]{
        get { return _currentResources[rt]; }
    }
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
        ResourceWeight.Add(ResourceType.Meat, 1);
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
        int initalAmount = amount;
        // Get how much weight we can use to store items
        int difference = maxWeight - CurrentWeight;
        difference = difference / (ResourceWeight[type]);
        amount = amount <= difference ? amount : difference;
        // Store the Resources
        _currentResources[type] += amount;
        // Increment the weight accordingly
        _currentWeight += amount * ResourceWeight[type];

        //Fire how many items were added event
        if (ResourceAdded != null)
            ResourceAdded(type, amount);

        // Fire Changed event to notify of resource change and it's new current amount within this Resource object
        if (ResourceChanged != null)
            ResourceChanged(type, _currentResources[type]);

        return initalAmount - amount;
    }

    /// <summary>
    /// Removes the specified amount of resources from this resource and returns the value that was
    /// actually removed
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int RemoveResource(ResourceType type, int amount)
    {
        if (amount < 0)
        {
            Debug.LogError("Amount should be greater than 0");
            return 0;
        }
        int rtrn = amount > _currentResources[type] ? _currentResources[type] : amount;
        _currentResources[type] -= rtrn;
        _currentWeight -= ResourceWeight[type] * rtrn;
        return rtrn;
    }

    /// <summary>
    /// Transfers resources from the other container into this container. Returns the left over amount.
    /// </summary>
    /// <param name="otherContainer"></param>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public int TransferResources(Resource otherContainer, ResourceType type, int amount)
    {
        amount = amount > GetMaxRemainder(type) ? GetMaxRemainder(type) : amount;
        return AddResource(type, otherContainer.RemoveResource(type, amount));
    }

    /// <summary>
    /// Returns the max number of units of the specified resource type that can be stored with the given weight
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetMaxRemainder(ResourceType type)
    {
        return (maxWeight - _currentWeight) / ResourceWeight[type];
    }

    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        b.AppendLine("CurrentWeight: " + CurrentWeight + " Maximum Weight: " + maxWeight);
        foreach (KeyValuePair<ResourceType, int> kvp in _currentResources)
        {
            b.AppendLine(kvp.Key + " : " + kvp.Value + " (" + ResourceWeight[kvp.Key] * kvp.Value + ")");
        }
        return b.ToString();
    }

    #endregion



}

public enum ResourceType
{
    Wood,
    Stone,
    Meat
}

