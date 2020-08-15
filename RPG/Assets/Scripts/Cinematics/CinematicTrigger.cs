using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool _alreadyTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag.Equals("Player") && !_alreadyTriggered)
            {
                GetComponent<PlayableDirector>().Play();
                _alreadyTriggered = true;
            }
        }
    }
}
