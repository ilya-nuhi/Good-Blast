using System;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public GamePiece[,] m_allGamePieces;
    public Tile[,] m_allTiles;
    public LevelData currentLevelData;
    public int width;
    public int height;

    //private bool canMove = true;

    private void OnEnable()
    {
        EventManager.Instance.OnTileClicked += HandleTileClicked;
        EventManager.Instance.OnLevelDataLoaded += HandleLevelDataLoaded;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null){
            EventManager.Instance.OnTileClicked -= HandleTileClicked;
            EventManager.Instance.OnLevelDataLoaded -= HandleLevelDataLoaded;
        }
    }

    private void HandleTileClicked(Tile tile)
    {
        GamePiece clickedPiece = PieceManager.Instance.GetPieceAt(tile.X_index, tile.Y_index);
        if (clickedPiece != null)
        {
            if (clickedPiece.DataSO.pieceType == PieceType.normal && !clickedPiece.m_isMoving)
            {
                EventManager.Instance.PieceBlasted(clickedPiece);
            }
        }
    }

    private void HandleLevelDataLoaded(LevelData data)
    {
        currentLevelData = data;
        width = data.grid_width;
        height = data.grid_height;
    }

}
