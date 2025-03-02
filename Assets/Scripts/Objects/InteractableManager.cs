using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    // Singleton instance of this object
    static public InteractableManager instance { get; private set; }

    // Keep a list of different kinds of interactables
    public List<InteractableChair> chairs { get; private set; } // All chairs in the cafe

    //
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Two InteractableManagers detected. Deleting latest one.");
            Destroy(this);
        }

        // Update the instance
        instance = this;

        // Initialize lists during awake so interactables can call back to them upon start
        chairs = new List<InteractableChair>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Register interactable furniture
    public void RegisterFurniture(BaseInteractable furniture)
    {
        // Register chair to chairs list
        if (furniture is InteractableChair)
        {
            chairs.Add((InteractableChair)furniture);
        }
    }
}
