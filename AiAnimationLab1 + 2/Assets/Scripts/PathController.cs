using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    [SerializeField]
    public PathManager pathManager;

    List<WayPoint> thePath;
    WayPoint target;

    public float MoveSpeed;
    public float RotateSpeed;

    public Animator animator;
    bool isSprinting;

    //start is called before the first frame update
    void Start()
    {
        isSprinting = false;
        animator.SetBool("Sprinting", isSprinting);
        thePath = pathManager.GetPath();
        if(thePath != null && thePath.Count > 0)
        {
            //set starting target for first waypoint
            target = thePath[0];
        }
    }
    void rotateTowardsTarget()
    {
        float stepSize = RotateSpeed * Time.deltaTime;

        Vector3 targetDir = target.pos - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, stepSize, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
    }

    void moveForward()
    {
        float stepSize = Time.deltaTime * MoveSpeed;
        float distanceToTarget = Vector3.Distance(transform.position, target.pos);
        if(distanceToTarget < stepSize)
        {
            //we will overshoot the target
            //so we should do something smarter here
            return;
        }
        //take a step forward
        Vector3 moveDir = Vector3.forward;
        transform.Translate(moveDir * stepSize);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            isSprinting = !isSprinting;
            animator.SetBool("Sprinting", isSprinting);
        }
        if (isSprinting)
        {
            rotateTowardsTarget();
            moveForward();
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit");
        //switch to next target
        target = pathManager.GetNextTarget();
    }
}
