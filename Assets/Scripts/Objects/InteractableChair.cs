using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractableChair : BaseInteractable
{
    // Keep a list of all the available places to sit
    private List<Seat> seats;

    // Start is called before the first frame update
    void Start()
    {
        // Get all the seats attached to this object
        seats = GetComponentsInChildren<Seat>().ToList<Seat>();

        InteractableManager.instance.RegisterFurniture(this);
    }

    // Whether or not the chair has space for another person. Returns null if no space is found.
    public Seat HasSpace()
    {
        // Check each seat until an available one is found
        foreach (Seat seat in seats)
        {
            if (!seat.isOccupied)
                return seat;
        }

        // Return null if no available seat is found
        return null;
    }

    // Occupy the first available seat
    public override void Interact(GameObject interactor)
    {
        Seat seat = HasSpace();
        seat.isOccupied = true;
    }
}
