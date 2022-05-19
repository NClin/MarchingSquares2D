using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class MarchingTiles : MonoBehaviour
{   
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    [SerializeField] private float threshold;
    [SerializeField] private GameObject marker;
    [SerializeField] private GameObject tileBase;
    [SerializeField] private GameObject tileSprite0000;
    [SerializeField] private GameObject tileSprite1000;
    [SerializeField] private GameObject tileSprite1001;
    [SerializeField] private GameObject tileSprite0111;
    [SerializeField] private GameObject tileSprite1111;
    [SerializeField] private GameObject tileSprite0101;
    [SerializeField] private GameObject tileSprite1010;

    private Hashtable tilesHash;
    private Hashtable baseRotationsHash;
    private MarchingSquares marchingSquares;
    private float[,] initValues;
    [ItemCanBeNull] private GameObject[,] placedTiles;
    
    void Start()
    {
        initValues = new float[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                initValues[x, y] = Random.Range(0, 1f);
                if (initValues[x, y] > 0.5f)
                {
                    PlaceMarker(new Vector2(x, y), Color.red);
                }
                else
                {
                    PlaceMarker(new Vector2(x, y), Color.blue);
                }
            }
        }

        placedTiles = new GameObject?[xSize, ySize];
        
        marchingSquares = new MarchingSquares(initValues);

        tilesHash = ConstructTilesHash();
        baseRotationsHash = ConstructBaseRotationsHash();
        
        UpdateAllTiles();
    }

    void UpdateAllTiles()
    {
        for (int x = 0; x < xSize - 1; x++)
        {
            for (int y = 0; y < ySize - 1; y++)
            {
                UpdateTile(new Vector2Int(x, y));
            }
        }
    }

    void PlaceMarker(Vector2 position, Color color)
    {
        // -0.5 to make up for tile pivot being center.
        Vector3 pos3 = new Vector3(position.x - 0.5f, position.y - 0.5f, 0);
        var toPlace = Instantiate(marker, pos3, quaternion.identity);
        toPlace.GetComponent<SpriteRenderer>().color = color;
    }
    
    Hashtable ConstructBaseRotationsHash()
    {
        Hashtable rotationsHash = new Hashtable();
        rotationsHash.Add("0000", tileSprite0000);
        rotationsHash.Add("1000", tileSprite1000);
        rotationsHash.Add("1001", tileSprite1001);
        rotationsHash.Add("0111", tileSprite0111);
        rotationsHash.Add("1111", tileSprite1111);
        rotationsHash.Add("0101", tileSprite0101);
        rotationsHash.Add("1010", tileSprite1010);

        return rotationsHash;
    }

    GameObject GetTile(MarchingSquare square)
    {
        GameObject tile = Instantiate(tileBase, this.transform);
        tile.SetActive(false);

        // look up tile string
        string tileString = (string)tilesHash[square.hashString()];

        // construct tile from string - base + sprite
        GameObject rotationSprite = (GameObject)baseRotationsHash[tileString.Substring(0, 4)];
        
        int rotation = Int32.Parse(tileString[4].ToString());
        var spriteObject = Instantiate(rotationSprite, tile.transform);
        spriteObject.transform.Rotate(0, 0, -90 * rotation);

        return tile;
    }

    void UpdateTile(Vector2Int marchingPoint)
    {
        ClearTile(marchingPoint);
        
        var square = marchingSquares.GetSquareFor(marchingPoint, threshold);
        var toPlace = GetTile(square);
        toPlace.transform.position = new Vector3(marchingPoint.x, marchingPoint.y, 0);
        float averageVal = marchingSquares.GetSquareAverageValue(marchingPoint);
        toPlace.SetActive(true);
        placedTiles[marchingPoint.x, marchingPoint.y] = toPlace;
        toPlace.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.green, Color.red, averageVal);
    }

    void ClearTile(Vector2Int marchingPoint)
    {
        var prevTile = placedTiles[marchingPoint.x, marchingPoint.y];
        if (prevTile != null)
        {
            placedTiles[marchingPoint.x, marchingPoint.y] = null;
            Destroy(prevTile);
        }
    }

    ///
    /// Here is how retrieving the tile will work:
    /// There is a hash of squares which includes one of two tiles and a rotation.
    /// The tile is returned as a gameobject (ready to spawn, be a collider, etc).
    /// Then I can make properties and scripts on the tile as a prefab, and build
    /// an architecture around that.
    ///

    // dictionary will contain each configuration, corresponding tile, and rotation.

    // e.g.:

    // key: 0100, value: 01001
    // means - key 0100 gets 0100 with one 90 degree clockwise rotation.

    // this determines the sprite and collider?

    private Hashtable ConstructTilesHash()
    {
        var outHash = new Hashtable();
        outHash.Add("0000", "00000");
        outHash.Add("0001", "10003");
        outHash.Add("0010", "10002");
        outHash.Add("0011", "10013");
        outHash.Add("0100", "10001");
        outHash.Add("0101", "01010"); // Diagonal. controversial. Perhaps make a rotation of each other.
        outHash.Add("0110", "10012");
        outHash.Add("0111", "01110");
        outHash.Add("1000", "10000");
        outHash.Add("1001", "10010");
        outHash.Add("1010", "10100"); // Diagonal. controversial.
        outHash.Add("1011", "01111");
        outHash.Add("1100", "10011");
        outHash.Add("1101", "01112");
        outHash.Add("1110", "01113");
        outHash.Add("1111", "11110");

        return outHash;
    }

    void SetClosestMarchingPoint(Vector2 pos, float value, float threshold = 0)
    {
        var closest = marchingSquares.GetClosestPoint(pos, threshold);
        closest.Value = value;
        marchingSquares.SetValue(new Vector2Int(closest.gridPosition.x, closest.gridPosition.y), 0);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("collision");
        
        SetClosestMarchingPoint(col.transform.position, 0, 0.5f);
        UpdateAllTiles();
        
    }
}
