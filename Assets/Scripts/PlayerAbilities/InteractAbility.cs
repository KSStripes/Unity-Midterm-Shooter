using UnityEngine;

public class InteractAbility : MonoBehaviour
{
    [Header("Interaction Variables")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask selectableLayer;
    [SerializeField] private Camera head;
    [SerializeField] private KeyCode buttonKey = KeyCode.F;

    private ISelectable currentHover;

    public void Interact()
    {
        Ray ray = head.ScreenPointToRay(Input.mousePosition); //fix target ray to mouse position

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, selectableLayer))
        {
            ISelectable sel = hit.collider.GetComponent<ISelectable>();

            if (sel != null)
            {
                if (currentHover != sel)
                {
                    currentHover?.OnHoverExit();
                    sel.OnHoverEnter();
                    currentHover = sel;
                }


                if (Input.GetKeyDown(buttonKey))
                {
                    sel.OnSelect();
                }
            }

        }
        else
        {
            if (currentHover != null)
            {
                currentHover.OnHoverExit();
                currentHover = null;
            }
        }
    }
}
