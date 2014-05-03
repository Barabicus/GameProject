using UnityEngine;
using System.Collections;

public class CloseCurrentBox : MonoBehaviour {

    public void Close()
    {
        BuildControlsGUIManager.Instance.CurrentControlBox = null;
    }

}
