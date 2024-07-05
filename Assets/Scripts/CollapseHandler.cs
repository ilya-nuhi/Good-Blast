using System.Collections.Generic;
using UnityEngine;

public class CollapseHandler : Singleton<CollapseHandler>
{
    private void OnEnable()
    {
        EventManager.Instance.OnCollapseAndRefill += CollapseRoutine;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null){
            EventManager.Instance.OnCollapseAndRefill -= CollapseRoutine;
        }
    }
    public void CollapseRoutine(List<Tile> emptyTiles)
    {
        List<int> columns = new List<int>();
        foreach (var tile in emptyTiles)
        {
            if (!columns.Contains(tile.X_index))
            {
                columns.Add(tile.X_index);
                CollapseColumn(tile.X_index);
                RefillColumn(tile.X_index);
            }
        }
    }

    private void CollapseColumn(int column, float collapseTime = 0.1f)
    {
        for (int i = 0; i < BoardManager.Instance.height - 1; i++)
        {
            if (BoardManager.Instance.m_allGamePieces[i, column] == null)
            {
                for (int j = i + 1; j < BoardManager.Instance.height; j++)
                {
                    if (BoardManager.Instance.m_allGamePieces[j, column] != null)
                    {
                        if(BoardManager.Instance.m_allGamePieces[j, column].DataSO.isMovable){

                            BoardManager.Instance.m_allGamePieces[j, column].Move(column, i, collapseTime * (j - i));
                            BoardManager.Instance.m_allGamePieces[i, column] = BoardManager.Instance.m_allGamePieces[j, column];
                            BoardManager.Instance.m_allGamePieces[i, column].SetCoord(column, i);
                            BoardManager.Instance.m_allGamePieces[j, column] = null;
                            break;
                        }
                        else{
                            // Non-movable piece encountered; stop further checks for this column
                            break;
                        }
                    }
                }
            }
        }
    }

    private void RefillColumn(int column, float refillTime = 0.1f)
    {
        int falseYOffset = 0;
        int blockerIndex = -1;
        for(int i = BoardManager.Instance.height-1; i >= 0; i--){
            if(BoardManager.Instance.m_allGamePieces[i, column]!=null){
                if (!BoardManager.Instance.m_allGamePieces[i, column].DataSO.isMovable){
                    blockerIndex = i;
                    break;
                }
            }
        }
        for (int i = blockerIndex+1; i < BoardManager.Instance.height; i++)
        {
            if (BoardManager.Instance.m_allGamePieces[i, column] == null)
            {
                RefillTile(column, i, falseYOffset, refillTime);
                falseYOffset++;
            }
        }
    }

    private void RefillTile(int x, int y, int falseYOffset, float refillTime = 0.1f)
    {
        string randItem = BoardManager.Instance.currentLevelData.rand_colors[Random.Range(0, BoardManager.Instance.currentLevelData.rand_colors.Length)];
        GameObject refillPiece = ObjectPool.Instance.GetFromPool(randItem, x, BoardManager.Instance.height + falseYOffset);
        refillPiece.transform.parent = ObjectPool.Instance.transform;
        GamePiece piece = refillPiece.GetComponent<GamePiece>();
        piece.Move(x, y, refillTime * (BoardManager.Instance.height + falseYOffset - y));
        piece.SetCoord(x, y);
        BoardManager.Instance.m_allGamePieces[y, x] = piece;
    }
}
