using Core;
using MobRoulette.Core.Domain;
using MobRoulette.Core.Interfaces;
using MobRoulette.Core.Utils;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class GunManager : MonoBehaviour
    {
        public IObservableValue<bool> Unlocked => unlocked;

        public IGun Gun => gun ??= GetComponent<IGun>();
        public string GunName => gunName;
        public int AmmoBatchSize => ammoBatchSize;
        public int AmmoPrice => ammoPrice;
        public int UnlockPrice => unlockPrice;
        
        public int Order { get; set; }
        
        [SerializeField] private int ammoPrice;
        [SerializeField] private int unlockPrice;
        [SerializeField] private int ammoBatchSize;
        [SerializeField] private string gunName;
        [SerializeField] private bool unlockedByDefault;
        [SerializeField] private bool equipped;
        private readonly Observable<bool> unlocked = new (false);

        private IGun gun;
        
        private void Start()
        {
            unlocked.Set(true);
            Gun.SetEquipped(equipped);
        }
        
        public bool Unlock()
        {
            if (unlocked.Value)
            {
                return false;
            }
            
            if (!Game.Instance.Player.WithdrawCoins(ammoPrice))
            {
                Sounds.PlaySound(SoundType.Error, Camera.main.transform.position);
                return false;
            }

            unlocked.Set(true);
            Game.Instance.Save.UnlockedGuns.Add(gunName, true);
            Sounds.PlaySound(SoundType.Purchase, Camera.main.transform.position);
            return true;
        }

        public bool BuyAmmo()
        {
            if (unlocked.Value == false)
            {
                return false;
            }
            
            if (!Game.Instance.Player.WithdrawCoins(ammoPrice))
            {
                Sounds.PlaySound(SoundType.Error, Camera.main.transform.position);
                return false;
            }

            Gun.AddAmmo(ammoBatchSize);
            Sounds.PlaySound(SoundType.Purchase, Camera.main.transform.position);
            return true;
        }
    }
}