using UnityEngine;
using System.Collections;
using System;

public class UIMainNoticeFacilityListSlot : FSlot {
 
    public enum eType
    {
        Facility,
        Dispatch
    }
    public eType _slotType = eType.Facility;

	public UISprite kBGSpr;
	public UISprite kSelSpr;
	public UISprite kFacilityIconSpr;
	public UILabel kTitleLabel;
    public UIGaugeUnit kGaugeUnit;

    private int _index;
    private NoticeFacilityData _data;
    private FacilityData _facilitydata;
    private string _strTimeColor;

    private NoticeDispatchData _noticeDispatchData;
    private Dispatch _dispatchdata;

    public void UpdateSlot( int index, NoticeFacilityData data) 	//Fill parameter if you need
	{
        _index = index;
        _data = data;
        _slotType = eType.Facility;

        kBGSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);

        _facilitydata = GameInfo.Instance.GetFacilityData(data.FacilityID);
        if (_facilitydata == null)
            return;

        //장착된 서포터가 있는지 검사
        CardData carddata = GameInfo.Instance.GetCardData(_facilitydata.EquipCardUID);
        if (carddata != null)
            _strTimeColor = FLocalizeString.Instance.GetText((int)eTEXTID.GREEN_TEXT_COLOR);
        else
            _strTimeColor = FLocalizeString.Instance.GetText((int)eTEXTID.WHITE_TEXT_COLOR);


        if (_facilitydata.TableData.EffectType == "FAC_CHAR_EXP")
        {
            kFacilityIconSpr.spriteName = "ico_Combat";
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CHAR_SP")
        {
            kFacilityIconSpr.spriteName = "ico_Skill";
        }
        else if (_facilitydata.TableData.EffectType == "FAC_WEAPON_EXP")
        {
            kFacilityIconSpr.spriteName = "ico_WeaponForce";
        }
        else if (_facilitydata.TableData.EffectType == "FAC_ITEM_COMBINE")
        {
            kFacilityIconSpr.spriteName = "ico_item";
        }
        else if (_facilitydata.TableData.EffectType == "FAC_CARD_TRADE")
        {
            kFacilityIconSpr.spriteName = "ico_exchange";
        }
        else if (_facilitydata.TableData.EffectType == "FAC_OPERATION_ROOM")
        {
            kFacilityIconSpr.spriteName = "ico_Operation";
        }

        if (_facilitydata.Stats != (int)eFACILITYSTATS.WAIT)
        {
            var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
            if (diffTime.Ticks > 0)     //진행중
            {
                string strremain = GameSupport.GetFacilityRemainTimeString(_facilitydata.RemainTime, GameSupport.GetCurrentServerTime());
                kTitleLabel.textlocalize = strremain;
                kGaugeUnit.InitGaugeUnit(0.0f);
            }
            else  //완료
            {
                kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
                kGaugeUnit.InitGaugeUnit(1.0f);
                
            }
        }            
    }

    public void UpdateSlot(int index, NoticeDispatchData data)
    {
        _index = index;
        _noticeDispatchData = data;
        _slotType = eType.Dispatch;

        kBGSpr.gameObject.SetActive(false);
        kSelSpr.gameObject.SetActive(false);
        
        _dispatchdata = GameInfo.Instance.Dispatches.Find(x => x.TableID == data.TableID && x.MissionID == data.MissionID);
        if (_dispatchdata == null)
            return;

        kFacilityIconSpr.SetActive(true);
        kFacilityIconSpr.spriteName = "ico_SupporterDisPatch";

        var diffTime = _dispatchdata.EndTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks > 0)     
        {
            string strremain = GameSupport.GetFacilityRemainTimeString(_dispatchdata.EndTime, GameSupport.GetCurrentServerTime());
            kTitleLabel.textlocalize = strremain;
            kGaugeUnit.InitGaugeUnit(0.0f);
        }
        else //완료
        {
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
            kGaugeUnit.InitGaugeUnit(1.0f);
        }

    }

    private void Update()
    {
        switch (_slotType)
        {
            case eType.Facility: Update_Facility(); break;
            case eType.Dispatch: Update_Dispatch(); break;
        }
    }

    private void Update_Facility()
    {
        if (_facilitydata == null)
            return;
        if (_facilitydata.Level == 0)
            return;

        var diffTime = _facilitydata.RemainTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks < 0)     //완료
        {
            kGaugeUnit.InitGaugeUnit(1.0f);
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
            return;
        }

        //진행중    
        float maxmin = GameSupport.GetFacilityTime(_facilitydata, _facilitydata.OperationCnt) * 60;
        float f = 1.0f - ((float)diffTime.TotalSeconds / (float)maxmin);
        string strremain = GameSupport.GetFacilityRemainTimeString(_facilitydata.RemainTime, GameSupport.GetCurrentServerTime());
        kTitleLabel.textlocalize = string.Format(_strTimeColor, strremain);
        kGaugeUnit.InitGaugeUnit(f);
    }

    private void Update_Dispatch()
    {
        if (_dispatchdata == null) return;

        var diffTime = _dispatchdata.EndTime - GameSupport.GetCurrentServerTime();
        if (diffTime.Ticks < 0)     //완료
        {
            kGaugeUnit.InitGaugeUnit(1.0f);
            kTitleLabel.textlocalize = FLocalizeString.Instance.GetText(1125);
            return;
        }

        //진행중    
        float workTime = _dispatchdata.TableData.Time * 60f;
        float f = 1.0f - ((float)diffTime.TotalSeconds / (float)workTime);
        kGaugeUnit.InitGaugeUnit(f);
        kTitleLabel.text = string.Format("{0:#0}:{1:00}:{2:00}", (int)diffTime.TotalHours, (int)diffTime.Minutes, (int)diffTime.Seconds);
    }

    public void OnClick_Slot()
	{
        switch(_slotType)
        {
            case eType.Facility: OnClick_Slot_Facility(); break;
            case eType.Dispatch: OnClick_Slot_Dispatch(); break;
        }
    }

    private void OnClick_Slot_Facility()
    {
        if (_data == null)
            return;
        if (_facilitydata == null)
            return;
        if (_facilitydata.TableData == null)
            return;
        if (ParentGO == null)
            return;

        UIMainPanel panel = ParentGO.GetComponent<UIMainPanel>();
        if (panel == null)
            return;
        panel.OnClick_FacilityBtn(_facilitydata.TableData.ParentsID - 1);
    }

    private void OnClick_Slot_Dispatch()
    {
        if (_dispatchdata == null)
            return;
        if (ParentGO == null)
            return;

        UIMainPanel panel = ParentGO.GetComponent<UIMainPanel>();
        if (panel == null)
            return;
        panel.OnClick_CardDispatch();
    }
}
