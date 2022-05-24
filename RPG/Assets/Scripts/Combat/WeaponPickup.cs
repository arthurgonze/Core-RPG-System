using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig _weapon = null;
        [SerializeField] float _healthRegen = 0;
        [SerializeField] private float _respawnTimeSeconds = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Debug.Log("Player entered pickup collider");
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if(_weapon != null)
            {
                Debug.Log("Weapon nao eh nula, equipando em fighter");
                subject.GetComponent<Fighter>().EquipWeapon(_weapon);
            }
            if(_healthRegen > 0)
                subject.GetComponent<Health>().Heal(_healthRegen);

            StartCoroutine(HideForSeconds(_respawnTimeSeconds));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            Debug.Log("Escondendo pickup");
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

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Debug.Log("Player clicou no pickup collider");
                Pickup(callingController.gameObject);   
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
