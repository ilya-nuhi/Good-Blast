using System;
using UnityEngine;

public interface IDestructible
{
    void TakeDamage(int amount);
}

public abstract class GamePiece : MonoBehaviour
{
    [Header("References")]
    public GamePieceSO data;
    [Header("Values")]
    public int xIndex;
    public int yIndex;

    public abstract void Initialize();

    public void LogPieceType()
    {
        Debug.Log($"This piece is of type: {data.pieceType}");
    }

    public void SetCoord(int x, int y)
    {
        xIndex = x;
		yIndex = y;
    }
}
