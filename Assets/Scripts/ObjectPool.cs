using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SetUpPieces setUpPieces;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    [SerializeField] private GamePieceFactory gamePieceFactory;

    void Awake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    public GameObject GetFromPool(string itemType, int x, int y)
    {
        if(itemType == "rand"){
            itemType = setUpPieces.currentLevel.rand_colors[Random.Range(0,setUpPieces.currentLevel.rand_colors.Length)];
        }
        if (poolDictionary.ContainsKey(itemType) && poolDictionary[itemType].Count > 0)
        {
            GameObject objectToReuse = poolDictionary[itemType].Dequeue();
            objectToReuse.SetActive(true);
            objectToReuse.transform.position = new Vector3(x, y, 0);
            return objectToReuse;
        }
        else
        {
            return gamePieceFactory.CreateGamePiece(itemType, x, y);
        }
    }

    public void ReturnToPool(GameObject objectToPool)
    {
        string itemType = objectToPool.GetComponent<GamePiece>().data.pieceName;
        objectToPool.SetActive(false);
        if (!poolDictionary.ContainsKey(itemType))
        {
            poolDictionary[itemType] = new Queue<GameObject>();
        }
        poolDictionary[itemType].Enqueue(objectToPool);
    }
}
