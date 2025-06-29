using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class Generation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject cornerPrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject placeholderPredab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject doorPrefab;

    [Header("Settings")]
    [SerializeField] private bool generateOnStart = true;
    public int Width = 10;
    public int Length = 10;
    public float segmentSize = 10f;

    public bool centerBuilding = true;

    [Header("Настройки заполнения")]
    [SerializeField] private bool generateTile = true;

    [Header("Cписки")]
    private List<GameObject> generatedOutline = new List<GameObject>();
    private List<GameObject> generatedTiles = new List<GameObject>();
    private List<GameObject> generatedPlaceholder = new List<GameObject>();
    private List<GameObject> generatedFloors = new List<GameObject>();
    public List<GameObject> generatedDoors = new List<GameObject>();
    private List<Animator> doorAnimators = new List<Animator>();

    private Animator animator;

    [SerializeField] private NavMeshSurface surface;
    private void Start()
    {

        if (generateOnStart)
        {
            GenerateCompleteBuilding();
            BakeNavMesh();
        }
    }

    public void OpenDoor(int doorIndex)
    {
        Animator animator = doorAnimators[doorIndex];
        animator.SetTrigger("OpenDoor");
    }

    public void GenerateCompleteBuilding()
    {
        ClearPreviousGeneration();
        GenerateOutline();
        if (generateTile) GenerateTiledInterior();

        // Добавляем генерацию предметов
        ItemGenerator itemGenerator = GetComponent<ItemGenerator>();
        if (itemGenerator != null)
        {
            itemGenerator.GenerateItems();
        }
    }
    private void GenerateOutline()
    {
        float totalWidth = Width * segmentSize;
        float totalLength = Length * segmentSize;
        Vector3 centerOffset = centerBuilding ?
            new Vector3(-totalWidth / 2f, 0, -totalLength / 2f) :
            Vector3.zero;

        // Углы
        Vector3[] corners = new Vector3[4]
        {
            new Vector3(0, 0, 0) + centerOffset,
            new Vector3(totalWidth, 0, 0) + centerOffset,
            new Vector3(totalWidth, 0, totalLength) + centerOffset,
            new Vector3(0, 0, totalLength) + centerOffset
        };

        for (int i = 0; i < 4; i++)
        {
            Vector3 cornerPos = corners[i];
            Quaternion rotation = Quaternion.Euler(0, -90 * i + 90, 0);
            GameObject corner = Instantiate(cornerPrefab, cornerPos, rotation, transform);
            generatedOutline.Add(corner);
            Debug.DrawLine(cornerPos, cornerPos + Vector3.up * 5f, Color.red, 5f);
        }

        // Стены
        for (int i = 1; i < Width; i++)
        {
            float xPos = i * segmentSize;

            Vector3 frontWallPos = new Vector3(xPos, 0, 0) + centerOffset;
            GameObject frontWall = Instantiate(wallPrefab, frontWallPos, Quaternion.Euler(0, 180, 0), transform);
            generatedOutline.Add(frontWall);

            Vector3 backWallPos = new Vector3(xPos, 0, totalLength) + centerOffset;
            GameObject backWall = Instantiate(wallPrefab, backWallPos, Quaternion.identity, transform);
            generatedOutline.Add(backWall);
        }

        for (int i = 1; i < Length; i++)
        {
            float zPos = i * segmentSize;

            Vector3 leftWallPos = new Vector3(0, 0, zPos) + centerOffset;
            GameObject leftWall = Instantiate(wallPrefab, leftWallPos, Quaternion.Euler(0, -90, 0), transform);
            generatedOutline.Add(leftWall);

            Vector3 rightWallPos = new Vector3(totalWidth, 0, zPos) + centerOffset;
            GameObject rightWall = Instantiate(wallPrefab, rightWallPos, Quaternion.Euler(0, 90, 0), transform);
            generatedOutline.Add(rightWall);
        }
    }

    private void GenerateTiledInterior()
    {
        float totalWidth = Width * segmentSize;
        float totalLength = Length * segmentSize;
        Vector3 centerOffset = centerBuilding ?
            new Vector3(-totalWidth / 2f, 0, -totalLength / 2f) : Vector3.zero;

        // Количество тайлов = размер в сегментах - 1
        int tileCountX = Width - 1;
        int tileCountZ = Length - 1;

        // Начальная позиция первого тайла (половина сегмента от угла)
        Vector3 startPos = new Vector3(segmentSize / 2f, 0, segmentSize / 2f) + centerOffset;

        if (generateTile)
        {
            CreateTileLayer(startPos, tileCountX, tileCountZ, 0f, "Tile_");
        }
    }

    private void CreateTileLayer(Vector3 startPos, int countX, int countZ, float height, string namePrefix)
    {
        int centerBuildingX = countX / 2;
        int centerBuildingZ = countZ / 2;

        //Генерация тайлов
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 tilePos = startPos + new Vector3(x * segmentSize, height, z * segmentSize);

                GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, transform);
                tile.transform.position = tilePos + new Vector3(5, 0, 5);
                tilePos = tilePos + new Vector3(5, 0, 5);
                tile.name = namePrefix + x + "_" + z;
                generatedTiles.Add(tile);
            }
        }

        //Генерация заполнителя
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 tilePos = startPos + new Vector3(x * segmentSize + 5, height, z * segmentSize + 5);

                for (int i = 0; i < UnityEngine.Random.Range(0, 3); i++)
                {
                    Vector3 placeholderPos = tilePos;

                    if (Math.Abs(x - centerBuildingX) <= 1 && Math.Abs(z - centerBuildingZ) <= 1)
                    {
                        continue;
                    }


                    else
                    {
                        {
                            GameObject placeholder = Instantiate(placeholderPredab, placeholderPos, Quaternion.identity, transform);
                            placeholder.transform.rotation = Quaternion.Euler(0, 90 * UnityEngine.Random.Range(0, 4), 0);
                            placeholder.name = "Placeholder_ " + x + "_" + z; 
                            generatedPlaceholder.Add(placeholder);
                        }
                    }
                }
            }
        }

        //Генерация полов
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 tilePos = startPos + new Vector3(x * segmentSize + 5, height, z * segmentSize + 5);

                if (x == centerBuildingX && z == centerBuildingZ)
                {
                    continue;
                }

                else
                {
                    Vector3 floorPos = tilePos;
                    GameObject floor = Instantiate(floorPrefab, floorPos, Quaternion.identity, transform);
                    floor.name = "Floor_" + x + "_" + z;
                    generatedFloors.Add(floor);
                }
            }
        }

        //Генерация дверей
        for (int x = 0; x < countX; x++)
        {
            for (int z = 0; z < countZ; z++)
            {
                Vector3 tilePos = startPos + new Vector3(x * segmentSize + 5, height, z * segmentSize + 5);

                if (x == centerBuildingX && z == centerBuildingZ)
                {
                    for (int i = 1; i < 5; i++)
                    {
                        Vector3 doorPos = tilePos;
                        GameObject door = Instantiate(doorPrefab, doorPos, Quaternion.identity, transform);

                        // Получаем и сохраняем Animator
                        Animator doorAnimator = door.GetComponent<Animator>();
                        doorAnimators.Add(doorAnimator);

                        door.transform.rotation = Quaternion.Euler(0, 90 * i, 0);
                        door.name = "Door_" + i.ToString();
                        generatedDoors.Add(door);
                    }
                }
            }
        }
    }

    private void ClearPreviousGeneration()
    {
        for (int i = generatedOutline.Count - 1; i >= 0; i--)
        {
            if (generatedOutline[i] != null)
            {
                if (Application.isPlaying) Destroy(generatedOutline[i]);
                else DestroyImmediate(generatedOutline[i]);
            }
        }
        generatedOutline.Clear();

        for (int i = generatedTiles.Count - 1; i >= 0; i--)
        {   
            if (generatedTiles[i] != null)
            {
                if (Application.isPlaying) Destroy(generatedTiles[i]);
                else DestroyImmediate(generatedTiles[i]);
            }
        }
        generatedTiles.Clear();

        for (int i = generatedPlaceholder.Count - 1; i >= 0; i--)
        {
            if (generatedPlaceholder[i] != null)
            {
                if (Application.isPlaying) Destroy(generatedPlaceholder[i]);
                else DestroyImmediate(generatedPlaceholder[i]);
            }
        }
        generatedPlaceholder.Clear();
    }
    private void OnValidate()
    {
        Width = Mathf.Max(2, Width);
        Length = Mathf.Max(2, Length);
        segmentSize = Mathf.Max(0.1f, segmentSize);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            float totalWidth = Width * segmentSize;
            float totalLength = Length * segmentSize;
            Vector3 centerOffset = centerBuilding ?
                new Vector3(-totalWidth / 2f, 0, -totalLength / 2f) : Vector3.zero;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(
                new Vector3(totalWidth / 2f, 0 , totalLength / 2f) + centerOffset,
                new Vector3(totalWidth, 0, totalLength));

            if (generateTile)
            {
                float innerWidth = totalWidth - 2 * segmentSize;
                float innerLength = totalLength - 2 * segmentSize;

                Gizmos.color = Color.green;
                if (generateTile)
                {
                    Gizmos.DrawWireCube(
                        new Vector3(totalWidth / 2f, 0, totalLength / 2f) + centerOffset,
                        new Vector3(innerWidth, 0.1f, innerLength));
                }
            }
        }
    }

    void BakeNavMesh()
    {
        if (surface != null)
        {
            surface.BuildNavMesh(); // Запекаем NavMesh
            Debug.Log("NavMesh baked successfully!");
        }
        else
        {
            Debug.LogError("NavMeshSurface not assigned!");
        }
    }
}