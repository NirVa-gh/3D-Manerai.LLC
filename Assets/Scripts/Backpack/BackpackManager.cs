using UnityEngine;

/// <summary>
/// Управляет предметами на рюкзаке.
/// </summary>
public class BackpackManager : MonoBehaviour
{
    [Header("Slots")]
    public Transform[] slots; // Слоты для прикрепления предметов

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager; // Ссылка на InventoryManager

    /// <summary>
    /// Прикрепляет объект к соответствующему слоту на основе его типа.
    /// </summary>
    public bool AttachObject(GameObject objectToAttach)
    {
        ItemGameObject itemObject = objectToAttach.GetComponent<ItemGameObject>();
        if (itemObject == null)
        {
            Debug.LogError("Объект не содержит компонент ItemGameObject.");
            return false;
        }

        Transform targetSlot = GetSlotByItemType(itemObject.item.itemType);
        if (targetSlot == null)
        {
            Debug.LogError("Не найден слот для данного типа предмета.");
            return false;
        }

        // Прикрепляем объект к слоту
        objectToAttach.transform.SetParent(targetSlot);
        objectToAttach.transform.localPosition = Vector3.zero;
        objectToAttach.transform.localRotation = Quaternion.identity;

        Rigidbody rb = objectToAttach.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Debug.Log($"Объект {itemObject.item.itemName} прикреплен к слоту {targetSlot.name}");
        return true;
    }

    /// <summary>
    /// Открепляет объект от слота и удаляет его из инвентаря.
    /// </summary>
    public void DetachObject(GameObject objectToDetach)
    {
        ItemGameObject itemObject = objectToDetach.GetComponent<ItemGameObject>();
        if (itemObject != null)
        {
            // Удаляем предмет из инвентаря
            inventoryManager.RemoveItem(itemObject.item.itemName);
        }

        objectToDetach.transform.SetParent(null);

        Rigidbody rb = objectToDetach.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log($"Объект {objectToDetach.name} откреплен от рюкзака.");
    }

    /// <summary>
    /// Возвращает слот для предмета на основе его типа.
    /// </summary>
    private Transform GetSlotByItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Food:
                return slots[0]; // Пример: первый слот для еды
            case ItemType.Weapon:
                return slots[1]; // Пример: второй слот для оружия
            case ItemType.Common:
                return slots[2]; // Пример: третий слот для обычных предметов
            default:
                return null;
        }
    }
}