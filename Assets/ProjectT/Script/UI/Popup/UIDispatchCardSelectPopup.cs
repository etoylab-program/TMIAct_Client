
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DispatchCardSelectPopup
{
    public static void Show(UICardDispatchUnit unit)
    {
        if (AppMgr.Instance.SceneType != AppMgr.eSceneType.Lobby)
            return;

        UIDispatchCardSelectPopup popup = LobbyUIManager.Instance.GetUI("DispatchCardSelectPopup") as UIDispatchCardSelectPopup;
        if(popup)
        {
            popup.SetUnit(unit);
            popup.SetUIActive(true);
        }
    }
}

public class UIDispatchCardSelectPopup : FComponent
{
    public UILabel kTimeLabel;
    public UILabel kEffectLabel;
    public UIButton kChangeBtn;
    public UIButton kRemoveBtn;

    [SerializeField] private FList _ItemListInstance;    
    private List<CardData> _cardlist = new List<CardData>();    
    private CardData _carddata;
    private CardBookData _cardbookdata;

    private UICardDispatchUnit Unit;

    public long SeleteCardUID
    {
        get
        {
            if (_carddata == null)
                return -1;
            return _carddata.CardUID;
        }
    }

    public override void Awake()
    {
        base.Awake();
        if (this._ItemListInstance == null) return;

        this._ItemListInstance.EventUpdate = this._UpdateItemListSlot;
        this._ItemListInstance.EventGetItemCount = this._GetItemElementCount;
        this._ItemListInstance.InitBottomFixing();
    }

    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }

    public void SetUnit(UICardDispatchUnit unit)
    {
        Unit = unit;

        _carddata = null;
        _carddata = GameInfo.Instance.CardList.Find(x =>
            x.PosKind == (int)eContentsPosKind.DISPATCH &&
            x.PosValue == Unit.DispatchData.TableID &&
            x.PosSlot == Unit.Index);
    }

    private int SortOptValue(CardData a, CardData b)
    {
        if (a.TableData.Grade == b.TableData.Grade)
        {
            if (a.TableData.ID == b.TableData.ID)
                return a.Level.CompareTo(b.Level);
            else
                return a.TableData.ID.CompareTo(b.TableData.ID);
        }
        else
        {
            if (a.TableData.Grade < b.TableData.Grade) return -1;
            if (a.TableData.Grade > b.TableData.Grade) return 1;
        }

        return 0;
    }

    public override void InitComponent()
    {
        _cardlist.Clear();        
        List<CardData> tmpOptcardlist = new List<CardData>();

        var SupportList = GameInfo.Instance.CardList;

        var usedList = SupportList.FindAll(x =>
        {
            if(_carddata != null)
            {
               return x.PosKind == (int)eContentsPosKind.DISPATCH && 
                x.PosValue == Unit.DispatchData.TableID &&
                x.TableID != _carddata.TableID;
            }

            return x.PosKind == (int)eContentsPosKind.DISPATCH && x.PosValue == Unit.DispatchData.TableID;
        });
           

        for (int i = 0; i < SupportList.Count; i++)
        {  
            // 사용되고 있는 서포터는 제외
            if (SupportList[i].PosKind != 0 || SupportList[i].PosSlot != 0 || SupportList[i].PosValue != 0)
                continue;

            // Slot에 사용된 서포터들의 TableID가 같은 데이터 제외
            if (usedList != null && usedList.Count > 0)
            {
                var finddata = usedList.Find(x => x.TableID == SupportList[i].TableID);
                if (finddata != null) continue;
            }

            //카드 타입 체크
            if(Unit.CardType != (int)eCARDTYPE.ALL)
            {
                if (SupportList[i].Type != Unit.CardType) continue;
            }

            tmpOptcardlist.Add(SupportList[i]);
        }

        if (tmpOptcardlist == null || tmpOptcardlist.Count == 0)
        {
            _ItemListInstance.UpdateList();
            return;
        }
        
        tmpOptcardlist.Sort(SortOptValue);

        for (int i = 0; i < tmpOptcardlist.Count; i++)
        {
            _cardlist.Insert(0, tmpOptcardlist[i]);
        }

        if (_carddata == null)
            _carddata = _cardlist[0];
        else
            _cardlist.Insert(0, _carddata);


        _ItemListInstance.UpdateList();
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        kTimeLabel.textlocalize = "";
        kEffectLabel.textlocalize = "";

        kChangeBtn.SetActive(false);
        kRemoveBtn.SetActive(false);
        kTimeLabel.SetActive(false);
        kEffectLabel.SetActive(false);

        if (_carddata == null)
            return;

        if (Unit.DispatchData.UsingCardData == null || Unit.DispatchData.UsingCardData.Count == 0)
        {
            kChangeBtn.SetActive(true);
        }
        else
        {
            var d = Unit.DispatchData.UsingCardData.Find(x => x.CardUID == _carddata.CardUID);

            if(d == null) kChangeBtn.SetActive(true);
            else kRemoveBtn.SetActive(true);
        }

        _ItemListInstance.RefreshNotMove();
    }

    private void _UpdateItemListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIItemListSlot card = slotObject.GetComponent<UIItemListSlot>();
            if (null == card) break;

            CardData data = null;
            if (0 <= index && _cardlist.Count > index)
            {
                data = _cardlist[index];
            }

            card.ParentGO = this.gameObject;
            card.UpdateSlot(UIItemListSlot.ePosType.DispatchCard_SelectList, index, data);
        } while (false);
    }

    private int _GetItemElementCount()
    {
        return _cardlist.Count;
    }

    public void SetSelectCardUID(long uid)
    {
        var carddata = GameInfo.Instance.GetCardData(uid);
        if (carddata == null)
            return;
       
        _carddata = carddata;
        _cardbookdata = GameInfo.Instance.GetCardBookData(_carddata.TableID);
        Renewal(true);
    }

    public void OnClick_BackBtn()
    {
        OnClickClose();
    }

    public void OnClick_ChangeBtn()
    {
        if (_carddata == null)
            return;

        var oldData = GameInfo.Instance.CardList.Find(x =>
            x.PosKind == (int)eContentsPosKind.DISPATCH &&
            x.PosSlot == Unit.Index &&
            x.PosValue == Unit.DispatchData.TableID);

        if (oldData != null)
        {
            oldData.InitPos();
        }

        _carddata.SetPos(Unit.DispatchData.TableID, (int)eContentsPosKind.DISPATCH, Unit.Index);
        
        Unit.DispatchData.RefreshData();
        Unit.ParentSlot.SelfUpdate();

        OnClickClose();
    }

    public void OnClick_RemoveBtn()
    {
        if (_carddata == null)
            return;

        _carddata.InitPos();

        Unit.DispatchData.RefreshData();
        Unit.ParentSlot.SelfUpdate();

        OnClickClose();
    }
}
