using UnityEngine;
using System.Collections;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ResourceRequestUnitTests : UnityUnitTest {

    [Test]
    public void RequestTests()
    {
        CityManager cityManager;
        GameObject CityManagerObject = CreateGameObject("Unit Test");
        CityManagerObject.AddComponent<CityManager>();
        cityManager = CityManagerObject.GetComponent<CityManager>();

        cityManager.Awake();
        cityManager.Start();
        Building b = Substitute.For<Building>();
        b.CityManager.Returns(cityManager);
   
        BuildingResourceRequestManager manager = new BuildingResourceRequestManager(b);

        Debug.Log(manager.Building.CityManager.name);
    }

}
