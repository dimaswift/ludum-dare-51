using System.Collections;
using Core;
using MobRoulette.Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace MobRoulette.UI
{
    public class MainMenu : Window
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        
        private void Awake()
        {
            quitButton.onClick.AddListener(Application.Quit);
            startButton.onClick.AddListener(() =>
            {
                Game.Instance.Begin();
                Hide();
            });
            Game.Instance.State.Subscribe(state =>
            {
                if (state == GameState.MainMenu)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            });
        }
    }
}
