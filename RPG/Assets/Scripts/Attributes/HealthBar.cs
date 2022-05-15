using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health _healthComponent = null;
        [SerializeField] RectTransform _foreground = null;
        [SerializeField] Canvas _rootCanvas = null;
        // Update is called once per frame
        void Update()
        {
            if(Mathf.Approximately(_healthComponent.GetFraction(), 0) || Mathf.Approximately(_healthComponent.GetFraction(), 1))
            {
                _rootCanvas.enabled = false;
                return;
            }
            
            _rootCanvas.enabled = true;
            _foreground.localScale = new Vector3(_healthComponent.GetFraction(), 1, 1);
        }
    }
}