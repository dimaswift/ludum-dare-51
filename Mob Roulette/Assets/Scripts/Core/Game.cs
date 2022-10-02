using System;
using MobRoulette.Core.Behaviours;
using MobRoulette.Core.Events;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
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
                    instance.State.Subscribe(EventBus.Raise<OnGameStateChanged, GameState>);
                }

                return instance;
            }
        }
        
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private MobSpawner mobSpawner;
        public Player Player { get; private set; }
        public MobSpawner MobSpawner => mobSpawner;
        
        private readonly Observable<GameState> state = new(GameState.MainMenu);
        public IObservableValue<GameState> State => state;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            instance = null;
        }

        public void Begin()
        {
            CleanUp();
            Player = Instantiate(playerPrefab.gameObject).GetComponent<Player>();
            Player.Prepare();
            Player.transform.position = spawnPoint.position;
            Player.transform.rotation = spawnPoint.rotation;
            state.Set(GameState.Playing);
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