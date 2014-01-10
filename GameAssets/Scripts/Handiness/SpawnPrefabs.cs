using UnityEngine;
using System.Collections;

public class SpawnPrefabs : MonoBehaviour {

    public Transform prefab;

    public int x = 0;
    public int y = 0;
    public int frequency = 0;
    public float minHeight;
    public float maxHeight;

	// Use this for initialization
	void Start () 
    {
        Vector3 pos = transform.position;
        pos.y = 200;
        for (int cy = 0; cy < y; cy++)
        {
            for (int cx = 0; cx < x; cx++)
            {
                if ((Random.Range(0, frequency) + 1) == frequency)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(pos + new Vector3(cx, 0, cy), -Vector3.up), out hit))
                    {
                        if (!hit.collider.tag.Equals("Ground"))
                            continue;
                        if (hit.point.y < minHeight || hit.point.y > maxHeight)
                            continue;
                        Instantiate(prefab, hit.point, Quaternion.identity);
                    }
                }
            }
        }
	}
	
}
