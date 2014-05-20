﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HighlightableObject))]
public class WorldResource : ActiveEntity
{

    #region Fields
    public ResourceType resource;
    public Transform deathPrefab;
    public bool isInfinite = false;
    public bool regenerates = false;
    public int currentAmount = 0;
    public int maxAmount = 0;
    /// <summary>
    /// Time in seconds it takes for the resource to regenerate
    /// </summary>
    public float regenTime = 1;
    public int regenAmount = 0;

    private float _timeCount = 0;
    private ResourceState _resourceState = ResourceState.Active;
    #endregion

    #region properties
    ResourceState ResourceState
    {
        get { return _resourceState; }
        set
        {
            switch (value)
            {
                case ResourceState.Dying:
                    Transform  t = (Transform)Instantiate(deathPrefab, transform.position, transform.rotation);
                    t.localScale = transform.parent.localScale;
                    Destroy(gameObject);
                    break;
            }
            _resourceState = value;
        }
    }
    #endregion


    public override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
        switch (_resourceState)
        {
            case global::ResourceState.Active:
                if (!isInfinite && regenerates)
                {
                    _timeCount += Time.deltaTime;
                    if (_timeCount > regenTime)
                    {
                        _timeCount = 0;
                        currentAmount = Mathf.Min(currentAmount + regenAmount, maxAmount);
                    }
                }
                break;
            case global::ResourceState.Dying:

                break;
        }
	}

    /// <summary>
    /// Attempts to harvest the resource type associated with this world resource for the amount specified and drops 
    /// it into the resource container "resource"
    /// </summary>
    /// <param name="resource">The resource container</param>
    /// <param name="amount">The amount of that resource to move</param>
    private void HarvestResource(Resource resource, int amount)
    {
        if (ResourceState == global::ResourceState.Active)
        {
            if (!isInfinite)
                currentAmount = Mathf.Clamp( (currentAmount - amount) + resource.AddResource(this.resource, amount), 0, maxAmount);
            else
                resource.AddResource(this.resource, amount);

            if (currentAmount == 0 && !isInfinite)
                ResourceState = global::ResourceState.Dying;
        }
    }

    public override void PerformAction(PerformActionVariables actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                // Harvest the resource for the calling mob 
                // int parameter passed in should specify the amount required
                HarvestResource(actionEvent.entity.GetComponent<Mob>().Resource, 1);
                break;
        }
    }

}

public enum ResourceState
{
    Active,
    Dying,
    Dead
}
