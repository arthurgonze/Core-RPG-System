using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private Coroutine _currentFadeRoutine = null;
        // Cached reference
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine FadeOut(float time)
        {
             return Fade(1, time);
        }

        public Coroutine Fade(float alphaTarget, float time)
        {
            if(_currentFadeRoutine != null)
                StopCoroutine(_currentFadeRoutine);
            _currentFadeRoutine = StartCoroutine(FadeRoutine(alphaTarget, time));
            return _currentFadeRoutine;
        }

        private IEnumerator FadeRoutine(float alphaTarget, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, alphaTarget))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, alphaTarget, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
