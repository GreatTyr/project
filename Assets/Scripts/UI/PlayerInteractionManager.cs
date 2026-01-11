// PlayerInteractionManager.cs
// Ставится на Player (или объект, управляющий взаимодействием).
// Обнаруживает hovered интерактивный объект (через вызов OnObjectHovered/Unhovered; эти вызовы выполняются InteractableBase при OnTriggerEnter/Exit).
// Подключает InputAction (кнопка F) и при нажатии вызывает Interact() на текущем hovered объекте.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInteractionManager : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Ссылка на Input Action (Player -> Interact)")]
    public InputActionReference interactAction; // назначить через инспектор

    Interactable currentHovered; // текущее наведённое (может быть null)

    void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            interactAction.action.Enable();
        }
        else
        {
            Debug.LogWarning("[PlayerInteractionManager] interactAction не назначен или action == null");
        }
    }

    void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    void Update()
    {
        // Возможное место для raycast-проверки "смотрит ли игрок на объект" (опционально).
    }

    // Вызывается InteractableBase при входе игрока в зону взаимодействия
    public void OnObjectHovered(Interactable interactable)
    {
        currentHovered = interactable;
        // Показать подсказку с текстом из интерактивного объекта (hintText)
        var baseComp = (interactable as MonoBehaviour)?.GetComponent<InteractableBase>();
        InteractionHintUI.Instance?.SetVisible(true, baseComp != null ? baseComp.hintText : "Нажмите F");
        // Вызвать callback hover
        interactable?.OnHoverEnter();
    }

    // Вызывается InteractableBase при выходе из зоны
    public void OnObjectUnhovered(Interactable interactable)
    {
        if (currentHovered == interactable) currentHovered = null;
        InteractionHintUI.Instance?.SetVisible(false);
        interactable?.OnHoverExit();
    }

    // Обработчик нажатия клавиши Interact (подписан на InputAction)
    void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (currentHovered != null)
        {
            currentHovered.Interact();
        }
    }
}