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
        yield return null;

        Lights.Clear();
        LightToGameObject.Clear();

        // Add all light sources
        foreach (var tile in generation.generatedTiles)
        {
            var light = tile.GetComponentInChildren<Light>(true);
            if (light != null && !Lights.Contains(light))
            {
                Lights.Add(light);
                LightToGameObject.Add(light.gameObject);
            }
        }

        // Add central lights
        if (cetralRoomLights != null)
        {
            foreach (var light in cetralRoomLights)
            {
                if (light != null && !Lights.Contains(light))
                {
                    Lights.Add(light);
                    LightToGameObject.Add(light.gameObject);
                }
            }
        }

        // Update generation list
        generation.objectsToDisable.AddRange(LightToGameObject);
        Debug.Log($"Initialized {Lights.Count} light sources");
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
        playerLight.gameObject.SetActive(state);

        // Обрабатываем все световые элементы
        foreach (Renderer renderer in lightRenderers)
        {

            if (renderer == null) continue; // Пропускаем null

            Material[] materials = renderer.materials;
            if (materials.Length > 1) // Проверяем, есть ли второй материал
            {
                // Меняем материал
                materials[1] = state ? lightOnMaterial : lightOffMaterial;
                materials[1].globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

                // Важно: присваиваем массив обратно!
                renderer.materials = materials;

                // Принудительно обновляем освещение (на всякий случай)
                RendererExtensions.UpdateGIMaterials(renderer);
            }
            else
            {
                Debug.LogWarning($"У объекта {renderer.gameObject.name} нет второго материала!", renderer.gameObject);
            }
        }

        if (Lights.Count > 0)
        {
            for (int i = 0; i < Lights.Count; i++)
            {
                Lights[i].gameObject.SetActive(state);
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

    public void TurnLightOn()
    {
        SetElectricity(true);
    }

    public void TurnLightOff()
    {
        SetElectricity(false);

        // Force disable all lights immediately
        foreach (var lightObj in LightToGameObject)
        {
            if (lightObj != null && lightObj.TryGetComponent<Light>(out _))
            {
                lightObj.SetActive(false);
            }
        }
    }
}