using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeliveryBuilding : JobBuilding {


    BuildingResourceRequestManager _currentRequest;


    protected override void Tick()
    {
        base.Tick();
        if (_currentRequest == null)
        {
            _currentRequest = CityManager.TakeResourceRequest(this);
            if (_currentRequest != null)
                _currentRequest.ResourceRequestFilled += DeliveryRequestFilled;
        }

        if (_currentRequest != null)
        {
            foreach (Mob m in UnassignedWorkers)
            {
                m.JobTask = DeliveryTask;
            }
        }
    }

    /// <summary>
    /// Infrom all the workers the delivery task has been filed and to stop what they are doing
    /// </summary>
    void DeliveryRequestFilled()
    {
        foreach (Mob m in Workers)
        {
            m.CurrentActivity = ActivityState.None;
        }
        _currentRequest = null;

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
        }
    }

}
