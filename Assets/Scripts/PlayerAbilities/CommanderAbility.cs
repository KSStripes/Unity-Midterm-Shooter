using UnityEngine;

public class CommanderAbility : MonoBehaviour
{
    [SerializeField] private LayerMask compatibleWithCommands;
    [SerializeField] private CompanionController companion;
    [SerializeField] private GameObject wayPointPrefab;
    [SerializeField] private Camera head;
    [SerializeField] private float commandRange = 15f;

    private void Awake()
    {
        if (companion == null)
        {
            companion = FindObjectOfType<CompanionController>();
        }
    }

    public void Command()
    {
        Ray ray = head.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, commandRange, compatibleWithCommands))
        {
            Instantiate(wayPointPrefab, hit.point, Quaternion.identity);
            companion.GiveCommand(new MoveCommand(hit.point));
        }
    }
}
