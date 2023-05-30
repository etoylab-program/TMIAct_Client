using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceItem : MonoBehaviour
{
    public enum eRaceItemType
    {
        coin,
        magnet,
        booster,
        hprecovery,
    }

    public eRaceItemType kRaceItemType = eRaceItemType.coin;

    Collider _collider;
    WorldBike mWorldBike = null;

    
    private void Start()
    {
        _collider = this.GetComponent<Collider>();
        mWorldBike = World.Instance as WorldBike;
    }

    private void Update()
    {
        if (mWorldBike.kRider == null)
            return;

        if (!mWorldBike.kRider.bIsMagnet)
            return;

        if(Vector3.Distance(this.transform.position, mWorldBike.kRider.kPlayerPos.position) <= mWorldBike.kRider.kMagnetDistance)
        {
            transform.LookAt(mWorldBike.kRider.kPlayerPos.position);
            transform.Translate(Vector3.forward * (mWorldBike.kRider.kSpeed * 2.5f) * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            switch (kRaceItemType)
            {
                case eRaceItemType.coin:
                    SoundManager.Instance.PlaySnd( SoundManager.eSoundType.FX, eRaceSound.get_coin.ToString());
                    EffectManager.Instance.Play(this.transform.position, 0, EffectManager.eType.Each_Monster_Normal_Hit);
                    break;
                case eRaceItemType.booster:
                    SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, eRaceSound.get_boost.ToString());
                    break;
                case eRaceItemType.hprecovery:
                    SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, eRaceSound.get_hp.ToString());
                    EffectManager.Instance.Play(this.transform.position, 2, EffectManager.eType.Each_Monster_Normal_Hit);
                    break;
                case eRaceItemType.magnet:
                    SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, eRaceSound.get_magnet.ToString());
                    break;
            }

            if (kRaceItemType == eRaceItemType.coin)
            {
                
            }
            this.gameObject.SetActive(false);
            
        }

    }
}
