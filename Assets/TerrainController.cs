﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using ProceduralToolkit;
using ProceduralToolkit.Examples;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainController : MonoBehaviour
{

    public GameObject chunk;
    public static LowPolyTerrainGenerator.Config config;
    public int sight = 3;
    public int desertSize = 30;
    private Dictionary<String, GameObject> chunks;
    public GameObject player;
    public Material greenLand;

    // Use this for initialization
    void Start ()
	{
	    config = new LowPolyTerrainGenerator.Config();
	    config.noiseInitial = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
	    config.heightScale = 8;
	    config.noiseScale = 1f;
        chunks = new Dictionary<string, GameObject>();

    }

    void UpdateTerrain()
    {
        Vector2Int center = new Vector2Int((int)(player.transform.position.x / config.terrainSize.x), (int)(player.transform.position.z / config.terrainSize.z));
        int xmin = -sight + center.x;
        int xmax = sight + center.x;
        int ymin = -sight + center.y;
        int ymax = sight + center.y;
        foreach (KeyValuePair<string, GameObject> keyValuePair in chunks.ToList())
        {
            int x = Convert.ToInt32(keyValuePair.Key.Substring(5, keyValuePair.Key.IndexOf("x") - 5));
            int y = Convert.ToInt32(keyValuePair.Key.Substring(keyValuePair.Key.IndexOf("x") + 1));
            if (!(x >= xmin && x <= xmax && y >= ymin && y <= ymax))
            {
                Destroy(keyValuePair.Value);
                chunks.Remove(keyValuePair.Key);
            }
        }
        for (int x = xmin; x <= xmax; x++)
        {
            for (int z = ymin; z <= ymax; z++)
            {
                if (!chunks.ContainsKey("Chunk" + x + "x" + z))
                    CreateChunk(x, z);
            }
        }
    }

    void CreateChunk(int x, int z)
    {
        GameObject newChunk = Instantiate(chunk, new Vector3(x * config.terrainSize.x, 0, z * config.terrainSize.z), new Quaternion(0, 0, 0, 1));
        newChunk.name = "Chunk" + x + "x" + z;
        config.terrainOffset = new Vector2(x, z);
        
        newChunk.GetComponent<MeshFilter>().sharedMesh = LowPolyTerrainGenerator.TerrainDraft(config).ToMesh();
        newChunk.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();

        newChunk.GetComponent<MeshCollider>().sharedMesh = newChunk.GetComponent<MeshFilter>().sharedMesh;

        if (x < -desertSize || x > desertSize || z < -desertSize || z > desertSize)
            newChunk.GetComponent<Renderer>().material = greenLand;

        chunks[newChunk.name] = newChunk;
    }

    public GameObject GetCurrentChunk()
    {
        Vector2Int center = new Vector2Int((int)(player.transform.position.x / config.terrainSize.x), (int)(player.transform.position.z / config.terrainSize.z));
        return chunks["Chunk" + center.x + "x" + center.y];
    }

    public bool ReachedGreenLand()
    {
        Vector2Int center = new Vector2Int((int)(player.transform.position.x / config.terrainSize.x), (int)(player.transform.position.z / config.terrainSize.z));
        return (center.x < -desertSize || center.x > desertSize || center.y < -desertSize || center.y > desertSize);
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateTerrain();

    }
}

