using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PieceManager : Singleton<PieceManager>
{
    [Header("Rules")]
    [SerializeField] int A = 4;
    [SerializeField] int B = 7;
    [SerializeField] int C = 9;

    private void OnEnable()
    {
        EventManager.Instance.OnPieceBlasted += HandlePieceBlasted;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null){
            EventManager.Instance.OnPieceBlasted -= HandlePieceBlasted;
        }
    }

    public GamePiece GetPieceAt(int x, int y)
    {
        return BoardManager.Instance.m_allGamePieces[y, x];
    }

    private void HandlePieceBlasted(GamePiece clickedPiece)
    {
        List<GamePiece> matchingPieces = new List<GamePiece>();
        List<GamePiece> breakablePieces = new List<GamePiece>();

        (matchingPieces, breakablePieces) = FindAdjacentMatches(clickedPiece);

        if (matchingPieces.Count < 2) return;

        List<Tile> emptyTiles = GetTilesOfPieces(matchingPieces);
        emptyTiles.AddRange(BreakPieces(breakablePieces));

        ClearPieces(matchingPieces);

        // if (clickedPiece.GetComponent<SpriteRenderer>().sprite == clickedPiece.tntStateSprite)
        // {
        //     MakeTnt(clickedPiece.xIndex, clickedPiece.yIndex);
        // }

        // Notify collapse manager to collapse and refill columns
        EventManager.Instance.CollapseAndRefill(emptyTiles);
        CheckPieceSprites();
    }

    private (List<GamePiece>, List<GamePiece>) FindAdjacentMatches(GamePiece clickedPiece)
    {
        List<GamePiece> matchingPieces = new List<GamePiece>();
        List<GamePiece> breakablePieces = new List<GamePiece>();

        Queue<GamePiece> pieceQueue = new Queue<GamePiece>();
        pieceQueue.Enqueue(clickedPiece);
        matchingPieces.Add(clickedPiece);

        while (pieceQueue.Count != 0)
        {
            GamePiece piece = pieceQueue.Dequeue();
            if (piece.DataSO.pieceType == PieceType.normal)
            {
                CheckAndAddPiece(piece, piece.xIndex + 1, piece.yIndex, matchingPieces, breakablePieces, pieceQueue); // Right
                CheckAndAddPiece(piece, piece.xIndex - 1, piece.yIndex, matchingPieces, breakablePieces, pieceQueue); // Left
                CheckAndAddPiece(piece, piece.xIndex, piece.yIndex + 1, matchingPieces, breakablePieces, pieceQueue); // Up
                CheckAndAddPiece(piece, piece.xIndex, piece.yIndex - 1, matchingPieces, breakablePieces, pieceQueue); // Down
            }
        }

        return (matchingPieces, breakablePieces);
    }

    private void CheckAndAddPiece(GamePiece currentPiece, int x, int y, List<GamePiece> matchingPieces, List<GamePiece> breakablePieces, Queue<GamePiece> pieceQueue)
    {
        if (IsWithinBounds(x, y))
        {
            GamePiece adjacentPiece = BoardManager.Instance.m_allGamePieces[y, x];
            if (adjacentPiece != null && !adjacentPiece.m_isMoving)
            {
                if(!matchingPieces.Contains(adjacentPiece) && !breakablePieces.Contains(adjacentPiece)){
                    if (currentPiece.DataSO.color == adjacentPiece.DataSO.color )
                    {
                        pieceQueue.Enqueue(adjacentPiece);
                        matchingPieces.Add(adjacentPiece);
                    }
                    else if (adjacentPiece.DataSO.pieceType == PieceType.breakable)
                    {
                        breakablePieces.Add(adjacentPiece);
                    }
                }
                
            }
        }
    }

    bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < BoardManager.Instance.width && y >= 0 && y < BoardManager.Instance.height;
    }

    private void ClearPieces(List<GamePiece> gamePieces)
    {
        foreach (var piece in gamePieces)
        {
            if (piece != null)
            {
                BoardManager.Instance.m_allGamePieces[piece.yIndex, piece.xIndex] = null;
                ObjectPool.Instance.ReturnToPool(piece.gameObject);
            }
        }
    }

    private List<Tile> BreakPieces(List<GamePiece> breakablePieces)
    {
        List<Tile> clearedTiles = new List<Tile>();
        foreach(var piece in breakablePieces){
            if (piece != null && piece is IDestructible destructiblePiece){
                    if(destructiblePiece.Health > 0){
                        destructiblePiece.TakeDamage(1);
                    }
                    else{
                        clearedTiles.Add(BoardManager.Instance.m_allTiles[piece.yIndex, piece.xIndex]);
                        
                        BoardManager.Instance.m_allGamePieces[piece.yIndex, piece.xIndex] = null;
                        ObjectPool.Instance.ReturnToPool(piece.gameObject);
                    }
            }
                
            
        }
        return clearedTiles;
    }

    private List<Tile> GetTilesOfPieces(List<GamePiece> gamePieces)
    {
        List<Tile> tiles = new List<Tile>();
        foreach(var piece in gamePieces){
            if(!tiles.Contains(BoardManager.Instance.m_allTiles[piece.yIndex, piece.xIndex])){
                tiles.Add(BoardManager.Instance.m_allTiles[piece.yIndex, piece.xIndex]);
            }
        }
        return tiles;
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {
            Debug.LogWarning("BOARD:  Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            BoardManager.Instance.m_allGamePieces[y, x] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }

    public void CheckPieceSprites()
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for(int i = 0; i < BoardManager.Instance.width; i++){
            for(int j = 0; j < BoardManager.Instance.height; j++){
                GamePiece currentPiece = BoardManager.Instance.m_allGamePieces[j, i];
                if(currentPiece!=null && !currentPiece.m_isMoving &&currentPiece.DataSO.pieceType == PieceType.normal){
                    if(!gamePieces.Contains(currentPiece)){
                        List<GamePiece> matchingPieces = new List<GamePiece>();
                        List<GamePiece> breakablePieces = new List<GamePiece>();
                        (matchingPieces, breakablePieces) = FindAdjacentMatches(BoardManager.Instance.m_allGamePieces[j, i]);
                        if(matchingPieces.Count>A && matchingPieces.Count<=B){
                            foreach(var piece in matchingPieces){
                                piece.GetComponent<SpriteRenderer>().sprite = piece.DataSO.pieceSprites[1];
                            }
                        }
                        else if(matchingPieces.Count>B && matchingPieces.Count<=C){
                            foreach(var piece in matchingPieces){
                                piece.GetComponent<SpriteRenderer>().sprite = piece.DataSO.pieceSprites[2];
                            }
                        }
                        else if(matchingPieces.Count>C){
                            foreach(var piece in matchingPieces){
                                piece.GetComponent<SpriteRenderer>().sprite = piece.DataSO.pieceSprites[3];
                            }
                        }
                        else{   // Assigning default state sprites
                            foreach(var piece in matchingPieces){
                                piece.GetComponent<SpriteRenderer>().sprite = piece.DataSO.pieceSprites[0];
                            }
                        }
                        gamePieces = gamePieces.Union(matchingPieces).Union(breakablePieces).ToList();
                    }
                }
            }
        }
    }

}
