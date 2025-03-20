using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Класс, представляющий слот инвентаря. Отвечает за хранение данных о предмете и отображение его иконки.
/// </summary>
public class InventorySlot : MonoBehaviour
{
    [Header("Item Data")]
    public ItemScriptableObject item; // Данные о предмете
    public int amount; // Количество предметов в слоте
    public bool isEmpty = true; // Флаг, указывающий, пуст ли слот

    [Header("UI Elements")]
    public GameObject iconGO; // Объект иконки предмета
    public TMP_Text itemAmountText; // Текст для отображения количества предметов

    /// <summary>
    /// Инициализация UI элементов при создании слота.
    /// </summary>
    private void Awake()
    {
        iconGO = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Устанавливает иконку предмета в слоте.
    /// </summary>
    /// <param name="icon">Иконка предмета.</param>
    public void SetIcon(Sprite icon)
    {
        iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        iconGO.GetComponent<Image>().sprite = icon;
    }
}