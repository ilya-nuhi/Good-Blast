using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    [SerializeField] private GamePieceFactory gamePieceFactory;

    public override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    public GameObject GetFromPool(string itemType, int x, int y)
    {
        if(itemType == "rand"){
            itemType = BoardManager.Instance.currentLevelData.rand_colors[Random.Range(0,BoardManager.Instance.currentLevelData.rand_colors.Length)];
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
        string itemType = objectToPool.GetComponent<GamePiece>().DataSO.pieceName;
        objectToPool.SetActive(false);
        objectToPool.GetComponent<SpriteRenderer>().sprite = objectToPool.GetComponent<GamePiece>().DataSO.pieceSprites[0];
        if (!poolDictionary.ContainsKey(itemType))
        {
            poolDictionary[itemType] = new Queue<GameObject>();
        }
        poolDictionary[itemType].Enqueue(objectToPool);
    }

    private void OnDestroy()
    {
        foreach (var pool in poolDictionary)
        {
            while (pool.Value.Count > 0)
            {
                GameObject obj = pool.Value.Dequeue();
                Destroy(obj);
            }
        }
    }
}
