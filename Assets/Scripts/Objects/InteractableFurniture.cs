using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableFurniture : BaseInteractable
{
    [Header("Settings")]
    [SerializeField] protected int maxOccupants = 1;

    // Whether or not the furniture is full
    private bool isFullyOccupied = false;

    // How many users occupy the furniture
    private int currentOccupants = 0;

    // A list that contains the references for anchoring AI to furniture, and whether or not said specific seat is occupied
    private List<KeyValuePair<FurnitureSeat, bool>> seats; // Will be initialized if FurnitureSeat children are found in the components

    public void Start()
    {
        // Find all seat anchors attached to the furniture, if any
        FurnitureSeat[] foundSeats = GetComponentsInChildren<FurnitureSeat>();
        if (foundSeats.Length > 0)
        {
            seats = new List<KeyValuePair<FurnitureSeat, bool>>();
            foreach (var seat in foundSeats)
            {
                seats.Add(new KeyValuePair<FurnitureSeat, bool>(seat, false));
            }
        }
    }

    // Use this to ensure an AI does not path to an already occupied or going to be occupied furniture
    public override bool CanInteract()
    {
        return !isFullyOccupied;
    }

    public override void Interact(MonoBehaviour interactor)
    {
        // Log an error if an interaction passes while the chair is occupied
        if (!CanInteract())
        {
            Debug.LogError($"Attempting to interact with an already occupied object {gameObject.name}");
            return;
        }

        // do something with the interactor here idk man
        // should the interactor or the furniture handle placing the interactor on the seat?
        // probably both, but the interactor needs to know its seated and move onto its next state of ordering
    }

    public virtual void AddOccupant()
    {
        // Log an error if adding an occupant to a full piece of furniture
        if (currentOccupants >= maxOccupants)
        {
            Debug.LogError($"Adding occupants to an already fully occupied object {gameObject.name}");
            return;
        }

        currentOccupants += 1;
        if (currentOccupants == maxOccupants)
            isFullyOccupied = true;
    }

    public virtual void RemoveOccupant()
    {
        // Log an error if removing an occupant to an empty piece of furniture
        if (currentOccupants >= maxOccupants)
        {
            Debug.LogError($"Removing occupants to an empty object {gameObject.name}");
            return;
        }

        currentOccupants -= 1;
        isFullyOccupied = false;
    }
}
