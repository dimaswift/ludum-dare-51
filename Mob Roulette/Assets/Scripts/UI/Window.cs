using UnityEngine;

namespace MobRoulette.UI
{
    public class Window : MonoBehaviour
    {
        public bool IsShown => isShown != null && isShown.Value;
        
        private WindowsManager windowsManager;

        private bool? isShown;
        
        public void Show()
        {
            if (isShown.HasValue && isShown.Value)
            {
                return;
            }
            isShown = true;
            windowsManager.TrackShown(this);
            OnShow();
        }

        public void Hide()
        {
            if (isShown.HasValue && !isShown.Value)
            {
                return;
            }
            isShown = false;
            windowsManager.TrackHidden(this);
            OnHide();
        }

        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }

        public virtual void OnShow()
        {
            gameObject.SetActive(true);
        }

        public virtual void Init(WindowsManager windowsManager)
        {
            this.windowsManager = windowsManager;
        }

    }
}