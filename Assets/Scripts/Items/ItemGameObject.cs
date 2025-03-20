using UnityEngine;

/// <summary>
/// �����, �������������� ������� ������ ��������. �������� ������ �� ScriptableObject �������� � ��� ����������.
/// </summary>
public class ItemGameObject : MonoBehaviour
{
    [Header("Item Data")]
    [Tooltip("������ � �������� (ScriptableObject).")]
    public ItemScriptableObject item; // ������ � ��������

    [Tooltip("���������� ��������� � ���� �������.")]
    public int amount; // ���������� ���������
}