using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillTrainingMenuSlot : FSlot
{
    public int kSlotIdx = 0;
    public UISprite kSkillIconSpr;
    public UIButton kSkillTrainingBtn;
	public UISprite kSpr;
	public UILabel kLabel;
	public UISprite kSelSpr;
	public UILabel kSelabel;
    public List<GameObject> kSkillCommds;

    private GameTable.CharacterSkillPassive.Param m_skillInfo = null;

    public void UpdateSlot(int selectIdx)
	{
        SetSelected(kSlotIdx.Equals(selectIdx));

        if(m_skillInfo == null)
        {
            SetSkillInfo();
        }
    }
 
	public void OnClick_Slot()
	{
        if (ParentGO == null)
            return;

        UISkillTrainingMenuPopup uiSkillTrainingPanel = (UISkillTrainingMenuPopup)GameUIManager.Instance.GetUI("SkillTrainingMenuPopup");
        if (uiSkillTrainingPanel != null)
            uiSkillTrainingPanel.OnClick_SkillTrainingBtn(m_skillInfo.Slot);
    }
 
    void SetSkillInfo()
    {
        CharData charData = GameInfo.Instance.GetCharData(GameInfo.Instance.SeleteCharUID);
        if(charData == null)
        {
            Debug.LogError("캐릭터 정보가 없습니다.");
            return;
        }
        m_skillInfo = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == charData.TableID && x.Slot == kSlotIdx && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
        if(m_skillInfo == null)
        {
            Debug.LogError("스킬 정보가 없습니다.");
            return;
        }

        //kSkillIconTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Skill/" + m_skillInfo.Icon + ".png") as Texture2D;
        GameSupport.SetSkillSprite(ref kSkillIconSpr, m_skillInfo.Atlus, m_skillInfo.Icon);

        //kLabel.textlocalize = kSelabel.textlocalize = FLocalizeString.Instance.GetText(m_skillInfo.Name);
        kLabel.textlocalize = kSelabel.textlocalize = string.Empty;

        for (int i = 0; i < kSkillCommds.Count; i++)
            kSkillCommds[i].SetActive(false);

        int index = m_skillInfo.CommandIndex;
        if (0 <= index && kSkillCommds.Count > index)
        {
            kSkillCommds[index].SetActive(true);
        }
    }

    void SetSelected(bool selectFlag)
    {
        kSelSpr.gameObject.SetActive(selectFlag);
        kSelabel.gameObject.SetActive(selectFlag);
    }
}
