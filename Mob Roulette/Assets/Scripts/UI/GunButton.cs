using Core;
using MobRoulette.Core.Behaviours;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MobRoulette.UI
{
    public class GunButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI unlockPriceText;
        [SerializeField] private TextMeshProUGUI ammoPriceText;
        [SerializeField] private TextMeshProUGUI ammoBatchSizeText;
        [SerializeField] private GameObject lockedContainer;
        [SerializeField] private GameObject unlockedContainer;
        [SerializeField] private GameObject equippedFrame;
        [SerializeField] private Button buyAmmoButton;
        [SerializeField] private Button unlockButton;
        [SerializeField] private Button equipButton;
        public void SetUp(GunManager gunManager)
        {
            titleText.text = gunManager.GunName;
            gunManager.Gun.Ammo.Subscribe(a => ammoText.text = a.ToString(), true);
            unlockPriceText.text = gunManager.UnlockPrice.ToString();
            ammoPriceText.text = gunManager.AmmoPrice.ToString();
            ammoBatchSizeText.text = "+" + gunManager.AmmoBatchSize.ToString();
            buyAmmoButton.onClick.AddListener(() =>
            {
                gunManager.BuyAmmo();
            });
            unlockButton.onClick.AddListener(() =>
            {
                gunManager.Unlock();
            });
            equipButton.onClick.AddListener(() => Game.Instance.Player.EquipGun(gunManager));
            gunManager.Unlocked.Subscribe(u =>
            {
                lockedContainer.SetActive(u == false);
                unlockedContainer.SetActive(u);
            }, true);
            gunManager.Gun.Equipped.Subscribe(e => equippedFrame.SetActive(e), true);
            transform.SetSiblingIndex(gunManager.Order);
        }
        
    }
}