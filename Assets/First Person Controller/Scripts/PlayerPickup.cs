using UnityEngine;

/// <summary>
/// Класс для управления подбором, переносом и выбрасыванием предметов игроком.
/// Также позволяет прикреплять предметы к рюкзаку.
/// </summary>
public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupDistance = 3f; // Дистанция подбора предметов
    [SerializeField] private float throwForce = 10f; // Сила броска предмета
    [SerializeField] private Transform holdPosition; // Позиция удержания предмета
    [SerializeField] private float maxPickupMass = 10f; // Максимальная масса подбираемого предмета
    [SerializeField] private string pickupTag = "Pickable"; // Тег подбираемых предметов
    [SerializeField] private string backpackTag = "Backpack"; // Тег рюкзака
    [SerializeField] private float attachSpeed = 5f; // Скорость прикрепления предмета к рюкзаку

    [Header("References")]
    [SerializeField] private BackpackManager backpackManager; // Менеджер рюкзака
    [SerializeField] private InventoryManager inventoryManager; // Менеджер инвентаря

    private GameObject heldObject; // Текущий удерживаемый объект
    private Rigidbody heldObjectRb; // Rigidbody удерживаемого объекта
    private bool isAttachingToBackpack; // Флаг прикрепления объекта к рюкзаку
    private Transform backpackTarget; // Целевой слот рюкзака для прикрепления

    /// <summary>
    /// Инициализация менеджера рюкзака при старте.
    /// </summary>
    private void Start()
    {
        GameObject backpackObject = GameObject.FindGameObjectWithTag(backpackTag);
        if (backpackObject != null)
        {
            backpackManager = backpackObject.GetComponent<BackpackManager>();
        }
    }

    /// <summary>
    /// Обновление логики подбора и перемещения предметов каждый кадр.
    /// </summary>
    private void Update()
    {
        HandlePickupInput();
        MoveHeldObject();

        if (isAttachingToBackpack)
        {
            AttachToBackpack();
        }
    }

    /// <summary>
    /// Обрабатывает ввод игрока для подбора, выбрасывания и прикрепления предметов.
    /// </summary>
    private void HandlePickupInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                ThrowObject();
            }
        }

        if (heldObject != null && Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, pickupDistance))
            {
                if (hit.collider.CompareTag(backpackTag))
                {
                    StartAttachToBackpack(hit.transform);
                }
            }
        }
    }

    /// <summary>
    /// Пытается подобрать объект, если он находится в зоне досягаемости.
    /// </summary>
    private void TryPickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupDistance))
        {
            if (hit.collider.CompareTag(pickupTag))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null && rb.mass <= maxPickupMass)
                {
                    PickupObject(hit.collider.gameObject, rb);
                    backpackManager.DetachObject(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log("Объект слишком тяжелый или не имеет Rigidbody.");
                }
            }
        }
    }

    /// <summary>
    /// Подбирает объект и начинает его удерживать.
    /// </summary>
    /// <param name="obj">Объект для подбора.</param>
    /// <param name="rb">Rigidbody объекта.</param>
    private void PickupObject(GameObject obj, Rigidbody rb)
    {
        heldObject = obj;
        heldObjectRb = rb;

        heldObjectRb.useGravity = false;
        heldObjectRb.isKinematic = false;
    }

    /// <summary>
    /// Перемещает удерживаемый объект к позиции удержания.
    /// </summary>
    private void MoveHeldObject()
    {
        if (heldObject != null && !isAttachingToBackpack)
        {
            Vector3 targetPosition = holdPosition.position;
            Vector3 moveDirection = targetPosition - heldObject.transform.position;
            heldObjectRb.velocity = moveDirection * 10f;

            heldObjectRb.angularVelocity = Vector3.zero;
            heldObject.transform.rotation = Quaternion.Slerp(heldObject.transform.rotation, holdPosition.rotation, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// Выбрасывает удерживаемый объект.
    /// </summary>
    private void ThrowObject()
    {
        if (heldObjectRb != null)
        {
            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
            heldObjectRb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }

        heldObject = null;
        heldObjectRb = null;
    }

    /// <summary>
    /// Начинает процесс прикрепления объекта к рюкзаку.
    /// </summary>
    /// <param name="backpack">Трансформ рюкзака.</param>
    private void StartAttachToBackpack(Transform backpack)
    {
        isAttachingToBackpack = true;
        backpackTarget = backpack;
    }

    /// <summary>
    /// Прикрепляет объект к рюкзаку.
    /// </summary>
    private void AttachToBackpack()
    {
        if (heldObject == null || backpackTarget == null)
        {
            isAttachingToBackpack = false;
            return;
        }

        Vector3 targetPosition = backpackTarget.position;
        Vector3 moveDirection = targetPosition - heldObject.transform.position;
        heldObjectRb.velocity = moveDirection * attachSpeed;

        if (moveDirection.magnitude < 0.8f)
        {
            FinishAttachToBackpack();
        }
    }

    /// <summary>
    /// Завершает процесс прикрепления объекта к рюкзаку.
    /// </summary>
    private void FinishAttachToBackpack()
    {
        bool isAttached = backpackManager.AttachObject(heldObject);
        if (isAttached)
        {
            ItemGameObject itemObject = heldObject.GetComponent<ItemGameObject>();
            if (itemObject != null)
            {
                inventoryManager.AddItem(itemObject.item, itemObject.amount);
            }

            heldObject = null;
            heldObjectRb = null;
        }
        else
        {
            ThrowObject();
        }

        isAttachingToBackpack = false;
        backpackTarget = null;
    }
}