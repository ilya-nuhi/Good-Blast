using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpPieces : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectPool objectPool;
    [Header("Values")]
    public GamePiece[,] m_allGamePieces;
    
    private int width;
    private int height;
    public LevelData currentLevel;
    

    void OnEnable()
    {
        LevelLoader.LevelDataLoaded += OnLevelDataLoaded;
    }

    void OnDisable()
    {
        LevelLoader.LevelDataLoaded -= OnLevelDataLoaded;
    }

    private void OnLevelDataLoaded(LevelData levelData)
    {
        width = levelData.grid_width;
        height = levelData.grid_height;
        currentLevel = levelData;
        m_allGamePieces = new GamePiece[height,width];
        SetupGamePieces();
    }

    void SetupGamePieces()
    {
        if (currentLevel != null)
        {
            for (int y = 0; y < currentLevel.grid_height; y++)
            {
                for (int x = 0; x < currentLevel.grid_width; x++)
                {
                    if (m_allGamePieces[y, x] == null)
                    {
                        if (currentLevel.grid[y][x] != null)
                        {
                            PlaceGamePiece(currentLevel.grid[y][x], x, y);
                        }
                    }
                }
            }
            
        }
    }

    void PlaceGamePiece(string itemType, int x, int y)
    {
        GameObject piece = objectPool.GetFromPool(itemType, x, y);

        if (piece != null)
        {
            piece.transform.parent = transform;
            GamePiece gamePiece = piece.GetComponent<GamePiece>();
            gamePiece.SetCoord(x, y);
            m_allGamePieces[y, x] = gamePiece;
        }
    }

}
