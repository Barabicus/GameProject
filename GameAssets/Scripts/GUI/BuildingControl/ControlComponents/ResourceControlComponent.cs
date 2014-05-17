using UnityEngine;
using System.Collections;

public class ResourceControlComponent : ControlComponent {

    public BinaryLabelBar wood;
    public BinaryLabelBar stone;
    public BinaryLabelBar meat;

    void Start()
    {
        wood.SecondLabel.text = ParentObject.Resource[ResourceType.Wood].ToString();
        stone.SecondLabel.text = ParentObject.Resource[ResourceType.Stone].ToString();
        meat.SecondLabel.text = ParentObject.Resource[ResourceType.Meat].ToString();
    }

    void Update()
    {
        wood.SecondLabel.text = ParentObject.Resource[ResourceType.Wood].ToString();
        stone.SecondLabel.text = ParentObject.Resource[ResourceType.Stone].ToString();
        meat.SecondLabel.text = ParentObject.Resource[ResourceType.Meat].ToString();
    }


}
