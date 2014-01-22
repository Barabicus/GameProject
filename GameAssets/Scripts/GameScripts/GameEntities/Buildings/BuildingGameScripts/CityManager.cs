using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityManager : Building
{

    #region Fields
    /// <summary>
    /// Reference to all the resource containers available to this city manager
    /// </summary>
    private List<Resource> _resources = new List<Resource>();
    /// <summary>
    /// Cached positions of the resource containers in game positions. 
    /// </summary>
    private List<Vector3> _resourcePositions = new List<Vector3>();
    /// <summary>
    /// Cached dictionary of the current amount of resources this city has. When resources are added or spent
    /// this is updated to provide quick real time data of the resources available.
    /// </summary>
    private Dictionary<ResourceType, int> _cachedResourceNumbers = new Dictionary<ResourceType, int>();
    private Transform _spawnPoint;
    private int _currentPopulation = 0;
    private List<Mob> _citizens;

    public int _maxPopulation = 3;
    public ParticleSystem[] spawnParticles;

    #endregion

    #region Properties
    public List<Mob> Citizens
    {
        get { return _citizens; }
    }
    #endregion

    #region Initilization
    void Start()
    {
        _spawnPoint = transform.FindChild("_SpawnPoint");
        if (_spawnPoint == null)
        {
            Debug.LogWarning("City Manager: " + gameObject.name + " does not have a spawn point!");
        }
        _citizens = new List<Mob>();
        // Initialize resource object with all the ResourceType values starting with an amount of 0
        foreach (ResourceType t in System.Enum.GetValues(typeof(ResourceType)))
        {
            _cachedResourceNumbers.Add(t, 0);
        }
        foreach(ParticleSystem ps in spawnParticles)
        {
            ps.Stop();
        }
    }
    #endregion

    #region Utility

    /// <summary>
    /// Spawns a monster with the given ID. ID is in reference to a MonsterList instance.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void SpawnMonster(int id)
    {
        // Don't spawn if our population is too great
        if (_currentPopulation >= _maxPopulation)
            return;
        Mob m = Instantiate(MonsterList.Instance.Monsters[id], _spawnPoint.position, _spawnPoint.rotation) as Mob;
        AddCitizen(m);
        foreach (ParticleSystem ps in spawnParticles)
        {
            ps.Emit(Mathf.RoundToInt(ps.emissionRate));
        }
    }


    public void AddCitizen(Mob mob)
    {
            _citizens.Add(mob);
            _currentPopulation++;
    }

    #endregion

    #region Logic
    public void AddResourceContainer(ResourceBuilding building)
    {
        building.Resource.ResourceChanged += UpdateCachedResources;
        // Cache all of the buildings current resources
        foreach (ResourceType t in building.Resource.CurrentResources.Keys)
        {
            UpdateCachedResources(t, building.Resource.CurrentResources[t]);
        }
        // Cache this Resource Containers position
        // Being a building type, it should never move.
        _resourcePositions.Add(building.transform.position);
        // Add the resource reference to the city's current resource pool
        _resources.Add(building.Resource);
    }

    private void UpdateCachedResources(ResourceType type, int amount)
    {
        _cachedResourceNumbers[type] = amount;
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnMonster(Random.Range(0, 3));
        }
    }

}
