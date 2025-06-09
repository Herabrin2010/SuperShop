using TMPro;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] float distance;
    [SerializeField] TextMeshProUGUI help;

    private InformationAboutObject takenItem;
    public InventoryController inventoryController;

    private GameObject lastHitObject; // Для отслеживания последнего задетого объекта

    public  void Start()
    {
        help.gameObject.SetActive(false);
    }

    public void Update()
    {
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 3, Color.red);
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(ray, out hit, distance);

        if (hitSomething && hit.collider.CompareTag("Obj"))
        {
            // Если навели на новый объект (отличается от предыдущего)
            if (lastHitObject != hit.collider.gameObject)
            {
                // Выключаем текст, если был другой объект
                if (lastHitObject != null)
                {
                    help.gameObject.SetActive(false);
                }

                // Включаем текст для нового объекта
                help.gameObject.SetActive(true);
                lastHitObject = hit.collider.gameObject;
            }

            // Действие по нажатию E
            if (Input.GetKeyDown(KeyCode.E))
            {
                takenItem = hit.collider.gameObject.GetComponent<InformationAboutObject>();
                inventoryController.SearchingFreeSlot(takenItem._name, takenItem._sprite);
                Destroy(hit.collider.gameObject);
                help.gameObject.SetActive(false);
                takenItem = null;
                lastHitObject = null;
            }
        }
        else
        {
            // Если ничего не задели, но текст был включен
            if (lastHitObject != null)
            {
                help.gameObject.SetActive(false);
                lastHitObject = null;
            }
        }
    }
}