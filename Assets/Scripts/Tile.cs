using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int X_index {get; private set;}
    public int Y_index {get; private set;}
    

    public void Init(int x, int y)
	{
        
		X_index = x;
		Y_index = y;
	}

	void OnMouseDown()
	{
		EventManager.Instance.TileClicked(this);
	}
}
