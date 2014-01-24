using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BuildingList : MonoBehaviour
{

    #region Fields
    public Building[] buildings;
    public List<Type> acceptedComponents = new List<Type>();
    public Material ConstructionMaterial;

    Transform[] _buildBlueprintPrefabs;
    Transform[] _buildPreviewPrefabs;

    /// <summary>
    /// Game Objects that were created from the builing prefabs but have every component but essential blueprint components stripped.
    /// </summary>
    public Transform[] BuildingBlueprintPrefabs { get { return _buildBlueprintPrefabs; } }

    public Transform[] BuildingPreviewPrefabs { get { return _buildPreviewPrefabs; } }

    private static BuildingList _instance;

    #endregion

    enum BuildingType
    {
        Preview,
        Blueprint
    }

    #region Properties

    public static BuildingList Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Building instance does not exist!, please define a building list!");
                return null;
            }
            return _instance;
        }
    }

    #endregion

    // Use this for initialization
    void Awake()
    {
        if (_instance != null)
        {
            Debug.LogWarning("Building Instance was not null!");
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;

            // Generate BuildingMeshPrefabs
            _buildBlueprintPrefabs = new Transform[buildings.Length];
            _buildPreviewPrefabs = new Transform[buildings.Length];

            //  Accepted components when constructing the blueprint prefab.
            acceptedComponents.Add(typeof(MeshFilter));
            acceptedComponents.Add(typeof(MeshRenderer));
            acceptedComponents.Add(typeof(BoxCollider));
            acceptedComponents.Add(typeof(MeshCollider));

            for (int i = 0; i < buildings.Length; i++)
            {
                // Create a new gameobject to use as per the object preview. 
                // Attatch and identical BuildingInfo script to it via the building info
                // script that exists on the prefab.
                // Also Loop through the prefabs and create all children but only attach renderes and set the
                // shader to the construction shader so the preview prefab appears completely like a preview.
                GameObject g = new GameObject("blueprintPrefab_" + buildings[i].name);
                g.AddComponent<BuildingInfo>();
                g.AddComponent<DynamicGridObstacle>();
                g.GetComponent<BuildingInfo>().CopyFromOther(buildings[i].GetComponent<BuildingInfo>());
                g.AddComponent<BuildingConstructor>();
                g.GetComponent<BuildingConstructor>().ConstructedPrefab = buildings[i].transform;
                ConstructPreviewPrefab(buildings[i].gameObject, g, BuildingType.Blueprint);
                g.transform.parent = transform;
                g.SetActive(false);
                _buildBlueprintPrefabs[i] = g.transform;
            }

            for (int i = 0; i < buildings.Length; i++)
            {
                GameObject g = new GameObject("previewPrefab_" + buildings[i].name);
                ConstructPreviewPrefab(buildings[i].gameObject, g, BuildingType.Preview);
                g.transform.parent = transform;
                g.SetActive(false);
                _buildPreviewPrefabs[i] = g.transform;

            }

        }
    }

    /// <summary>
    /// Constructs the preview prefab from a blueprint object. Constructing a gameobject
    /// this way constructs it specifically for previewing the object as a player is about to place the 
    /// desired prefab. This method ensures that only visual components are added to the display prefab,
    /// so not to interfere with other game mechanics that a real building may present. This is why
    /// instances of the actual prefab can not be directly used.
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="obj"></param>
    private void ConstructPreviewPrefab(GameObject blueprint, GameObject obj, BuildingType btype)
    {
        switch (btype)
        {
            case BuildingType.Blueprint:
                obj.tag = "BluePrint";
                break;
            case BuildingType.Preview:
                obj.tag = "Preview";
                break;
        }
        obj.layer = 11;
        // Set the transform properties first
        obj.transform.position = blueprint.transform.position;
        obj.transform.rotation = blueprint.transform.rotation;
        obj.transform.localScale = blueprint.transform.localScale;

        // Add components
        foreach (Component comp in blueprint.GetComponents<Component>())
        {
            if (acceptedComponents.Contains(comp.GetType()))
            {
                obj.AddComponent(comp.GetType());
                if (comp.GetType() == typeof(MeshFilter))
                {
                    obj.GetComponent<MeshFilter>().mesh = blueprint.GetComponent<MeshFilter>().sharedMesh;
                }
                else if (comp.GetType() == typeof(MeshRenderer))
                {
                    obj.GetComponent<MeshRenderer>().material = ConstructionMaterial;
                }
                else if (comp.GetType() == typeof(MeshCollider))
                {
                    switch (btype)
                    {
                        case BuildingType.Blueprint:
                            obj.GetComponent<MeshCollider>().isTrigger = false;
                            break;
                        case BuildingType.Preview:
                            obj.GetComponent<MeshCollider>().isTrigger = true;
                            break;
                    }
                }
                else if (comp.GetType() == typeof(BoxCollider))
                {
                    switch (btype)
                    {
                        case BuildingType.Blueprint:
                            obj.GetComponent<BoxCollider>().isTrigger = false;
                            break;
                        case BuildingType.Preview:
                            obj.GetComponent<BoxCollider>().isTrigger = true;
                            break;
                    }
                    obj.GetComponent<BoxCollider>().size = blueprint.GetComponent<BoxCollider>().size;
                    obj.GetComponent<BoxCollider>().center = blueprint.GetComponent<BoxCollider>().center;
                }

            }
        }
        // Add Sub Objects
        for (int i = 0; i < blueprint.transform.childCount; i++)
        {
            GameObject child = new GameObject(blueprint.transform.GetChild(i).name);
            child.transform.parent = obj.transform;
            ConstructPreviewPrefab(blueprint.transform.GetChild(i).gameObject, child, btype);
        }

    }

}
