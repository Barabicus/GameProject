using UnityEngine;
using System.Collections;

public class MonsterList : MonoBehaviour {

    public Mob[] Monsters;

    private static MonsterList _instance;
    public static MonsterList Instance
    {
        get
        {
            return _instance;
        }
    }

	// Use this for initialization
    void Start()
    {
        if (_instance != null)
        {
            Debug.LogWarning("A Monster list already exists!");
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
	
}
