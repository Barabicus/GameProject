using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleGenericJobBuilding : JobBuilding
{
    List<BuildingConstructor> blueprints;

    Mob lumberWorker;
    Mob builderWorker;
    Mob deliveryWorker;

    BuildingResourceRequestManager _currentRequest;

    public override void Start()
    {
        base.Start();
        blueprints = BlueprintList.Instance.Blueprints;
    }

    public override void Awake()
    {
        base.Awake();
    }

    protected override void Tick()
    {
        base.Tick();

        if (_currentRequest == null)
        {
            _currentRequest = CityManager.TakeResourceRequest(this);
            if (_currentRequest != null)
                _currentRequest.ResourceRequestFilled += DeliveryRequestFilled;
        }

        if (Workers.Count > 0)
            lumberWorker = Workers[0];

        if (Workers.Count > 1)
            builderWorker = Workers[1];
        if (Workers.Count > 2)
            deliveryWorker = Workers[2];

        if (lumberWorker != null && lumberWorker.JobTask != LumberTask)
            lumberWorker.JobTask = LumberTask;

        if (builderWorker != null && builderWorker.JobTask != BuildTask)
            builderWorker.JobTask = BuildTask;

        if (deliveryWorker != null && deliveryWorker.JobTask != DeliveryTask)
            deliveryWorker.JobTask = DeliveryTask;
    }

    void DeliveryTask(Mob mob)
    {
        switch (mob.CurrentActivity)
        {
            case ActivityState.None:
                // Check if this mob has all the required resources for the construction task
                if (_currentRequest != null && _currentRequest.HasNext())
                {
                    // If we don't have any resources 
                    if (mob.Resource.GetMaxRemainder(_currentRequest.NextRequest.ResourceType) != 0 && mob.Resource.CurrentResources[_currentRequest.NextRequest.ResourceType] != _currentRequest.NextRequest.Amount && mob.Resource.CurrentResources[_currentRequest.NextRequest.ResourceType] < _currentRequest.NextRequest.Amount)
                    {
                        // Get resource from storage house
                        // Try to retrieve the number of required resources from the storehouse. If this number can't be satisfied
                        // It will just fill the units Resource container with the maximum amount.
                        mob.PerformActionVariables = new PerformActionVariables(mob, _currentRequest.NextRequest.ResourceType, Mathf.Abs(mob.Resource.CurrentResources[_currentRequest.NextRequest.ResourceType] - _currentRequest.NextRequest.Amount));
                        mob.CurrentActivity = ActivityState.Retrieving;
                        mob.SetEntityAndFollow(CityManager.StorageBuildings[0]);
                    }
                    //If we have enough resources, deliver
                    else
                    {
                        mob.CurrentActivity = ActivityState.Supplying;
                        mob.PerformActionVariables = new PerformActionVariables(mob, _currentRequest.NextRequest.ResourceType, _currentRequest.NextRequest.Amount);
                        mob.SetEntityAndFollow(_currentRequest.Building);
                    }
                }
                break;
            case ActivityState.Supplying:
   
                break;
        }
    }

    void DeliveryRequestFilled()
    {
        deliveryWorker.CurrentActivity = ActivityState.None;
        _currentRequest.ResourceRequestFilled -= DeliveryRequestFilled;
        _currentRequest = null;
        ForceTick();
    }

    void BuildTask(Mob mob)
    {
        foreach (BuildingConstructor bc in blueprints)
        {
            if (bc.HasBeenSupplied && mob.CurrentActivity != ActivityState.Building)
            {
                mob.CurrentActivity = ActivityState.Building;
                mob.PerformActionVariables = new PerformActionVariables(mob);
                mob.SetEntityAndFollow(bc);
                break;
            }
        }
    }

    void LumberTask(Mob mob)
    {
        if (mob.CurrentActivity == ActivityState.None)
        {
            Collider[] c = Physics.OverlapSphere(transform.position, 50f, 1 << 11);
            List<Collider> cl = new List<Collider>();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i].tag.Equals("Tree"))
                    cl.Add(c[i]);
            }
            if (cl.Count > 0)
                mob.PerformAction(new PerformActionVariables(cl[UnityEngine.Random.Range(0, cl.Count)].GetComponent<WorldResource>()));
        }

        if (mob.Resource.GetMaxRemainder(ResourceType.Wood) == 0 && mob.CurrentActivity != ActivityState.Supplying)
        {
            // We have enough resources, time to supply
            mob.CurrentActivity = ActivityState.Supplying;
            mob.PerformActionVariables = new PerformActionVariables(mob, ResourceType.Wood, 10);
            mob.SetEntityAndFollow(CityManager.StorageBuildings[0]);
        }
    }

}

