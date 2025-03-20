using UnityEngine;

/// <summary>
/// Класс для управления рюкзаком. Отвечает за прикрепление предметов к слотам.
/// </summary>
public class BackpackManager : MonoBehaviour
{
    [Header("Slots")]
    public Transform[] slots; // Слоты для прикрепления предметов
    public GameObject[] attachedObjects; // Объекты, прикрепленные к слотам

    [Header("Item Type Slots")]
    [SerializeField] private Transform appleSlot; // Слот для яблока
    [SerializeField] private Transform flashlightSlot; // Слот для фонарика
    [SerializeField] private Transform ropeSlot; // Слот для веревки

    private void Start()
    {
        attachedObjects = new GameObject[slots.Length];
    }

    /// <summary>
    /// Прикрепляет объект к соответствующему слоту на основе его типа.
    /// </summary>
    /// <param name="objectToAttach">Объект, который нужно прикрепить.</param>
    /// <returns>True, если объект успешно прикреплен, иначе False.</returns>
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
    /// Возвращает слот для предмета на основе его типа.
    /// </summary>
    /// <param name="itemType">Тип предмета.</param>
    /// <returns>Слот для предмета.</returns>
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
    /// Открепляет объект от слота.
    /// </summary>
    /// <param name="objectToDetach">Объект, который нужно открепить.</param>
    public void DetachObject(GameObject objectToDetach)
    {
        objectToDetach.transform.SetParent(null);

        Rigidbody rb = objectToDetach.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Debug.Log($"Объект {objectToDetach.name} откреплен от рюкзака.");
    }
}