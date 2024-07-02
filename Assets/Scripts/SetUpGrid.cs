using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SetUpGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject gridFrame;

    [Header("Values")]
    public int width;
    public int height;

    Tile[,] m_allTiles;
    void Start()
    {
        StartCoroutine(WaitForLevelData());
    }

    IEnumerator WaitForLevelData()
    {
        // Wait until the level data is loaded
        while (levelLoader.currentLevelData == null)
        {
            yield return null;
        }

        // Now that the data is loaded, set up the grid
        GetGridInfo();
        SetupTiles();
        SetupCamera();
    }

    private void GetGridInfo()
    {
        Debug.Log(levelLoader.currentLevelData.grid_width);
        width = levelLoader.currentLevelData.grid_width;
        height = levelLoader.currentLevelData.grid_height;
        m_allTiles = new Tile[width, height];
        Debug.Log(width);
    }

    void SetupTiles()
    {
        for (int i = 0; i < width ; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allTiles[i, j] == null)
                {
                    MakeTile(tilePrefab, i, j);
                }
            }
        }
        //setting gridbackground
        float gridPosX = (m_allTiles[width-1,0].transform.position.x + m_allTiles[0,0].transform.position.x)/2;
        float gridPosY = (m_allTiles[0,height-1].transform.position.y + m_allTiles[0,0].transform.position.y)/2;
        GameObject gridBG = Instantiate(gridFrame, new Vector3(gridPosX, gridPosY, 0), Quaternion.identity);
        gridBG.GetComponent<SpriteRenderer>().size = new Vector2(width+0.4f,height+0.4f);
    }

    void MakeTile(GameObject prefab, int x, int y, int z = 0)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            tile.name = "Tile (" + x + "," + y + ")";
            m_allTiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            m_allTiles[x, y].Init(x, y);
        }
    }

    bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height*1.3f - 1) / 2f, -10f);

        float aspectRatio = (float)Screen.width / (float)Screen.height;

        float verticalSize = (float)height * 1.6f / 2f;

        float horizontalSize = ((float)width * 1.3f / 2f ) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }
}
