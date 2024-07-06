using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SetUpGrid : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject tilePrefab;
    //[SerializeField] GameObject groundPrefab;
    [SerializeField] GameObject gridFrame;

    [Header("Values")]
    public int width;
    public int height;

    void OnEnable()
    {
        EventManager.Instance.OnLevelDataLoaded += OnLevelDataLoaded;
    }

    void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnLevelDataLoaded -= OnLevelDataLoaded;
        }
    }

    private void OnLevelDataLoaded(LevelData levelData)
    {
        width = levelData.grid_width;
        height = levelData.grid_height;
        BoardManager.Instance.m_allTiles = new Tile[height, width];

        SetupTiles();
        SetupCamera();
    }


    void SetupTiles()
    {
        for (int i = 0; i < width ; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (BoardManager.Instance.m_allTiles[j, i] == null)
                {
                    MakeTile(tilePrefab, i, j);
                }
            }
        }
        //setting gridbackground
        float gridPosX = (BoardManager.Instance.m_allTiles[0,width-1].transform.position.x + BoardManager.Instance.m_allTiles[0,0].transform.position.x)/2;
        float gridPosY = (BoardManager.Instance.m_allTiles[height-1,0].transform.position.y + BoardManager.Instance.m_allTiles[0,0].transform.position.y)/2;
        GameObject gridBG = Instantiate(gridFrame, new Vector3(gridPosX, gridPosY, 0), Quaternion.identity);
        gridBG.GetComponent<SpriteRenderer>().size = new Vector2(width+0.4f,height+0.4f);
        //setting ground
        // GameObject ground = Instantiate(groundPrefab, new Vector3(gridPosX, -1, 0), Quaternion.identity);
        // ground.name = "ground";
        // ground.transform.parent = transform;
        // ground.GetComponent<BoxCollider2D>().size = new Vector2(BoardManager.Instance.width, 1);
    }

    void MakeTile(GameObject prefab, int x, int y, int z = 0)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
            tile.name = "Tile (" + x + "," + y + ")";
            BoardManager.Instance.m_allTiles[y, x] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            BoardManager.Instance.m_allTiles[y, x].Init(x, y);
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
