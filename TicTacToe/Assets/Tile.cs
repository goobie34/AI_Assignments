using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject highlight;
    public int ID { get; set; }

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        GameManager.Instance.HumanMove(ID);
    }
}
