using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void LateUpdate()
        {
            this.transform.position = _target.position;
        }
    }
}
