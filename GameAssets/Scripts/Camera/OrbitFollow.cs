using UnityEngine;
using System.Collections;

public class OrbitFollow : MonoBehaviour {

    public Transform camera;
    public float distance = 10.0f;

	// Use this for initialization
	void Start () {
        transform.position = camera.position + camera.forward * distance;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = camera.position + camera.forward * distance;
	}
}
