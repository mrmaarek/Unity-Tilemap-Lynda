using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
    public TileMap map;

    private TileBrush brush;
    private Vector3 mouseHitPosition;
                                              
    bool mouseOnMap
    {
        get
        {
            return  mouseHitPosition.x > 0 && 
                    mouseHitPosition.x < map.gridSize.x && 
                    mouseHitPosition.y < 0 && 
                    mouseHitPosition.y > -map.gridSize.y;
        }
    }
    
	public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //EditorGUILayout.LabelField("Our Custom Editor!");
        EditorGUILayout.BeginVertical();

        var oldSize = map.mapSize;
        map.mapSize = EditorGUILayout.Vector2Field("Map Size", map.mapSize);

        if (map.mapSize != oldSize)
        {
            ReCalculate();
        }


        map.texture2D = (Texture2D)EditorGUILayout.ObjectField("Texture 2D:", map.texture2D, typeof(Texture2D), false);

        if (map.texture2D == null)
        {
            EditorGUILayout.HelpBox("You haven't selected a Tilemap/2D Texture yet.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.LabelField("Tile Size:", map.tileSize.x+" x "+map.tileSize.y);
            EditorGUILayout.LabelField("Grid Size in Units:", map.gridSize.x + " x " + map.gridSize.y);
            EditorGUILayout.LabelField("Pixels to Units:", map.pixelsToUnits.ToString());

            //Update the Brush from the Tilemap
            UpdateBrush(map.currentTileBrush);
        }

        EditorGUILayout.EndVertical();
    }

    void OnEnable()
    {
        map = target as TileMap;
        Tools.current = Tool.View;

        if (map.texture2D != null)
        {
            ReCalculate();
            NewBrush();
        }
    }

    void OnDisable()
    {
        DestroyBrush();
    }

    void OnSceneGUI()
    {
        if (brush != null)
        {
            UpdateHitPosition();
            MoveBrush();
        }
    }

    void ReCalculate()
    {

        var path = AssetDatabase.GetAssetPath(map.texture2D);
        map.spriteReferences = AssetDatabase.LoadAllAssetsAtPath(path);

        var sprite = (Sprite)map.spriteReferences[1];
        var width = sprite.textureRect.width;
        var height = sprite.textureRect.height;

        map.tileSize = new Vector2(width, height);

        map.pixelsToUnits = (int)(sprite.rect.width / sprite.bounds.size.x);

        map.gridSize = new Vector2((width / map.pixelsToUnits) * map.mapSize.x, (height / map.pixelsToUnits) * map.mapSize.y);
    }

    void CreateBrush()
    {
        // reference to the sprite from the tilemap 
        Sprite sprite = map.currentTileBrush;
        if (sprite != null)
        {
            // Create a new gameobject for the brush
            GameObject go = new GameObject("Brush");
            // Set the tilemap to it's parent
            go.transform.SetParent(map.transform);
            // Reference to the brush
            brush = go.AddComponent<TileBrush>();
            // Add a spriterenderer to the GO
            brush.renderedSprite = go.AddComponent<SpriteRenderer>();
            // Reference to the Brushsize based on the pixels to units from the Tilemap.
            int pixelsToUnits = map.pixelsToUnits;
            
            // Calculate the size of the brush based on P to U and the size of the sprite.
            brush.brushSize = new Vector2(sprite.textureRect.width / pixelsToUnits,
                                         sprite.textureRect.height / pixelsToUnits);

            brush.UpdateBrush(sprite); 
        }
    }

    // If there ain't a brush, create one.
    void NewBrush()
    {
        if (brush == null)
        {
            CreateBrush();
        }
    }

    // Destroy the brush (the editor way)
    void DestroyBrush()
    {
        if (brush != null)
        {
            // Destroy the brushes gameObject.
            DestroyImmediate(brush.gameObject); 
        }
    }

    public void UpdateBrush(Sprite sprite)
    {
        if (brush != null)
        {
            brush.UpdateBrush(sprite);
        }
    }

    private void UpdateHitPosition()
    {
        var p = new Plane(map.transform.TransformDirection(Vector3.forward), Vector3.zero);
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        var hit = Vector3.zero;
        var distance = 0f;

        if (p.Raycast(ray, out distance))
        {
            hit = ray.origin + ray.direction.normalized * distance;
            mouseHitPosition = map.transform.InverseTransformPoint(hit);
        }
    }

    private void MoveBrush()
    {
        var tileSize = map.tileSize.x / map.pixelsToUnits;
        var x = Mathf.Floor(mouseHitPosition.x / tileSize) * tileSize;
        var y = Mathf.Floor(mouseHitPosition.y / tileSize) * tileSize;

        var row = x / tileSize;
        var column = Mathf.Abs(y / tileSize) - 1;

        // Unique id
        if (!mouseOnMap)
        {
            return;
        }

        var id = (int)((column * map.mapSize.x) + row);

        brush.tileID = id;

        x += map.transform.position.x + tileSize / 2;
        y += map.transform.position.y + tileSize / 2;

        brush.transform.position = new Vector3(x, y, map.transform.position.z);
    }
}
