using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePiece : GamePiece , IDestructible
{
    public int health;
    public int Health => health;
    public override void Initialize()
    {
        health = DataSO.health;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health >= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = DataSO.pieceSprites[health];
        }
        else
        {
            ObjectPool.Instance.ReturnToPool(this.gameObject);
        }
    }
}
