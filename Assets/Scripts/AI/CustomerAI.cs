using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Sources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : BaseAI
{
    // Start is called before the first frame update
    void Start()
    {
        // Find nearest available seat
        // Path to seat
        // When at seat, change state to sitting
        // Generate an order
        // Eat order, leave money
        // OR leave angrily due to wait

        // Find the nearest available seat
        foreach (InteractableChair chair in InteractableManager.instance.chairs)
        {
            Seat seat = chair.HasSpace();
            if (seat != null)
            {
                navMeshAgent.SetDestination(chair.transform.position);
                chair.Interact(this.gameObject);
                StartCoroutine(EnterSeat(seat)); // Start coroutine for seating
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Coroutine process to enter seat once arrived and begin order generation after
    IEnumerator EnterSeat(Seat seat)
    {
        yield return new WaitForSeconds(0.5f); // Wait so that the stupid nav mesh actually has time to think
        yield return new WaitUntil(() => navMeshAgent.remainingDistance < 0.51f);
        SittingState(seat);

        // Change state to sitting
        // Generate an order
        // Eat order, leave money
        // OR leave angrily due to wait
    }

    private void SittingState(Seat seat)
    {
        // Disable navmesh
        navMeshAgent.enabled = false;

        // Set transform to seat transform
        transform.position = seat.transform.position + new Vector3(0, 0.5f, 0); // TODO: actually generate proper offset for animation for customers
        transform.rotation = seat.transform.rotation;
    }
}
