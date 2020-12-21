using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TMap : MonoBehaviour
{
    [SerializeField]
    private Tilemap tileMap;

    [SerializeField]
    private TileBase[] _tiles;

    private byte[] map;
    void Start()
    {
        map = MapData.Map.DeepClone();
        tileMap.size = new Vector3Int(6, 8, 0);
        LoadScreen(Vector2Int.one);
    }

    int offset(Vector2 pPoint)
    {
        return (int) (pPoint.x * 48 + pPoint.y * 48 * 8);
    }
    
    public void LoadScreen(Vector2Int pPoint)
    {
        int start = offset(pPoint);
        Vector3Int point = Vector3Int.zero;
        while (point.y < 6)
        {
            point.x = 0;
            while (point.x < 8)
            {
                tileMap.SetTile(point, _tiles[ map[start++]]);
                point.x++;
            }
            point.y++;
        }
    }

}
