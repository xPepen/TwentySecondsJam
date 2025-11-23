using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Item
{
    public class ProgressBar : MonoBehaviour
    {
        protected Image ProgressBarImage;

        [SerializeField] private Gradient colorGradient;
        [SerializeField] private float progressAnimationSpeed = 1f;
        private Coroutine _progressBarCoroutine;

        private void Awake()
        {
            ProgressBarImage = GetComponent<Image>();
        }

        public void SetProgress(float progress)
        {
            if(_progressBarCoroutine != null)
            {
                StopCoroutine(_progressBarCoroutine);
            }

            if (gameObject.activeInHierarchy)
            {
                _progressBarCoroutine = StartCoroutine(AnimateProgress(Mathf.Clamp01(progress), progressAnimationSpeed));
            }
            else
            {
                ProgressBarImage.fillAmount = Mathf.Clamp01(progress);
                ProgressBarImage.color = colorGradient.Evaluate(1 - ProgressBarImage.fillAmount);
            }

        }
        
        private IEnumerator AnimateProgress(float progress, float speed)
        {
            float time = 0;
            float initialProgress = ProgressBarImage.fillAmount;

            while (time < 1)
            {
                ProgressBarImage.fillAmount = Mathf.Lerp(initialProgress, progress, time);
                time += Time.deltaTime * speed;

                ProgressBarImage.color = colorGradient.Evaluate(1 - ProgressBarImage.fillAmount);

                yield return null;
            }

            ProgressBarImage.fillAmount = Mathf.Clamp01(progress);
            ProgressBarImage.color = colorGradient.Evaluate(1 - ProgressBarImage.fillAmount);
        }

        public GameObject GetParent()
        {
            if (!transform)
            {
                return null;
            }
            if (transform.parent.gameObject)
            {
                return transform.parent.gameObject;
            }

            return null;
        }
    }
}