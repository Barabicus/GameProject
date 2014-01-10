using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitDetailsGUI : MonoBehaviour {

    public UIGrid uiGrid;
    /// <summary>
    /// Pool the avatar's for the mobs here.
    /// </summary>
    public GameObject[] AvatarPrefabs;
    /// <summary>
    /// Assign the prefabs their ID here in the same order
    /// </summary>
    public string[] AvatarID;
    /// <summary>
    /// Since unity can't serialize dictionaries, create it during runtime.
    /// </summary>
    Dictionary<string, GameObject> avatarDictionary;

    SelectController _selCont;


	// Use this for initialization
	void Start () {
        // Ensure everything is consturcted properly
        if (AvatarPrefabs.Length != AvatarID.Length)
        {
            Debug.LogError("The AvatarPrefabs list and the Avatar ID's list do not match. Ensure this is properly constructed or Avatars will be mismatched during gameplay");
            Destroy(gameObject);
            return;
        }
        avatarDictionary = new Dictionary<string, GameObject>();
        // Create the Dictionary and instantiate the prefabs which will be pooled
        for (int i = 0; i < AvatarID.Length; i++)
        {
            GameObject g = Instantiate(AvatarPrefabs[i]) as GameObject;
            g.name = AvatarID[i];
            g.transform.parent = uiGrid.transform;
            // Need to reset scale since anytime a gameobject is added to the GUI transform unity
            // Automatically accounts for the of the parent transform which is adjusted depending
            // on the viewing screen. 
            g.transform.localScale = new Vector3(1, 1, 1);
            // Add to the dictionary
            avatarDictionary.Add(AvatarID[i], g);
            g.SetActive(false);
        }

        _selCont = StateController.Instance.GetController<SelectController>();
        _selCont.SelectedListChanged += SelectionListChanged;
	}

    void SelectionListChanged(List<ActiveEntity> list)
    {
        if (list.Count > 0)
            GetComponent<TweenTransform>().PlayForward();
        else
            GetComponent<TweenTransform>().PlayReverse();

        // Deactivate all Avatars
        foreach (GameObject g in avatarDictionary.Values)
        {
            g.SetActive(false);
        }

        // Activate only selected avatars
        foreach (ActiveEntity a in list)
        {
            try
            {
                avatarDictionary[a.EntityID].SetActive(true);
            }
            catch (KeyNotFoundException ex)
            {
                Debug.LogError("This entity does not have an ID corresponding to any avatars");
            }
        }

        // Reposition the list
        uiGrid.Reposition();
    }
}
