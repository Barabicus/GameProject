using UnityEngine;
using System.Collections;

public class MaterialColorChanger : MonoBehaviour {

    public delegate void ChangeMaterialColor(Color c);
    public event ChangeMaterialColor ColorChanged;

    public void ChangeColor(Color c)
    {
        if (ColorChanged != null)
            ColorChanged(c);
    }
}
