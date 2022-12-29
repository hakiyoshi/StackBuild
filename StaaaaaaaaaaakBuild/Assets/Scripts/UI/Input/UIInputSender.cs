using UnityEngine;

namespace StackBuild.UI
{
    [CreateAssetMenu(menuName = "Input/UIInputSender")]
    public class UIInputSender : ScriptableObject
    {
        public SenderProperty<bool> Select = new SenderProperty<bool>();
        public SenderProperty<bool> Cancel = new SenderProperty<bool>();
    }
}