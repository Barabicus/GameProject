using UnityEngine;
using System.Collections;

public class DetailsGUI : MonoBehaviour {

    public UILabel lifeForce;

	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        lifeForce.text = PlayerManager.Instance.LifeForceParsed;
	}
}
