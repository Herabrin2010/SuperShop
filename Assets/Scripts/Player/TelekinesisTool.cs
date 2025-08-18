using UnityEngine;

public class TelekinesisTool : MonoBehaviour
{
    [Header("Settings")]
    public float pickUpRange = 10f;
    public float minHoldDistance = 1f;
    public float maxHoldDistance = 5f;
    public float throwForce = 50f;
    public float lerpSpeed = 15f;
    public float zoomSpeed = 2f;

    [Header("References")]
    public Transform holdPosition; 
    public Transform toolTip;     
    public Camera playerCamera;

    private Rigidbody _heldObject;
    private bool _isHolding;
    private bool _telekinesisActive;
    private float _currentHoldDistance;

    [Header ("Links")]
    private KeyRebinder keyRebinder;

    private void Awake()
    {
        keyRebinder = FindAnyObjectByType<KeyRebinder>();
        _currentHoldDistance = holdPosition.localPosition.z; // Начальная дистанция 
    }

    void Update()
    {
        if (keyRebinder.GetActionDown("Telekinesis"))
        {
            _telekinesisActive = !_telekinesisActive;
            if (!_telekinesisActive && _isHolding)
                ReleaseObject();
        }

        if (!_telekinesisActive) return;

        // Прокрутка колёсика мыши для изменения дистанции
        if (_isHolding)
        {
            float scroll = - Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                _currentHoldDistance = Mathf.Clamp(
                    _currentHoldDistance - scroll * zoomSpeed,
                    minHoldDistance,
                    maxHoldDistance
                );
                // Обновляем позицию точки удержания
                holdPosition.localPosition = new Vector3(0, 0, _currentHoldDistance);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!_isHolding)
                TryAttractObject();
            else
                ReleaseObject();
        }

        if (Input.GetMouseButtonDown(0) && _isHolding)
            ThrowObject();

        if (_isHolding)
            MoveHeldObject();
    }

    void TryAttractObject()
    {
        RaycastHit hit;
        Vector3 rayOrigin = toolTip != null ? toolTip.position : playerCamera.transform.position;
        Vector3 rayDirection = toolTip != null ? toolTip.forward : playerCamera.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, pickUpRange))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                _heldObject = rb;
                _heldObject.useGravity = false;
                _heldObject.linearDamping = 10f;
                _isHolding = true;
            }
        }
    }

    void MoveHeldObject()
    {
        if (_heldObject == null)
        {
            _isHolding = false;
            return;
        }

        Vector3 targetPos = holdPosition.position;
        _heldObject.linearVelocity = (targetPos - _heldObject.position) * lerpSpeed;
    }

    void ReleaseObject()
    {
        if (_heldObject == null) return;

        _heldObject.useGravity = true;
        _heldObject.linearDamping = 1f;
        _heldObject = null;
        _isHolding = false;
    }

    void ThrowObject()
    {
        if (_heldObject == null) return;

        _heldObject.useGravity = true;
        _heldObject.linearDamping = 1f;
        Vector3 throwDirection = playerCamera.transform.forward; // ������ �� ������� ������
        _heldObject.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        _heldObject = null;
        _isHolding = false;
    }
}