
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eGachaParticleIcon
{
    Supporter = 12,
    Weapon,
    Item,
    Jewel,
}

public class GachaItem : MonoBehaviour
{
    [Header("[Normal]")]
    public ParticleSystem psNormal;
    public ParticleSystem psNIcon;

    [Header("[Rare]")]
    public ParticleSystem psRare;
    public ParticleSystem psRIcon;

    [Header("[URare]")]
    public ParticleSystem psURare;
    public ParticleSystem psURIcon;

    public ParticleSystem psChange;

    private bool m_changeGrade = false;
    private eGachaParticleIcon m_icon;
    private int m_grade;

    public int grade { get { return m_grade; } }


    public void Init(RewardData rewardData)
    {
        m_changeGrade = rewardData.ChangeGrade;
        psChange.gameObject.SetActive(false);

        m_icon = eGachaParticleIcon.Supporter;
        if(rewardData.Type == (int)eREWARDTYPE.CARD)
        {
            CardData cardData = GameInfo.Instance.GetCardData(rewardData.UID);
            m_grade = cardData.TableData.Grade;
            m_icon = eGachaParticleIcon.Supporter;
        }
        else if(rewardData.Type == (int)eREWARDTYPE.WEAPON)
        {
            WeaponData weaponData = GameInfo.Instance.GetWeaponData(rewardData.UID);
            m_grade = weaponData.TableData.Grade;
            m_icon = eGachaParticleIcon.Weapon;
        }
        else if(rewardData.Type == (int)eREWARDTYPE.GEM)
        {
            GemData gemData = GameInfo.Instance.GetGemData(rewardData.UID);
            m_grade = gemData.TableData.Grade;
            m_icon = eGachaParticleIcon.Jewel;
        }
        else if(rewardData.Type == (int)eREWARDTYPE.ITEM)
        {
            ItemData itemData = GameInfo.Instance.GetItemData(rewardData.UID);
            m_grade = itemData.TableData.Grade;
            m_icon = eGachaParticleIcon.Item;
        }

        ParticleSystem.TextureSheetAnimationModule texSheetAniMoudle;

        if( m_grade == (int)eGRADE.GRADE_UR )
        {
            if(!m_changeGrade)
            {
                psNormal.gameObject.SetActive(false);
                psRare.gameObject.SetActive(false);
                psURare.gameObject.SetActive(true);
                texSheetAniMoudle = psURIcon.textureSheetAnimation;
            }
            else
            {
                int i = Random.Range(0, 2);
                if( i == 0 )
                {
                    psNormal.gameObject.SetActive(false);
                    psRare.gameObject.SetActive(true);
                    psURare.gameObject.SetActive(false);
                }
                else
                {
                    psNormal.gameObject.SetActive(true);
                    psRare.gameObject.SetActive(false);
                    psURare.gameObject.SetActive(false);
                }
                
                texSheetAniMoudle = psRIcon.textureSheetAnimation;
            }
        }
        else if (m_grade == (int)eGRADE.GRADE_SR)
        {
            if (!m_changeGrade)
            {
                psNormal.gameObject.SetActive(false);
                psRare.gameObject.SetActive(true);
                psURare.gameObject.SetActive(false);
                texSheetAniMoudle = psRIcon.textureSheetAnimation;
            }
            else
            {
                psNormal.gameObject.SetActive(true);
                psRare.gameObject.SetActive(false);
                psURare.gameObject.SetActive(false);
                texSheetAniMoudle = psNIcon.textureSheetAnimation;
            }
        }
        else
        {
            psNormal.gameObject.SetActive(true);
            psRare.gameObject.SetActive(false);
            psURare.gameObject.SetActive(false);
            texSheetAniMoudle = psNIcon.textureSheetAnimation;
        }
        ParticleSystem.TextureSheetAnimationModule texSheetAniMoudleN = psNIcon.textureSheetAnimation;
        ParticleSystem.TextureSheetAnimationModule texSheetAniMoudleR = psRIcon.textureSheetAnimation;
        ParticleSystem.TextureSheetAnimationModule texSheetAniMoudleUR = psURIcon.textureSheetAnimation;
        texSheetAniMoudleN.startFrame = new ParticleSystem.MinMaxCurve((float)m_icon / (float)(texSheetAniMoudleN.numTilesX * texSheetAniMoudleN.numTilesY));
        texSheetAniMoudleR.startFrame = new ParticleSystem.MinMaxCurve((float)m_icon / (float)(texSheetAniMoudleR.numTilesX * texSheetAniMoudleR.numTilesY));
        texSheetAniMoudleUR.startFrame = new ParticleSystem.MinMaxCurve((float)m_icon / (float)(texSheetAniMoudleUR.numTilesX * texSheetAniMoudleUR.numTilesY));

        //texSheetAniMoudle.startFrame = new ParticleSystem.MinMaxCurve((float)m_icon / (float)(texSheetAniMoudle.numTilesX * texSheetAniMoudle.numTilesY));
    }

    public void ChangeGrade(float changeDuration)
    {
        if (m_changeGrade)
        {
            psChange.gameObject.SetActive(true);

            SoundManager.sSoundInfo soundInfo = SoundManager.Instance.PlayDelayedSnd(SoundManager.eSoundType.FX, 2.05f, 102, FSaveData.Instance.GetSEVolume());
            if (soundInfo != null)
                SoundManager.Instance.PlayDelayedSnd(SoundManager.eSoundType.FX, 2.05f + soundInfo.clip.length, 103, FSaveData.Instance.GetSEVolume());

            StartCoroutine("ChangeNtoR", changeDuration);
        }
    }

    private IEnumerator ChangeNtoR(float changeDuration)
    {
        yield return new WaitForSeconds(changeDuration * 0.8f);

        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, 103, FSaveData.Instance.GetSEVolume());

        if (m_grade == (int)eGRADE.GRADE_UR)
        {
            psNormal.gameObject.SetActive(false);
            psRare.gameObject.SetActive(false);
            psURare.gameObject.SetActive(true);
        }
        else if (m_grade == (int)eGRADE.GRADE_SR)
        {
            psNormal.gameObject.SetActive(false);
            psRare.gameObject.SetActive(true);
            psURare.gameObject.SetActive(false);
        }

        ParticleSystem.TextureSheetAnimationModule texSheetAniMoudle = psRIcon.textureSheetAnimation;
        texSheetAniMoudle.startFrame = new ParticleSystem.MinMaxCurve((float)m_icon / (float)(texSheetAniMoudle.numTilesX * texSheetAniMoudle.numTilesY));
    }
}
