using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public interface IDestructible
{
    int Health { get; }
    void TakeDamage(int amount);
}


public abstract class GamePiece : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GamePieceSO dataSO; // Assign this in the inspector
    public GamePieceSO DataSO => dataSO; // Public getter to access the dataSO
    [Header("Values")]
    public int xIndex;
    public int yIndex;
    public bool m_isMoving;
	private Vector3 targetDestination;
    public abstract void Initialize();

    public void SetCoord(int x, int y)
    {
        xIndex = x;
		yIndex = y;
    }

    public void DestroyPiece(){
        Destroy(gameObject);
    }

    public void Move (int destX, int destY, float timeToMove)
    {
		targetDestination = new Vector3(destX, destY, 0);
    	if (!m_isMoving)
    	{
    		StartCoroutine(MoveRoutine(timeToMove));	
    	}
		// else{
		// 	UpdateDestination(xIndex, yIndex);
		// }
    }

    IEnumerator MoveRoutine(float timeToMove)
    {
    	Vector3 startPosition = transform.position;

    	bool reachedDestination = false;

    	float elapsedTime = 0f;

    	m_isMoving = true;

    	while (!reachedDestination)
    	{
    		// if we are close enough to destination
    		if (Vector3.Distance(transform.position, targetDestination) < 0.01f)
    		{
    			reachedDestination = true;

				yield return StartCoroutine(bouncePieceRoutine(startPosition.y-targetDestination.y));

    			PieceManager.Instance.PlaceGamePiece(this, Mathf.RoundToInt(targetDestination.x), Mathf.RoundToInt(targetDestination.y));

    			break;
    		}

    		// track the total running time
    		elapsedTime += Time.deltaTime;

    		// calculate the Lerp value
    		float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

    		//t =  t*t*t*(t*(t*6 - 15) + 10);
			t = t*t;

    		// move the game piece
    		transform.position = Vector3.Lerp(startPosition, targetDestination, t);

    		// wait until next frame
    		yield return null;
    	}

    	m_isMoving = false;
		PieceManager.Instance.CheckPieceSprites();
    }

	public void UpdateDestination(int newDestX, int newDestY)
    {
		Debug.Log($"eski taş {this.gameObject.name} {newDestX} {newDestY} yerine gçeiyor.");
        targetDestination = new Vector3(newDestX, newDestY, 0);
    }

	IEnumerator bouncePieceRoutine(float fallDistance){
		yield return null;
	}


}
