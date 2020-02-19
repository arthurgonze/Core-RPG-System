using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    // We can remove the serializeField, its on just for debug purposes
    [SerializeField] private Transform target;

    // Cached References
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    // Move to Cursor
    private Ray ray;
    private bool hasHit = false;
    private RaycastHit hit;

    // Update Animator
    private Vector3 globalVelocity;
    private Vector3 localVelocity;
    private float speed;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        UpdateAnimator();
    }

    private void MoveToCursor()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hasHit = Physics.Raycast(ray, out hit);
        if(hasHit)
        {
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            navMeshAgent.SetDestination(hit.point);
        }
    }

    private void UpdateAnimator()
    {
        globalVelocity = navMeshAgent.velocity;
        localVelocity = transform.InverseTransformDirection(globalVelocity);
        speed = localVelocity.z;
        animator.SetFloat("ForwardSpeed", speed);
    }
}
