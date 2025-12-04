using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 5;
    Vector2 movementInput = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);
        controller.Move(movement * speed * Time.deltaTime);

    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }
}
