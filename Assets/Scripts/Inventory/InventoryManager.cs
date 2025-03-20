using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System;

/// <summary>
/// ����� ��� ���������� ����������. �������� �� ��������/�������� ���������, ���������� � �������� ���������.
/// </summary>
public class InventoryManager : UIWidget
{
    [Header("UI Elements")]
    [SerializeField] private GameObject backgroundPanel; // ������� ������ ���������
    [SerializeField] private GameObject crosshairImage; // ����������� �������
    [SerializeField] private Transform inventoryPanel; // ������ ������ ���������

    [Header("Inventory Settings")]
    public List<InventorySlot> slots = new List<InventorySlot>(); // ������ ������ ���������
    public List<Transform> backpackSlots = new List<Transform>(); // ������ ������ �������

    [Header("Interaction Settings")]
    [SerializeField] private float reachDistance = 3f; // ��������� �������������� � ����������

    [Header("References")]
    [SerializeField] private BackpackManager backpackManager; // �������� �������
    private Camera mainCamera; // �������� ������
    private FirstPersonLook firstPersonLook; // ��������� ���������� ������� �� ������� ����

    [Header("State")]
    public bool isOpened; // ��������� ��������� (������/������)

    /// <summary>
    /// ������������� UI ��������� ��� ������.
    /// </summary>
    private void Awake()
    {
        backgroundPanel.SetActive(true);
        inventoryPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// ������������� ����������� � ������ ���������.
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;
        firstPersonLook = FindFirstObjectByType<FirstPersonLook>();

        // ��������� ������ ������ ���������
        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            InventorySlot slot = inventoryPanel.GetChild(i).GetComponent<InventorySlot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }

        // �������� ��������� ��� ������
        backgroundPanel.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        firstPersonLook.freezeCamera = false;
    }

    /// <summary>
    /// ���������� ������ ��������� ������ ����.
    /// </summary>
    private void Update()
    {
        // �������� �� �������� ��������� �� ������� ������� ������ ����
        if (Input.GetKey(KeyCode.Q))
        {
            CheckInventoryUI();
        }

        // �������� �� ��������/�������� ��������� �� ������� Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        // �������������� � ���������� �� ������� E
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                ItemGameObject itemObject = hit.collider.gameObject.GetComponent<ItemGameObject>();
                if (itemObject != null)
                {
                    AddItem(itemObject.item, itemObject.amount);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// ����������� ��������� ��������� (������/������).
    /// </summary>
    public void ToggleInventory()
    {
        isOpened = !isOpened;
        if (isOpened)
        {
            OpenInventory(true, true, CursorLockMode.Locked);
        }
        else
        {
            CloseInventory();
        }
    }

    /// <summary>
    /// ���������, ����� �� ������� ��������� ��� ��������� �� ������.
    /// </summary>
    private void CheckInventoryUI()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance))
        {
            if (hit.collider.CompareTag("Backpack"))
            {
                OpenInventory(false, false, CursorLockMode.None);
            }
            else
            {
                CloseInventory();
            }
        }
        else
        {
            CloseInventory();
        }
    }

    /// <summary>
    /// ��������� ���������.
    /// </summary>
    /// <param name="freezeCamera">������������ �� ������ ��� ��������.</param>
    public void OpenInventory(bool freezeCamera, bool cursorVisible, Enum cursorLockState)
    {
        isOpened = true;
        backgroundPanel.SetActive(true);
        inventoryPanel.gameObject.SetActive(true);
        crosshairImage.SetActive(false);

        // �������� Enum � CursorLockMode
        if (cursorLockState is CursorLockMode)
        {
            Cursor.lockState = (CursorLockMode)cursorLockState;
        }
        else
        {
            Debug.LogError("Invalid cursor lock state provided.");
        }

        Cursor.visible = cursorVisible;
        firstPersonLook.freezeCamera = freezeCamera;
    }

    /// <summary>
    /// ��������� ���������.
    /// </summary>
    public void CloseInventory()
    {
        isOpened = false;
        backgroundPanel.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        crosshairImage.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        firstPersonLook.freezeCamera = false;
    }

    /// <summary>
    /// ���������, ������ �� ���������.
    /// </summary>
    /// <returns>True, ���� ��������� ������, ����� False.</returns>
    public bool IsInventoryOpen()
    {
        return isOpened;
    }

    /// <summary>
    /// ������� ������� �� ��������� �� �����.
    /// </summary>
    /// <param name="name">��� ��������.</param>
    public void RemoveItem(string itemName)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null && slot.item.itemName == itemName)
            {
                slot.item = null;
                slot.amount = 0;
                slot.isEmpty = true;
                slot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                slot.iconGO.GetComponent<Image>().sprite = null;
                slot.itemAmountText.text = "";

                Debug.Log($"������� {itemName} ������ �� ���������.");
                return;
            }
        }

        Debug.LogWarning($"������� {itemName} �� ������ � ���������.");
    }

    /// <summary>
    /// ��������� ������� � ���������.
    /// </summary>
    /// <param name="_item">������ ��������.</param>
    /// <param name="_amount">���������� ���������.</param>
    public void AddItem(ItemScriptableObject _item, int _amount)
    {
        // ���������, ���� �� ��� ����� ������� � ���������
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == _item)
            {
                if (slot.amount + _amount <= _item.maximumAmount)
                {
                    slot.amount += _amount;
                    slot.itemAmountText.text = slot.amount.ToString();
                    return;
                }
                break;
            }
        }

        // ���� ������ ���� ��� ������ ��������
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                slot.itemAmountText.text = _amount.ToString();
                break;
            }
        }
    }
}