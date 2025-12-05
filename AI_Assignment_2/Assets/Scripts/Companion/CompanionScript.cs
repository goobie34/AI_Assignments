using Mono.Cecil.Cil;
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
    [SerializeField] float goToOrbThreshold;
    [SerializeField] GameObject player;

    [SerializeField] float searchSpeed;
    [SerializeField] float searchAngle;
    float searchTimer = 0f;
    [SerializeField] float maxSearchTime;
    bool isSearching = false;
    bool hasSearched = false;

    [SerializeField] float deliverCooldown;
    [SerializeField] float deliverDistance;
    float deliverTimer;

    Transform bodyRotationBeforeSearch;

    public bool HasPlayerGivenCommand() { /*Debug.Log("HAS PLAYER GIVEN COMMAND???");*/ return hasPlayerGivenComand; }
    public bool HasTargetBeenVisited() { /*Debug.Log("HAS TARGET BEEN VISITED???");*/  return hasTargetBeenVisited; }
    public void GiveCommand(Vector3 targetPosition)
    {
        Debug.Log("COMMAND GIVEN");
        hasPlayerGivenComand = true;
        hasTargetBeenVisited = false;
        targetPoint = targetPosition;
    }
    public bool HasSearched()
    {
        return hasSearched;
    }
    public bool LookAround()
    {
        Debug.Log("LOOKING AROUND");
        //if (!isSearching)
        //{
        //    //bodyRotationBeforeSearch = body;
        //    isSearching = true;
        //}
        searchTimer += Time.deltaTime;
        searchAngle = searchSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, searchAngle);

        if (CanSenseOrbs()) return true;

        if (searchTimer > maxSearchTime)
        {
            searchTimer = 0;
            hasSearched = true;
            return true;
        }

        return false;
    }

    //private void Update()
    //{
    //    GoToTarget();
    //}
    public void SetTargetPosition(Vector3 targetPosition)
    {
        targetPoint = targetPosition;
    }
    public bool GoToTarget()
    {
        Debug.Log("Going to target");

        if (targetPoint == null) return false;

        if (CanSenseOrbs()) return true;

        Vector3 tempVec = (Vector3)targetPoint;
        tempVec.y = transform.position.y;

        if (Vector3.Distance(tempVec, transform.position) < targetPointAcceptanceRadius)
        {
            targetPoint = null;
            hasTargetBeenVisited = true;
            return true;
        }

        MoveTowards((Vector3)targetPoint);
        return false;
    }

    public void FollowPlayer()
    {
        Debug.Log("Following player");

        if (Vector3.Distance(transform.position, player.transform.position) > returnToPlayerThreshold)
            MoveTowards(player.transform.position);
    }
    public bool GoToOrb()
    {
        Debug.Log("Going to orb");


        if (closestOrb != null)
            MoveTowards((Vector3)closestOrb);

        if (Vector3.Distance(transform.position, (Vector3)closestOrb) < goToOrbThreshold)
        {
            return true;
        }
        return false;
    }
    public void PickUpOrb()
    {
        Debug.Log("PICKING UP ORB");
        hasSearched = false; 
    }

    public bool SearchForOrbs()
    {
        //Debug.Log("IS SEARCHING FOR ORBS");
        //Debug.Log(searchTimer);
        //if (isSearching == false)
        //{
        //    isSearching = true;
        //    StartSearchAnimation();
        //    searchTimer = 0;
        //}

        //searchTimer += Time.deltaTime;
        ////float angle = Mathf.Sin(searchTimer) * searchAngle;
        ////transform.localRotation = Quaternion.Euler(0f, angle, 0f);

        //if (searchTimer > maxSearchTime)
        //{
        //    isSearching = false;
        //    InterruptSearchAnimation();
        //    Debug.Log("ITS FALSE ITS FALSE I PROMISE");
        //}

        //return isSearching;
        return false;
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
    public bool ReturnToPlayer()
    {
        Debug.Log("Returning to player");

        if (CanSenseOrbs()) //interrupt this action if orbs are sensed nearby
        {
            return true;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < deliverDistance)
        {
            //while (orbPickupScript.OrbCount > 0)
            //{
            //    orbPickupScript.Remove();
            //    player.GetComponent<PickUpOrbScript>().Add();
            //}
            hasPlayerGivenComand = false;
            return true;
        }

        MoveTowards(player.transform.position);

        return false;
    }
    public bool HasOrbs()
    {
        return orbPickupScript.OrbCount > 0;
    }
    public bool DeliverOrbs()
    {
        Debug.Log("Delivering orbs");

        if (!HasOrbs()) return true;

        if (Vector3.Distance(transform.position, player.transform.position) < deliverDistance)
        {
            deliverTimer += Time.deltaTime;
            if (deliverTimer > deliverCooldown)
            {
                deliverTimer = 0;
                orbPickupScript.Remove();
                player.GetComponent<PickUpOrbScript>().Add();
            }
        }
        else
        {
            MoveTowards(player.transform.position);
        }

        return false;
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
