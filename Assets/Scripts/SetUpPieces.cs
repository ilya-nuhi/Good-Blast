using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpPieces : MonoBehaviour
{
    
    private int width;
    private int height;
    public LevelData currentLevel;
    

    void OnEnable()
    {
        EventManager.Instance.OnLevelDataLoaded += OnLevelDataLoaded;
    }

    void OnDisable()
    {
        if (EventManager.Instance != null){
            EventManager.Instance.OnLevelDataLoaded -= OnLevelDataLoaded;
        }
    }

    private void OnLevelDataLoaded(LevelData levelData)
    {
        width = levelData.grid_width;
        height = levelData.grid_height;
        currentLevel = levelData;
        BoardManager.Instance.m_allGamePieces = new GamePiece[height,width];
        SetupGamePieces();
        PieceManager.Instance.CheckPieceSprites();
    }

    void SetupGamePieces()
    {
        if (currentLevel != null)
        {
            for (int y = 0; y < currentLevel.grid_height; y++)
            {
                for (int x = 0; x < currentLevel.grid_width; x++)
                {
                    if (BoardManager.Instance.m_allGamePieces[y, x] == null)
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
        GameObject piece = ObjectPool.Instance.GetPieceFromPool(itemType, x, y);

        if (piece != null)
        {
            piece.transform.parent = ObjectPool.Instance.transform;
            GamePiece gamePiece = piece.GetComponent<GamePiece>();
            gamePiece.SetCoord(x, y);
            BoardManager.Instance.m_allGamePieces[y, x] = gamePiece;
        }
    }

}
