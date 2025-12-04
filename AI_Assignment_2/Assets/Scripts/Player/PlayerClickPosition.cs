using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickPosition : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] LayerMask layerMask;
    [SerializeField] CompanionScriptWrapper companion;

    public void OnAttack(InputValue value)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 200f, layerMask))
        {
            Vector3 clickPoint = hit.point;

            companion.GetCompanion.GiveCommand(clickPoint);
        }
    }
}
