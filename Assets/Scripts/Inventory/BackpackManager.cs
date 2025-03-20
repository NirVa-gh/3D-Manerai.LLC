using UnityEngine;

/// <summary>
/// ����� ��� ���������� ��������. �������� �� ������������ ��������� � ������.
/// </summary>
public class BackpackManager : MonoBehaviour
{
    [Header("Slots")]
    public Transform[] slots; // ����� ��� ������������ ���������
    public GameObject[] attachedObjects; // �������, ������������� � ������

    [Header("Item Type Slots")]
    [SerializeField] private Transform appleSlot; // ���� ��� ������
    [SerializeField] private Transform flashlightSlot; // ���� ��� ��������
    [SerializeField] private Transform ropeSlot; // ���� ��� �������

    private void Start()
    {
        attachedObjects = new GameObject[slots.Length];
    }

    /// <summary>
    /// ����������� ������ � ���������������� ����� �� ������ ��� ����.
    /// </summary>
    /// <param name="objectToAttach">������, ������� ����� ����������.</param>
    /// <returns>True, ���� ������ ������� ����������, ����� False.</returns>
    public bool AttachObject(GameObject objectToAttach)
    {
        ItemGameObject itemObject = objectToAttach.GetComponent<ItemGameObject>();
        if (itemObject == null)
        {
            Debug.LogError("������ �� �������� ��������� ItemGameObject.");
            return false;
        }

        Transform targetSlot = GetSlotByItemType(itemObject.item.itemType);
        if (targetSlot == null)
        {
            Debug.LogError("�� ������ ���� ��� ������� ���� ��������.");
            return false;
        }

        // ����������� ������ � �����
        objectToAttach.transform.SetParent(targetSlot);
        objectToAttach.transform.localPosition = Vector3.zero;
        objectToAttach.transform.localRotation = Quaternion.identity;

        Rigidbody rb = objectToAttach.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Debug.Log($"������ {itemObject.item.itemName} ���������� � ����� {targetSlot.name}");
        return true;
    }

    /// <summary>
    /// ���������� ���� ��� �������� �� ������ ��� ����.
    /// </summary>
    /// <param name="itemType">��� ��������.</param>
    /// <returns>���� ��� ��������.</returns>
    private Transform GetSlotByItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Food:
                return appleSlot;
            case ItemType.Weapon:
                return flashlightSlot;
            case ItemType.Common:
                return ropeSlot;
            default:
                return null;
        }
    }

    /// <summary>
    /// ���������� ������ �� �����.
    /// </summary>
    /// <param name="objectToDetach">������, ������� ����� ���������.</param>
    public void DetachObject(GameObject objectToDetach)
    {
        objectToDetach.transform.SetParent(null);

        Rigidbody rb = objectToDetach.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log($"������ {objectToDetach.name} ��������� �� �������.");
    }
}