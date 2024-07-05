using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public event Action<LevelData> OnLevelDataLoaded;
    public event Action<Tile> OnTileClicked;
    public event Action<GamePiece> OnPieceBlasted;
    public event Action<GamePiece> OnTntTriggered;
    public event Action<List<Tile>> OnCollapseAndRefill;

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

    public void TntTriggered(GamePiece piece)
    {
        OnTntTriggered?.Invoke(piece);
    }

    public void CollapseAndRefill(List<Tile> emptyTiles)
    {
        OnCollapseAndRefill?.Invoke(emptyTiles);
    }
}