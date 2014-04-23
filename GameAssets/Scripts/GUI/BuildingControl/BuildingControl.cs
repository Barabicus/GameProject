using UnityEngine;
using System.Collections;

public class BuildingControl : MonoBehaviour {

    public UILabel BuildingLabel;
    public Transform ContentArea;

    public ControlComponent BasicInfo;

    GameObject _prefabInstance;

    public Building ParentObject { get; set; }

    void Start()
    {
        BuildingLabel.text = ParentObject.GetComponent<BuildingInfo>().BuildingName;
        ControlComponent t = Instantiate(BasicInfo) as ControlComponent;
        t.BuildingControl = this;
        t.transform.parent = ContentArea;
        t.transform.localPosition = Vector3.zero;
        t.transform.localScale = new Vector3(1, 1, 1);
    }

    void AddTab()
    {

    }

}
