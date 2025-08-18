using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptimizeObjectsAroundPlayer : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Объекты, которые будут отключаться при выходе за радиус")]
    public List<GameObject> objectsToDisable = new List<GameObject>();

    [Tooltip("Радиус, в котором объекты остаются включенными")]
    [SerializeField] private float activeRadius = 15f;
    private float activeRadiusSqr;

    [Tooltip("Игрок (если не задан, ищет по тегу 'Player')")]
    [SerializeField] private Transform player;

    [Header("Links")]
    private Generation generation;
    private LaptopControll laptopControll;
    private ElectricityController electricityController;

    private void Start()
    {
        #region Links
        generation = FindAnyObjectByType<Generation>();
        laptopControll = FindAnyObjectByType<LaptopControll>();
        electricityController = FindAnyObjectByType<ElectricityController>();
        #endregion

        activeRadiusSqr = activeRadius * activeRadius;
        StartCoroutine(InitializeObjects());
    }

    private IEnumerator InitializeObjects()
    {
        // Ждем пока Generation и ElectricityController полностью инициализируются
        yield return new WaitUntil(() =>
            generation != null &&
            generation.isCompleteBuilding &&
            electricityController != null &&
            electricityController.LightToGameObject.Count > 0);

        objectsToDisable = new List<GameObject>(generation.objectsToDisable);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) Debug.LogError("Player not found!");
        }

        Debug.Log($"Optimizer initialized with {objectsToDisable.Count} objects");
    }

    private void Update()
    {
        if (player == null || electricityController == null) return;

        bool electricityOn = electricityController.startWithElectricityOn;
        objectsToDisable.RemoveAll(obj => obj == null);

        foreach (var obj in objectsToDisable)
        {
            if (obj == null) continue;

            bool isInRadius = IsInRadius(obj);
            bool shouldBeActive = isInRadius && electricityOn;

            if (obj.TryGetComponent<Light>(out var light))
            {
                // Управляем только компонентом света, не трогаем активность объекта
                light.enabled = shouldBeActive;
            }
            else
            {
                // Обычные объекты
                obj.SetActive(shouldBeActive);
            }
        }
    }

    private bool IsInRadius(GameObject obj)
    {
        return (player.position - obj.transform.position).sqrMagnitude <= activeRadiusSqr;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.position, activeRadius);
    }
}