using UnityEngine;
using System.Collections;
using System.Text;

public class PlayerManager : MonoBehaviour
{

    #region Fields
    private CityManager _cityManager;
    private int _lifeForce = 0;

    #endregion

    #region Properties
    public int LifeForce { get { return _lifeForce; } }
    public string LifeForceParsed
    {
        get
        {
            return _lifeForce.ToString();
        }
    }
    #endregion

    // Use this for initialization
	void Start () {
        _cityManager = new CityManager();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
