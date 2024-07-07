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
	public string particleFXName;	// Assign this in the inspector
    public GamePieceSO DataSO => dataSO; // Public getter to access the dataSO
    [Header("Values")]
    public int xIndex;
    public int yIndex;
    public bool m_isMoving;
	private Vector3 targetDestination;
	private Vector3 startPosition;
	private float totalTimeToMove;
    public abstract void Initialize();

    public void SetCoord(int x, int y)
    {
        xIndex = x;
		yIndex = y;
    }

    public void DestroyPiece(){
        Destroy(gameObject);
    }

    public void Move (int destX, int destY, float timeToMove = 0.2f)
    {	
		float addedDistance = targetDestination.y-destY;
		targetDestination = new Vector3(destX, destY, 0);
		totalTimeToMove = timeToMove * (transform.position.y - targetDestination.y);
    	if (!m_isMoving)
    	{
    		StartCoroutine(MoveRoutine());	
    	}
		else{
			totalTimeToMove += timeToMove*addedDistance*0.75f;
			startPosition = transform.position;
		}
    }

    IEnumerator MoveRoutine()
    {
		// we can change the start position later when the below piece is blasted while we are already moving
		// so we are storing the initial position of y in order to calculate bouncing power later
    	startPosition = transform.position;
		float initialStartY = startPosition.y;

    	bool reachedDestination = false;

    	float elapsedTime = 0f;

    	m_isMoving = true;

    	while (!reachedDestination)
    	{
    		// if we are close enough to destination
    		if (Vector3.Distance(transform.position, targetDestination) < 0.01f)
    		{
    			reachedDestination = true;

				yield return StartCoroutine(BouncePieceRoutine(initialStartY-targetDestination.y));

    			transform.position = targetDestination;
    			break;
    		}

    		// track the total running time
    		elapsedTime += Time.deltaTime;

    		// calculate the Lerp value
    		float t = Mathf.Clamp(elapsedTime / totalTimeToMove, 0f, 1f);
			// ease-in function to give the gravity feeling 
			t = t*t*t;

    		// move the game piece
    		transform.position = Vector3.Lerp(startPosition, targetDestination, t);

    		// wait until next frame
    		yield return null;
    	}

    	m_isMoving = false;
		PieceManager.Instance.CheckSpritesAndDeadlock();
    }

	IEnumerator BouncePieceRoutine(float fallDistance)
    {
        // Calculate bounce height and number of bounces
        float bounceHeight = fallDistance * 0.1f; // Adjust this multiplier for desired bounce height
        float bounceDuration = 0.1f + bounceHeight * 0.1f; // Duration of each bounce

		float startTime = Time.time;
		Vector3 bounceStart = transform.position;
		Vector3 bounceEnd = new Vector3(bounceStart.x, bounceStart.y + bounceHeight, 0);

		while (Time.time < startTime + bounceDuration)
		{
			float t = (Time.time - startTime) / bounceDuration;
			t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease out
			transform.position = Vector3.Lerp(bounceStart, bounceEnd, t);
			yield return null;
		}

		startTime = Time.time;
		bounceStart = transform.position;
		bounceEnd = new Vector3(bounceStart.x, targetDestination.y, 0);

		while (Time.time < startTime + bounceDuration)
		{
			float t = (Time.time - startTime) / bounceDuration;
			t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f); // Ease in
			transform.position = Vector3.Lerp(bounceStart, bounceEnd, t);
			yield return null;
		}
        
    }


}
