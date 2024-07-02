using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Board m_board;

    [Header("Values")]
    public int xIndex;
    public int yIndex;
    

    public void Init(int x, int y)
	{
        
		xIndex = x;
		yIndex = y;
	}

	void OnMouseDown()
	{
		if (m_board !=null)
		{
			m_board.ClickTile(this);
		}
	}
}
