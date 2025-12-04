using Unity.VisualScripting;
using UnityEngine;

public class CompanionScript : MonoBehaviour, ICompanion
{
    Vector3? targetPoint = null;
    [SerializeField] float targetPointAcceptanceRadius;
    Vector3? closestOrb = null;

    bool hasPlayerGivenComand = false;
    bool hasTargetBeenVisited = false;

    [SerializeField] OrbSensorScript sensorScript;
    [SerializeField] PickUpOrbScript orbPickupScript;
    [SerializeField] CharacterController controller;
    [SerializeField] Transform body;
    [SerializeField] float speed;

    [SerializeField] float returnToPlayerThreshold;
    [SerializeField] GameObject player;

    [SerializeField] float searchSpeed;
    [SerializeField] float searchAngle;
    float searchTimer;
    [SerializeField] float maxSearchTime;    
    bool isSearching = false;

    [SerializeField] float deliverCooldown;
    [SerializeField] float deliverDistance;
    float deliverCooldownTimer;

    public bool HasPlayerGivenCommand() { return hasPlayerGivenComand; }
    public bool HasTargetBeenVisited() { return hasTargetBeenVisited; }
    public void GiveCommand(Vector3 targetPosition)
    {
        Debug.Log("COMMAND GIVEN");
        hasPlayerGivenComand = true;
        hasTargetBeenVisited = false;
        targetPoint = targetPosition;
    }

    private void Update()
    {
        GoToTarget();
    }
    public void SetTargetPosition(Vector3 targetPosition)
    {
        targetPoint = targetPosition;
    }
    public void GoToTarget()
    {
        if (targetPoint == null) return;

        MoveTowards((Vector3)targetPoint);

        Vector3 tempVec = (Vector3)targetPoint;
        tempVec.y = transform.position.y;

        if (Vector3.Distance(tempVec, transform.position) < targetPointAcceptanceRadius)
        {
            targetPoint = null;
            hasTargetBeenVisited = true;
        }
    }

    public void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > returnToPlayerThreshold)
            MoveTowards(player.transform.position);
    }
    public void GoToOrb()
    {
        if (closestOrb != null)
            MoveTowards((Vector3)closestOrb);
    }
    public bool SearchForOrbs()
    {
        Debug.Log("IS SEARCHING FOR ORBS");
        Debug.Log(searchTimer);
        if (isSearching == false)
        {
            isSearching = true;
            StartSearchAnimation();
            searchTimer = 0;
        }

        searchTimer += Time.deltaTime;
        //float angle = Mathf.Sin(searchTimer) * searchAngle;
        //transform.localRotation = Quaternion.Euler(0f, angle, 0f);

        if (searchTimer > maxSearchTime)
        {
            isSearching = false;
            InterruptSearchAnimation();
            Debug.Log("ITS FALSE ITS FALSE I PROMISE");
        }

        return isSearching;
    }

    void StartSearchAnimation()
    {
        //animator.enabled = true;
        //animator.SetBool("IsSearching", true);
    }
    void InterruptSearchAnimation()
    {
        //animator.SetBool("IsSearching", false);
        //animator.enabled = false;
    }
    public void ReturnToPlayer()
    {
        MoveTowards(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) < deliverDistance)
        {
            while (orbPickupScript.OrbCount > 0)
            {
                orbPickupScript.Remove();
                player.GetComponent<PickUpOrbScript>().Add();
            }
            hasPlayerGivenComand = false;
        }

    }

    public bool CanSenseOrbs()
    {
        if (sensorScript != null)
        {
            closestOrb = sensorScript.GetClosestOrbPos(transform.position);
            return sensorScript.SensesOrbs;
        }

        closestOrb = null;
        return false;
    }
    void MoveTowards(Vector3 targetPos)
    {
        Vector3 moveDirectionWithSpeed = (targetPos - transform.position).normalized * speed;
        controller.Move(moveDirectionWithSpeed * Time.deltaTime);

        Vector3 moveDirectionWithSpeed_2d= new Vector3(moveDirectionWithSpeed.x, 0f, moveDirectionWithSpeed.z);
        
        if (moveDirectionWithSpeed_2d.sqrMagnitude > 0.001f)
        {
            body.rotation = Quaternion.LookRotation(moveDirectionWithSpeed_2d, Vector3.up);
        }
    }

    public static void MoveTowards(CharacterController characterController, Transform characterTransform, Vector3 targetPosition, float speed)
    {
        Vector3 moveDirectionWithSpeed = (targetPosition - characterTransform.position).normalized * speed;
        characterController.Move(moveDirectionWithSpeed * Time.deltaTime);

        Vector3 moveDirectionWithSpeed_2d = new Vector3(moveDirectionWithSpeed.x, 0f, moveDirectionWithSpeed.z);
        
        if (moveDirectionWithSpeed_2d.sqrMagnitude > 0.001f)
        {
            characterTransform.rotation = Quaternion.LookRotation(moveDirectionWithSpeed_2d, Vector3.up);
        }
    }
}
