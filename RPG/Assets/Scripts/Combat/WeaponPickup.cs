using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon = null;
        [SerializeField] private float _respawnTimeSeconds = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(_weapon);
                StartCoroutine(HideForSeconds(_respawnTimeSeconds));
            }
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            TogglePickup(false);
            yield return new WaitForSeconds(seconds);
            TogglePickup(true);
        }

        private void TogglePickup(bool toggle)
        {
            GetComponent<Collider>().enabled = toggle;
            foreach (Transform child in transform)
                child.gameObject.SetActive(toggle);
        }
    }
}
