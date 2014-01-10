using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public float forwardSpeed = 5.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
            transform.position += transform.forward * Input.GetAxis("Vertical") * forwardSpeed * Time.deltaTime;
            transform.position += transform.right * Input.GetAxis("Horizontal") * forwardSpeed * Time.deltaTime;
            transform.position += transform.up * Input.GetAxis("PanUpDown") * forwardSpeed * Time.deltaTime;
	}
}
