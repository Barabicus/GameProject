using UnityEngine;
using System.Collections;

public class BuildingInfo : MonoBehaviour {

    public string BuildingName;
    public int RequiredBuildUnits;
    public ResourceType[] requiredResources;
    public int[] requiredResourceAmount;
    public BuildingPlaceController.PlaceType placeType = BuildingPlaceController.PlaceType.Single;

    /// <summary>
    /// Creates a new BuildingInfo with from the references passed in via the info variable
    /// </summary>
    /// <param name="info"></param>
    public void CopyFromOther(BuildingInfo info)
    {
        // It is easier to just have a single info parameter as if we ever need to add additional info
        // everything can be easily changed here rather than having to change the variables passed in
        // throughout the entire code base of anything that calls this method
        this.BuildingName = info.BuildingName;
        this.RequiredBuildUnits = info.RequiredBuildUnits;
        this.requiredResources = info.requiredResources;
        this.requiredResourceAmount = info.requiredResourceAmount;
        this.placeType = info.placeType;

    }

}
