using System.Collections.Generic;
using UnityEngine;

public class GamePieceFactory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SetUpPieces setUpPieces;
    [SerializeField] GameObject[] gamePiecePrefabs; // Assuming the prefabs are set in the inspector

    public GameObject CreateGamePiece(string itemName, int x, int y)
    {
        GameObject piece = null;
        foreach (var piecePrefab in gamePiecePrefabs){
            if(piecePrefab.GetComponent<GamePiece>().data.pieceName == itemName){
                piece = Instantiate(piecePrefab, new Vector3(x,y,0), Quaternion.identity);
            }
        }
        
        if(piece != null){
            piece.transform.parent = transform;
            GamePiece gamePiece = piece.GetComponent<GamePiece>();
            gamePiece.SetCoord(x,y);
            setUpPieces.m_allGamePieces[y, x] = gamePiece;
        }
        else{
            Debug.LogWarning("Item type doesn't match.");
        }
        
        return piece;
    }
}
