using UnityEngine;
using System.Collections;

public class BuildControlsGUIManager : MonoBehaviour {

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

	
}
