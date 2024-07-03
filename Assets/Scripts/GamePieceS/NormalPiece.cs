using UnityEngine;

public class NormalPiece : GamePiece
{
    public override void Initialize()
    {
        Debug.Log($"Initializing {data.pieceType} piece with color {data.color}");
    }
}
