using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardShuffler : Singleton<BoardShuffler>
{
    bool isShuffling = false;

    BoardManager board;
    private void OnEnable()
    {
        EventManager.Instance.OnShuffleBoard += ShuffleBoard;
    }


    private void OnDisable()
    {
        if (EventManager.Instance != null){
            EventManager.Instance.OnShuffleBoard -= ShuffleBoard;
        }
    }

    private void ShuffleBoard()
    {
        if (isShuffling){return;}
        isShuffling = true;
        // remove any normalPieces from m_allGamePieces and store them in a List
        board = BoardManager.Instance;
        var (normalPieces, locations) = RemoveNormalPieces();

        // shuffle the list of normal pieces
        ShuffleList(normalPieces);

        // use the shuffled list to fill the Board
        FillBoardFromList(normalPieces, locations);

        // find a match in pieces on board and makes them adjacent which prevents blind shuffling untill no deadlock
        AddGuaranteedMatch();

        // move the pieces to their correct onscreen positions
        MovePieces(locations);
        isShuffling = false;
    }

    private (List<GamePiece> normalPieces, List<(int, int)>) RemoveNormalPieces()
    {
        List<GamePiece> normalPieces = new List<GamePiece>();
        // x, y coordinates of the swaped pieces
        List<(int, int)> locations = new List<(int, int)>();
        // foreach position in the array...
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // ... if it's not a null object (i.e. a hole caused by Obstacle)
                if (board.m_allGamePieces[j, i] != null)
                {

                    // add to normalPieces list if gamePiece is a normal piece
                    if (board.m_allGamePieces[j, i].DataSO.pieceType == PieceType.normal)
                    {
                        normalPieces.Add(board.m_allGamePieces[j, i]);

                        // and clear position from original array
                        board.m_allGamePieces[j, i] = null;
                        locations.Add((i,j));
                    }
                }
            }
        }
        return (normalPieces, locations);
    }

    private void ShuffleList(List<GamePiece> piecesToShuffle)
    {
        int maxCount = piecesToShuffle.Count;

        // count up to maxCount minus 1 (last item has no other GamePieces to swap with)
        for (int i = 0; i < maxCount - 1; i++)
        {
            // generate a random number from current item to end of the list (note: maxCount is exclusive for integers)
            int r = Random.Range(i, maxCount);

            // if we have selected the current GamePiece, skip to next count
            if (r == i)
            {
                continue;
            }

            // swap the current items with the randomly selected item
            GamePiece temp = piecesToShuffle[r];

            piecesToShuffle[r] = piecesToShuffle[i];

            piecesToShuffle[i] = temp;

        }
    }

    public void FillBoardFromList(List<GamePiece> gamePieces, List<(int, int)> locations)
    {
        for(int i = 0; i < locations.Count; i++){
            board.m_allGamePieces[locations[i].Item2, locations[i].Item1] = gamePieces[i];
        }
    }
    private void AddGuaranteedMatch()
    {
        int x = 0;
        int y = 0;
        List<Color> checkedColors = new List<Color>();
        bool swappingPieceFound = false;
        int maxAttempts = 100;
        int maxAttempts2 = 100;
        while(!swappingPieceFound && maxAttempts > 0){
            bool pieceFound = false;
            while(!pieceFound && maxAttempts2 > 0){
                x = Random.Range(0,board.width);
                y = Random.Range(0,board.height);
                if(board.m_allGamePieces[y, x]!=null){
                    if(board.m_allGamePieces[y, x].DataSO.pieceType == PieceType.normal
                        && !checkedColors.Contains(board.m_allGamePieces[y, x].DataSO.color)
                        && CheckAdjacentPieces(x, y))
                    {
                        pieceFound = true;
                    }
                }
                maxAttempts2--;
            }

            if(!pieceFound){
                Debug.LogWarning("There are no piece left to match");
                return;
            }
            
            checkedColors.Add(board.m_allGamePieces[y, x].DataSO.color);

            for(int i = 0; i < board.width; i++){
                for(int j = 0; j < board.height; j++){
                    if(i == x && j == y){continue;}
                    if(board.m_allGamePieces[j, i] != null && 
                        board.m_allGamePieces[j, i].DataSO.color == board.m_allGamePieces[y, x].DataSO.color)
                    {
                        swappingPieceFound = true;
                        BringPieceNear(x, y, i, j);
                        return;
                    }
                }
            }
            maxAttempts--;
        }
        if (!swappingPieceFound)
        {
            Debug.LogWarning("Failed to find swapping piece");
        }
        
    }

    private bool CheckAdjacentPieces(int x, int y){

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            if(PieceManager.Instance.IsWithinBounds(x + dx[i], y + dy[i])){
                if (board.m_allGamePieces[y + dy[i], x + dx[i]]!=null 
                    && board.m_allGamePieces[y + dy[i], x + dx[i]].DataSO.pieceType == PieceType.normal)
                {
                    return true;
                }
            }
            
        }
        return false;
    }

    private void BringPieceNear(int firstX, int firstY, int secondX, int secondY){
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        bool isPlaced = false;
        int maxAttempts = 10;
        while(!isPlaced && maxAttempts > 0){
            int direction = Random.Range(0, 4);
            if(PieceManager.Instance.IsWithinBounds(firstX + dx[direction], firstY + dy[direction])){
                if(board.m_allGamePieces[firstY + dy[direction], firstX + dx[direction]]!=null){
                    GamePiece temp = board.m_allGamePieces[firstY + dy[direction], firstX + dx[direction]];
                    board.m_allGamePieces[firstY + dy[direction], firstX + dx[direction]] = board.m_allGamePieces[secondY ,secondX];
                    board.m_allGamePieces[secondY, secondX] = temp;
                    isPlaced = true;
                    Debug.Log($"Matched ({firstX}, {firstY}) with ({firstX + dx[direction]}, {firstY + dy[direction]})");
                }
            }
            maxAttempts--;
        }

        if (!isPlaced)
        {
            Debug.LogWarning("Failed to place the piece near the target position. Consider additional logic to handle this scenario.");
        }
    }


    private void MovePieces(List<(int, int)> locations)
    {
        
        for(int i = 0; i < locations.Count; i++){
            StartCoroutine(ShuffleMoveRoutine(locations[i].Item1, locations[i].Item2, 0.5f));
            board.m_allGamePieces[locations[i].Item2, locations[i].Item1].SetCoord(locations[i].Item1, locations[i].Item2);
        }
    }

    IEnumerator ShuffleMoveRoutine(int x, int y, float totalTimeToMove)
    {
		// we can change the start position later when the below piece is blasted while we are already moving
		// so we are storing the initial position of y in order to calculate bouncing power later
        GamePiece piece = board.m_allGamePieces[y, x];
    	Vector3 startPosition = piece.transform.position;
        Vector3 targetDestination = new Vector3(x, y, 0);
    	bool reachedDestination = false;

    	float elapsedTime = 0f;

    	piece.m_isMoving = true;

    	while (!reachedDestination)
    	{
    		// if we are close enough to destination
    		if (Vector3.Distance(piece.transform.position, targetDestination) < 0.01f)
    		{
    			reachedDestination = true;
    			piece.transform.position = targetDestination;
    			break;
    		}

    		// track the total running time
    		elapsedTime += Time.deltaTime;

    		// calculate the Lerp value
    		float t = Mathf.Clamp(elapsedTime / totalTimeToMove, 0f, 1f);
			// Smoother Step Function
			t =  t*t*t*(t*(t*6 - 15) + 10);

    		// move the game piece
    		piece.transform.position = Vector3.Lerp(startPosition, targetDestination, t);

    		// wait until next frame
    		yield return null;
    	}

    	piece.m_isMoving = false;
    }

}
