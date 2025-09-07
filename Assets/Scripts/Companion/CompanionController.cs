using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// implementation of the command pattern for the companion character

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
        if (commandQueue.Count > 0)
        {
            //Debug.Log("commands in queue: " + commandQueue.Count);
            commandQueue.Peek().Execute();
            if (commandQueue.Peek().IsCommandComplete())
            {
                //Debug.Log("command complete, ending");
                FinishCommand();
            }
        }

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
