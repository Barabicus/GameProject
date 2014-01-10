using UnityEngine;
using System.Collections;

public class ShowHighlight : MonoBehaviour
{

    private Transform highlight;

    void Awake()
    {
        highlight = transform.FindChild("_highlightTransform");
        if (highlight == null)
            Debug.LogWarning(gameObject.name + "'s highlight transform has not been set!");
        highlight.gameObject.SetActive(false);
    }


    public void OnMouseEnter()
    {
        highlight.gameObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        highlight.gameObject.SetActive(false);
    }

}
