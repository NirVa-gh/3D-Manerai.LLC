using UnityEngine;

/// <summary>
/// ������������ ����� ���������.
/// </summary>
public enum ItemType
{
    Food,    // ���
    Weapon,  // ������
    Common   // ������� �������
}

/// <summary>
/// ������� ����� ��� �������� ScriptableObject ���������.
/// �������� ����� �������������� ���������, ����� ��� ���, ���, ��� � ������.
/// </summary>
public class ItemScriptableObject : ScriptableObject
{
    [Header("Common Characteristics")]
    [Tooltip("�������� ��������.")]
    public string itemName; // �������� ��������

    [Tooltip("��� ��������.")]
    public float weight; // ��� ��������

    [Tooltip("��� �������� (���, ������, �������).")]
    public ItemType itemType; // ��� ��������

    [Space]
    [Tooltip("������������ ���������� ��������� � ����� ������.")]
    public int maximumAmount; // ������������ ���������� ��������� � ����� ������

    [Tooltip("������ �������� ��� �������� � ����.")]
    public GameObject itemPrefab; // ������ ��������

    [Tooltip("������ �������� ��� ����������� � ���������.")]
    public Sprite icon; // ������ ��������
}