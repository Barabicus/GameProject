using UnityEngine;
using System.Collections;
using System;

public class BuildingConstructor : Building
{

    #region Fields

    /// <summary>
    /// Essentially the time it takes to build this building. Rather than
    /// having this as "buildTime" it is recognized as a set amount of units for construction
    /// because different in game builder bots will have varying skill levels for contructing
    /// and will be able to apply more build units per tick based on their ability.
    /// </summary>
    public int requiredBuildUnits;
    int currentBuildUnits = 0;
    public Transform ConstructedPrefab;
    private FactionFlags _factionFlags = FactionFlags.None;
    private ResourceType[] requiredResources;
    private int[] requiredResourceAmount;
    private bool _resourceRequirementMet = false;


    #endregion

    #region Events
    public event Action Constructed;
    #endregion

    #region Properties
    public bool HasBeenSupplied
    {
        get { return _resourceRequirementMet; }
    }
    public FactionFlags EnemyFlags
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
    public FactionFlags FactionFlags
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
    public int CurrentBuildUnits
    {
        get { return currentBuildUnits; }
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
        BlueprintList.Instance.Blueprints.Add(this);
        Resource.maxWeight = 5000;

        // Add Resource Requests for this building
        for (int i = 0; i < requiredResources.Length; i++)
        {
            BuildingResourceRequestManager.AddRequest(requiredResources[i], requiredResourceAmount[i]);
        }

    }

    #endregion

    #region Logic

    void OnDestroy()
    {
        BlueprintList.Instance.Blueprints.Remove(this);
    }

    public new bool Damage(int damage)
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

    public override void PerformAction(PerformActionVariables actionEvent)
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
                }
                break;
        }
    }

    protected override void Supply(Mob mob, PerformActionVariables actionVariables)
    {
        base.Supply(mob, actionVariables);
        SupplyResources(mob.Resource, actionVariables);
    }

    /// <summary>
    /// Check to ensure that resource requirement is met
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="actionVariables"></param>
    void SupplyResources(Resource resource, PerformActionVariables actionVariables)
    {
        if (_resourceRequirementMet)
            return;
        // Perform check to see if we have all the required resources
        _resourceRequirementMet = true;
        for (int i = 0; i < requiredResources.Length; i++)
        {
            if (Resource[requiredResources[i]] != requiredResourceAmount[i])
            {
                _resourceRequirementMet = false;
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
