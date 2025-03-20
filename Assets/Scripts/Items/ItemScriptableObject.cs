using UnityEngine;

/// <summary>
/// Перечисление типов предметов.
/// </summary>
public enum ItemType
{
    Food,    // Еда
    Weapon,  // Оружие
    Common   // Обычный предмет
}

/// <summary>
/// Базовый класс для создания ScriptableObject предметов.
/// Содержит общие характеристики предметов, такие как имя, вес, тип и иконка.
/// </summary>
public class ItemScriptableObject : ScriptableObject
{
    [Header("Common Characteristics")]
    [Tooltip("Название предмета.")]
    public string itemName; // Название предмета

    [Tooltip("Вес предмета.")]
    public float weight; // Вес предмета

    [Tooltip("Тип предмета (еда, оружие, обычный).")]
    public ItemType itemType; // Тип предмета

    [Space]
    [Tooltip("Максимальное количество предметов в одной стопке.")]
    public int maximumAmount; // Максимальное количество предметов в одной стопке

    [Tooltip("Префаб предмета для создания в мире.")]
    public GameObject itemPrefab; // Префаб предмета

    [Tooltip("Иконка предмета для отображения в инвентаре.")]
    public Sprite icon; // Иконка предмета
}