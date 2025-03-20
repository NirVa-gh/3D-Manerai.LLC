using UnityEngine;

/// <summary>
/// Класс, представляющий игровой объект предмета. Содержит ссылку на ScriptableObject предмета и его количество.
/// </summary>
public class ItemGameObject : MonoBehaviour
{
    [Header("Item Data")]
    [Tooltip("Данные о предмете (ScriptableObject).")]
    public ItemScriptableObject item; // Данные о предмете

    [Tooltip("Количество предметов в этом объекте.")]
    public int amount; // Количество предметов
}