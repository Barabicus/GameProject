using UnityEngine;
using System.Collections;

public class BuildControlsGUIManager : MonoBehaviour {

    public Camera UICamera;

    private static BuildControlsGUIManager _instance;
    private GameObject _currentControlBox;

    public GameObject CurrentControlBox
    {
        get { return _currentControlBox; }
        set
        {
            if (_currentControlBox != null)
                _currentControlBox.SetActive(false);
            _currentControlBox = value;
            value.SetActive(true);
        }
    }

    public static BuildControlsGUIManager Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        _instance = this;
    }

    public static GameObject AddBuildingControl(GameObject control)
    {
        GameObject go = Instantiate(control) as GameObject;
        go.transform.parent = _instance.gameObject.transform;
        go.SetActive(false);
        go.transform.localScale = new Vector3(1, 1, 1);
        return go;
    }

    public static void SetCurrentAndPosition(GameObject control, Vector3 controlPosition)
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(controlPosition);
        pos.z = 0;
        control.transform.position = pos;
        _instance.CurrentControlBox = control;
    }
	
}
