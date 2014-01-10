using UnityEngine;
using System.Collections;

public class Crystal : MonoBehaviour {

    Animation anim;

	// Use this for initialization
	void Awake () {
        anim = gameObject.GetComponent<Animation>();
	}

    void OnSelect(string command)
    {
        if (command.Equals("play"))
        {
            anim.Play("Crystal");
        }
        else if (command.Equals("rewind"))
        {
            anim.Rewind("Crystal");
        }
    }
}
