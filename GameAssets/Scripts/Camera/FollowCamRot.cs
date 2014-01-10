using UnityEngine;
using System.Collections;

public class FollowCamRot : MonoBehaviour {

    public Transform camToFollow;
	
	void LateUpdate () {
        transform.rotation = camToFollow.rotation;
	}
}
