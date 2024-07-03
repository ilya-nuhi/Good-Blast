using System.Collections;
using UnityEngine;

public enum PieceType {
    normal,
    box,
    bomb
}

public enum Color{
    colorless,
    red,
    green,
    blue,
    yellow,
    purple,
    pink
}

[CreateAssetMenu(fileName = "GamePieceData", menuName = "ScriptableObjects/GamePieceData", order = 1)]
public class GamePieceSO : ScriptableObject
{
    public string pieceName;
    public PieceType pieceType;
    public Color color;
    public Sprite[] pieceSprites;
    public int health;
}