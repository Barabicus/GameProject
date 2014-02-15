using UnityEngine;
using System.Collections;
using System;

public class BuildingConstructor : ActiveEntity
{

    #region Fields

    /// <summary>
    /// Essentially the time it takes to build this building. Rather than
    /// having this as "buildTime" it is recognized as a set amount of units for construction
    /// because different in game builder bots will have varying skill levels for contructing
    /// and will be able to apply more build units per tick based on their ability.
    /// </summary>
    public int requiredBuildUnits;
    protected int currentBuildUnits = 0;
    public Transform ConstructedPrefab;
    private FactionFlags _factionFlags = FactionFlags.None;
    private ResourceType[] requiredResources;
    private int[] requiredResourceAmount;
    private int[] currentResourceAmount;
    private bool _resourceRequirementMet = false;

    #endregion

    #region Events
    public event Action Constructed;
    #endregion

    #region Properties
    public bool ResourceRequirementMet
    {
        get { return _resourceRequirementMet; }
    }
    public override FactionFlags EnemyFlags
    {
        get
        {
            // No point in having enemy flags for a building blueprint
            return FactionFlags.None;
        }
        set
        {
        }
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

    #endregion

    #region Initilization

    // Use this for initialization
    void Start()
    {
        base.Start();
        IsHighlightable = false;
        name = GetComponent<BuildingInfo>().BuildingName;
        requiredBuildUnits = GetComponent<BuildingInfo>().RequiredBuildUnits;
        requiredResources = GetComponent<BuildingInfo>().requiredResources;
        requiredResourceAmount = GetComponent<BuildingInfo>().requiredResourceAmount;
        FactionFlags = GetComponent<BuildingInfo>().factionFlags;
        currentResourceAmount = new int[requiredResourceAmount.Length];
    }

    #endregion

    #region Logic

    // Update is called once per frame
    void Update()
    {

    }

    public override bool Damage(int damage)
    {
        // Substract build units. If build units is less than 0, the blueprint has been destroyed
        currentBuildUnits -= damage;
        if (currentBuildUnits < 0)
        {
            Destroy(gameObject);
            return true;
        }
        else
            return false;

    }

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                Mob mob = actionEvent.entity.GetComponent<Mob>();
                switch (mob.CurrentActivity)
                {
                    case ActivityState.Building:
                        Construct(mob.Skills.buildPower);
                        break;
                    case ActivityState.Supplying:
                        SupplyResources(mob.Resource);
                        break;
                }
                break;
        }
    }

    public void SupplyResources(Resource resource)
    {
        if (_resourceRequirementMet)
            return;
        // Remove all resources from this container that this building constructor needs to advance.
        for (int i = 0; i < requiredResources.Length; i++)
        {
            if (currentResourceAmount[i] < requiredResourceAmount[i])
            {
                currentResourceAmount[i] += resource.RemoveResource(requiredResources[i], (requiredResourceAmount[i] - currentResourceAmount[i]));
            }
        }

        // Perform check to see if we have all the required resources
        _resourceRequirementMet = true;
        for (int i = 0; i < requiredResources.Length; i++)
        {
            if (currentResourceAmount[i] != requiredResourceAmount[i])
            {
                _resourceRequirementMet = false;
                return;
            }
        }
    }

    /// <summary>
    /// This method is called during construction to actually construct the building. 
    /// </summary>
    /// <param name="contributedBuildUnits"></param>
    /// <returns>Returns true if the buildUnits have been added to the current
    /// construction units. Returns false if the building cannot be built.
    /// i.e is it already constructed, destroying or destroyed</returns>
    public bool Construct(int contributedBuildUnits)
    {
        if (!_resourceRequirementMet)
            return false;
        currentBuildUnits += contributedBuildUnits;

        if (currentBuildUnits >= requiredBuildUnits)
        {
            if (Constructed != null)
                Constructed();
            // Set Object in active while we create the building
            gameObject.SetActive(false);
            if (ConstructedPrefab == null)
            {
                Debug.LogError("Construction prefab has not been set, cannot create this blueprint!");
                Destroy(gameObject);
                return false;
            }
            Transform t = Instantiate(ConstructedPrefab, transform.position, transform.rotation) as Transform;
            t.parent = transform.parent;
            t.GetComponent<ActiveEntity>().FactionFlags = FactionFlags;
            Destroy(gameObject);
            return true;
        }
        else
            return false;

    }

    #endregion

}
