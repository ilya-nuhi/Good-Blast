using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public event Action<LevelData> OnLevelDataLoaded;
    public event Action<Tile> OnTileClicked;
    public event Action<GamePiece> OnPieceBlasted;
    public event Action<List<Tile>> OnCollapseAndRefill;
    public event Action OnShuffleBoard;

    public void LevelDataLoaded(LevelData levelData)
    {
        OnLevelDataLoaded?.Invoke(levelData);
    }

    public void TileClicked(Tile tile)
    {
        OnTileClicked?.Invoke(tile);
    }

    public void PieceBlasted(GamePiece piece)
    {
        OnPieceBlasted?.Invoke(piece);
    }

    public void CollapseAndRefill(List<Tile> emptyTiles)
    {
        OnCollapseAndRefill?.Invoke(emptyTiles);
    }

    public void ShuffleBoard()
    {
        OnShuffleBoard?.Invoke();
    }
}
