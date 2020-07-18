using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;

        private void Start()
        {
            GetComponent<PlayableDirector>().played += DisablePlayerControl;
            GetComponent<PlayableDirector>().stopped += EnablePlayerControl;
            player = GameObject.FindWithTag("Player");
        }

        void DisablePlayerControl(PlayableDirector playableDirector)
        {
            print("Player Controls Disabled");
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnablePlayerControl(PlayableDirector playableDirector)
        {
            print("Player Controls Enabled");
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
