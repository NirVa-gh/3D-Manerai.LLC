using System;

/// <summary>
/// ����������� ����� ��� ���������� ���������, ���������� � ����������.
/// ������������� ������� ��� ���������� � �������� ���������.
/// </summary>
public static class InventoryEvents
{
    /// <summary>
    /// �������, ���������� ��� ���������� �������� � ���������.
    /// </summary>
    public static Action<string> OnItemAdded;

    /// <summary>
    /// �������, ���������� ��� �������� �������� �� ���������.
    /// </summary>
    public static Action<string> OnItemRemoved;

    /// <summary>
    /// �������� ������� ���������� ��������.
    /// </summary>
    /// <param name="itemId">������������� ������������ ��������.</param>
    public static void ItemAdded(string itemId)
    {
        OnItemAdded?.Invoke(itemId);
    }

    /// <summary>
    /// �������� ������� �������� ��������.
    /// </summary>
    /// <param name="itemId">������������� ���������� ��������.</param>
    public static void ItemRemoved(string itemId)
    {
        OnItemRemoved?.Invoke(itemId);
    }
}