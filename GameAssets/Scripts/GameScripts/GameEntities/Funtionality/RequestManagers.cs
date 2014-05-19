using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlueprintResourceRequestManager : IRequestManager
{
    private BlueprintContract _blueprint;
}

public class BuildingResourceRequestManager : IRequestManager
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

public struct BlueprintContract
{
    private BuildingConstructor _buildingConst;
    private Building _hasContract;

    public BuildingConstructor BuildingConstructer { get { return _buildingConst; } }
    public Building HasContract { get { return _hasContract; } set { _hasContract = value; } }

    public BlueprintContract(BuildingConstructor buildingConst)
    {
        this._buildingConst = buildingConst;
        this._hasContract = null;
    }
}
