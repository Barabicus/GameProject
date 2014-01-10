using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MouseOrbitCSharp))]
public class CameraOrbitControl : MonoBehaviour {

    public Transform orbitTransform;
    MouseOrbitCSharp orbit;

	// Use this for initialization
	void Awake () {
        orbit = GetComponent<MouseOrbitCSharp>();
        // Ensure orbit begins as inactive
        orbit.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                orbitTransform.position = hit.point;
                orbit.distance = hit.distance;
                orbit.enabled = true;
            }
            return;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            orbit.enabled = false;
        }
	}
}
