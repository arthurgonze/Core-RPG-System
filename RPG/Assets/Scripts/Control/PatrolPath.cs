using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] float waypointGizmosRadius = 0.5f;
        private void OnDrawGizmos()
        {
            for(int i=0; i < transform.childCount; i++)
            {
                if (i == 0) Gizmos.color = Color.red;
                else if (i == 1) Gizmos.color = Color.green;
                else Gizmos.color = Color.blue;

                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(GetNextIndex(i)));
            }
        }

        private int GetNextIndex(int i)
        {
            if(i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        private Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).transform.position;
        }
    }
}
