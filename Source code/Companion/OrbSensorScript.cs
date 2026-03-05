using System.Collections.Generic;
using UnityEngine;

public class OrbSensorScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<GameObject> orbs = new();
    public bool SensesOrbs { get { return orbs.Count > 0; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Orb") && !orbs.Contains(other.gameObject))
            orbs.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Orb"))
            orbs.RemoveAll(o => o == other.gameObject);

    }

    public Vector3? GetClosestOrbPos(Vector3 worldPos)
    {
        if (orbs.Count == 0) return null;

        //removes destroyed orbs from list, prevents null reference exception
        orbs.RemoveAll(x => x == null);

        List<int> orbsToRemove = new List<int>();
        for (int i = 0; i < orbs.Count; i++)
        {
            if (!orbs[i].activeInHierarchy)
            {
                orbsToRemove.Add(i);
            }
        }

        for (int i = orbsToRemove.Count - 1; i >= 0; i--)
        {
            orbs.RemoveAt(orbsToRemove[i]);
        }

        int closestIndex = -1;
        float closestDistance = float.MaxValue;
        for(int i = 0; i < orbs.Count; i++)
        {
            float currentOrbDist = Vector3.Distance(orbs[i].transform.position, worldPos);
            if (currentOrbDist < closestDistance)
            {
                closestDistance = currentOrbDist;
                closestIndex = i;
            }

        }

        if (closestIndex >= 0 && orbs.Count > 0)
            return orbs[closestIndex].transform.position;
        else
            return null;
    }
}
