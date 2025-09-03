using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// implementation of the command pattern for the companion character

public class CompanionController : MonoBehaviour
{
    [SerializeField] private Queue<Command> commandQueue = new Queue<Command>();
    private NavMeshAgent agent;

    private void Update()
    {
        if (commandQueue.Count > 0)
        {
            Debug.Log("commands in queue: " + commandQueue.Count);
            commandQueue.Peek().Execute();
            if (commandQueue.Peek().IsCommandComplete())
            {
                Debug.Log("command complete, ending");
                FinishCommand();
            }
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
