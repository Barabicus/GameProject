using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueprintList : MonoBehaviour {

    private static BlueprintList _instance;
    private List<BuildingConstructor> _blueprints;

    public List<BuildingConstructor> Blueprints
    {
        get { return _blueprints; }
    }
    public static BlueprintList Instance
    {
        get { return _instance; } 
    }

    void Awake()
    {
        _instance = this;
        _blueprints = new List<BuildingConstructor>();
    }

}
