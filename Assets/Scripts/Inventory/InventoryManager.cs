using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System;

/// <summary>
/// Класс для управления инвентарем. Отвечает за открытие/закрытие инвентаря, добавление и удаление предметов.
/// </summary>
public class InventoryManager : UIWidget
{
    [Header("UI Elements")]
    [SerializeField] private GameObject backgroundPanel; // Фоновая панель инвентаря
    [SerializeField] private GameObject crosshairImage; // Изображение прицела
    [SerializeField] private Transform inventoryPanel; // Панель слотов инвентаря

    [Header("Inventory Settings")]
    public List<InventorySlot> slots = new List<InventorySlot>(); // Список слотов инвентаря
    public List<Transform> backpackSlots = new List<Transform>(); // Список слотов рюкзака

    [Header("Interaction Settings")]
    [SerializeField] private float reachDistance = 3f; // Дистанция взаимодействия с предметами

    [Header("References")]
    [SerializeField] private BackpackManager backpackManager; // Менеджер рюкзака
    private Camera mainCamera; // Основная камера
    private FirstPersonLook firstPersonLook; // Компонент управления камерой от первого лица

    [Header("State")]
    public bool isOpened; // Состояние инвентаря (открыт/закрыт)

    /// <summary>
    /// Инициализация UI элементов при старте.
    /// </summary>
    private void Awake()
    {
        backgroundPanel.SetActive(true);
        inventoryPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Инициализация компонентов и слотов инвентаря.
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;
        firstPersonLook = FindFirstObjectByType<FirstPersonLook>();

        // Заполняем список слотов инвентаря
        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            InventorySlot slot = inventoryPanel.GetChild(i).GetComponent<InventorySlot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }

        // Скрываем инвентарь при старте
        backgroundPanel.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        firstPersonLook.freezeCamera = false;
    }

    /// <summary>
    /// Обновление логики инвентаря каждый кадр.
    /// </summary>
    private void Update()
    {
        // Проверка на открытие инвентаря по нажатию средней кнопки мыши
        if (Input.GetKey(KeyCode.Q))
        {
            CheckInventoryUI();
        }

        // Проверка на открытие/закрытие инвентаря по нажатию Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        // Взаимодействие с предметами по нажатию E
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
    /// Переключает состояние инвентаря (открыт/закрыт).
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
    /// Проверяет, нужно ли открыть инвентарь при наведении на рюкзак.
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
    /// Открывает инвентарь.
    /// </summary>
    /// <param name="freezeCamera">Замораживает ли камеру при открытии.</param>
    public void OpenInventory(bool freezeCamera, bool cursorVisible, Enum cursorLockState)
    {
        isOpened = true;
        backgroundPanel.SetActive(true);
        inventoryPanel.gameObject.SetActive(true);
        crosshairImage.SetActive(false);

        // Приводим Enum к CursorLockMode
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
    /// Закрывает инвентарь.
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
    /// Проверяет, открыт ли инвентарь.
    /// </summary>
    /// <returns>True, если инвентарь открыт, иначе False.</returns>
    public bool IsInventoryOpen()
    {
        return isOpened;
    }

    /// <summary>
    /// Удаляет предмет из инвентаря по имени.
    /// </summary>
    /// <param name="name">Имя предмета.</param>
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

                Debug.Log($"Предмет {itemName} удален из инвентаря.");
                return;
            }
        }

        Debug.LogWarning($"Предмет {itemName} не найден в инвентаре.");
    }

    /// <summary>
    /// Добавляет предмет в инвентарь.
    /// </summary>
    /// <param name="_item">Данные предмета.</param>
    /// <param name="_amount">Количество предметов.</param>
    public void AddItem(ItemScriptableObject _item, int _amount)
    {
        // Проверяем, есть ли уже такой предмет в инвентаре
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

        // Ищем пустой слот для нового предмета
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