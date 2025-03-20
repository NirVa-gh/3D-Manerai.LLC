using UnityEngine;

public class BackpackUIHandler : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject Inventory; // Ссылка на UI рюкзака

    [Header("Raycast Settings")]
    public float pickupDistance = 3f; // Дистанция для Raycast
    public string backpackTag = "Backpack"; // Тег рюкзака

    private Camera mainCamera;
    private InventoryManager inventoryManager;

    public bool fromBackpackOpen;

    private void Start()
    {
        // Получаем основную камеру
        mainCamera = Camera.main;

        // Находим InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager не найден!");
        }

        if (Inventory != null)
        {
            Inventory.SetActive(false);
        }
    }

    private void Update()
    {
        // Проверка наведения на рюкзак и удержания ЛКМ
        CheckInventoryUI();
    }

    private void CheckInventoryUI()
    {
        // Если инвентарь открыт, не открываем UI рюкзака
        if (inventoryManager != null && inventoryManager.IsInventoryOpen())
        {
            CloseInventoryUI();
            return;
        }

        // Создаем луч из центра экрана
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Проверяем, попал ли луч в объект с тегом "Backpack"
        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag(backpackTag))
            {
                // Если наведен на рюкзак и удерживается ЛКМ
                if (Input.GetMouseButton(0)) // ЛКМ удерживается
                {
                    OpenInventoryUI();
                }
                else
                {
                    CloseInventoryUI();
                }
            }
            else
            {
                CloseInventoryUI();
            }
        }
        else if (inventoryManager.IsInventoryOpen())
        {
            OpenInventoryUI();
        }
        else
        {
            CloseInventoryUI();
        }
    }

    private void OpenInventoryUI()
    {
        fromBackpackOpen = true;
        if (Inventory != null && !inventoryManager.IsInventoryOpen())
        {
            Inventory.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseInventoryUI()
    {
        fromBackpackOpen = false;
        if (Inventory != null && Inventory.activeSelf)
        {
            Inventory.SetActive(false);
            if (inventoryManager == null || !inventoryManager.IsInventoryOpen())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}