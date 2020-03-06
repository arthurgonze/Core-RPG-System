using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;


        private void Update()
        {
            if (DistanceToPlayer() < chaseDistance)
            {
                Debug.Log("Chase! - " + this.gameObject.name);
            }
        }

        private float DistanceToPlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            float distance = Vector3.Distance(this.transform.position, player.transform.position);
            return distance;
        }
    }
}
