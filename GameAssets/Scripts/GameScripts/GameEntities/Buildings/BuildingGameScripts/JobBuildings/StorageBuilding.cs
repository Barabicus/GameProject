using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageBuilding : Building
{

    public ResourceType[] acceptedResources;
    private List<ResourceType> _acceptedResources;

    protected override void Start()
    {
        base.Start();
        Resource.AddResource(ResourceType.Meat, 50);
        _acceptedResources = new List<ResourceType>(acceptedResources);
    }

    public override void PerformAction(PerformActionVariables actionEvent)
    {
        base.PerformAction(actionEvent);
        if (actionEvent.tag.Equals("Mob"))
        {
            Mob m = actionEvent.entity.GetComponent<Mob>();

            switch (m.CurrentActivity)
            {
                case ActivityState.Supplying:
                    bool isMobResourceEmpty = true;
                    foreach (ResourceType rt in actionEvent.resourceTypesArgs)
                    {
                        if (_acceptedResources.Contains(rt))
                        {
                            if (m.Resource.CurrentResources[rt] > 0)
                            {
                                Resource.TransferResources(m.Resource, rt, 1);
                                isMobResourceEmpty = false;
                                break;
                            }
                        }
                    }
                    if (isMobResourceEmpty)
                        m.CurrentActivity = ActivityState.None;
                    break;
                case ActivityState.Retrieving:
                    m.Resource.AddResource(actionEvent.resourceTypesArgs[0], Resource.RemoveResource(actionEvent.resourceTypesArgs[0], actionEvent.intArgs[0]));
                    break;
            }
        }
    }

}
