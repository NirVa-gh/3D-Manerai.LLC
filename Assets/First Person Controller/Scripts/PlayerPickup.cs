using UnityEngine;

/// <summary>
/// ����� ��� ���������� ��������, ��������� � ������������� ��������� �������.
/// ����� ��������� ����������� �������� � �������.
/// </summary>
public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupDistance = 3f; // ��������� ������� ���������
    [SerializeField] private float throwForce = 10f; // ���� ������ ��������
    [SerializeField] private Transform holdPosition; // ������� ��������� ��������
    [SerializeField] private float maxPickupMass = 10f; // ������������ ����� ������������ ��������
    [SerializeField] private string pickupTag = "Pickable"; // ��� ����������� ���������
    [SerializeField] private string backpackTag = "Backpack"; // ��� �������
    [SerializeField] private float attachSpeed = 5f; // �������� ������������ �������� � �������

    [Header("References")]
    [SerializeField] private BackpackManager backpackManager; // �������� �������
    [SerializeField] private InventoryManager inventoryManager; // �������� ���������

    private GameObject heldObject; // ������� ������������ ������
    private Rigidbody heldObjectRb; // Rigidbody ������������� �������
    private bool isAttachingToBackpack; // ���� ������������ ������� � �������
    private Transform backpackTarget; // ������� ���� ������� ��� ������������

    /// <summary>
    /// ������������� ��������� ������� ��� ������.
    /// </summary>
    private void Start()
    {
        GameObject backpackObject = GameObject.FindGameObjectWithTag(backpackTag);
        if (backpackObject != null)
        {
            backpackManager = backpackObject.GetComponent<BackpackManager>();
        }
    }

    /// <summary>
    /// ���������� ������ ������� � ����������� ��������� ������ ����.
    /// </summary>
    private void Update()
    {
        HandlePickupInput();
        MoveHeldObject();

        if (isAttachingToBackpack)
        {
            AttachToBackpack();
        }
    }

    /// <summary>
    /// ������������ ���� ������ ��� �������, ������������ � ������������ ���������.
    /// </summary>
    private void HandlePickupInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                ThrowObject();
            }
        }

        if (heldObject != null && Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupDistance))
            {
                if (hit.collider.CompareTag(backpackTag))
                {
                    StartAttachToBackpack(hit.transform);
                }
            }
        }
    }

    /// <summary>
    /// �������� ��������� ������, ���� �� ��������� � ���� ������������.
    /// </summary>
    private void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag(pickupTag))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null && rb.mass <= maxPickupMass)
                {
                    PickupObject(hit.collider.gameObject, rb);
                    backpackManager.DetachObject(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("������ ������� ������� ��� �� ����� Rigidbody.");
                }
            }
        }
    }

    /// <summary>
    /// ��������� ������ � �������� ��� ����������.
    /// </summary>
    /// <param name="obj">������ ��� �������.</param>
    /// <param name="rb">Rigidbody �������.</param>
    private void PickupObject(GameObject obj, Rigidbody rb)
    {
        heldObject = obj;
        heldObjectRb = rb;

        heldObjectRb.useGravity = false;
        heldObjectRb.isKinematic = false;
    }

    /// <summary>
    /// ���������� ������������ ������ � ������� ���������.
    /// </summary>
    private void MoveHeldObject()
    {
        if (heldObject != null && !isAttachingToBackpack)
        {
            Vector3 targetPosition = holdPosition.position;
            Vector3 moveDirection = targetPosition - heldObject.transform.position;
            heldObjectRb.velocity = moveDirection * 10f;

            heldObjectRb.angularVelocity = Vector3.zero;
            heldObject.transform.rotation = Quaternion.Slerp(heldObject.transform.rotation, holdPosition.rotation, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// ����������� ������������ ������.
    /// </summary>
    private void ThrowObject()
    {
        if (heldObjectRb != null)
        {
            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
            heldObjectRb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }

        heldObject = null;
        heldObjectRb = null;
    }

    /// <summary>
    /// �������� ������� ������������ ������� � �������.
    /// </summary>
    /// <param name="backpack">��������� �������.</param>
    private void StartAttachToBackpack(Transform backpack)
    {
        isAttachingToBackpack = true;
        backpackTarget = backpack;
    }

    /// <summary>
    /// ����������� ������ � �������.
    /// </summary>
    private void AttachToBackpack()
    {
        if (heldObject == null || backpackTarget == null)
        {
            isAttachingToBackpack = false;
            return;
        }

        Vector3 targetPosition = backpackTarget.position;
        Vector3 moveDirection = targetPosition - heldObject.transform.position;
        heldObjectRb.velocity = moveDirection * attachSpeed;

        if (moveDirection.magnitude < 0.8f)
        {
            FinishAttachToBackpack();
        }
    }

    /// <summary>
    /// ��������� ������� ������������ ������� � �������.
    /// </summary>
    private void FinishAttachToBackpack()
    {
        bool isAttached = backpackManager.AttachObject(heldObject);
        if (isAttached)
        {
            ItemGameObject itemObject = heldObject.GetComponent<ItemGameObject>();
            if (itemObject != null)
            {
                inventoryManager.AddItem(itemObject.item, itemObject.amount);
            }

            heldObject = null;
            heldObjectRb = null;
        }
        else
        {
            ThrowObject();
        }

        isAttachingToBackpack = false;
        backpackTarget = null;
    }
}