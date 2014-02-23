using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SampleTerrain : MonoBehaviour
{

    const int size = 35; // the diameter of terrain portion that will raise under the game object
    float[,] lastHeight;
    int lastXpos, lastYpos;
    Dictionary<Terrain, float[,]> resetHeights;

    void Start()
    {
        resetHeights = new Dictionary<Terrain, float[,]>();
        foreach (Terrain t in Terrain.activeTerrains)
        {
            resetHeights.Add(t, t.terrainData.GetHeights(0, 0, t.terrainData.heightmapWidth, t.terrainData.heightmapHeight));
        }
        StartCoroutine(TerrainTest());
    }

    IEnumerator TerrainTest()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, Mathf.Infinity, 1 << 9))
            {
                if (hit.collider.tag.Equals("Ground"))
                {
                    try
                    {
                        ModifyTerrain(hit.collider.GetComponent<Terrain>());
                    }
                    catch(UnityException ex)
                    {
                        Debug.Log("Caught Exception: " + ex.Message);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void ModifyTerrain(Terrain terr)
    {
        int hmWidth = terr.terrainData.heightmapWidth, hmHeight = terr.terrainData.heightmapHeight;
        int posXInTerrain; // position of the game object in terrain width (x axis)
        int posYInTerrain; // position of the game object in terrain height (z axis)
        float desiredHeight = 0; // the height we want that portion of terrain to be

        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terr.terrainData.size.x;
        coord.y = tempCoord.y / terr.terrainData.size.y;
        coord.z = tempCoord.z / terr.terrainData.size.z;

        // get the position of the terrain heightmap where this game object is
        posXInTerrain = (int)(coord.x * hmWidth);
        posYInTerrain = (int)(coord.z * hmHeight);

        // we set an offset so that all the raising terrain is under this game object
        int offset = size / 2;

        // get the heights of the terrain under this game object
        float[,] heights = terr.terrainData.GetHeights(posXInTerrain - offset, posYInTerrain - offset, size, size);
        lastHeight = heights;

        // we set each sample of the terrain in the size to the desired height
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                heights[i, j] = heights[i, j] + Time.deltaTime;
            }
        }

        // go raising the terrain slowly
        desiredHeight += Time.deltaTime * 0.5f;

        // set the new height
        int xpos = posXInTerrain - offset;
        int ypos = posYInTerrain - offset;
        lastXpos = xpos;
        lastYpos = ypos;

        if (xpos <= terr.terrainData.heightmapWidth && ypos <= terr.terrainData.heightmapHeight)
            terr.terrainData.SetHeights(xpos, ypos, heights);
    }


}
