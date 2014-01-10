using UnityEngine;
using System.Collections;

public class ShowRim : MonoBehaviour {

    public Transform materialTransform;

    public void Start()
    {
        if (renderer != null)
            materialTransform = transform;

        materialTransform.renderer.material.SetFloat("_RimPower", RimSelectProperties.instance.rimPower);
        materialTransform.renderer.material.SetColor("_RimColor", RimSelectProperties.instance.nonSelectColor);
    }

    public void OnMouseEnter()
    {
        materialTransform.renderer.material.SetColor("_RimColor", RimSelectProperties.instance.selectedColor);
    }

    public void OnMouseExit()
    {
        materialTransform.renderer.material.SetColor("_RimColor", RimSelectProperties.instance.nonSelectColor);
    }

}
