using UnityEngine;
using System.Collections;

public class Skip : MonoBehaviour {

    public LoadLevel loadLevel;
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            loadLevel.LoadScene();
        }
    }
}
