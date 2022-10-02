
using Core;
using MobRoulette.Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace MobRoulette.UI
{
    public class GameOverWindow : Window
    {
        [SerializeField] private Button restartButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(() =>
            {
                Game.Instance.Begin();
                Hide();
            });
            
            Game.Instance.State.Subscribe(state =>
            {
                if (state == GameState.GameOver)
                {
                    TimerUtils.DelayRealtime(3, Show);
                }
            });
        }
    }
}