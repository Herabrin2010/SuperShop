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
    private float activeRadiusSqr; // Квадрат радиуса для оптимизации

    [Tooltip("Игрок (если не задан, ищет по тегу 'Player')")]
    [SerializeField] private Transform player;

    [Header ("Links")]
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
        while (generation == null || generation.objectsToDisable == null || generation.objectsToDisable.Count == 0)
        {
            yield return null;
        }

        objectsToDisable = new List<GameObject>(generation.objectsToDisable);

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null) Debug.LogError("Player not found!");
        }
    }

    private void Update()
    {
        if (player == null) return;

        objectsToDisable.RemoveAll(obj => obj == null);
        Camera activeCamera = laptopControll?.GetCurrentCamera();
        bool electricityOn = electricityController?.startWithElectricityOn ?? true;

        foreach (var obj in objectsToDisable)
        {
            if (obj == null) continue;

            // Camera handling
            if (obj.TryGetComponent<Camera>(out var cam))
            {
                bool shouldBeCameraActive = (cam == activeCamera);
                if (cam.gameObject.activeSelf != shouldBeCameraActive)
                {
                    cam.gameObject.SetActive(shouldBeCameraActive);
                }
                continue;
            }

            // Light handling
            if (obj.TryGetComponent<Light>(out var light))
            {
                bool shouldBeLightActive = electricityOn && IsInRadius(obj);
                if (light.gameObject.activeSelf != shouldBeLightActive)
                {
                    light.gameObject.SetActive(shouldBeLightActive);
                }
                continue;
            }

            // Regular objects
            bool shouldBeActive = IsInRadius(obj);
            if (obj.activeSelf != shouldBeActive)
            {
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