using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICampaignMarkUnit : FUnit {

    public enum eMarkType
    {
        None = 0,
        Parents,
        Child,
    }
    [System.Serializable]
    public class CampaignType
    {
        public eGuerrillaCampaignType kType;
        public int kCondition;
    }
    public List<CampaignType> kCampaignTypeList = new List<CampaignType>();
    public eMarkType kMarkType = eMarkType.None;
    public UISprite kCampaignTextSpr;

    public void UpdateSlot(GuerrillaCampData campdata, bool isChildShow = false)
	{
        bool bshow = false;
        eGuerrillaCampaignType etype = GameSupport.GetGuerrillaCampaignType(campdata.Type);
        if (kMarkType == eMarkType.None)
        {
            var data = kCampaignTypeList.Find(x => x.kType == etype && x.kCondition == campdata.Condition);
            if (data == null && campdata.Condition == 0)
            {
                data = kCampaignTypeList.Find(x => x.kType == etype);
            }
            if (data != null)
                bshow = true;
        }
        else if (kMarkType == eMarkType.Parents)
        {
            var data = kCampaignTypeList.Find(x => x.kType == etype);
            if (data != null)
                bshow = true;
        }
        else if (kMarkType == eMarkType.Child)
        {
            if (isChildShow == true)
            {
                bshow = true;
            }
        }

        if (bshow)
        {
            this.gameObject.SetActive(true);
            if (kCampaignTextSpr != null)
            {
                if (kCampaignTextSpr != null)
                {
                    kCampaignTextSpr.spriteName = GameSupport.GetGuerrillaCampaignSprName(campdata.Type);
                    kCampaignTextSpr.MakePixelPerfect();
                }
            }
        }
	}
 
	public void OnClick_Slot()
	{

	}

    public bool IsCampaignTypeCondition(GuerrillaCampData campdata, int condi)
    {
        eGuerrillaCampaignType etype = GameSupport.GetGuerrillaCampaignType(campdata.Type);

        var data = kCampaignTypeList.Find(x => x.kType == etype);
        if (data == null)
            return false;
        if (campdata.Condition != condi)
            return false;
        return true;
    }

    public bool IsCampaignType(eGuerrillaCampaignType type)
    {
        var data = kCampaignTypeList.Find(x => x.kType == type);
        if (data == null)
            return false;
        return true;
    }
}
