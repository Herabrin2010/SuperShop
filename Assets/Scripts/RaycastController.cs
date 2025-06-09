using TMPro;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] float distance;
    [SerializeField] TextMeshProUGUI help;

    private InformationAboutObject takenItem;
    public InventoryController inventoryController;

    private GameObject lastHitObject; // ��� ������������ ���������� �������� �������

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
            // ���� ������ �� ����� ������ (���������� �� �����������)
            if (lastHitObject != hit.collider.gameObject)
            {
                // ��������� �����, ���� ��� ������ ������
                if (lastHitObject != null)
                {
                    help.gameObject.SetActive(false);
                }

                // �������� ����� ��� ������ �������
                help.gameObject.SetActive(true);
                lastHitObject = hit.collider.gameObject;
            }

            // �������� �� ������� E
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
            // ���� ������ �� ������, �� ����� ��� �������
            if (lastHitObject != null)
            {
                help.gameObject.SetActive(false);
                lastHitObject = null;
            }
        }
    }
}