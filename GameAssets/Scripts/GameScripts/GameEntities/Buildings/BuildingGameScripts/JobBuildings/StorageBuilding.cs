using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageBuilding : Building
{

    public ResourceType[] acceptedResources;
    private List<ResourceType> _acceptedResources;

    public override void Start()
    {
        base.Start();
        Resource.AddResource(ResourceType.Meat, 50);
        _acceptedResources = new List<ResourceType>(acceptedResources);
    }

    protected override void Supply(Mob mob, PerformActionVariables actionVariables)
    {
        bool isMobResourceEmpty = true;
        foreach (ResourceType rt in actionVariables.resourceTypesArgs)
        {
            if (_acceptedResources.Contains(rt))
            {
                if (mob.Resource.CurrentResources[rt] > 0)
                {
                    Resource.TransferResources(mob.Resource, rt, 1);
                    isMobResourceEmpty = false;
                    break;
                }
            }
        }
        if (isMobResourceEmpty)
            mob.CurrentActivity = ActivityState.None;
    }

}
