using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickPosition : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] CompanionScriptWrapper companion;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float maxRayDistance = 200f;

    public void OnAttack(InputValue value)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
        {
            Vector3 clickPoint = hit.point;

            companion.GetICompanion.GiveCommand(clickPoint);
        }
    }
}
