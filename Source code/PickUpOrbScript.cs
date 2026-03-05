using TMPro;
using UnityEngine;

public class PickUpOrbScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI orbCountText;
    [SerializeField] int orbCount;
    public int OrbCount { get { return orbCount; } }
    void UpdateText() => orbCountText.text = orbCount.ToString();

    private void Start()
    {
        UpdateText();
    }
    public void Add()
    {
        orbCount++;
        UpdateText();
    }
    public void Remove()
    {
        orbCount--;
        UpdateText();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
        {
            Add();
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}
