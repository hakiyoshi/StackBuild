using TMPro;
using UnityEngine;

namespace StackBuild.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class GameVersionText : MonoBehaviour
    {

        [SerializeField] private TMP_Text text;
        [SerializeField] private string pattern = "{}";

        private void Reset()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            text.text = text.text.Replace(pattern, Application.version);
        }

    }
}
