using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enumerator to define the type of interaction
public enum EInteractableType
{
    Furniture = 0,
    Workstation = 1
}

public abstract class BaseInteractable : MonoBehaviour
{
    [SerializeField] protected EInteractableType _InteractableType = EInteractableType.Furniture;
    public EInteractableType InteractableType => _InteractableType;

    public abstract bool CanInteract(); // Whether or not someone can interact with this
    public abstract void Interact(MonoBehaviour interactor); // Interaction logic for this interactable
}
