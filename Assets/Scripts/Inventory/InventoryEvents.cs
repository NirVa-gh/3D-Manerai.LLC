using System;

/// <summary>
/// Статический класс для управления событиями, связанными с инвентарем.
/// Предоставляет события для добавления и удаления предметов.
/// </summary>
public static class InventoryEvents
{
    /// <summary>
    /// Событие, вызываемое при добавлении предмета в инвентарь.
    /// </summary>
    public static Action<string> OnItemAdded;

    /// <summary>
    /// Событие, вызываемое при удалении предмета из инвентаря.
    /// </summary>
    public static Action<string> OnItemRemoved;

    /// <summary>
    /// Вызывает событие добавления предмета.
    /// </summary>
    /// <param name="itemId">Идентификатор добавленного предмета.</param>
    public static void ItemAdded(string itemId)
    {
        OnItemAdded?.Invoke(itemId);
    }

    /// <summary>
    /// Вызывает событие удаления предмета.
    /// </summary>
    /// <param name="itemId">Идентификатор удаленного предмета.</param>
    public static void ItemRemoved(string itemId)
    {
        OnItemRemoved?.Invoke(itemId);
    }
}