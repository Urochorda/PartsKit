using UnityEngine;

namespace PartsKit
{
    public class EffectItem : MonoBehaviour
    {
        public static void Play(EffectItem effectItem, EffectController controllerVal)
        {
            effectItem.controller = controllerVal;
            effectItem.OnPlay();
        }

        public static void Init(EffectItem effectItem)
        {
            effectItem.OnInit();
        }

        public static void Recycle(EffectItem effectItem)
        {
            effectItem.controller = null;
            effectItem.OnRecycle();
        }

        [SerializeField] private float playTime;

        private EffectController controller;
        private ParticleSystem particle;
        private float curPlayTime;
        private bool isPlay;
        private Vector3 initScale;

        public Transform ParentPoint { get; set; }

        protected virtual void OnInit()
        {
            particle = GetComponent<ParticleSystem>();
            gameObject.SetActive(false);
            isPlay = false;
            initScale = transform.localScale;
        }

        protected virtual void OnPlay()
        {
            isPlay = true;
            curPlayTime = 0;
            gameObject.SetActive(true);
            particle.Play();
        }

        protected virtual void OnRecycle()
        {
            isPlay = false;
            transform.localScale = initScale;
            gameObject.SetActive(false);
        }

        public void Recycle()
        {
            controller.Recycle(this);
        }

        private void Update()
        {
            if (!isPlay)
            {
                return;
            }

            playTime += Time.deltaTime;
            if (ParentPoint != null)
            {
                transform.position = ParentPoint.transform.position;
            }

            if (curPlayTime > playTime)
            {
                Recycle();
            }
        }
    }
}