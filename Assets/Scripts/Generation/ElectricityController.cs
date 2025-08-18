using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting; // Необходимо для работы с List<>

public class ElectricityController : MonoBehaviour
{
    [Header ("Links")]
    private Generation generation;

    [Header("Settings")]
    public bool startWithElectricityOn = true;

    [Header("Fuse")]
    [SerializeField] private Renderer fuseLightRenderer;
    [SerializeField] private Material fuseLightOnMaterial;
    [SerializeField] private Material fuseLightOffMaterial;

    [Header("Light")]
    [SerializeField] private Light playerLight;
    [SerializeField] private Renderer centralRoom;
    [SerializeField] private Light[] cetralRoomLights;
    [SerializeField] private Material lightOnMaterial;
    [SerializeField] private Material lightOffMaterial;
    public List<GameObject> LightToGameObject = new List<GameObject>();
    private List<Light> Lights = new List<Light>();
    private List<Renderer> lightRenderers = new List<Renderer>();

    [Header ("Camera")]
    [SerializeField] private Camera mainCamera = Camera.main;
    private float originalFarClipPlane;
    private Color oroginalBacroundColor;

    private void Awake()
    {
        #region Links
        generation = FindAnyObjectByType<Generation>();
        #endregion

        #region Save Comera's preferenses
        originalFarClipPlane = mainCamera.farClipPlane;
        oroginalBacroundColor = mainCamera.backgroundColor;
        #endregion

        SetElectricity(startWithElectricityOn);

        StartCoroutine(InitializeLights());
    }

    private IEnumerator InitializeLights()
    {
        yield return new WaitUntil(() => generation.isCompleteBuilding);

        Lights.Clear();
        LightToGameObject.Clear();

        // Добавляем все источники света из тайлов
        foreach (var tile in generation.generatedTiles)
        {
            var lights = tile.GetComponentsInChildren<Light>(true);
            foreach (var light in lights)
            {
                if (!Lights.Contains(light))
                {
                    Lights.Add(light);
                    LightToGameObject.Add(light.gameObject);
                    light.gameObject.SetActive(true);
                    light.enabled = false;
                }
            }
        }

        // Добавляем центральные света
        if (cetralRoomLights != null)
        {
            foreach (var light in cetralRoomLights)
            {
                if (!Lights.Contains(light))
                {
                    Lights.Add(light);
                    LightToGameObject.Add(light.gameObject);
                }
            }
        }

        // Теперь добавляем в общий список
        generation.objectsToDisable.AddRange(LightToGameObject);
        Debug.Log($"Lights initialized: {Lights.Count} sources added");
    }

    public void RefreshLightStates()
    {
        foreach (var light in Lights)
        {
            if (light != null)
            {
                light.gameObject.SetActive(true);
                light.enabled = startWithElectricityOn;
            }
        }
    }

    private void Start()
    {

        foreach (GameObject obj in generation.generatedTiles)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            Light light = obj.GetComponentInChildren<Light>(true);

            if (renderer != null) lightRenderers.Add(renderer);
            if (centralRoom != null) lightRenderers.Add(centralRoom);

            if (light != null) Lights.Add(light);
            if (centralRoom != null)
            {
                foreach (Light lightObjects in cetralRoomLights)
                {
                    if (lightObjects != null) Lights.Add(lightObjects);
                }
            }
        }
        foreach (Light light in Lights)
        {
            LightToGameObject.Add(light.gameObject);
        }

    }

    private void SetElectricity(bool state)
    {
        startWithElectricityOn = state;

        // Обновляем состояние всех источников света
        foreach (var light in Lights)
        {
            if (light != null)
            {
                light.enabled = state;
            }
        }

        // Обрабатываем все световые элементы
        foreach (Renderer renderer in lightRenderers)
        {
            if (renderer == null) continue;

            Material[] materials = renderer.materials;
            if (materials.Length > 1)
            {
                materials[1] = state ? lightOnMaterial : lightOffMaterial;
                materials[1].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                renderer.materials = materials;
                RendererExtensions.UpdateGIMaterials(renderer);
            }
        }

        // Обработка предохранителя
        if (fuseLightRenderer != null)
        {
            fuseLightRenderer.material = state ? fuseLightOnMaterial : fuseLightOffMaterial;
        }

        // Настройка камеры
        mainCamera.farClipPlane = state ? originalFarClipPlane : 10;
        mainCamera.backgroundColor = state ? oroginalBacroundColor : Color.black;
    }

    public void TurnLightOff()
    {
        SetElectricity(false);
        // Не нужно принудительно отключать объекты, этим займется OptimizeObjectsAroundPlayer
    }

    public void TurnLightOn()
    {
        SetElectricity(true);
    }
}