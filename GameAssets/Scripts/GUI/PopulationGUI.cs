using UnityEngine;
using System.Collections;

public class PopulationGUI : MonoBehaviour {

    public UILabel maxLabel;
    public UILabel currentLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        maxLabel.text = PlayerManager.Instance.MaxPopulation + "";
        currentLabel.text = PlayerManager.Instance.CurrentPopulation + "";
	}
}
