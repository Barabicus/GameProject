using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Resource))]
public class ResourceBuilding : Building {

    private Resource _resource;
    public Resource Resource
    {
        get { return _resource; }
    }

    public virtual void Awake()
    {
        _resource = GetComponent<Resource>();
        base.Awake();
    }

}
