// SimpleInteractable.cs
using UnityEngine;

public class SimpleInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactMessage = "Взаимодействие";

    public void Interact()
    {
        Debug.Log($"{interactMessage} с объектом: {gameObject.name}");
        // Тут потом: открыть дверь / поднять предмет / начать диалог и т.д.
    }
}