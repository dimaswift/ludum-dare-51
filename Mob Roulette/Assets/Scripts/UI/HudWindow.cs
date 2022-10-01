using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobRoulette.UI
{
    public class HudWindow : Window
    {
        [SerializeField] private TextMeshProUGUI healthText;

        public override void Init(WindowsManager windowsManager)
        {
            base.Init(windowsManager);
            Game.Instance.State.Subscribe(state =>
            {
                if (state == GameState.Playing)
                {
                    Game.Instance.Player.Health.Subscribe(h =>
                    {
                        healthText.text = h.ToString("F0");
                    });
                    healthText.text = Game.Instance.Player.Health.Value.ToString("F0");
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