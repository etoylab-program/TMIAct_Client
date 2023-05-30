using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillTrainingMenuPopup : FComponent
{

	public UIButton kSkillTrainingBtn;
    public List<UISkillTrainingMenuSlot> kTrainingSkillSlot;

    private int m_selectId;
    private CharData m_charData;
    private List<GameTable.CharacterSkillPassive.Param> m_skillList = new List<GameTable.CharacterSkillPassive.Param>();

    private InGameCamera.EMode m_prevMode;

 
	public override void OnEnable()
	{
		InitComponent();
		base.OnEnable();
	}

	public override void OnDisable() {
		if( AppMgr.Instance.IsQuit ) {
			return;
		}

		World.Instance.InGameCamera.Mode = InGameCamera.EMode.DEFAULT;
		World.Instance.InGameCamera.EndTouch();

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].Input.Pause( false );
		}

		base.OnDisable();
	}

	public override void InitComponent() {
		m_prevMode = World.Instance.InGameCamera.Mode;
		World.Instance.InGameCamera.Mode = InGameCamera.EMode._COUNT;

		for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
			World.Instance.ListPlayer[i].Input.Pause( true );
		}

		m_charData = GameInfo.Instance.GetCharData( GameInfo.Instance.SeleteCharUID );
		
		m_skillList.Clear();
		m_skillList = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive( x => x.CharacterID == m_charData.TableID && x.ParentsID == -1 && ( x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL ) );
		m_selectId = TrainingroomManager.Instance.kSkillSlotIdx;

		Renewal( true );
	}

	public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);

        for(int i = 0; i < kTrainingSkillSlot.Count; i++)
        {
            kTrainingSkillSlot[i].ParentGO = this.gameObject;
            kTrainingSkillSlot[i].UpdateSlot(m_selectId);
        }
	}
 
	public void OnClick_SkillTrainingBtn(int skillIdx)
	{
        TrainingroomManager.Instance.SetSkillSlot(skillIdx);
        
        OnClick_CloseBtn();
    }

	
	public void OnClick_SkillTrainingMenuSlot()
	{
	}
	
	public void OnClick_CloseBtn()
	{
        GameUIManager.Instance.HideUI("SkillTrainingMenuPopup", true);
	}
}
