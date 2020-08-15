using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        // Cached reference
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOut(float time)
        {
            _canvasGroup.alpha = 0;
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public IEnumerator FadeIn(float time)
        {
            _canvasGroup.alpha = 1;
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
