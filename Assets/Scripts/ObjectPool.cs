using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    private Dictionary<string, Queue<GameObject>> piecePoolDictionary;
    private Dictionary<string, Queue<GameObject>> particleFXPoolDictionary;

    [SerializeField] private GameObjectFactory gameObjectFactory;

    protected override void Awake()
    {
        base.Awake();
        piecePoolDictionary = new Dictionary<string, Queue<GameObject>>();
        particleFXPoolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    public GameObject GetPieceFromPool(string itemType, int x, int y)
    {
        if(itemType == "rand"){
            itemType = BoardManager.Instance.currentLevelData.rand_colors[Random.Range(0,BoardManager.Instance.currentLevelData.rand_colors.Length)];
        }
        if (piecePoolDictionary.ContainsKey(itemType) && piecePoolDictionary[itemType].Count > 0)
        {
            GameObject objectToReuse = piecePoolDictionary[itemType].Dequeue();
            objectToReuse.SetActive(true);
            objectToReuse.transform.position = new Vector3(x, y, 0);
            return objectToReuse;
        }
        else
        {
            return gameObjectFactory.CreateGamePiece(itemType, x, y);
        }
    }

    public void ReturnPieceToPool(GameObject objectToPool)
    {
        string itemType = objectToPool.GetComponent<GamePiece>().DataSO.pieceName;
        objectToPool.SetActive(false);
        objectToPool.GetComponent<SpriteRenderer>().sprite = objectToPool.GetComponent<GamePiece>().DataSO.pieceSprites[0];
        if (!piecePoolDictionary.ContainsKey(itemType))
        {
            piecePoolDictionary[itemType] = new Queue<GameObject>();
        }
        piecePoolDictionary[itemType].Enqueue(objectToPool);
    }

    public GameObject GetParticleFromPool(string itemType, int x, int y){
        if (particleFXPoolDictionary.ContainsKey(itemType) && particleFXPoolDictionary[itemType].Count > 0)
        {
            GameObject objectToReuse = particleFXPoolDictionary[itemType].Dequeue();
            objectToReuse.transform.position = new Vector3(x, y, 0);
            objectToReuse.SetActive(true);
            
            return objectToReuse;
        }
        else
        {
            return gameObjectFactory.CreateParticleFX(itemType, x, y);
        }
    }

    public void ReturnParticleToPool(GameObject objectToPool)
    {
        string itemType = objectToPool.GetComponent<Discriminator>().discriminatorName;
        objectToPool.SetActive(false);
        if (!particleFXPoolDictionary.ContainsKey(itemType))
        {
            particleFXPoolDictionary[itemType] = new Queue<GameObject>();
        }
        particleFXPoolDictionary[itemType].Enqueue(objectToPool);
    }


    private void OnDestroy()
    {
        foreach (var pool in piecePoolDictionary)
        {
            while (pool.Value.Count > 0)
            {
                GameObject obj = pool.Value.Dequeue();
                Destroy(obj);
            }
        }
    }
}
