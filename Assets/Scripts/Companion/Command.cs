using UnityEngine;

// Abstract base class for companion commands
public abstract class Command
{
    protected CompanionController companionController; // Reference to companion
    public void SetCompanionController(CompanionController companionController)
    {
        this.companionController = companionController;
    }
    public abstract void Execute(); // Run command logic
    public abstract bool IsCommandComplete(); // Check if done
    public abstract void Cancel(); // Cancel command
}