using UnityEngine;
using System.Collections;

public class TileBrush : MonoBehaviour
{
    public Vector2 brushSize = Vector2.zero;
    public int tileID;
    public SpriteRenderer renderedSprite;


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, brushSize);


    }

    public void UpdateBrush(Sprite sprite)
    {
        renderedSprite.sprite = sprite;
    }
}