using UnityEngine;

public class BackpackUIHandler : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject Inventory; // ������ �� UI �������

    [Header("Raycast Settings")]
    public float pickupDistance = 3f; // ��������� ��� Raycast
    public string backpackTag = "Backpack"; // ��� �������

    private Camera mainCamera;
    private InventoryManager inventoryManager;

    public bool fromBackpackOpen;

    private void Start()
    {
        // �������� �������� ������
        mainCamera = Camera.main;

        // ������� InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager �� ������!");
        }

        if (Inventory != null)
        {
            Inventory.SetActive(false);
        }
    }

    private void Update()
    {
        // �������� ��������� �� ������ � ��������� ���
        CheckInventoryUI();
    }

    private void CheckInventoryUI()
    {
        // ���� ��������� ������, �� ��������� UI �������
        if (inventoryManager != null && inventoryManager.IsInventoryOpen())
        {
            CloseInventoryUI();
            return;
        }

        // ������� ��� �� ������ ������
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // ���������, ����� �� ��� � ������ � ����� "Backpack"
        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag(backpackTag))
            {
                // ���� ������� �� ������ � ������������ ���
                if (Input.GetMouseButton(0)) // ��� ������������
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