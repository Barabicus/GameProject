using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {

    public string sceneName = "";

    public void LoadScene()
    {
        if (sceneName.Equals(""))
        {
            Debug.LogError("No Scene name set");
            return;
        }
        Application.LoadLevel(sceneName);
    }

}
