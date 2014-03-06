using UnityEngine;
using System.Collections;
using System.Text;

public class PlayerManager : MonoBehaviour
{

    #region Fields
    public int maxPopulation = 5;

    private int _currentPopulation = 0;
    private float _lastTimeForceTick;
    private static PlayerManager _instance;

    #endregion

    #region Properties
    public static PlayerManager Instance { 
        get
        { return _instance; 
        }
        set
        {
            _instance = value;
        }
    }
    public int CurrentPopulation
    {
        get { return _currentPopulation; }
    }
    public int MaxPopulation
    {
        get { return maxPopulation; }
    }
    #endregion

	void Start () {
        if (Instance != null)
        {
            Debug.LogError("Player Manager already set!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _lastTimeForceTick = Time.time;
	}

    /// <summary>
    /// Spawns a monster with the given ID. ID is in reference to a MonsterList instance.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns a mob is a monster was spawned or null if a mob was not spawned</returns>
    public Mob SpawnMonster(int id, Transform spawnPoint, ParticleSystem[] spawnParticles)
    {
        // Don't spawn if our population is too great
        if (_currentPopulation >= maxPopulation)
            return null;
        _currentPopulation++;
        Mob m = Instantiate(MonsterList.Instance.Monsters[id], spawnPoint.position, spawnPoint.rotation) as Mob;
        foreach (ParticleSystem ps in spawnParticles)
        {
            Instantiate(ps, spawnPoint.position, Quaternion.Euler(new Vector3(90, 0, 0)));
        }
        return m;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
