using UnityEngine;
using System.Collections;

public class BuildingGUI : MonoBehaviour
{

    public UIGrid buildingGrid;
    public Transform buildingGridPrefab;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < BuildingList.Instance.buildings.Length; i++)
        {
            Transform t = Instantiate(buildingGridPrefab) as Transform;
            t.parent = buildingGrid.transform;
            t.localScale = new Vector3(1, 1, 1);
            t.localPosition = Vector3.zero;
            UILabel label = t.transform.FindChild("label").GetComponent<UILabel>();
            label.text = BuildingList.Instance.BuildingBlueprintPrefabs[i].GetComponent<BuildingInfo>().BuildingName;


            Transform _blueprintRef = BuildingList.Instance.BuildingBlueprintPrefabs[i];
            Transform _previewRef = BuildingList.Instance.BuildingPreviewPrefabs[i];
            
            EventDelegate.Add(t.GetComponent<UIButton>().onClick, () =>
            {
                StateController.Instance.ControllerState = ControllerState.Building;
                // Set the building display
                StateController.Instance.GetController<BuildingPlaceController>().Blueprint = _blueprintRef;
                StateController.Instance.GetController<BuildingPlaceController>().BuildingPreview = _previewRef;

            });
        }
        buildingGrid.Reposition();
    }

}
