using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild
{
    public class PlayerAnime : MonoBehaviour
    {
        [SerializeField] private ModelSetup modelSetup;
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private InputSender inputSender;

        private Animator animator;

        private float animeBlend = 0.0f;

        private void Start()
        {
            modelSetup.modelObject.TryGetComponent(out animator);
        }

        private void Update()
        {
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

        Vector3 CreateMoveDirection()
        {
            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}