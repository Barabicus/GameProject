using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SampleTerrain : MonoBehaviour
{

    const int size = 30; // the diameter of terrain portion that will raise under the game object

    void Start()
    {

        StartCoroutine(TerrainTest());
    }

    IEnumerator TerrainTest()
    {
        while (true)
        {
            foreach (Terrain t in Terrain.activeTerrains)
            {
                ModifyTerrain(t);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {

    }

    void ModifyTerrain(Terrain terr)
    {
        int hmWidth = terr.terrainData.heightmapWidth, hmHeight = terr.terrainData.heightmapHeight;
        int posXInTerrain; // position of the game object in terrain width (x axis)
        int posYInTerrain; // position of the game object in terrain height (z axis)

        // get the normalized position of this game object relative to the terrain
        Vector3 tempCoord = (transform.position - terr.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / terr.terrainData.size.x;
        coord.y = tempCoord.y / terr.terrainData.size.y;
        coord.z = tempCoord.z / terr.terrainData.size.z;


        // we set an offset so that all the raising terrain is under this game object
        int offset = size / 2;

        // get the position of the terrain heightmap where this game object is
        posXInTerrain = (int)(coord.x * hmWidth) - offset;
        posYInTerrain = (int)(coord.z * hmHeight) - offset;

        if (posXInTerrain > terr.terrainData.heightmapWidth || posYInTerrain > terr.terrainData.heightmapHeight || (posXInTerrain + size < 0) || (posYInTerrain + size < 0) )
            return;

        int sizeHeight, sizeWidth;
        float[,] heights = getHeights(ref posXInTerrain, ref posYInTerrain, out sizeWidth, out sizeHeight, terr);

        // we set each sample of the terrain in the size to the desired height
        for (int i = 0; i < sizeHeight; i++)
        {
            for (int j = 0; j < sizeWidth; j++)
            {
                heights[i, j] = heights[i, j] + 0.1f * Time.deltaTime;
            }
        }

        // set the new height
        int xpos = posXInTerrain;
        int ypos = posYInTerrain;

        if (xpos <= terr.terrainData.heightmapWidth && ypos <= terr.terrainData.heightmapHeight)
            terr.terrainData.SetHeights(xpos, ypos, heights);
    }

    public float[,] getHeights(ref int xpos,ref int ypos, out int sizeWidth, out int sizeHeight, Terrain terr)
    {
        sizeWidth = (xpos + size) < terr.terrainData.heightmapWidth ? xpos < 0 ? xpos + size : size : size - ((xpos + size) - terr.terrainData.heightmapWidth);
        sizeHeight = (ypos + size) < terr.terrainData.heightmapHeight ? ypos < 0 ? ypos + size : size : size - ((ypos + size) - terr.terrainData.heightmapHeight);
        xpos = xpos < 0 ? 0 : xpos;
        ypos = ypos < 0 ? 0 : ypos;
        return terr.terrainData.GetHeights(xpos, ypos, sizeWidth, sizeHeight);
    }


}
