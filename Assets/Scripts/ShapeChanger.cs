using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShapeChanger : MonoBehaviour
{
    private Tasks tasks;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        if (tasks != null)
        {
            UpdateShapeFromTask();
        }
    }

    public void UpdateShapeFromTask()
    {
        if (tasks == null || tasks.itemPrefabs == null || tasks.itemPrefabs.Count == 0)
            return;

        GameObject targetPrefab = null;

        if (!string.IsNullOrEmpty(tasks.currentTaskName))
        {
            foreach (var prefab in tasks.itemPrefabs)
            {
                var info = prefab.GetComponent<InformationAboutObject>();
                if (info != null && info._name == tasks.currentTaskName)
                {
                    targetPrefab = prefab;
                    break;
                }
            }
        }

        if (targetPrefab != null)
        {
            MeshFilter targetMeshFilter = targetPrefab.GetComponentInChildren<MeshFilter>(true);
            MeshRenderer targetMeshRenderer = targetPrefab.GetComponentInChildren<MeshRenderer>(true);

            if (targetMeshFilter != null && targetMeshRenderer != null)
            {
                _meshFilter.mesh = targetMeshFilter.sharedMesh;
                _meshRenderer.materials = targetMeshRenderer.sharedMaterials;
            }
        }
    }


    // Для обновления в редакторе
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (tasks != null)
        {
            UpdateShapeFromTask();
        }
    }
#endif
}