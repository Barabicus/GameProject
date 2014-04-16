using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
internal class ResourceUnitTests : UnityUnitTest
{
    [Test]
    public void AddTest()
    {
        GameObject go = CreateGameObject();
        go.AddComponent<Resource>();
        Resource r = go.GetComponent<Resource>();
        r.Awake();
        r.maxWeight = 10;
        r.AddResource(ResourceType.Wood, 6);
        Assert.That(r.CurrentWeight == 10 && r[ResourceType.Wood] == 5);
    }

    [Test]
    public void WeightTest()
    {
        GameObject go = CreateGameObject();
        go.AddComponent<Resource>();
        Resource r = go.GetComponent<Resource>();
        r.Awake();
        r.maxWeight = 10;
        r.AddResource(ResourceType.Meat, 1);
        r.AddResource(ResourceType.Stone, 1);
        r.AddResource(ResourceType.Wood, 1);
        Assert.That(r.CurrentWeight == 8);
    }

    [Test]
    public void LeftOverTest()
    {
        GameObject go = CreateGameObject();
        go.AddComponent<Resource>();
        Resource r = go.GetComponent<Resource>();
        r.Awake();
        r.maxWeight = 10;
        int leftOver = r.AddResource(ResourceType.Wood, 3);
        leftOver = r.AddResource(ResourceType.Stone, 5);
        Assert.That(r[ResourceType.Wood] == 3 && r[ResourceType.Stone] == 0 && r.CurrentWeight == 6 && leftOver == 5);
    }
    
    [Test]
    public void TransferTest()
    {
        GameObject one = CreateGameObject();
        GameObject two = CreateGameObject();
        one.AddComponent<Resource>();
        two.AddComponent<Resource>();
        Resource r1 = one.GetComponent<Resource>();
        Resource r2 = two.GetComponent<Resource>();
        r1.Awake();
        r2.Awake();
        r1.maxWeight = 10;
        r2.maxWeight = 3;
        r1.AddResource(ResourceType.Wood, 3);
        r1.AddResource(ResourceType.Meat, 5);
        r2.TransferResources(r1, ResourceType.Wood, 1);
        r2.TransferResources(r1, ResourceType.Meat, 5);
        Assert.That(r1[ResourceType.Wood] == 2 && r1[ResourceType.Meat] == 3 && r2[ResourceType.Wood] == 1 && r2[ResourceType.Meat] == 1);
    }

}
