using UnityEngine;

namespace StackBuild
{
    public class ModelSetup : MonoBehaviour
    {
        [field: SerializeField] public PlayerProperty playerProperty { get; private set; }
        [field: SerializeField] public InputSender inputSender { get; private set; }

        public GameObject modelObject { get; private set; }

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private void Awake()
        {
            Setup();
        }

        void Setup()
        {
            if(playerProperty.characterProperty == null)
                return;

            modelObject = GameObject.Instantiate(property.Model.CharacterModelPrefab, transform);
        }
    }
}