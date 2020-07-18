using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool alreadyTriggered = false;
        public event Action onStart;
        public event Action onFinish;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag.Equals("Player") && !alreadyTriggered)
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }
    }
}
