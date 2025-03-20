using UnityEngine;

/// <summary>
/// ��������� ���������� �� �������.
/// </summary>
public class BackpackManager : MonoBehaviour
{
    [Header("Slots")]
    public Transform[] slots; // ����� ��� ������������ ���������

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager; // ������ �� InventoryManager

    /// <summary>
    /// ����������� ������ � ���������������� ����� �� ������ ��� ����.
    /// </summary>
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
    /// ���������� ������ �� ����� � ������� ��� �� ���������.
    /// </summary>
    public void DetachObject(GameObject objectToDetach)
    {
        ItemGameObject itemObject = objectToDetach.GetComponent<ItemGameObject>();
        if (itemObject != null)
        {
            // ������� ������� �� ���������
            inventoryManager.RemoveItem(itemObject.item.itemName);
        }

        objectToDetach.transform.SetParent(null);

        Rigidbody rb = objectToDetach.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log($"������ {objectToDetach.name} ��������� �� �������.");
    }

    /// <summary>
    /// ���������� ���� ��� �������� �� ������ ��� ����.
    /// </summary>
    private Transform GetSlotByItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Food:
                return slots[0]; // ������: ������ ���� ��� ���
            case ItemType.Weapon:
                return slots[1]; // ������: ������ ���� ��� ������
            case ItemType.Common:
                return slots[2]; // ������: ������ ���� ��� ������� ���������
            default:
                return null;
        }
    }
}