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

        private void Awake()
        {
            startButton.onClick.AddListener(() => Game.Instance.Begin());
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
