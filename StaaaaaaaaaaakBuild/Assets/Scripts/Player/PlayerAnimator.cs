using System;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private ModelSetup modelSetup;
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private InputSender inputSender;

        private Animator animator = null;

        private float animeBlend = 0.0f;

        private void Start()
        {
            modelSetup.modelObject.TryGetComponent(out animator);

            inputSender.Dash.sender.ThrottleFirst(TimeSpan.FromSeconds(playerProperty.characterProperty.Dash.DashCoolTime)).Subscribe(x =>
            {
                if (animator == null)
                    return;

                DashAnimation();
            }).AddTo(this);
        }

        private void Update()
        {
            if (animator == null)
                return;

            MoveAnimation();
        }

        void MoveAnimation()
        {
            //傾き率を計算
            var raito = Mathf.Max(Mathf.Abs(inputSender.Move.Value.x), Mathf.Abs(inputSender.Move.Value.y));

            animeBlend = Mathf.Lerp(animeBlend, raito, playerProperty.characterProperty.Move.SlopeTime * Time.deltaTime);

            //アニメブレンド値変更
            animator.SetFloat("Blend", animeBlend);
        }

        void DashAnimation()
        {
            animator.SetBool("Dash", true);

            Observable.Timer(TimeSpan.FromSeconds(playerProperty.characterProperty.Dash.DashAccelerationTime)).Subscribe(x =>
            {
                animator.SetBool("Dash", false);
            }).AddTo(this);
        }
    }
}