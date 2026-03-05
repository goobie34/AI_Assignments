using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera cam;
    private void Start()
    {
        if (cam == null) cam = Camera.main;
    }
    void Update()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}