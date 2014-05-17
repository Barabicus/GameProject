using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BuildingInfo))]
[RequireComponent(typeof(Resource))]
[RequireComponent(typeof(DynamicGridObstacle))]
public class Building : ActiveEntity, ISelectable, ICitymanager, IResource, IFactionFlag
{

    #region Fields
    public float tickFrequency = 5;
    public BuildingCategory buildingCategory;
    [HideInInspector]
    public List<ControlType> ControlComponents;

    public BuildState buildState = BuildState.Constructed;

    private BuildingControl _controlInstance;
    private FactionFlags _factionFlags = FactionFlags.None;
    private FactionFlags _enemyFlags = FactionFlags.None;
    private float _lastTick;
    private BuildingResourceRequestManager _buildingResourceRequestManager;

    #endregion

    #region Events
    public delegate void BuildingEvent(Building building);
    public event BuildingEvent Destroyed;
    #endregion

    #region Properties
    public Resource Resource
    {
        get;
        set;
    }
    public CityManager CityManager
    {
        get;
        set;
    }
    protected BuildingResourceRequestManager BuildingResourceRequestManager
    {
        get { return _buildingResourceRequestManager; }
    }
    public FactionFlags FactionFlags
    {
        get;
        set;
    }
    public FactionFlags EnemyFlags
    {
        get;
        set;
    }
    #endregion

    #region States

    public enum BuildState
    {
        Constructed,
        Destroyed
    }

    public enum BuildingCategory
    {
        Funtional,
        Housing,
        Storage
    }

    public bool IsSelected
    {
        get;
        set;
    }

    #endregion

    #region Initialization

    public override void Awake()
    {
        base.Awake();
        FactionFlags = global::FactionFlags.one;
    }

    public override void Start()
    {
        base.Start();
        if (HUDRoot.go != null)
        {
            _controlInstance = NGUITools.AddChild(HUDRoot.go, BuildingGUIProperties.Instance.BasePrefab).GetComponent<BuildingControl>();
            _controlInstance.ParentObject = this;
            foreach (ControlType c in ControlComponents)
            {
                _controlInstance.AddTab(c);
            }
            // Make the UI follow the target
            if (transform.FindChild("_pivot") == null)
            {
                GameObject go = new GameObject("_pivot");
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(0, 15, 0);
            }
            _controlInstance.gameObject.AddComponent<UIFollowTarget>().target = transform.FindChild("_pivot");
            _controlInstance.gameObject.SetActive(false);
        }
        Resource = GetComponent<Resource>();
        _lastTick = Time.time;
        transform.parent.GetComponent<IslandManager>().cityManager.AddBuilding(this);
        _buildingResourceRequestManager = new BuildingResourceRequestManager(this);
    }


    #endregion

    #region Logic

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
    /// Forces a tick to be fired.
    /// </summary>
    protected void ForceTick()
    {
        _lastTick += tickFrequency;
    }

    public override void PerformAction(PerformActionVariables actionVariables)
    {
        base.PerformAction(actionVariables);
        switch (actionVariables.tag)
        {
            case "Mob":
                Mob m = actionVariables.entity.GetComponent<Mob>();
                switch (m.CurrentActivity)
                {
                    case ActivityState.Supplying:
                        Supply(m, actionVariables);
                        break;
                    case ActivityState.Retrieving:
                        Retrieve(m, actionVariables);
                        break;
                }
                break;
        }
    }

    protected virtual void Retrieve(Mob mob, PerformActionVariables actionVariables)
    {
        mob.Resource.TransferResources(Resource, actionVariables.resourceTypesArgs[0], actionVariables.intArgs[0]);
        mob.CurrentActivity = ActivityState.None;
    }

    protected virtual void Supply(Mob mob, PerformActionVariables actionVariables)
    {
        bool isMobResourceEmpty = true;
        foreach (ResourceType rt in actionVariables.resourceTypesArgs)
        {
            if (mob.Resource.CurrentResources[rt] > 0)
            {
              //  Resource.TransferResources(mob.Resource, rt, actionVariables.intArgs[0]);
                Resource.TransferResources(mob.Resource, rt, 1);

                isMobResourceEmpty = false;
                break;
            }
        }
        if (isMobResourceEmpty)
            mob.CurrentActivity = ActivityState.None;
    }

    /// <summary>
    /// Building tick. Used to advance building progression if any exists. For example
    /// if a building is producing some sort of resource progrssion will be added via the tick.
    /// </summary>
    protected virtual void Tick() { }

    /// <summary>
    /// Fired after a period of time judged by the CityManager. Assumed a day game time. This is fired synchronously across all buildings
    /// under the city's control.
    /// </summary>
    public virtual void DayTick() { Debug.Log("Day Tick"); }

    void OnMouseDown()
    {
        if (UICamera.hoveredObject)
            return;
        if (_controlInstance != null)
        {
            BuildControlsGUIManager.Instance.CurrentControlBox = _controlInstance.gameObject;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_controlInstance != null)
        {
            BuildControlsGUIManager.Instance.CurrentControlBox = null;
            Destroy(_controlInstance);
        }
    }

    #endregion

    #region Static Mob State Method

    /// <summary>
    ///  A supply state helper method to transfer resources from the mob to the specified building. 
    /// </summary>
    /// <param name="building"></param>
    /// <param name="mob"></param>
    /// <param name="actionVariables"></param>
    public static void MobSupplyAndTransfer(Building building, Mob mob, PerformActionVariables actionVariables)
    {
        if (mob.CurrentActivity != ActivityState.Supplying)
        {
            Debug.LogError("Mob State should be supplying");
            return;
        }
        bool isMobResourceEmpty = true;
        foreach (ResourceType rt in actionVariables.resourceTypesArgs)
        {
            if (mob.Resource.CurrentResources[rt] > 0)
            {
                building.Resource.TransferResources(mob.Resource, rt, actionVariables.intArgs[0]);
                isMobResourceEmpty = false;
                break;
            }
        }
        if (isMobResourceEmpty)
            mob.CurrentActivity = ActivityState.None;
    }

    #endregion
}

public class BuildingResourceRequestManager
{
    /// <summary>
    /// The Building that this object looks out for to take Resource Orders for
    /// </summary>
    private Building _building;
    /// <summary>
    /// Meta info about what building is managing the supply for this contract.
    /// </summary>
    private Building _hasSupplyContract;
    /// <summary>
    /// The Requests that this object is handling
    /// </summary>
    private List<ResourceContract> _contracts;

    public event Action ResourceRequestFilled;

    public ResourceContract NextRequest
    {
        get
        {
            return _contracts[0];
        }
    }

    public bool HasNext()
    {
        return _contracts.Count > 0;
    }

    public Building Building
    {
        get { return _building; }
    }
    public Building HasSupplyContract
    {
        get { return _hasSupplyContract; }
        set { _hasSupplyContract = value; }
    }
    public BuildingResourceRequestManager(Building building)
    {
        this._building = building;
        _contracts = new List<ResourceContract>();
        building.Resource.ResourceAdded += ResourceAdded;
    }

    private void ResourceAdded(ResourceType rtype, int amount)
    {
        if (HasNext())
        {
            while (true)
            {
                // If amount is greater then the request is at an overflow
                if (amount > NextRequest.Amount)
                {
                    amount = Mathf.Abs(NextRequest.Amount - amount);
                    ContractFilled();
                }
                else if (amount == NextRequest.Amount)
                {
                    ContractFilled();
                    break;
                }
                else
                {
                    // Otherwise remove the amount of resources on the current contract
                    if (HasNext())
                    {
                        _contracts[0] = new ResourceContract(_contracts[0].ResourceType, _contracts[0].Amount - amount);
                        break;
                    }
                }
            }
        }
    }

    public void AddRequest(ResourceContract request)
    {
        _contracts.Add(request);
        _building.CityManager.AddResourceOrderRequest(this);
    }

    public void AddRequest(ResourceType type, int amount)
    {
        _contracts.Add(new ResourceContract(type, amount));
        _building.CityManager.AddResourceOrderRequest(this);

    }

    private void ContractFilled()
    {
        _contracts.RemoveAt(0);
        if (_contracts.Count == 0)
            _building.CityManager.RemoveResourceOrderRequest(this);
        if (ResourceRequestFilled != null)
            ResourceRequestFilled();
    }

    public int RequestsCount()
    {
        return _contracts.Count;
    }
}

public struct ResourceContract
{
    ResourceType _type;
    int _amount;

    public ResourceType ResourceType
    {
        get { return this._type; }
    }
    public int Amount
    {
        get { return this._amount; }
    }
    public ResourceContract(ResourceType type, int amount)
    {
        this._type = type;
        this._amount = amount;
    }
}
