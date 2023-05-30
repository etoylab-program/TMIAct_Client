using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillTrainingPanel : FComponent
{
    public UIButton kSkillTrainingBtn;
    public UISprite kSkillIconSpr;
    public UILabel kSkillName;
    public List<GameObject> kSkillCommands;

    public GameObject PCObject;
    public UILabel kLeftKeyLabel;
    public UILabel kRightKeyLabel;

    private GameTable.CharacterSkillPassive.Param m_skillInfo = null;
    private int m_skillSlotIdx = 1;

    private int MaxSkillIndex = 0;
    private List<GameTable.CharacterSkillPassive.Param> m_skillList = new List<GameTable.CharacterSkillPassive.Param>();


    public override void OnEnable()
    {
        InitComponent();
        base.OnEnable();
    }
    
    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

#if !DISABLESTEAMWORKS
    if (!GameSupport.IsInGameTutorial())
    {
        CustomPCInput pcInput = AppMgr.Instance.CustomInput as CustomPCInput;
        if (pcInput != null)
        {
            pcInput.OnKeyMappingCallBack = null;
        }
    }
#endif
		base.OnDisable();
    }

    public override void InitComponent()
    {
        PCObject?.SetActive(false);

#if !DISABLESTEAMWORKS        
        if (!GameSupport.IsInGameTutorial())
        {
            PCObject?.SetActive(true);

            UIGameStoryPausePopup f = GameUIManager.Instance.GetUI("GameStoryPausePopup") as UIGameStoryPausePopup;
            if (f) f.onVoidCallBack = UpdateKeyLabel;

            CustomPCInput pcinput = AppMgr.Instance.CustomInput as CustomPCInput;
            pcinput.OnKeyMappingCallBack = UpdateKeyLabel;

            UpdateKeyLabel();

            var m_charData = GameInfo.Instance.GetCharData(GameInfo.Instance.SeleteCharUID);
            m_skillList.Clear();
            m_skillList = GameInfo.Instance.GameTable.FindAllCharacterSkillPassive(x => x.CharacterID == m_charData.TableID && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
            MaxSkillIndex = m_skillList.Count;
        }        
#endif
    }

    public void SetSkill(int skillSlotIdx)
    {
        m_skillSlotIdx = skillSlotIdx;
        Renewal(true);
    }

    public override void Renewal(bool bChildren)
    {
        base.Renewal(bChildren);

        CharData charData = GameInfo.Instance.GetCharData(GameInfo.Instance.SeleteCharUID);
        if (charData == null)
        {
            Debug.LogError("캐릭터 정보가 없습니다.");
            return;
        }
        m_skillInfo = GameInfo.Instance.GameTable.FindCharacterSkillPassive(x => x.CharacterID == charData.TableID && x.Slot == m_skillSlotIdx && x.ParentsID == -1 && (x.Type == (int)eCHARSKILLPASSIVETYPE.SELECT_NORMAL));
        if (m_skillInfo == null)
        {
            Debug.LogError("스킬 정보가 없습니다.");
            return;
        }

        //kSkillIconTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Skill/" + m_skillInfo.Icon + ".png") as Texture2D;
        GameSupport.SetSkillSprite(ref kSkillIconSpr, m_skillInfo.Atlus, m_skillInfo.Icon);

        for (int i = 0; i < kSkillCommands.Count; i++)
            kSkillCommands[i].SetActive(false);

        int index = m_skillInfo.CommandIndex;
        if (0 <= index && kSkillCommands.Count > index)
        {
            kSkillCommands[index].SetActive(true);
        }
    }

    public void OnClick_SkillTrainingBtn()
    {
        if(Time.timeScale < 1.0f)
        {
            return;
        }

        GameUIManager.Instance.ShowUI("SkillTrainingMenuPopup", true);
    }


    public void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            OnClick_SkillTrainingBtn();
        }
#endif

#if !DISABLESTEAMWORKS
        FixedUpdateTrigger();

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            PrevSkill();
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            NextSkill();
        }
#endif
    }

    private void UpdateKeyLabel()
    {
        if (!PlayerPrefs.HasKey("KeyMappingType") || PlayerPrefs.GetInt("KeyMappingType") == 0)
        {
            kLeftKeyLabel.text = string.Format("{0}\n{1}", "Page", "Up");
            kRightKeyLabel.text = string.Format("{0}\n{1}", "Page", "Down");
        }
        else
        {
            kLeftKeyLabel.text = "LT";
            kRightKeyLabel.text = "RT";
        }
    }

#if !DISABLESTEAMWORKS

    private enum eGamePadTrigger
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,

        ALL = int.MaxValue
    }
    eGamePadTrigger eTriggerState = eGamePadTrigger.None;
    float CurTriggerValue_L = 0f;
    float CurTriggerValue_R = 0f;
    private void FixedUpdateTrigger()
    {
        CurTriggerValue_L = Input.GetAxis("GamePad L Trigger");
        CurTriggerValue_R = Input.GetAxis("GamePad R Trigger");

        if (CurTriggerValue_L == 0f && CurTriggerValue_R == 0f)
            eTriggerState = eGamePadTrigger.None;

        if (eTriggerState == eGamePadTrigger.ALL)
            return;


        if (CurTriggerValue_L != 0f && (eTriggerState & eGamePadTrigger.Left) == 0)
        {
            eTriggerState |= eGamePadTrigger.Left;
            PrevSkill();
        }
        else if (CurTriggerValue_R != 0f && (eTriggerState & eGamePadTrigger.Right) == 0)
        {
            eTriggerState |= eGamePadTrigger.Right;
            NextSkill();
        }
    }

    private void PrevSkill()
    {
        if (TrainingroomManager.Instance.IsUsingSkill())
        {
            return;
        }

        int index = TrainingroomManager.Instance.kSkillSlotIdx;
        index--;
        if (index <= 0)
        {
            index = MaxSkillIndex;
        }
        TrainingroomManager.Instance.SetSkillSlot(index);
    }

    private void NextSkill()
    {
        if(TrainingroomManager.Instance.IsUsingSkill())
        {
            return;
        }

        int index = TrainingroomManager.Instance.kSkillSlotIdx;
        index++;        
        if (index > MaxSkillIndex)
        {
            index = 1;
        }        
        TrainingroomManager.Instance.SetSkillSlot(index);
    }
#endif
}
