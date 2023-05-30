
using Platforms;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class UIMainCharSetPopup : FComponent
{
    [Header("[Property]")]
    public UITexture[]  ArrTexChar;
    public UISprite     SprSelect;
    public GameObject   CharListPopup;

    [Header("[Char List Property]")]
    public FList        CharSlotList;
    public UIButton     BtnRemoveChar;
    
    public List<GameObject> kFavorObjList;
    public List<UILabel> kFavorLabelList;
    public List<UILabel> kFavorBuffLabelList;
    public List<GameObject> kFavorLockObjList;
    
    private List<CharData>  mListCharData       = new List<CharData>();
    private int             mSelectedSlotIndex  = -1;


    public override void Awake()
    {
        base.Awake();

        CharSlotList.EventUpdate = UpdateCharSlotList;
        CharSlotList.EventGetItemCount = GetCharListCount;
    }

    public override void OnEnable()
    {
		SprSelect.gameObject.SetActive( false );
		CharListPopup.SetActive( false );

		base.OnEnable();
    }

    public override void Renewal(bool bChildren = false)
    {
        base.Renewal(bChildren);

        mListCharData.Clear();
        mListCharData.AddRange(GameInfo.Instance.CharList);

        for (int i = GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length - 1; i >= 0;  i--)
        {
            CharData charData = mListCharData.Find(x => x.CUID == GameInfo.Instance.UserData.ArrLobbyBgCharUid[i]);

            ArrTexChar[i].gameObject.SetActive(charData != null);
            kFavorObjList[i].gameObject.SetActive(charData != null);

            if (charData != null)
            {
                mListCharData.Remove(charData);
                mListCharData.Insert(0, charData);

                kFavorLabelList[i].textlocalize = charData.FavorLevel.ToString();
                ArrTexChar[i].mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon",
                    Utility.AppendString("Icon/Char/ListSlot/ListSlot_", charData.TableData.Icon, "_", charData.EquipCostumeID.ToString(), ".png"));
            }
            else
            {
                kFavorLabelList[i].textlocalize = string.Empty;
            }
        }
    }

    public void SelectLobbyBgChar(CharData charData)
    {
        if(charData == null)
        {
            return;
        }

        int slot = GameInfo.Instance.UserData.GetLobbyBgCharSlotIndex(charData.CUID);
        if(GameInfo.Instance.UserData.ArrLobbyBgCharUid[mSelectedSlotIndex] <= 0 && slot == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(1729));
            return;
        }

        Lobby.Instance.ChangeBgCharUid(mSelectedSlotIndex, charData.CUID, OnChangeBgChar);
    }

    public override void OnClickClose()
    {
        base.OnClickClose();
    }

    public void OnBtnSelectChar1()
    {
        BtnRemoveChar.gameObject.SetActive(false);

        mSelectedSlotIndex = 0;
        SelectChar();
    }

    public void OnBtnSelectChar2()
    {
        BtnRemoveChar.gameObject.SetActive(true);

        mSelectedSlotIndex = 1;
        SelectChar();
    }

    public void OnBtnSelectChar3()
    {
        BtnRemoveChar.gameObject.SetActive(true);

        mSelectedSlotIndex = 2;
        SelectChar();
    }

    public void OnBtnRemoveChar()
    {
        Lobby.Instance.RemoveBgCharUid(mSelectedSlotIndex, OnChangeBgChar);
    }

    public void OnClick_CardFormationBtn() {
        LobbyUIManager.Instance.ShowUI("ArmoryPopup", true);

        OnClickClose();
    }

    private void SelectChar()
    {
        CharListPopup.gameObject.SetActive(true);
        BtnRemoveChar.gameObject.SetActive(mSelectedSlotIndex != 0 && ArrTexChar[mSelectedSlotIndex].gameObject.activeSelf);

        SprSelect.gameObject.SetActive(true);
        SprSelect.transform.SetParent(ArrTexChar[mSelectedSlotIndex].transform.parent);
        Utility.InitTransform(SprSelect.gameObject);

        CharSlotList.UpdateList();
    }

    private void UpdateCharSlotList(int index, GameObject slotObj)
    {
        UIUserCharListSlot slot = slotObj.GetComponent<UIUserCharListSlot>();
        if (slot == null || index < 0 || index >= mListCharData.Count)
        {
            return;
        }

        int slotIndex = -1;
        for(int i = 0; i < GameInfo.Instance.UserData.ArrLobbyBgCharUid.Length; i++)
        {
            if (GameInfo.Instance.UserData.ArrLobbyBgCharUid[i] == mListCharData[index].CUID)
            {
                slotIndex = i;
                break;
            }
        }

        slot.ParentGO = gameObject;
        slot.UpdateLobbyBgCharSlot(mSelectedSlotIndex, mListCharData[index], slotIndex);
    }

    private int GetCharListCount()
    {
        return mListCharData.Count;
    }

    private void OnChangeBgChar(int result, PktMsgType pktmsg)
    {
        if (result != 0)
        {
            return;
        }

        Lobby.Instance.ChangeMainChar();
        Renewal(true);

        CharListPopup.gameObject.SetActive(false);
        SprSelect.gameObject.SetActive(false);
        
        LobbyUIManager.Instance.Renewal("TopPanel");
        LobbyUIManager.Instance.Renewal("CharMainPanel");
        LobbyUIManager.Instance.Renewal("UserInfoPopup");
    }
}
