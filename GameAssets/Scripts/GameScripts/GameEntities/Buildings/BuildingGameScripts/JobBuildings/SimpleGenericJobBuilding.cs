using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleGenericJobBuilding : JobBuilding
{
    List<BuildingConstructor> blueprints;

    Mob lumberWorker;
    Mob builderWorker;
    Mob deliveryWorker;

    List<ResourceOrderRequest> _resourceJobs;

    ResourceOrderRequest? _currentRequest;

    protected override void Start()
    {
        base.Start();
        blueprints = BlueprintList.Instance.Blueprints;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PerformAction(PerformActionVariables actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                Mob m = actionEvent.entity as Mob;
                AddWorker(actionEvent.entity as Mob);

                switch (m.CurrentActivity)
                {
                    case ActivityState.Supplying:
                        Resource.AddResource(ResourceType.Wood, m.Resource.RemoveResource(ResourceType.Wood, 10));
                        if (Resource.CurrentWeight > Resource.maxWeight)
                        {
                            m.CurrentActivity = ActivityState.None;
                        }
                        if (m.Resource.CurrentResources[ResourceType.Wood] == 0)
                        {
                            m.CurrentActivity = ActivityState.None;
                        }
                        break;
                    case ActivityState.Retrieving:
                        // Give Resource
                        m.Resource.AddResource(ResourceType.Wood, Resource.RemoveResource(ResourceType.Wood, 10));
                        break;
                }
                break;
        }
    }

    protected override void Tick()
    {
        base.Tick();

        if (_currentRequest == null)
            _currentRequest = CityManager.TakeResourceOrderRequest();

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
        Debug.Log(mob.CurrentActivity);
        if (mob.CurrentActivity == ActivityState.None)
        {
            // Check if this mob has all the required resources for the construction task
            if (_currentRequest.HasValue)
            {
                // If we don't have any resources 
                if (mob.Resource.CurrentResources[_currentRequest.Value.type] != _currentRequest.Value.amount && mob.Resource.CurrentResources[_currentRequest.Value.type] < _currentRequest.Value.amount)
                {
                    // Get resource from storage house
                    mob.PerformActionVariables = new PerformActionVariables(mob, _currentRequest.Value.type, Mathf.Abs(mob.Resource.CurrentResources[_currentRequest.Value.type] - _currentRequest.Value.amount));
                    mob.CurrentActivity = ActivityState.Retrieving;
                    mob.SetEntityAndFollow(CityManager.FindStorageBuildings()[0]);
                }
                else if (mob.Resource.CurrentResources[_currentRequest.Value.type] == _currentRequest.Value.amount)
                {
                    mob.CurrentActivity = ActivityState.Supplying;
                }
            }
        }
    }

    void BuildTask(Mob mob)
    {
        foreach (BuildingConstructor bc in blueprints)
        {
            if (!bc.HasBeenSupplied)
            {
                if (1 == 1)
                    return;
                if (mob.Resource.CurrentResources[ResourceType.Wood] == 0 && mob.ActionEntity != this)
                {
                    // Need wood, get wood
                    mob.CurrentActivity = ActivityState.Retrieving;
                    mob.PerformActionVariables = new PerformActionVariables(mob, ResourceType.Wood, 10);
                    mob.SetEntityAndFollow(CityManager.FindStorageBuildings()[0]);
                }
                else if (mob.Resource.CurrentResources[ResourceType.Wood] >= 1)
                {
                    // We have enough, supply
                    mob.CurrentActivity = ActivityState.Supplying;
                    mob.SetEntityAndFollow(bc);
                }
                break;
            }
            else
            {
                mob.CurrentActivity = ActivityState.Building;
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

        if (mob.Resource.CurrentResources[ResourceType.Wood] >= 10 && mob.CurrentActivity != ActivityState.Supplying)
        {
            // We have enough resources, time to supply
            mob.CurrentActivity = ActivityState.Supplying;
            mob.PerformActionVariables = new PerformActionVariables(mob, ResourceType.Wood, 10);
            mob.SetEntityAndFollow(CityManager.FindStorageBuildings()[0]);
        }
    }

}

