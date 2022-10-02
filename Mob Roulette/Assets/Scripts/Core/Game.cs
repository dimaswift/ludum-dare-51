using System;
using System.Collections.Generic;
using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Core
{
    public class Game : MonoBehaviour
    {
        private static Game instance;

        public static Game Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Game>();
                    if (instance == null)
                    {
                        return null;
                    }
                    instance.LoadSave();
                    instance.State.Subscribe(EventBus.Raise<OnGameStateChanged, GameState>);
                }

                return instance;
            }
        }
        
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private WaveManager waveManager;
        public Player Player { get; private set; }
        public WaveManager WaveManager => waveManager;
        
        private readonly Observable<GameState> state = new(GameState.MainMenu);
        public IObservableValue<GameState> State => state;

        public PlayerSave Save => save;
        
        private PlayerSave save;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            instance = null;
        }
        
        private void LoadSave()
        {
            if (!PlayerPrefs.HasKey(nameof(PlayerSave)))
            {
                save = new PlayerSave();
                return;
            }
            var json = PlayerPrefs.GetString(nameof(PlayerSave));
            save = JsonConvert.DeserializeObject<PlayerSave>(json);
            if (save.UnlockedGuns == null)
            {
                save.UnlockedGuns = new Dictionary<string, bool>();
            }
        }

        private void WriteSave()
        {
            if (save == null)
            {
                return;
            }
            
            save.Coins = Player.Coins.Value;
            save.RecordWave = waveManager.WaveRecord.Value;
            save.MobsKilledRecord = Player.MobsKilledRecord.Value;
            save.MobsKilledTotal += Player.MobsKilled.Value;
            
            var json = JsonConvert.SerializeObject(save);
            PlayerPrefs.SetString(nameof(PlayerSave), json);
        }

        private void OnApplicationQuit()
        {
            WriteSave();
        }

        public void Begin()
        {
            CleanUp();
            Player = Instantiate(playerPrefab.gameObject).GetComponent<Player>();
            Player.Prepare();
            Player.transform.position = spawnPoint.position;
            Player.transform.rotation = spawnPoint.rotation;
            
            TimerUtils.Delay(0.5f, () => state.Set(GameState.Playing));
        }

        public void CleanUp()
        {
            if (Player != null)
            {
                Destroy(Player.gameObject);
                Player = null;
            }
            PoolDisposal.ReleaseAll();
        }

        public void GoToMenu()
        {
            CleanUp();
            state.Set(GameState.MainMenu);
        }

        public void GameOver()
        {
            state.Set(GameState.GameOver);
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}