using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        // Cached reference
        private GameObject _player;

        private void Awake()
        {
            GetComponent<PlayableDirector>().played += DisablePlayerControl;
            GetComponent<PlayableDirector>().stopped += EnablePlayerControl;
        }
        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
        }

        private void DisablePlayerControl(PlayableDirector playableDirector)
        {
            //print("Player Controls Disabled");
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        private void EnablePlayerControl(PlayableDirector playableDirector)
        {
            //print("Player Controls Enabled");
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
