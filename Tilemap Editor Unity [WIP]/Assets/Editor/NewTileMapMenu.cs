using UnityEngine;
using System.Collections;
using UnityEditor;

public class NewTileMapMenu
{
    [MenuItem("GameObject/Tile Map")]
    public static void CreateTileMap()
    {
        //Debug.Log("Create a new tile map");
        GameObject go = new GameObject("Tilemap");
        go.AddComponent<TileMap>();
    }
}