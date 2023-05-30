using UnityEngine;
using System.Collections;

public class UICardDispatchUnit : FUnit
{
    public UICardDispatchListSlot ParentSlot;
    public int Index;

    public UISprite sprCardType;

    public UITexture texSupport;
    public UISprite sprSupportBG;
    public UISprite sprEmptyIcon;
    public UISprite sprEmptyBG;
    public UISprite sprChangeIcon;
    
    public GameObject goChange;

    private CardData _SupportData = null;
    public CardData SupportCardData { get { return _SupportData; } }

    private Dispatch _DispatchData;
    public Dispatch DispatchData { get { return _DispatchData; } }

    private bool IsChange = true;

    private int _cardType = 0;
    public int CardType { get { return _cardType; } }

    public bool IsEmpty { get { return _SupportData == null; } }
    public bool IsActive {  get { return gameObject.activeSelf; } }

    public void Init()
    {
        _SupportData = null;
        _DispatchData = null;
        _cardType = 0;
    }

    public void SetData(CardData _Support, Dispatch _Dispatch, bool isChange)
    {
        _SupportData = _Support;
        _DispatchData = _Dispatch;
        IsChange = isChange;

        Refresh();
    }

    private void Refresh()
    {
        SetActive(true);

        sprCardType.SetActive(true);
        texSupport.SetActive(false); 
        sprSupportBG.SetActive(false);
        sprEmptyIcon.SetActive(false);
        sprEmptyBG.SetActive(false);        
        goChange.SetActive(IsChange);

        if (_SupportData == null)
        {
            //Empty 표현            
            sprEmptyIcon.SetActive(true);
            sprEmptyBG.SetActive(true);

            _cardType = 0;
            switch (Index)
            {
                case 0: _cardType = _DispatchData.TableData.SocketType1; break;
                case 1: _cardType = _DispatchData.TableData.SocketType2; break;
                case 2: _cardType = _DispatchData.TableData.SocketType3; break;
                case 3: _cardType = _DispatchData.TableData.SocketType4; break;
                case 4: _cardType = _DispatchData.TableData.SocketType5; break;
            }
            if (_cardType == 99)
                sprCardType.spriteName = "SupporterType_0";
            else
                sprCardType.spriteName = "SupporterType_" + _cardType.ToString();

            return;
        }

        //할당된 서포터 표현
        sprCardType.SetActive(true);
        texSupport.SetActive(true);
        sprSupportBG.SetActive(true);
        texSupport.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", _SupportData.TableData.Icon));
        sprCardType.spriteName = "SupporterType_" + _SupportData.Type.ToString();

        sprSupportBG.spriteName = "itembgSlot_weapon_" + _SupportData.TableData.Grade.ToString();
    }

    public void OnClick_ChangeSupporter()
    {
        //서포터 교체 요청
        //Debug.Log("OnClick_ChangeSupporter " + SupportData.TableData.ID);
        Debug.Log("OnClick_ChangeSupporter ");

        if (!IsChange)
            return;

        DispatchCardSelectPopup.Show(this);
    }
}