using System;
using System.Collections;
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

		if (!m_isMoving)
		{
			StartCoroutine(MoveRoutine(new Vector3(destX, destY,0), timeToMove));	
		}
	}

    IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
	{
		Vector3 startPosition = transform.position;

		bool reachedDestination = false;

		float elapsedTime = 0f;

		m_isMoving = true;

		// if(xIndex!=destination.x || yIndex!=destination.y){
		// 	destination.x = xIndex;
		// 	destination.y = yIndex;
		// }

		while (!reachedDestination)
		{
			// if we are close enough to destination
			if (Vector3.Distance(transform.position, destination) < 0.01f)
			{
				reachedDestination = true;
				
				PieceManager.Instance.PlaceGamePiece(this, (int) destination.x, (int) destination.y);

				break;
			}

			// track the total running time
			elapsedTime += Time.deltaTime;

			// calculate the Lerp value
			float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);
			
			t =  t*t*t*(t*(t*6 - 15) + 10);
					
			// move the game piece
			transform.position = Vector3.Lerp(startPosition, destination, t);

			// wait until next frame
			yield return null;
		}

		m_isMoving = false;
	}


    
}
