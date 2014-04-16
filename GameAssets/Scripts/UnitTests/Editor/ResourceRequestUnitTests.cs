using UnityEngine;
using System.Collections;

public class ResourceRequestUnitTests : UnityUnitTest {

    public void RequestTests()
    {
        IslandManager islandManager;
        CityManager cityManager;
        Building building;
        GameObject CityManagerObject = CreateGameObject();
        GameObject IslandManagerObject = CreateGameObject();
        GameObject BuildingObject = CreateGameObject();
        IslandManagerObject.AddComponent<IslandManager>();
        CityManagerObject.AddComponent<CityManager>();
        BuildingObject.AddComponent<SimpleGenericJobBuilding>();
        islandManager = IslandManagerObject.GetComponent<IslandManager>();
        cityManager = CityManagerObject.GetComponent<CityManager>();
        building = BuildingObject.GetComponent<SimpleGenericJobBuilding>();

        islandManager.cityManager = cityManager;
        building.transform.parent = islandManager.transform;
        cityManager.Awake();
        cityManager.Start();
        building.Awake();
        building.Start();
        BuildingResourceRequestManager manage;
    }

}
