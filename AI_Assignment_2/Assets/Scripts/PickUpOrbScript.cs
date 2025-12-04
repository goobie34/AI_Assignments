using TMPro;
using UnityEngine;

public class PickUpOrbScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI orbCountText;
    [SerializeField] int orbCount;
    public int OrbCount { get { return orbCount; }}

    private void Start()
    {
        orbCountText.text = orbCount.ToString();
    }
    public void Add()
    {
        orbCount++;
        orbCountText.text = orbCount.ToString();
    }
    public void Remove()
    {
        orbCount--;
        orbCountText.text = orbCount.ToString();
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
