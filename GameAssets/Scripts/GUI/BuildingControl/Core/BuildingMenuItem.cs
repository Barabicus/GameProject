using UnityEngine;
using System.Collections;

public class BuildingMenuItem : MonoBehaviour {

    public string MenuName;
    public Transform MenuPrefab;


    void Start()
    {
        UILabel label = transform.Find("MenuName").GetComponent<UILabel>();
        label.text = this.MenuName;
    }

}
