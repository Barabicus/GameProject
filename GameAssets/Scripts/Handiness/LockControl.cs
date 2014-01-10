using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MouseLook))]
public class LockControl : MonoBehaviour {

    void Start()
    {
        Screen.lockCursor = true;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.T))
        {
            Screen.lockCursor = !Screen.lockCursor;
            gameObject.GetComponent<MouseLook>().enabled = !gameObject.GetComponent<MouseLook>().enabled;
        }
	}
}
