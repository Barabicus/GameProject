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

    Transform[] _buildMeshPrefabs;

    /// <summary>
    /// Game Objects that were created from the builing prefabs but have every, but mesh components stripped.
    /// Used for previewing buildings. This should be used as a pooled instance list.
    /// </summary>
    public Transform[] BuildingMeshPreviewPrefabs { get { return _buildMeshPrefabs; } }

    private static BuildingList _instance;

    #endregion

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
	void Awake () {
        if (_instance != null)
        {
            Debug.LogWarning("Building Instance was not null!");
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;

            //  Accepted components when constructing the preview prefab.
            acceptedComponents.Add(typeof(MeshFilter));
            acceptedComponents.Add(typeof(MeshRenderer));
            acceptedComponents.Add(typeof(BoxCollider));
            acceptedComponents.Add(typeof(MeshCollider));

            // Generate BuildingMeshPrefabs
            _buildMeshPrefabs = new Transform[buildings.Length];
            for (int i = 0; i < buildings.Length; i++)
            {
                // Create a new gameobject to use as per the object preview. 
                // Attatch and identical BuildingInfo script to it via the building info
                // script that exists on the prefab.
                // Also Loop through the prefabs and create all children but only attach renderes and set the
                // shader to the construction shader so the preview prefab appears completely like a preview.
                GameObject g = new GameObject("previewPrefab_" + buildings[i].name);
                g.AddComponent<BuildingInfo>();
                g.AddComponent<DynamicGridObstacle>();
                g.GetComponent<BuildingInfo>().CopyFromOther(buildings[i].GetComponent<BuildingInfo>());
                g.AddComponent<BuildingConstructor>();
                g.GetComponent<BuildingConstructor>().ConstructedPrefab = buildings[i].transform;
                ConstructPreviewPrefab(buildings[i].gameObject, g);
                g.transform.parent = transform;
                g.SetActive(false);
                _buildMeshPrefabs[i] = g.transform;
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
    private void ConstructPreviewPrefab(GameObject blueprint, GameObject obj)
    {
        obj.tag = "BluePrint";
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
                }
                else if (comp.GetType() == typeof(BoxCollider))
                {

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
            ConstructPreviewPrefab(blueprint.transform.GetChild(i).gameObject, child);
        }

    }
	
}
