using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class ModalSpawner : MonoBehaviour
    {

        public static ModalSpawner Instance { get; private set; }

        [SerializeField] private MessageModal messageModalPrefab;
        [SerializeField] private StackbuildButton buttonPrefab;

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public async UniTask ShowMessageModal(string title, string body)
        {
            var tcs = AutoResetUniTaskCompletionSource.Create();

            var modal = Instantiate(messageModalPrefab);
            modal.Title = title;
            modal.Body = body;

            var button = Instantiate(buttonPrefab);
            button.Text = "OK";
            button.GetComponent<Button>().navigation = new Navigation
            {
                mode = Navigation.Mode.None,
            };
            modal.AddButton(button.transform);

            button.OnClick.AddListener(() => tcs.TrySetResult());

            EventSystem.current.SetSelectedGameObject(button.gameObject);
            await modal.ShowAsync();
            await tcs.Task;
            await modal.HideAsync();

            Destroy(modal.gameObject);
        }

    }
}
