using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharInfoTabSkillPanel : FComponent
{
    public List<UISkillListSlot> kSkillListSlotList;
    public List<UISkillLevelUpListSlot> kSkillLevelUpList;
    public List<UISprite> kNoticeList;
    public UIButton kTrainingBtn;

    private CharData        mCharData       = null;
    private List<CardData>  mListCardData   = new List<CardData>();


	public override void OnEnable()
	{
		Type = TYPE.Tab;

		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        long uid = (long)UIValue.Instance.GetValue(UIValue.EParamType.CharSelUID);
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.CharSelTableID);
        mCharData = GameInfo.Instance.GetCharData(uid);

        mListCardData.Clear();
        for (int i = 0; i < (int)eCOUNT.CARDSLOT; i++)
        {
            CardData cardData = GameInfo.Instance.GetCardData(mCharData.EquipCard[i]);
            mListCardData.Add(cardData);
        }

        if( RenderTargetChar.Instance.RenderPlayer ) {
            RenderTargetChar.Instance.ShowAttachedObject( false );
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.InGameOnly, false );
            RenderTargetChar.Instance.RenderPlayer.costumeUnit.ShowObject( RenderTargetChar.Instance.RenderPlayer.costumeUnit.Param.LobbyOnly, true );
        }

        if (LobbyUIManager.Instance.kBlackScene.activeSelf)
            LobbyUIManager.Instance.kBlackScene.SetActive(false);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        for( int i = 0; i < (int)eCOUNT.SKILLSLOT; i++ )
        {
            var passviedata = mCharData.PassvieList.Find(x => x.SkillID == mCharData.EquipSkill[i]);
            kSkillListSlotList[i].ParentGO = this.gameObject;
            kSkillListSlotList[i].UpdateSlot( UISkillListSlot.ePOS.EQUIPED, i, passviedata, mCharData, mListCardData);
        }

        for (int i = 0; i < kSkillLevelUpList.Count; i++ )
        {
            kSkillLevelUpList[i].ParentGO = this.gameObject;
            kSkillLevelUpList[i].UpdateSlot(i, mCharData);
        }

        for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
            kNoticeList[i].gameObject.SetActive(false);

        if ( GameSupport.IsCharSkillUp(mCharData) )
        {
            for (int i = 0; i < (int)eCOUNT.SKILLSLOT; i++)
                if ( !kSkillListSlotList[i].kLock.gameObject.activeSelf )
                    kNoticeList[i].gameObject.SetActive(true);
        }
    }
    public void OnClick_TrainingBtn()
    {
        if (GameSupport.IsEmptyInEquipMainWeapon(ePresetKind.STAGE, mCharData.CUID))
        {
            return;
        }

        GameInfo.Instance.IsPrevSkillTrainingRoom = true;
        GameInfo.Instance.SeleteCharUID = mCharData.CUID;
        UIValue.Instance.RemoveValue(UIValue.EParamType.ArenaCharInfoFlag);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingType, (int)UILoadingPopup.eLoadingType.LobbyToTraining);
        UIValue.Instance.SetValue(UIValue.EParamType.LoadingStage, -1);
        LobbyUIManager.Instance.ShowUI("LoadingPopup", false);

        UIValue.Instance.SetValue(UIValue.EParamType.LobbyToTrainingRoom, "CharInfoTabSkillPanel");
        UIValue.Instance.SetValue(UIValue.EParamType.TrainingCharTID, mCharData.TableData.ID);

        AppMgr.Instance.LoadScene(AppMgr.eSceneType.Training, "Stage_skill_trainingroom");
    }

    public void HideSkillLevelUpEffectWithSlot()
    {
        for (int i = 0; i < kSkillLevelUpList.Count; i++)
            kSkillLevelUpList[i].HideEffect();
    }
}
