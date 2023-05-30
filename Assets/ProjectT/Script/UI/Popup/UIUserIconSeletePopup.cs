using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIUserIconSeletePopup : FComponent
{
    public UIButton kChangeBtn;
	public UITexture kUserIconTex;
    public UILabel kGetLabel;
    public UILabel kGetDescLabel;
	public UILabel kConceptDescLabel;
    [SerializeField] private FList _UserIconListInstance;
    private List<GameTable.UserMark.Param> _usermarklist = new List<GameTable.UserMark.Param>();
    private int seletemarkid;
    public override void Awake()
	{
		base.Awake();

		if(this._UserIconListInstance == null) return;
		
		this._UserIconListInstance.EventUpdate = this._UpdateUserIconListSlot;
		this._UserIconListInstance.EventGetItemCount = this._GetUserIconElementCount;
        this._UserIconListInstance.InitBottomFixing();

    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
    public int CompareFuncOrderNum(GameTable.UserMark.Param a, GameTable.UserMark.Param b)
    {
        if (a.OrderNum < b.OrderNum) return -1;
        if (a.OrderNum > b.OrderNum) return 1;
        return 0;
    }
    public override void InitComponent()
	{
        seletemarkid = GameInfo.Instance.UserData.UserMarkID;

        _usermarklist.Clear();

        for( int i = 0; i < GameInfo.Instance.GameTable.UserMarks.Count; i++ )
        {
            var data = GameInfo.Instance.GameTable.UserMarks[i];
            if( data.PreVisible == 0 )
            {
                if (!GameInfo.Instance.IsUserMark(data.ID))
                    continue;
            }
            _usermarklist.Add(data);
        }
        _usermarklist.Sort(CompareFuncOrderNum);

        _UserIconListInstance.UpdateList();
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        var tabledata = GameInfo.Instance.GameTable.FindUserMark(seletemarkid);
        if(tabledata == null)
        {
            kUserIconTex.mainTexture = null;
            kGetLabel.textlocalize = "";
            kGetDescLabel.textlocalize = "";
            kConceptDescLabel.textlocalize = "";
        }
        else
        {
            LobbyUIManager.Instance.GetUserMarkIcon(this.gameObject, this.gameObject, tabledata.ID, ref kUserIconTex);

            kGetLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.Name);
            kGetDescLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.Desc);
            kConceptDescLabel.textlocalize = FLocalizeString.Instance.GetText(tabledata.GetDesc);

            kChangeBtn.isEnabled = GameInfo.Instance.IsUserMark(tabledata.ID);
        }

        _UserIconListInstance.RefreshNotMove();
    }
 
	private void _UpdateUserIconListSlot(int index, GameObject slotObject)
	{
        do
        {
            UIUserIconListSlot card = slotObject.GetComponent<UIUserIconListSlot>();
            if (null == card) break;
          
            GameTable.UserMark.Param data = null;
            if (0 <= index && _usermarklist.Count > index)
                data = _usermarklist[index];

            card.ParentGO = this.gameObject;
            card.UpdateSlot(index, data, seletemarkid);
        }while(false);
	}
	
	private int _GetUserIconElementCount()
	{
		return _usermarklist.Count;
	}
	
	public void OnClick_ChangeBtn()
	{
        var tabledata = GameInfo.Instance.GameTable.FindUserMark(seletemarkid);
        if (tabledata == null)
            return;

        if (!GameInfo.Instance.IsUserMark(tabledata.ID))
            return;

        if (GameInfo.Instance.UserData.UserMarkID == seletemarkid)
        {
            OnClick_BackBtn();
            return;
        }

        GameInfo.Instance.Send_ReqUserSetMark(seletemarkid, OnNetChangeUserMark);
    }

	
	public void OnClick_BackBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        GameInfo.Instance.DeleteNewIcon();
        base.OnClickClose();
    }

    public void SetSeleteID( int id )
    {
        seletemarkid = id;
        Renewal(true);
    }

    public void OnNetChangeUserMark(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;

        //MessageToastPopup.Show(FLocalizeString.Instance.GetText(3065));

        LobbyUIManager.Instance.Renewal("UserInfoPopup");
        LobbyUIManager.Instance.Renewal("TopPanel");
        Renewal(true);
        OnClick_BackBtn();
    }
}
