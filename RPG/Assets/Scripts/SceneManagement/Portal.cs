using System.Collections;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Core;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        {
            Alfa, Bravo, Charlie, Delta, Echo
        }

        [SerializeField] private int _sceneToLoad;
        [SerializeField] private float _fadeOutTime = 1f;
        [SerializeField] private float _fadeInTime = 0.5f;
        [SerializeField] private float _fadeWaitTime = 0.5f;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private DestinationIdentifier _destination;

        GameObject _player = null;

        private void Awake() {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                StartCoroutine(Transition());
        }
         
        private IEnumerator Transition()
        {
            if(_sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set");
                yield break;
            }
            DontDestroyOnLoad(this.gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            TogglePlayerControl(false);

            fader.FadeOut(_fadeOutTime);
            yield return new WaitForSeconds(_fadeOutTime);
            MovePlayerToSpawnPoint();

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(_sceneToLoad);
            savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            TogglePlayerControl(false);

            savingWrapper.Save();

            yield return new WaitForSeconds(_fadeWaitTime);
            fader.FadeIn(_fadeInTime);

            TogglePlayerControl(true);
            Destroy(this.gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            _player.GetComponent<NavMeshAgent>().enabled = false;
            _player.transform.position = otherPortal._spawnPoint.position;
            _player.transform.rotation = otherPortal._spawnPoint.rotation;
            _player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();
            foreach(Portal portal in portals)
            {
                if(portal._destination == this._destination && portal != this)
                {
                    return portal;
                }
            }
            return null;
        }

        private void MovePlayerToSpawnPoint()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _player.GetComponent<NavMeshAgent>().enabled = false;
            _player.transform.position = this._spawnPoint.position;
            _player.transform.rotation = this._spawnPoint.rotation;
            _player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private void TogglePlayerControl(bool toggle)
        {
            if(!toggle)
                _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = toggle;
        }
    }
}
