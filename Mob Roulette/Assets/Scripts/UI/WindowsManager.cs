using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobRoulette.UI
{
    public class WindowsManager : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            windowsManager = null;
        }
        
        private static WindowsManager windowsManager;

        public static WindowsManager Instance
        {
            get
            {
                if (windowsManager == null)
                {
                    windowsManager = FindObjectOfType<WindowsManager>();
                }
                return windowsManager;
            }
        }
        
        private readonly Dictionary<Type, Window> windows = new();

        private readonly List<Window> openWindows = new();

        private void Start()
        {
            foreach (var prefab in Resources.LoadAll<Window>("Windows"))
            {
                var win = Instantiate(prefab.gameObject).GetComponent<Window>();
                win.transform.SetParent(transform);
                win.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                win.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                win.Init(this);
                win.name = prefab.name;
                win.Hide();
                win.transform.localScale = Vector3.one;
                windows[win.GetType()] = win;
            }
            Show<MainMenu>();
        }
        
        public void Show<T>() where T : Window
        {
            windows[typeof(T)].Show();
        }

        public void TrackShown(Window window)
        {
            openWindows.Add(window);
        }

        public void TrackHidden(Window window)
        {
            openWindows.Remove(window);
        }

        public void HideAll()
        {
            for (int i = openWindows.Count - 1; i >= 0; i--)
            {
                openWindows[i].Hide();
            }
        }
    }
}