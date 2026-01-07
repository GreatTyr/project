using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference interactAction; // Action = F

    [Header("Interaction")]
    [SerializeField] private float interactCooldown = 0.1f;

    [Header("UI")]
    [SerializeField] private GameObject interactHintUI; // Объект с текстом "F — взаимодействовать"

    private IInteractable currentInteractable;
    private float lastInteractTime;
    private bool interactRequested;

    private void Awake()
    {
        // На всякий случай выключим подсказку при старте
        if (interactHintUI != null)
        {
            interactHintUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[PlayerInteraction] interactHintUI не назначен в инспекторе");
        }

        // Проверим, есть ли триггер-коллайдер на этом объекте
        var colliders = GetComponents<Collider>();
        bool hasTrigger = false;
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                hasTrigger = true;
                break;
            }
        }

        if (!hasTrigger)
        {
            Debug.LogWarning("[PlayerInteraction] На объекте игрока нет Collider.isTrigger = true. " +
                             "Для зоны взаимодействия добавь, например, SphereCollider isTrigger=true.");
        }
    }

    private void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            interactAction.action.Enable();
        }
        else
        {
            Debug.LogWarning("[PlayerInteraction] InteractAction не назначен или action == null");
        }
    }

    private void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        interactRequested = true;
    }

    private void Update()
    {
        if (!interactRequested)
            return;

        interactRequested = false;
        TryInteract();
    }

    private void TryInteract()
    {
        if (currentInteractable == null)
        {
            Debug.Log("[PlayerInteraction] TryInteract: currentInteractable == null");
            return;
        }

        if (Time.time - lastInteractTime < interactCooldown)
            return;

        lastInteractTime = Time.time;
        Debug.Log("[PlayerInteraction] Взаимодействуем с: " +
                  ((MonoBehaviour)currentInteractable).gameObject.name);

        currentInteractable.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Игнорируем собственные коллайдеры
        if (other.gameObject == gameObject)
        {
            Debug.Log("[PlayerInteraction] OnTriggerEnter: игнорируем self (" + other.name + ")");
            return;
        }

        Debug.Log("[PlayerInteraction] OnTriggerEnter from: " + other.name);

        // Ищем IInteractable на объекте, родителях и детях
        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInChildren<IInteractable>();

        if (interactable != null)
        {
            Debug.Log("[PlayerInteraction] Нашли IInteractable на/рядом с объектом: " + other.name);

            currentInteractable = interactable;
            SetInteractHintVisible(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Игнорируем выход собственного коллайдера
        if (other.gameObject == gameObject)
        {
            Debug.Log("[PlayerInteraction] OnTriggerExit: игнорируем self (" + other.name + ")");
            return;
        }

        Debug.Log("[PlayerInteraction] OnTriggerExit from: " + other.name);

        if (currentInteractable == null)
            return;

        var interactable = other.GetComponentInParent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInChildren<IInteractable>();

        if (interactable != null && ReferenceEquals(currentInteractable, interactable))
        {
            Debug.Log("[PlayerInteraction] Вышли из зоны взаимодействия объекта: " + other.name);

            currentInteractable = null;
            SetInteractHintVisible(false);
        }
    }

    private void SetInteractHintVisible(bool visible)
    {
        Debug.Log("[PlayerInteraction] SetInteractHintVisible: " + visible);

        if (interactHintUI != null)
        {
            interactHintUI.SetActive(visible);
        }
        else
        {
            Debug.LogWarning("[PlayerInteraction] interactHintUI == null — UI-подсказка не будет показана");
        }
    }
}