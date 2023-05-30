using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//시설 무기 경험치, 아이템 조합기 패널
public class UIFacilityItemPanel : FComponent
{
    public UILabel kLevelLabel;
    public UILabel kNameLabel;
    public UILabel kDescLabel;

    public UILabel kInfoEffectTitleLabel;
    public UILabel kInfoEffectDescLabel;

    public GameObject kInfo;
    public GameObject kLock;
    public UISprite kLockSpr;
    public UILabel kLockLevelLabel;
    public UIButton kActiveBtn;
    public UIButton kUpgradeBtn;

    public GameObject kLevelUpEff;
    private int _prevLv = -1;

    private int mFacilityId = 0;
    private List<FacilityData> _facilitydata;

    //인스펙터에 등록된 시설 갯수
    public List<UIFacilityItemUnit> kFacilityItemUnitList;

    public List<GameObject> kFacilityBtns = new List<GameObject>();
    

    private int _GetFacilityItemUnitListCount()
    {
        if (_facilitydata == null)
            return 0;
        return _facilitydata.Count;
    }
 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}
 
	public override void InitComponent()
	{
        var obj = UIValue.Instance.GetValue(UIValue.EParamType.FacilityID);
        if (obj == null)
            return;

        mFacilityId = (int)obj;

        if (_facilitydata != null)
        {
            if ((int)obj != _facilitydata[0].TableID)
                _prevLv = -1;
        }

        FacilityData facilitydata = GameInfo.Instance.GetFacilityData((int)obj);
        if(facilitydata == null)
        {
            Debug.LogError((int)obj + " 에 해당하는 시설이 없습니다.");
            return;
        }

        _facilitydata = GameInfo.Instance.FacilityList.FindAll(x => x.TableData.ParentsID == facilitydata.TableData.ParentsID);
        if(_facilitydata == null || _facilitydata.Count <= 0)
        {
            Debug.LogError("Table에 " + facilitydata.TableData.EffectType + " 리스트가 없습니다.");
            return;
        }

        if (_prevLv < 0)
            _prevLv = _facilitydata[0].Level;

        

        if (!_prevLv.Equals(_facilitydata[0].Level))
        {
            _prevLv = _facilitydata[0].Level;
            kLevelUpEff.SetActive(true);
        }
        else
        {
            kLevelUpEff.SetActive(false);
        }

        kInfoEffectTitleLabel.textlocalize = FLocalizeString.Instance.GetText(facilitydata.TableData.EffectDesc);
        kInfoEffectDescLabel.textlocalize = string.Format("{0:#,##0}", GameSupport.GetFacilityEffectValue(facilitydata));
        kInfoEffectDescLabel.transform.localPosition = new Vector3(kInfoEffectTitleLabel.transform.localPosition.x + kInfoEffectTitleLabel.printedSize.x + 10, kInfoEffectTitleLabel.transform.localPosition.y, 0);

        kInfoEffectTitleLabel.gameObject.SetActive(false);
        kInfoEffectDescLabel.gameObject.SetActive(false);
    }
 
	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        //최대 레벨인지 체크 후, 업그레이드 버튼 활성화/비활성화
        FacilityData maxlevelCheck = _facilitydata.Find(x => x.TableData.MaxLevel != 0);
        if(maxlevelCheck != null)
        {
            if (maxlevelCheck.Level >= maxlevelCheck.TableData.MaxLevel)
                kUpgradeBtn.gameObject.SetActive(false);
            else
                kUpgradeBtn.gameObject.SetActive(true);
        }


        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata[0].TableData.Name);
        kDescLabel.textlocalize = FLocalizeString.Instance.GetText(_facilitydata[0].TableData.Desc);

        
        kInfo.gameObject.SetActive(false);
        kLock.gameObject.SetActive(false);

        for (int i = 0; i < kFacilityBtns.Count; i++)
        {
            FIndex fIdx = kFacilityBtns[i].GetComponent<FIndex>();

            if (fIdx.kIndex == (_facilitydata[0].TableID - 1))
            {
                kFacilityBtns[i].transform.Find("dis").gameObject.SetActive(true);
            }
            else
            {
                kFacilityBtns[i].transform.Find("dis").gameObject.SetActive(false);
            }
        }

        //시설 레벨이 0일때 (시설 활성화가 되지 않았을 때) 오브젝트 비활성화
        if (_facilitydata[0].Level == 0)
        {
            kLevelLabel.textlocalize = "";
            kLock.gameObject.SetActive(true);
            if (GameInfo.Instance.UserData.Level < _facilitydata[0].TableData.FacilityOpenUserRank)
            {
                kActiveBtn.gameObject.SetActive(false);
                kLockSpr.gameObject.SetActive(true);
                kLockLevelLabel.gameObject.SetActive(true);
                kLockLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.RANK_TXT_NOW_LV), _facilitydata[0].TableData.FacilityOpenUserRank);
            }
            else
            {
                kActiveBtn.gameObject.SetActive(true);
                kLockSpr.gameObject.SetActive(false);
                kLockLevelLabel.gameObject.SetActive(false);
            }
        }
        else
        {
            kLevelLabel.textlocalize = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LEVEL_TXT_NOW_LV), _facilitydata[0].Level);
            kLevelLabel.transform.localPosition = new Vector3(kNameLabel.transform.localPosition.x + kNameLabel.printedSize.x + 10, kNameLabel.transform.localPosition.y, 0);
            if (_facilitydata[0].TableData.EffectType == "FAC_WEAPON_EXP")
            {
                kInfoEffectTitleLabel.gameObject.SetActive(true);
                kInfoEffectDescLabel.gameObject.SetActive(true);
            }
            kInfo.gameObject.SetActive(true);

            for(int i = 0; i < kFacilityItemUnitList.Count; i++)
            {
                kFacilityItemUnitList[i].UpdateSlot(_facilitydata[i].TableID);
            }
        }

       
    }

    //업그레이드 버튼 클릭
    public void OnClick_UpgradeBtn()
	{
        if (_facilitydata == null)
            return;

        for(int i = 0; i < _facilitydata.Count; i++)
        {
            if (_facilitydata[i].Stats != (int)eFACILITYSTATS.WAIT)
            {
                MessageToastPopup.Show(FLocalizeString.Instance.GetText(3024));
                return;
            }
        }

        UIFacilityUpGradePopup popup = LobbyUIManager.Instance.GetUI<UIFacilityUpGradePopup>("FacilityUpGradePopup");
        if(!popup)
        {
            return;
        }

        popup.FacilityId = mFacilityId;
        LobbyUIManager.Instance.ShowUI("FacilityUpGradePopup", true);
        /*
        LobbyUIManager.Instance.HideUI("TopPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityItemPanel", false);
        */
    }

    //활성화 버튼 클릭
    public void OnClick_ActiveBtn()
    {
        if (GameInfo.Instance.UserData.Level < _facilitydata[0].TableData.FacilityOpenUserRank)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3012), _facilitydata[0].TableData.FacilityOpenUserRank));
            return;
        }

        UIFacilityUpGradePopup popup = LobbyUIManager.Instance.GetUI<UIFacilityUpGradePopup>("FacilityUpGradePopup");
        if (!popup)
        {
            return;
        }

        popup.FacilityId = mFacilityId;
        LobbyUIManager.Instance.ShowUI("FacilityUpGradePopup", true);
        /*
        LobbyUIManager.Instance.HideUI("TopPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityPanel", false);
        LobbyUIManager.Instance.HideUI("FacilityItemPanel", false);
        */
    }
       
    public override bool IsBackButton()
    {
        return true;
    }

    public void OnClick_MoveFacility(int idx)
    {
        if (idx == (_facilitydata[0].TableID - 1))
        {
            return;
        }


        var data = GameInfo.Instance.FacilityList[idx];
        //LobbyUIManager.Instance.ChangeFacility("FAC_WEAPON_EXP");
        LobbyUIManager.Instance.ChangeFacility(data.TableData.EffectType);
    }
}
