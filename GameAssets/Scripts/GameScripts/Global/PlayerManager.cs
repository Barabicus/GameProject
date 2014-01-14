using UnityEngine;
using System.Collections;
using System.Text;

public class PlayerManager : MonoBehaviour
{

    #region Fields
    private int _lifeForce = 0;
    private int _maxLifeForce = 5000;
    private int _currentPop = 0;
    private int _maxPop = 8;
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
    public int LifeForce { get { return _lifeForce; } }
    public string LifeForceParsed
    {
        get
        {
            return _lifeForce.ToString();
        }
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
	
	// Update is called once per frame
	void Update () {
        if (Time.time - _lastTimeForceTick > 10 && _lifeForce != _maxLifeForce)
        {
            _lastTimeForceTick = Time.time;
            _lifeForce = Mathf.Min(_lifeForce + 5, _maxLifeForce);
        }
	}
}
