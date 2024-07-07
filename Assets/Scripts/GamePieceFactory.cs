using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactory : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] gamePiecePrefabs; // Prefabs should be set in the inspector
    [SerializeField] GameObject[] particleFXPrefabs; // Prefabs should be set in the inspector

    public GameObject CreateGamePiece(string itemName, int x, int y)
    {
        GameObject piece = null;
        foreach (var piecePrefab in gamePiecePrefabs){
            if(piecePrefab.GetComponent<GamePiece>().DataSO.pieceName == itemName){
                piece = Instantiate(piecePrefab, new Vector3(x,y,0), Quaternion.identity);
                break;
            }
        }
        
        if(piece != null){
            piece.transform.parent = transform;
            GamePiece gamePiece = piece.GetComponent<GamePiece>();
            gamePiece.SetCoord(x,y);
            gamePiece.Initialize();
        }
        else{
            Debug.LogWarning("Item type doesn't match.");
        }
        
        return piece;
    }

    public GameObject CreateParticleFX(string itemName, int x, int y)
    {
        GameObject particleFX = null;
        foreach (var particlePrefab in particleFXPrefabs){
            if(particlePrefab.GetComponent<Discriminator>().discriminatorName == itemName){
                particleFX = Instantiate(particlePrefab, new Vector3(x,y,0), Quaternion.identity);
                break;
            }
        }
        
        if(particleFX != null){
            particleFX.transform.parent = transform;
        }
        else{
            Debug.LogWarning("Item type doesn't match.");
        }
        
        return particleFX;
    }
}
