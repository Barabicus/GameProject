using UnityEngine;
using System.Collections;

public class ProjectorRotate : MonoBehaviour {

    public float rotateSpeed = 1f;
	
	// Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotateSpeed);
    }

    
}
