using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PartsKit
{
    public class DotJumpAnim : MonoBehaviour
    {
        [SerializeField] private bool playAwake;
        [SerializeField] private List<Transform> dotList;
        [SerializeField] private float jumpHeight = 40f;
        [SerializeField] private float jumpScale = 1.2f;
        [SerializeField] private float jumpDuration = 0.15f;
        [SerializeField] private float delayBetweenJumps = 0f;
        [SerializeField] private int loopCount = -1;

        private Sequence sequence;
        private readonly List<float> initPosYList = new List<float>();
        private readonly List<Vector3> initScaleList = new List<Vector3>();
        private bool isInit;

        private void OnEnable()
        {
            if (playAwake)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        private void TryInit()
        {
            if (isInit)
            {
                return;
            }

            isInit = true;

            foreach (Transform dotTrans in dotList)
            {
                if (dotTrans == null)
                {
                    initPosYList.Add(0);
                    initScaleList.Add(Vector3.zero);
                }
                else
                {
                    initPosYList.Add(dotTrans.localPosition.y);
                    initScaleList.Add(dotTrans.localScale);
                }
            }
        }

        private void DoAnim()
        {
            TryInit();
            Stop();

            // 创建一个序列动画
            sequence = DOTween.Sequence();

            for (var i = 0; i < dotList.Count; i++)
            {
                Transform dotTrans = dotList[i];
                if (dotTrans == null)
                {
                    continue;
                }

                float initPosY = initPosYList[i];
                float jumpEndPosY = initPosY + jumpHeight;
                Vector3 initScale = initScaleList[i];
                Vector3 jumpEndScale = initScale * jumpScale;

                Vector3 initLocalPos = dotTrans.localPosition;
                initLocalPos.y = initPosY;
                dotTrans.localPosition = initLocalPos;
                dotTrans.localScale = initScale;

                // 添加点的动画
                sequence.Append(dotTrans.DOLocalMoveY(jumpEndPosY, jumpDuration).SetEase(Ease.OutQuad));
                sequence.Join(dotTrans.DOScale(jumpEndScale, jumpDuration * 0.5f));
                sequence.Append(dotTrans.DOLocalMoveY(initPosY, jumpDuration).SetEase(Ease.InQuad));
                sequence.Join(dotTrans.DOScale(initScale, jumpDuration * 0.5f));
                // 添加延迟
                sequence.AppendInterval(delayBetweenJumps);
            }

            // 循环播放
            sequence.SetLoops(loopCount);
            sequence.OnKill(() => { sequence = null; });
        }

        public void Play()
        {
            DoAnim();
        }

        public void Stop()
        {
            sequence?.Kill();
        }
    }
}