using UnityEngine;
using System.Collections.Generic;

public class ItemGenerator : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField][Range(0, 100)] private int spawnChance = 70;
    [SerializeField] private int minItems = 5;
    [SerializeField] private int maxItems = 20;
    [SerializeField] private Vector2 itemScaleRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private float itemHeightOffset = 0.01f;
    [SerializeField] private string itemTag;

    [Header("Rotation Settings")] // Новый блок настроек поворота
    [SerializeField] private bool randomRotationY = true;
    [SerializeField] private float rotationYMin = 0f;
    [SerializeField] private float rotationYMax = 360f;

    [Header("Placement Settings")]
    [SerializeField] private float minDistanceFromWalls = 1f;
    [SerializeField] private LayerMask placementSurfaceMask;
    [SerializeField] private bool alignToSurfaceNormal = true;

    [Header("Spawn Zones")]
    [SerializeField] public bool AutoDetectFloorTiles = true; // Новый параметр
    [SerializeField] private List<Transform> customZones = new List<Transform>();

    private List<GameObject> spawnedItems = new List<GameObject>();
    private Generation generation;

    private void Awake()
    {
        generation = FindAnyObjectByType<Generation>();
        if (generation == null) Debug.LogError("Generation script not found!");
    }

    public void GenerateItems()
    {
        if (generation == null) return;

        ClearItems();
        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("No item prefabs assigned!");
            return;
        }

        if (AutoDetectFloorTiles) FindFloorTilesAndAddAsZones();

        int itemsToSpawn = Random.Range(minItems, maxItems + 1);
        int attempts = 0;
        int maxAttempts = itemsToSpawn * 3;

        while (spawnedItems.Count < itemsToSpawn && attempts < maxAttempts)
        {
            attempts++;
            Vector3 spawnPosition = GetRandomSpawnPosition();
            if (spawnPosition == Vector3.negativeInfinity) continue;
            if (Random.Range(0, 100) > spawnChance) continue;
            SpawnRandomItem(spawnPosition);
        }

        Debug.Log($"Generated {spawnedItems.Count} items");
    }

    // Находит все плитки пола и добавляет их как зоны спавна
    private void FindFloorTilesAndAddAsZones()
    {
        customZones.Clear();

        // Ищем все объекты с тегом "Floor" (или другим, который ты используешь)
        GameObject[] floorTiles = GameObject.FindGameObjectsWithTag("Floor");

        // Если нет тега, ищем по имени (если у плиток есть префикс в имени)
        if (floorTiles.Length == 0)
        {
            floorTiles = GameObject.FindGameObjectsWithTag("Floor_");
            if (floorTiles.Length == 0)
            {
                Debug.LogWarning("No floor tiles found! Check if they have a tag or correct naming.");
                return;
            }
        }

        foreach (GameObject tile in floorTiles)
        {
            // Добавляем коллайдер, если его нет (для зоны спавна)
            if (tile.GetComponent<Collider>() == null)
            {
                BoxCollider collider = tile.AddComponent<BoxCollider>();
                collider.size = new Vector3(
                    generation.segmentSize - minDistanceFromWalls * 2,
                    0.1f,
                    generation.segmentSize - minDistanceFromWalls * 2
                );
                collider.isTrigger = true;
            }

            customZones.Add(tile.transform);
        }

        Debug.Log($"Added {customZones.Count} floor tiles as spawn zones");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (customZones.Count == 0)
        {
            Debug.LogWarning("No spawn zones available!");
            return Vector3.negativeInfinity;
        }

        Transform zone = customZones[Random.Range(0, customZones.Count)];
        Collider zoneCollider = zone.GetComponent<Collider>();

        if (zoneCollider == null)
        {
            Debug.LogWarning("Zone has no collider!");
            return Vector3.negativeInfinity;
        }

        // Получаем случайную точку внутри коллайдера
        Vector3 randomPoint = GetRandomPointInCollider(zoneCollider);
        return randomPoint;
    }

    private Vector3 GetRandomPointInCollider(Collider collider)
    {
        Vector3 point = new Vector3(
            Random.Range(collider.bounds.min.x, collider.bounds.max.x),
            collider.bounds.max.y,
            Random.Range(collider.bounds.min.z, collider.bounds.max.z)
        );

        // Проверяем, чтобы предмет не спавнился в стене (рейкаст вниз)
        if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, Mathf.Infinity, placementSurfaceMask))
        {
            return hit.point + Vector3.up * itemHeightOffset;
        }

        return point;
    }

    private void SpawnRandomItem(Vector3 position)
    {
        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        GameObject item = Instantiate(prefab, position, Quaternion.identity, transform);

        // Убираем "(Clone)" из имени
        item.name = prefab.name;

        // Назначаем тег
        if (!string.IsNullOrEmpty(itemTag))
        {
            item.tag = itemTag;
        }

        // Случайный поворот по Y (если включено)
        if (randomRotationY)
        {
            float randomYRotation = Random.Range(rotationYMin, rotationYMax);
            item.transform.rotation = Quaternion.Euler(0, randomYRotation, 0);
        }

        // Случайный масштаб
        float scale = Random.Range(itemScaleRange.x, itemScaleRange.y);
        item.transform.localScale = Vector3.one * scale;

        // Выравнивание по нормали поверхности
        if (alignToSurfaceNormal && Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit))
        {
            item.transform.up = hit.normal;
            item.transform.position = hit.point + Vector3.up * itemHeightOffset;
        }

        spawnedItems.Add(item);
    }

    public void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
            {
                if (Application.isPlaying) Destroy(item);
                else DestroyImmediate(item);
            }
        }
        spawnedItems.Clear();
    }
}