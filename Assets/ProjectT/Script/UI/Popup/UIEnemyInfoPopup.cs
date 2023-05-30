using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEnemyInfoPopup : FComponent
{

	[SerializeField] private FList _GameStageInfoListInstance;

    List<GameClientTable.HelpEnemyInfo.Param> _enemyInfoList;

	public override void Awake()
	{
		base.Awake();

		if(this._GameStageInfoListInstance == null) return;
		
		this._GameStageInfoListInstance.EventUpdate = this._UpdateGameStageInfoListSlot;
		this._GameStageInfoListInstance.EventGetItemCount = this._GetGameStageInfoElementCount;
	}
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnClick_BackBtn();
        }
    }
 
	public override void InitComponent()
	{
        int stageid = 0;
        int stageType = 1;

        if (UIValue.Instance.ContainsKey(UIValue.EParamType.StageType))
        {
            stageType = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageType, true);
        }

        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {
	        stageid = (int)UIValue.Instance.GetValue(UIValue.EParamType.StageID);
	        if (stageType == 3)
	        {
		        SecretQuestOptionData optionData = GameInfo.Instance.ServerData.SecretQuestOptionList.Find(x => x.GroupId == stageid);
		        if (optionData != null)
		        {
			        stageid = optionData.GroupId * 100 + optionData.LevelId;
		        }
	        }
        }
        else
        {
            stageid = World.Instance.StageData.ID;
        }
        _enemyInfoList = GameInfo.Instance.GameClientTable.FindAllHelpEnemyInfo(x => x.StageType == stageType && x.StageID == stageid);
        Renewal(true);
	}
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        _GameStageInfoListInstance.UpdateList();
        _GameStageInfoListInstance.RefreshNotMove();
	}
 

	
	private void _UpdateGameStageInfoListSlot(int index, GameObject slotObject)
	{
		do
		{
            UIGameStageInfoSlot slot = slotObject.GetComponent<UIGameStageInfoSlot>();
            GameClientTable.HelpEnemyInfo.Param data = null;
            if (null == slot) break;
            if (0 <= index && _enemyInfoList.Count > index)
            {
                data = _enemyInfoList[index];
            }
            if (data == null)
                return;
            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(data);
        } while(false);
	}
	
	private int _GetGameStageInfoElementCount()
	{
        if(_enemyInfoList == null || _enemyInfoList.Count <= 0)
    		return 0;

        return _enemyInfoList.Count;
	}

	
	public void OnClick_BackBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
            base.OnClickClose();
        else
        {
            this.gameObject.SetActive(false);
            //OnClose();
        }
    }
}
