using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharFavorLevelUpPopup : FComponent
{
    public enum eFavorLevelUp
    {
        LevelUp,
        LevelUpMax,
        Max,
    }

    [Header("LevelMax")]
    public UILabel kLevelMaxLabel;
    public UILabel kLevelMaxCompensationLabel;
    
    [Header("LevelUp")]
    public UILabel kLevelUpNowLabel;
    public UILabel kLevelUpNextLabel;
    public UIItemListSlot kLevelUpItemListSlot;
    public UIExOpacityWidget kRootSlot;

    private readonly Dictionary<eFavorLevelUp, string> _animationClipDict = new Dictionary<eFavorLevelUp, string>();

    private bool _bLevelUpMax;
    
    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < _aninamelist.Count; i++)
        {
            if (_aninamelist[i].Contains("[OUT]"))
            {
                continue;
            }
            
            _animationClipDict.Add((eFavorLevelUp)i, _aninamelist[i]);
        }
    }
    
    public override void OnEnable()
    {
        base.OnEnable();

        kRootSlot.kOpacity = 0;

        UIAni.Play(_animationClipDict[eFavorLevelUp.LevelUp]);
        SoundManager.Instance.PlayUISnd(87);
    }
    
    public void SetLevelUp(bool bLevelUpMax)
    {
        _bLevelUpMax = bLevelUpMax;
    }

    public void SetLevelUpMaxLabel(int level, string compensation)
    {
        kLevelMaxLabel.textlocalize = FLocalizeString.Instance.GetText(211, level);
        kLevelMaxCompensationLabel.textlocalize = compensation;
    }

    public void SetLevelUpLabel(int nowLevel, int nextLevel, int rewardId)
    {
        GameTable.Random.Param tableData = GameInfo.Instance.GameTable.FindRandom(rewardId);
        
        kLevelUpItemListSlot.UpdateSlot(UIItemListSlot.ePosType.RewardTable, 0, tableData);
        
        kLevelUpNowLabel.textlocalize = FLocalizeString.Instance.GetText(211, nowLevel);
        kLevelUpNextLabel.textlocalize = FLocalizeString.Instance.GetText(211, nextLevel);
    }
    
    public void ExitLevelUpPopup()
    {
        if (_bLevelUpMax)
        {
            _bLevelUpMax = false;
            UIAni.Play(_animationClipDict[eFavorLevelUp.LevelUpMax]);
            SoundManager.Instance.PlayUISnd(87);
        }
        else
        {
            SetUIActive(false, false);
        }
    }
    
    public void OnClick_BackBtn()
    {
        ExitLevelUpPopup();
    }
}
