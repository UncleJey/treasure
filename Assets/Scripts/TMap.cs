using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TMap : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private Tile[] _tiles;
    // Start is called before the first frame update
    void Start()
    {
        map.size = new Vector3Int(10, 10, 0);
        map.SetTile(new Vector3Int(0,0,0), _tiles[0]);
        map.SetTile(new Vector3Int(1,0,0), _tiles[0]);
        map.SetTile(new Vector3Int(2,0,0), _tiles[0]);
        map.SetTile(new Vector3Int(0,1,0), _tiles[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
