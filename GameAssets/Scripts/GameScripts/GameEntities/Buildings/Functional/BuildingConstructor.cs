using UnityEngine;
using System.Collections;
using System;

public class BuildingConstructor : ActiveEntity
{

    #region Fields

    /// <summary>
    /// Essentially the time it takes to build this building. Rather than
    /// having this as "buildTime" it is recognized as a set amount of units for construction
    /// because different in game builder bots will have varying skill levels for contructing
    /// and will be able to apply more build units per tick based on their ability.
    /// </summary>
    public int requiredBuildUnits;
    protected int currentBuildUnits = 0;
    public Transform ConstructedPrefab;
    private FactionFlags _factionFlags = FactionFlags.None;

    #endregion

    #region Events
    public event Action Constructed;
    #endregion

    #region Properties

    public override FactionFlags EnemyFlags
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
    public override FactionFlags FactionFlags
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

    #endregion

    #region Initilization

    // Use this for initialization
	void Start () {
        name = GetComponent<BuildingInfo>().BuildingName;
        requiredBuildUnits = GetComponent<BuildingInfo>().RequiredBuildUnits;
	}

    #endregion

    #region Logic

    // Update is called once per frame
	void Update () {
	
	}

    public override bool Damage(int damage)
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

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        if (actionEvent.tag == "Mob")
        {
            Mob mob = actionEvent.entity.GetComponent<Mob>();
            // If the mob can build, build
            if ((mob.MobAbiltiyFlags & Mob.MobFlags.CanBuild) == Mob.MobFlags.CanBuild)
                Construct(mob.Skills.buildPower);
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
            Instantiate(ConstructedPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
            return true;
        }
        else
            return false;

    }

    #endregion

}
