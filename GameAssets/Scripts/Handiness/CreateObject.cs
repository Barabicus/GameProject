using UnityEngine;
using System.Collections;

public class CreateObject : MonoBehaviour {

    public Transform prefab;
    public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(2))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Transform g = (Transform)Instantiate(prefab, hit.point, Quaternion.identity);
                g.GetComponent<AIPath>().target = this.target;
            }
        }
	}
}
