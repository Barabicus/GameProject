using UnityEngine;
using System.Collections;

public class ResourceControlComponent : ControlComponent {

    public UILabel textLabel;

    void Start()
    {

    }

    void Update()
    {
        textLabel.text = ParentObject.Resource.ToString();
    }

}
