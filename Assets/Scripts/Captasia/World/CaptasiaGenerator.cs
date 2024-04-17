using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Photon.Pun;

public class CaptasiaGenerator : MonoBehaviour
{
    #region Coroutine locks

    public bool generatedTiles = false;
    public bool spawnedPodiums = false;
    public bool spawnedRituals = false;
    public bool spawnedPoints = false;
    public bool spawnedChests = false;
    public bool spawnedTrees = false;

    const float MAX_TIMEOUT_TIME = 120;
    private float timeout_timer = 0.0f;

    #endregion

    #region Public Fields

    [Header("Shadow Caster Generator")]
    public GridShadowCastersGenerator shadowGenerator;

    [Header("Tilebases")]
    public List<TileBase> tileBases;

    [Header("Tilemap Components")]
    public Grid tilemapGrid;
    public Tilemap tilemap;

    [Header("Collidermap Components")]
    public Grid collidermapGrid;
    public Tilemap collidermap;

    [Header("List of tile prefabs")]
    public List<GameObject> tilePrefabs;
    public List<GameObject> wallPrefabs;

    #endregion

    #region Private Fields

    [Header("Private Fields")]
    
    // World Ground Tiles
    private List<Vector3Int> tiles = new List<Vector3Int>();

    // World Wall Tiles
    private List<Vector3Int> walls = new List<Vector3Int>();

    // Podiums
    private List<GameObject> podiums = new List<GameObject>();

    // Rituals
    private List<GameObject> rituals = new List<GameObject>();

    // Spawnpoints
    private List<GameObject> spawnPoints = new List<GameObject>();

    // Chests
    private List<GameObject> chests = new List<GameObject>();

    // Trees
    private List<GameObject> trees = new List<GameObject>();

    #endregion

    #region Generator Properties
    public int Seed;
    public int WorldSize;

    private static CaptasiaGenerator instance;

    public static CaptasiaGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CaptasiaGenerator>();
            }

            return instance;
        }
    }

    #endregion

    void Update()
    {
        timeout_timer += Time.deltaTime;
    }

    public void GenerateWorld()
    {
        if (!PhotonNetwork.IsConnected)
        {
            StartCoroutine(GenerateTerrain());
            return;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.WORLD_SEED, out object seedNum) &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CustomProperties.WORLD_SIZE, out object worldNum))
        {
            Seed = (int)seedNum;
            WorldSize = (int)worldNum;
        }
        else
        {
            Debug.LogWarning("Could not fetch room world seeds and sizes...");
        }

        StartCoroutine(GenerateTerrain());
    }

    /// <summary>
    /// Start Coroutines to spawn objective objects
    /// </summary>
    public void SpawnObjectives()
    {
        if (!generatedTiles)
            return;

        // Only Master Client will spawn key objects
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnPodiums());
            StartCoroutine(SpawnRituals());
            StartCoroutine(SpawnPoints());
            StartCoroutine(SpawnChests());
            StartCoroutine(SpawnTrees());
        }
    }

    /// <summary>
    /// This method will generate the terrain and walls in a match
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateTerrain()
    {
        Simplex.Noise.Seed = Seed;

        for (int x = -WorldSize; x <= WorldSize; x++)
        {
            for (int z = -WorldSize; z <= WorldSize; z++)
            {
                float noiseValue = Simplex.Noise.CalcPixel2D(x, z, 0.1f);
                int tileIndex = (int)noiseValue / (int)(256.0f / (tilePrefabs.Count + wallPrefabs.Count));

                Vector3Int tilePos = new Vector3Int(x, z, 0);

                if (x == -WorldSize || x == WorldSize || z == -WorldSize || z == WorldSize)
                {
                    collidermap.SetTile(tilePos, tileBases[1]);
                    walls.Add(tilePos);
                } 
                else if (tileIndex >= tilePrefabs.Count)
                {
                    collidermap.SetTile(tilePos, tileBases[1]);
                    walls.Add(tilePos);
                }
                else
                {
                    tilemap.SetTile(tilePos, tileBases[0]);
                    tiles.Add(tilePos);
                }
            }
        }

        yield return new WaitForEndOfFrame();

        shadowGenerator.Generate();
        this.transform.position = new Vector3(-0.5f, -0.5f, 0);

        generatedTiles = true;
    }

    /// <summary>
    /// This method will spawn the podiums into the match
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnPodiums()
    {
        while (!generatedTiles)
            yield return new WaitForSeconds(1f);

        int worldSize = 2 * (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.WORLD_SIZE];

        int spawnCountRequirement = 4 + (worldSize / 10);

        while (spawnCountRequirement > 0 && timeout_timer < MAX_TIMEOUT_TIME)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                if (Random.Range(0, 100) >= 99)
                {
                    bool tooClose = false;
                    foreach (GameObject podium in podiums)
                    {
                        if (Vector2.Distance(new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), podium.transform.position) <= worldSize / 5)
                        {
                            tooClose = true;
                        }
                    }

                    if (!tooClose)
                    {
                        GameObject podRef = PhotonNetwork.InstantiateRoomObject(CaptasiaResources.KeyObjectsPath.Podium,
                            new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), Quaternion.identity);

                        podiums.Add(podRef);
                        tiles.RemoveAt(i);
                        spawnCountRequirement--;
                    }

                    if (spawnCountRequirement <= 0)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        spawnedPodiums = true;
    }

    /// <summary>
    /// This method will spawn the rituals into the match
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnRituals()
    {
        while (!spawnedPodiums)
            yield return new WaitForSeconds(1f);

        int worldSize = 2 * (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.WORLD_SIZE];

        int spawnCountRequirement = 2 + (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.MAX_RITUAL_COUNTER_KEY];

        while (spawnCountRequirement > 0 && timeout_timer < MAX_TIMEOUT_TIME)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                if (Random.Range(0, 100) >= 99)
                {
                    bool tooClose = false;
                    foreach (GameObject ritual in rituals)
                    {
                        if (Vector2.Distance(new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), ritual.transform.position) < worldSize / 5)
                        {
                            tooClose = true;
                        }
                    }

                    if (!tooClose)
                    {
                        GameObject ritualRef = PhotonNetwork.InstantiateRoomObject(CaptasiaResources.KeyObjectsPath.Ritual,
                            new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), Quaternion.identity);

                        ritualRef.GetComponent<Ritual>().ritualType = (Ritual.RitualType) Random.Range(0, Ritual.RITUAL_TYPE_COUNT);

                        rituals.Add(ritualRef);
                        tiles.RemoveAt(i);
                        spawnCountRequirement--;
                    }

                    if (spawnCountRequirement <= 0)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        spawnedRituals = true;
    }

    private IEnumerator SpawnPoints()
    {
        while (!spawnedRituals)
            yield return new WaitForSeconds(1f);

        int worldSize = 2 * (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.WORLD_SIZE];

        int spawnCountRequirement = PhotonNetwork.CurrentRoom.PlayerCount;

        int spawnNum = 1;

        while (spawnCountRequirement > 0 && timeout_timer < MAX_TIMEOUT_TIME)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                if (Random.Range(0, 100) >= 99)
                {
                    bool tooClose = false;
                    foreach (GameObject spawnPoint in spawnPoints)
                    {
                        if (Vector2.Distance(new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), spawnPoint.transform.position) <= worldSize / 4)
                        {
                            tooClose = true;
                        }
                    }

                    if (!tooClose)
                    {
                        GameObject spawnRef = PhotonNetwork.InstantiateRoomObject(CaptasiaResources.KeyObjectsPath.SpawnPoint,
                            new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), Quaternion.identity);
                        spawnRef.GetComponent<SpawnPoint>().spawnPointNumber = spawnNum++;

                        spawnPoints.Add(spawnRef);
                        tiles.RemoveAt(i);
                        spawnCountRequirement--;
                    }

                    if (spawnCountRequirement <= 0)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        spawnedPoints = true;
    }

    /// <summary>
    /// This method will spawn the podiums into the match
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnChests()
    {
        while (!spawnedPoints)
            yield return new WaitForSeconds(1f);

        int worldSize = 2 * (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.WORLD_SIZE];

        int spawnCountRequirement = (worldSize / 4);

        while (spawnCountRequirement > 0 && timeout_timer < MAX_TIMEOUT_TIME)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                if (Random.Range(0, 100) >= 99)
                {
                    bool tooClose = false;
                    foreach (GameObject chest in chests)
                    {
                        if (Vector2.Distance(new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), chest.transform.position) <= worldSize / 5)
                        {
                            tooClose = true;
                        }
                    }

                    if (!tooClose)
                    {
                        GameObject chestRef = PhotonNetwork.InstantiateRoomObject(CaptasiaResources.KeyObjectsPath.Chest,
                            new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), Quaternion.identity);

                        chests.Add(chestRef);
                        tiles.RemoveAt(i);
                        spawnCountRequirement--;
                    }

                    if (spawnCountRequirement <= 0)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        spawnedChests = true;
    }

    /// <summary>
    /// This method will spawn trees
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnTrees()
    {
        while (!spawnedPoints)
            yield return new WaitForSeconds(1f);

        int worldSize = 2 * (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomProperties.WORLD_SIZE];

        int spawnCountRequirement = (worldSize / 2);

        while (spawnCountRequirement > 0 && timeout_timer < MAX_TIMEOUT_TIME)
        {
            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                if (Random.Range(0, 100) >= 99)
                {
                    bool tooClose = false;
                    foreach (Vector3Int wall in walls)
                    {
                        if (Vector2.Distance(new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), new Vector3(wall.x, wall.y, wall.z)) <= 2)
                        {
                            tooClose = true;
                        }
                    }

                    if (!tooClose)
                    {
                        GameObject treeRef = PhotonNetwork.InstantiateRoomObject(CaptasiaResources.KeyObjectsPath.TreePink,
                            new Vector3(tiles[i].x, tiles[i].y, tiles[i].z), Quaternion.identity);

                        trees.Add(treeRef);
                        tiles.RemoveAt(i);
                        spawnCountRequirement--;
                    }

                    if (spawnCountRequirement <= 0)
                    {
                        break;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        spawnedTrees = true;
    }
}

