using UnityEngine.UIElements;

namespace Improbable.Gdk.Debug.WorkerInspector.Codegen
{
    public class VisualElementConcealer
    {
        private VisualElement Element { get; }

        public bool HideIfEmpty { get; private set; }

        public VisualElementConcealer(VisualElement concealable, bool hideSetting = false)
        {
            Element = concealable;
            HideIfEmpty = hideSetting;
        }

        public void HandleSettingChange(HideCollectionEvent evt)
        {
            HideIfEmpty = evt.HideIfEmpty;
        }

        public void SetVisibility(bool isEmpty)
        {
            if (isEmpty && HideIfEmpty)
            {
                Element.AddToClassList("hidden");
            }
            else
            {
                Element.RemoveFromClassList("hidden");
            }
        }
    }
}
