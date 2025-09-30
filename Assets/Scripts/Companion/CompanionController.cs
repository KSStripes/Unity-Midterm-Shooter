using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Handles command queue and movement for companion character
public class CompanionController : MonoBehaviour
{
    [SerializeField] private Queue<Command> commandQueue = new Queue<Command>();
    private NavMeshAgent agent;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Execute and finish commands in queue
        if (commandQueue.Count > 0)
        {
            commandQueue.Peek().Execute();
            if (commandQueue.Peek().IsCommandComplete())
            {
                FinishCommand();
            }
        }
        // Update animation based on movement
        anim.SetFloat("Velocity", agent.velocity.sqrMagnitude);
    }

    public void GiveCommand(Command newCommand)
    {
        newCommand.SetCompanionController(this);
        commandQueue.Enqueue(newCommand);
    }

    public void FinishCommand()
    {
        commandQueue.Dequeue();
    }

    public NavMeshAgent GetNavMeshAgent() => agent;
}