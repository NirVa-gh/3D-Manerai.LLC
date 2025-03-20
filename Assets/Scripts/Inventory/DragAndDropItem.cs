using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Класс для реализации перетаскивания предметов в инвентаре.
/// Позволяет перемещать предметы между слотами, выбрасывать их и разделять стопки.
/// </summary>
public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("References")]
    [SerializeField] private Transform player; // Ссылка на игрока
    [SerializeField] private BackpackManager backpackManager; // Менеджер рюкзака

    private InventorySlot oldSlot; // Слот, из которого перетаскивается предмет

    /// <summary>
    /// Инициализация ссылок на игрока и менеджер рюкзака.
    /// </summary>
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        GameObject backpackObject = GameObject.FindGameObjectWithTag("Backpack");
        backpackManager = backpackObject != null ? backpackObject.GetComponent<BackpackManager>() : null;
        oldSlot = transform.GetComponentInParent<InventorySlot>();
    }

    /// <summary>
    /// Обрабатывает перетаскивание предмета.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;

        // Перемещаем объект вслед за курсором
        GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    /// <summary>
    /// Обрабатывает нажатие на предмет.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;

        Debug.Log("Grab");
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        GetComponentInChildren<Image>().raycastTarget = false;
        transform.SetParent(transform.parent.parent.parent); // Перемещаем объект на верхний уровень UI
    }

    /// <summary>
    /// Обрабатывает отпускание предмета.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;

        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
        GetComponentInChildren<Image>().raycastTarget = true;
        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;

        // Если предмет отпущен на фоне (не на другом слоте)
        if (eventData.pointerCurrentRaycast.gameObject.name == "Background")
        {
            HandleDropToWorld();
        }
        // Если предмет отпущен на другом слоте
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent != null &&
                 eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
        {
            ExchangeSlotData(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>());
        }
    }

    /// <summary>
    /// Обрабатывает выбрасывание предмета в мир.
    /// </summary>
    private void HandleDropToWorld()
    {
        if (oldSlot.item.maximumAmount > 1 && oldSlot.amount > 1)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                DropItem(Mathf.CeilToInt((float)oldSlot.amount / 2));
                oldSlot.amount -= Mathf.CeilToInt((float)oldSlot.amount / 2);
                oldSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                DropItem(1);
                oldSlot.amount--;
                oldSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            else
            {
                DropItem(oldSlot.amount);
                NullifySlotData();
            }
        }
        else
        {
            DropItem(oldSlot.amount);
            NullifySlotData();
        }
    }

    /// <summary>
    /// Создает объект в мире и выбрасывает его.
    /// </summary>
    /// <param name="amount">Количество предметов для выбрасывания.</param>
    private void DropItem(int amount)
    {
        GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity);
        itemObject.GetComponent<Rigidbody>().AddForce(player.forward * 2f, ForceMode.Impulse);
        itemObject.GetComponent<ItemGameObject>().amount = amount;
    }

    /// <summary>
    /// Очищает данные слота.
    /// </summary>
    private void NullifySlotData()
    {
        oldSlot.item = null;
        oldSlot.amount = 0;
        oldSlot.isEmpty = true;
        oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        oldSlot.iconGO.GetComponent<Image>().sprite = null;
        oldSlot.itemAmountText.text = "";
    }

    /// <summary>
    /// Обменивает данные между двумя слотами.
    /// </summary>
    /// <param name="newSlot">Слот, в который перемещается предмет.</param>
    private void ExchangeSlotData(InventorySlot newSlot)
    {
        ItemScriptableObject item = newSlot.item;
        int amount = newSlot.amount;
        bool isEmpty = newSlot.isEmpty;
        GameObject iconGO = newSlot.iconGO;
        TMP_Text itemAmountText = newSlot.itemAmountText;

        if (item == null)
        {
            if (oldSlot.item.maximumAmount > 1 && oldSlot.amount > 1)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    SplitStack(newSlot, Mathf.CeilToInt((float)oldSlot.amount / 2));
                    return;
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    SplitStack(newSlot, 1);
                    return;
                }
            }
        }

        if (newSlot.item != null)
        {
            if (oldSlot.item.name.Equals(newSlot.item.name))
            {
                MergeStacks(newSlot);
                return;
            }
        }

        SwapSlots(newSlot, item, amount, isEmpty, iconGO, itemAmountText);
    }

    /// <summary>
    /// Разделяет стопку предметов между двумя слотами.
    /// </summary>
    /// <param name="newSlot">Слот, в который перемещается часть стопки.</param>
    /// <param name="amount">Количество предметов для перемещения.</param>
    private void SplitStack(InventorySlot newSlot, int amount)
    {
        newSlot.item = oldSlot.item;
        newSlot.amount = amount;
        newSlot.isEmpty = false;
        newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
        newSlot.itemAmountText.text = newSlot.amount.ToString();

        oldSlot.amount -= amount;
        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
    }

    /// <summary>
    /// Объединяет стопки предметов в одном слоте.
    /// </summary>
    /// <param name="newSlot">Слот, в который объединяются предметы.</param>
    private void MergeStacks(InventorySlot newSlot)
    {
        if (Input.GetKey(KeyCode.LeftShift) && oldSlot.amount > 1)
        {
            int transferAmount = Mathf.CeilToInt((float)oldSlot.amount / 2);
            if (transferAmount <= newSlot.item.maximumAmount - newSlot.amount)
            {
                newSlot.amount += transferAmount;
                oldSlot.amount -= transferAmount;
            }
            else
            {
                int difference = newSlot.item.maximumAmount - newSlot.amount;
                newSlot.amount = newSlot.item.maximumAmount;
                oldSlot.amount -= difference;
            }
        }
        else if (Input.GetKey(KeyCode.LeftControl) && oldSlot.amount > 1)
        {
            if (newSlot.amount < newSlot.item.maximumAmount)
            {
                newSlot.amount++;
                oldSlot.amount--;
            }
        }
        else
        {
            if (newSlot.amount + oldSlot.amount <= newSlot.item.maximumAmount)
            {
                newSlot.amount += oldSlot.amount;
                NullifySlotData();
            }
            else
            {
                int difference = newSlot.item.maximumAmount - newSlot.amount;
                newSlot.amount = newSlot.item.maximumAmount;
                oldSlot.amount -= difference;
            }
        }

        newSlot.itemAmountText.text = newSlot.amount.ToString();
        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
    }

    /// <summary>
    /// Обменивает данные между двумя слотами.
    /// </summary>
    /// <param name="newSlot">Слот, в который перемещается предмет.</param>
    /// <param name="item">Предмет из нового слота.</param>
    /// <param name="amount">Количество предметов в новом слоте.</param>
    /// <param name="isEmpty">Пуст ли новый слот.</param>
    /// <param name="iconGO">Иконка нового слота.</param>
    /// <param name="itemAmountText">Текст количества предметов в новом слоте.</param>
    private void SwapSlots(InventorySlot newSlot, ItemScriptableObject item, int amount, bool isEmpty, GameObject iconGO, TMP_Text itemAmountText)
    {
        newSlot.item = oldSlot.item;
        newSlot.amount = oldSlot.amount;
        newSlot.isEmpty = oldSlot.isEmpty;
        newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
        newSlot.itemAmountText.text = oldSlot.amount > 1 ? oldSlot.amount.ToString() : "";

        oldSlot.item = item;
        oldSlot.amount = amount;
        oldSlot.isEmpty = isEmpty;
        oldSlot.SetIcon(item != null ? item.icon : null);
        oldSlot.itemAmountText.text = amount > 1 ? amount.ToString() : "";
    }
}