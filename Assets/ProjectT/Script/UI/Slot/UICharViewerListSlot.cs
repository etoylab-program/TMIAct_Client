using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharViewerListSlot : FSlot
{
    public UISprite kEmotionSpr;
    public UISprite kLockSpr;
    public UILabel kNameLabel;

    private eAnimation _animation;
    private eFaceAnimation _faceAnimation;

    private UICharViewerPopup _parentPanel;
    
    public void Start()
    {
        Object atlasObj = FLocalizeAtlas.Instance.GetLocalizeAtlas("Lobby");
        if (atlasObj != null)
        {
            GameObject obj = atlasObj as GameObject;
            if (obj != null)
            {
                UIAtlas atlas = obj.GetComponent<UIAtlas>();
                if (atlas != null)
                {
                    kEmotionSpr.atlas = atlas;
                }
            }
        }
        
        _parentPanel = ParentGO.GetComponent<UICharViewerPopup>();
    }
    
    public void UpdateSlot(int index, int lockFlag, GameTable.LobbyAnimation.Param tableData)
    {
        kEmotionSpr.spriteName = tableData.Icon;
        
        kLockSpr.gameObject.SetActive(lockFlag == 1);

        System.Enum.TryParse(tableData.Animation, true, out _animation);
        System.Enum.TryParse(tableData.Face, true, out _faceAnimation);
        
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(tableData.Name);
    }
    
    public void OnClick_PlayAni()
    {
        if (kLockSpr.gameObject.activeSelf)
        {
            return;
        }
        
        if (_parentPanel != null)
        {
            _parentPanel.PlayAnimation(_animation, _faceAnimation);
        }
    }
}
