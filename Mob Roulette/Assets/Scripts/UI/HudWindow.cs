using System.Collections.Generic;
using Core;
using MobRoulette.Core.Behaviours;
using TMPro;
using UnityEngine;

namespace MobRoulette.UI
{
    public class HudWindow : Window
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI waveRecordText;
        [SerializeField] private TextMeshProUGUI mobsKilledText;
        [SerializeField] private TextMeshProUGUI mobsKilledRecordText;
        [SerializeField] private TextMeshProUGUI mobsLeftText;
        [SerializeField] private GunButton gunButtonPrefab;

        private readonly List<GunButton> gunButtons = new();

        public override void Init(WindowsManager windowsManager)
        {
            base.Init(windowsManager);
            gunButtonPrefab.gameObject.SetActive(false);
            Game.Instance.WaveManager.CountDown.Subscribe(c => countDownText.text = c.ToString(), true);
            Game.Instance.WaveManager.Wave.Subscribe(c => waveText.text = $"WAVE {c} IN", true);
            Game.Instance.WaveManager.WaveRecord.Subscribe(c => waveRecordText.text = c.ToString(), true);
            Game.Instance.WaveManager.MobsLeft.Subscribe(c => mobsLeftText.text = c.ToString(), true);

            Game.Instance.State.Subscribe(state =>
            {
                if (state == GameState.Playing)
                {
                    InitPlayer(Game.Instance.Player);
                    Show();
                }
                else
                {
                    Hide();
                }
            });
        }

        private void InitPlayer(Player player)
        {
            gunButtons.ForEach(b => Destroy(b.gameObject));
            gunButtons.Clear();

            var guns = player.GetComponentsInChildren<GunManager>(true);

            foreach (var gun in guns)
            {
                var btn = Instantiate(gunButtonPrefab.gameObject).GetComponent<GunButton>();
                btn.SetUp(gun);
                btn.transform.SetParent(gunButtonPrefab.transform.parent);
                btn.gameObject.SetActive(true);
                gunButtons.Add(btn);
                btn.transform.localScale = Vector3.one;
            }

            player.MobsKilled.Subscribe(c => mobsKilledText.text = c.ToString(), true);
            player.MobsKilledRecord.Subscribe(c => mobsKilledRecordText.text = c.ToString(), true);
            player.Coins.Subscribe(c => coinsText.text = c.ToString(), true);
            player.Health.Subscribe(h => healthText.text = h.ToString("F0"), true);
        }
    }
}