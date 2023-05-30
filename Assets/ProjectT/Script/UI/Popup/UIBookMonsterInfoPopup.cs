using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIBookMonsterInfoPopup : FComponent
{
	public UITexture kMonsterTex;
    public UIButton kCvPlayBtn;
    public UILabel kNameLabel;
    public UISprite kTypeSpr;
    public UILabel kTypeLabel;
    public UILabel kCvLabel;
    public UILabel kIllustLabel;
    public UILabel kTalkLabel;
    public UITextList kTextList;
    public UIButton kViewBtn;
    public UILabel kBookNoLabel;
    public UIButton kArrow_RBtn;
    public UIButton kArrow_LBtn;
    private GameClientTable.BookMonster.Param _monstertabledata;
    private List<GameClientTable.Book.Param> _havebooklist = new List<GameClientTable.Book.Param>();
    private int _nowIndex = 0;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();

        kArrow_RBtn.gameObject.SetActive(true);
        kArrow_LBtn.gameObject.SetActive(true);

        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.BookItemID);

        // 보유 도감 리스트
        _havebooklist.Clear();
        _havebooklist = GameInfo.Instance.GameClientTable.FindAllBook(x => x.Group == (int)eBookGroup.Monster);

        _nowIndex = 0;
        for (int idx = 0; idx < _havebooklist.Count; idx++)
        {
            if (_havebooklist[idx].ItemID == tableid)
            {
                _nowIndex = idx;
                break;
            }
        }
    }

    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        RenderTargetChar.Instance.DestroyRenderTarget();
    }

    public override void InitComponent()
	{
        int tableid = (int)UIValue.Instance.GetValue(UIValue.EParamType.BookItemID);
        _monstertabledata = GameInfo.Instance.GameClientTable.FindBookMonster(tableid);
        
        RenderTargetChar.Instance.gameObject.SetActive(true);
        RenderTargetChar.Instance.InitRenderTargetFigure(tableid, eCharacterType.Figure);

        var data = GameInfo.Instance.GameClientTable.FindBook(x => x.Group == (int)eBookGroup.Monster && x.ItemID == tableid);
        if (data == null)
            return;
        kBookNoLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText(216), data.Num);
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        FLocalizeString.SetLabel(kNameLabel, _monstertabledata.Name);
        kTypeSpr.spriteName = string.Format("TribeType_{0}", _monstertabledata.MonType);
        FLocalizeString.SetLabel(kTypeLabel, FLocalizeString.Instance.GetText((int)eTEXTID.MON_TYPE_TEXT_START + _monstertabledata.MonType));
        FLocalizeString.SetLabel(kCvLabel, _monstertabledata.Name+ 1000);
        FLocalizeString.SetLabel(kIllustLabel, _monstertabledata.Name + 2000);
        FLocalizeString.SetLabel(kTalkLabel, _monstertabledata.Name + 3000);

        kTextList.textLabel.textlocalize = "";
        kTextList.Clear();
        kTextList.Add(Utility.AppendColorBBCodeString(FLocalizeString.Instance.GetText(_monstertabledata.Name + 4000)));
    }
 

	
	public void OnClick_BackBtn()
	{
        OnClickClose();
    }

    public override void OnClickClose()
    {
        LobbyUIManager.Instance.Renewal("BookItemListPopup");
        base.OnClickClose();
    }

    public void OnClick_CvPlayBtn()
    {

    }

    public void OnClick_ViewBtn()
    {
        //Tween중에는 동작하지 않도록
        if (kMonsterTex.GetComponent<TweenPosition>() != null)
        {
            if (kMonsterTex.GetComponent<TweenPosition>().enabled)
                return;
        }
        //VoiceMgr.Instance.PlayMonster(eVOICEMONSTER.Test, _monstertabledata.ID);
        CharViewer.ShowCharPopup("BookMonsterInfoPopup", kMonsterTex.gameObject, kMonsterTex.transform.parent);
        //UITopPanel toppanel = LobbyUIManager.Instance.GetUI<UITopPanel>("TopPanel");
        //toppanel.SetTopStatePlay(UITopPanel.eTOPSTATE.OUT);
    }

    public void OnClick_Arrow_LBtn()
    {
        int id = GetNextBookID(true);
        if (id != -1)
        {
            SendBookNewConfirm(id);
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, id);
            InitComponent();
            Renewal(true);
        }
    }

    public void OnClick_Arrow_RBtn()
    {
        int id = GetNextBookID(false);
        if (id != -1)
        {
            SendBookNewConfirm(id);
            UIValue.Instance.SetValue(UIValue.EParamType.BookItemID, id);
            InitComponent();
            Renewal(true);
        }
    }

    private int GetNextBookID(bool bleft)
    {
        if (bleft)
        {
            _nowIndex -= 1;
            if (_nowIndex < 0)
                _nowIndex = _havebooklist.Count - 1;
        }
        else
        {
            _nowIndex += 1;
            if (_nowIndex >= _havebooklist.Count)
                _nowIndex = 0;
        }

        return _havebooklist[_nowIndex].ItemID;
    }

    private void SendBookNewConfirm(int id)
    {
        var monstertabledata = GameInfo.Instance.GameClientTable.FindBookMonster(id);
        if (monstertabledata == null)
            return;
        var monsterbookdata = GameInfo.Instance.GetMonsterBookData(monstertabledata.ID);
        if (monsterbookdata == null)
            return;
        if (monsterbookdata.IsOnFlag(eBookStateFlag.NEW_CHK))
            return;

        GameInfo.Instance.Send_ReqBookNewConfirm((int)eBookGroup.Monster, monsterbookdata.TableID, OnNetSetBookState);
    }

    public void OnNetSetBookState(int result, PktMsgType pktmsg)
    {
        if (result != 0)
            return;
    }
}
