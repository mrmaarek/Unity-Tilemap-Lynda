using UnityEngine;
using System.Collections;


public class TileMap : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(25, 25);
    public Texture2D texture2D;
    public Vector2 tileSize = new Vector2();

    public Object[] spriteReferences;

    public Vector2 gridSize = new Vector2();

    public int pixelsToUnits = 100;
    public int tileID = 0;

    public GameObject tiles;

    //Get the current tile brush
    public Sprite currentTileBrush
    {
        // Sprite reference based on the tile id, returned/casted as a Sprite
        get { return spriteReferences[tileID] as Sprite; }
    }

    void Start()
    { 
    
    }

    void Update()
    { 
    
    }

    void OnDrawGizmosSelected()
    {
        var position = transform.position;

        if (texture2D != null)
        {
            Gizmos.color = Color.grey;
            var row = 0;
            var maxColumns = mapSize.x;
            var total = mapSize.x * mapSize.y;
            var tile = new Vector3(tileSize.x / pixelsToUnits, tileSize.y / pixelsToUnits);
            var offset = new Vector2(tile.x / 2, tile.y / 2);

            for (var i = 0; i < total; i++)
            {
                var column = i % maxColumns;
                var newX = (column * tile.x) + offset.x + position.x;
                var newY = -(row * tile.y) - offset.y + position.y;

                Gizmos.DrawWireCube(new Vector2(newX, newY), tile);

                if (column == maxColumns - 1)
                {
                    row++;
                }
            }

            Gizmos.color = Color.white;
            var centerX = position.x + (gridSize.x / 2);
            var centerY = position.y - (gridSize.y / 2);

            Gizmos.DrawWireCube(new Vector2(centerX, centerY), gridSize);
        }
    }
}
