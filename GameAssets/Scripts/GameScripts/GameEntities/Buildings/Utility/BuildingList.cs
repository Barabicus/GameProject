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
    public BuildingControl constructionControlPrefab;

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
                //Create prefabs of all the buildings that will act as the blueprint.
                // The blueprint is the building that exists in game but is not yet built
                // and will appear as a green transparent structure on the map.
                GameObject g = new GameObject("blueprintPrefab_" + buildings[i].name);
                g.AddComponent<BuildingInfo>();
                g.GetComponent<BuildingInfo>().CopyFromOther(buildings[i].GetComponent<BuildingInfo>());
                g.AddComponent<BuildingConstructor>();
                g.GetComponent<BuildingConstructor>().ConstructedPrefab = buildings[i].transform;
                g.GetComponent<BuildingConstructor>().controlPrefab = constructionControlPrefab;
                ConstructPreviewPrefab(buildings[i].gameObject, g, BuildingType.Blueprint);
                g.transform.parent = transform;
                g.SetActive(false);
                _buildBlueprintPrefabs[i] = g.transform;
            }

            for (int i = 0; i < buildings.Length; i++)
            {
                // Create prefabs for the preview buildings. Like a blueprint except it is
                // dragged around on the map by the player until the player actually places it 
                // somewhere in which a blueprint in initialized. The preview building
                // is used to show that player what he is placing and how he is placing.
                GameObject g = new GameObject("previewPrefab_" + buildings[i].name);
                g.AddComponent<Rigidbody>();
                g.AddComponent<MaterialColorChanger>();
                g.GetComponent<Rigidbody>().isKinematic = true;
                g.AddComponent<TriggerListener>();
                ConstructPreviewPrefab(buildings[i].gameObject, g, BuildingType.Preview, g.transform);
                g.transform.parent = transform;
                g.SetActive(false);
                _buildPreviewPrefabs[i] = g.transform;

            }

        }
    }

    private void ConstructPreviewPrefab(GameObject blueprint, GameObject obj, BuildingType btype)
    {
        ConstructPreviewPrefab(blueprint, obj, btype, null);
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
    private void ConstructPreviewPrefab(GameObject blueprint, GameObject obj, BuildingType btype, Transform ParentTransform)
    {
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
                            if (obj.GetComponent<DynamicGridObstacle>() == null)
                                obj.AddComponent<DynamicGridObstacle>();
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
                            if (obj.GetComponent<DynamicGridObstacle>() == null)
                                obj.AddComponent<DynamicGridObstacle>();
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

        if (ParentTransform != null && ParentTransform.GetComponent<MaterialColorChanger>() != null)
        {
            if (obj.renderer != null)
            {
                if (obj.renderer.material != null)
                {
                    // Add this material to the parent transforms color changer
                    ParentTransform.GetComponent<MaterialColorChanger>().ColorChanged += (c) =>
                    {
                        obj.renderer.material.color = c;
                    };
                }
            }

        }

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
        obj.transform.position = blueprint.transform.position;
        obj.transform.rotation = blueprint.transform.rotation;
        obj.transform.localScale = blueprint.transform.localScale;

        // Add Sub Objects
        for (int i = 0; i < blueprint.transform.childCount; i++)
        {
            GameObject child = new GameObject(blueprint.transform.GetChild(i).name);
            child.transform.parent = obj.transform;
            switch (btype)
            {
                case BuildingType.Blueprint:
                    ConstructPreviewPrefab(blueprint.transform.GetChild(i).gameObject, child, btype);
                    break;
                case BuildingType.Preview:
                    ConstructPreviewPrefab(blueprint.transform.GetChild(i).gameObject, child, btype, ParentTransform);
                    break;
            }
        }

    }

}
