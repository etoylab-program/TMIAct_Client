using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScreenEffect : MonoBehaviour
{
    public GameObject kComboFixed;
    public GameObject kDamage;
    public GameObject kBossWarning;
    public GameObject kHUD;

    public UILabel kComboFixedCount;
    public UILabel kComboFixedCountShadow;
    public UILabel comboCount10n;
    public UILabel comboCountShadow10n;
    public UISprite kComboBG;
    public UISprite kComboText;
    public Animation kComboAni;
    public UISprite KComboGauge;
    public GameObject dangerHp;

    [Header("Combo Text Object")]
    public GameObject kComboObj;
    public Vector3 kCombo2DigitsPos;
    public Vector3 kCombo3DigitsPos;

	/*
    [Header("Evade")]
    public GameObject kMissScreen;
	*/

    [Header("Player Skill")]
    public Animation[]  aniSkills;
    public Animation    AniWpnSkill;

    [Header("Supporter Skill")]
    public GameObject kSupporterSkill;
    public UITexture kSupporterTex;
    public List<UITexture> kSupporterTexList;
    public List<ParticleSystem> kSuppoterEffectList;

    [Header("HUD")]
    public UIGrid HUDGrid;
    public UILabel kHUDLabel;
    public UISprite[] kHUDSprs;
    public GameObject HUDRootSupporter;
    public UITexture HUDTexSupporter;


    private const string ANI_SKILL_NAME_IN = "[In]SkillName";
    private const string ANI_SKILL_NAME_OUT = "[OUT]SkillName";

    private Coroutine m_crComboTime = null;
    private float m_comboRemainTime = 0.0f;

    private Animation m_aniSupporterSkill = null;
    private UILabel[] m_lbSkillNames;
    private Queue<string[]> m_qShowSkillName = new Queue<string[]>();

    private UILabel mlbWpnSkillName;

    private List<ConnectCommonEffect>	mListConnectCommonEff	= new List<ConnectCommonEffect>();
	private WaitForFixedUpdate			mWaitForFixedUpdate		= new WaitForFixedUpdate();

	public bool pauseUpdateCombo = false;


    private void Awake()
    {
        m_lbSkillNames = new UILabel[aniSkills.Length];
        for (int i = 0; i < m_lbSkillNames.Length; i++)
            m_lbSkillNames[i] = aniSkills[i].GetComponentInChildren<UILabel>();

        mlbWpnSkillName = AniWpnSkill.GetComponentInChildren<UILabel>();

        ConnectCommonEffect[] finds = gameObject.GetComponentsInChildren<ConnectCommonEffect>(true);
        if(finds != null && finds.Length > 0)
        {
            mListConnectCommonEff.AddRange(finds);
        }

        //kSupporterTex.shader = Shader.Find("eTOYLab/AlphaBlend");
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            //ShowCombo(1);
        }
        if (Input.GetKeyUp(KeyCode.F2))
        {
            ShowDamage();
        }
        if (Input.GetKeyUp(KeyCode.F3))
        {
            //ShowBossWarning();
        }
        if (Input.GetKeyUp(KeyCode.F4))
        {
            ShowSupporterSkill(true, 1, null);
        }
    }
#endif

    /*
    public void InitSlotWeigth(List<float> slotweigthlist)
    {
        for (int i = 0; i < kSlotList.Count; i++)
        {
            if (kSlotList[i] == null)
                continue;

            var flabel = kSlotFixedList[i].GetComponentInChildren<UILabel>();
            if (flabel == null)
                continue;

            var slabel = kSlotList[i].GetComponentInChildren<UILabel>();
            if (slabel == null)
                continue;
          

            if (slotweigthlist[i] == 0.0f)
            {
                flabel.text = string.Format("[FF0000FF]{0}[-]", StageManager.CardNumber[i]);
                flabel.text = string.Format("[FF0000FF]{0}[-]", StageManager.CardNumber[i]);
                continue;
            }

            if ( slotweigthlist[i] == 1.0f )
            {
                flabel.text = string.Format("[FFFFFFFF]{0}[-]", StageManager.CardNumber[i]);
                flabel.text = string.Format("[FFFFFFFF]{0}[-]", StageManager.CardNumber[i]);
                continue;
            }

            if (slotweigthlist[i] > 1.0f)
            {
                flabel.text = string.Format("[00FF00FF]{0}[-]", StageManager.CardNumber[i]);
                flabel.text = string.Format("[00FF00FF]{0}[-]", StageManager.CardNumber[i]);
            }
            else
            {
                flabel.text = string.Format("[FF8000FF]{0}[-]", StageManager.CardNumber[i]);
                flabel.text = string.Format("[FF8000FF]{0}[-]", StageManager.CardNumber[i]);
            }
        }
    }
    */

    public void HideAll()
    {
        kComboFixed.gameObject.SetActive(false);
        kDamage.gameObject.SetActive(false);
        kBossWarning.gameObject.SetActive(false);
        //kMissScreen.gameObject.SetActive(false);
        kSupporterSkill.gameObject.SetActive(false);
        kHUD.gameObject.SetActive(false);

        for (int i = 0; i < aniSkills.Length; i++)
            aniSkills[i].gameObject.SetActive(false);

        AniWpnSkill.gameObject.SetActive(false);

        dangerHp.gameObject.SetActive(false);
    }

    public void ShowCombo( int count, bool ignoreTime )
    {
        kComboFixedCount.text = string.Format("{0:#,##0}", count);
        if(kComboFixedCountShadow)
            kComboFixedCountShadow.text = kComboFixedCount.text;

        if(count % 10 == 0)
        {
            if (comboCount10n)
                comboCount10n.text = kComboFixedCount.text;
            if (comboCountShadow10n)
                comboCountShadow10n.text = kComboFixedCountShadow.text;

            if (comboCount10n)
                comboCount10n.gameObject.SetActive(true);
            if (comboCountShadow10n)
                comboCountShadow10n.gameObject.SetActive(true);

            Invoke("HideComboCount10n", GameSupport.GetMaxTweenerDuration(comboCount10n.gameObject));
        }

        if (count <= 99)
            kComboObj.transform.localPosition = kCombo2DigitsPos;
        else
            kComboObj.transform.localPosition = kCombo3DigitsPos;

        if ( !kComboFixed.activeSelf )
            GameSupport.TweenerPlay(kComboFixed);

        if (kComboFixedCount)
        {
            GameSupport.TweenerPlay(kComboFixedCount.gameObject);

            if (kComboFixedCountShadow)
                GameSupport.TweenerPlay(kComboFixedCountShadow.gameObject);
        }

        if (KComboGauge && !ignoreTime)
        {
            m_comboRemainTime = GameInfo.Instance.BattleConfig.ComboClearTime;
            KComboGauge.fillAmount = 1.0f;

            if (m_crComboTime == null)
            {
                pauseUpdateCombo = false;
                m_crComboTime = StartCoroutine(UpdateComboTime());
            }
        }
        else
        {
            Utility.StopCoroutine(this, ref m_crComboTime);
            KComboGauge.fillAmount = 1.0f;
        }

        /*
        if (count >= 30)
        {
            kComboText.color = new Color(1.0f, 0.88f, 0.77f);
            kComboCount.color = new Color(1.0f, 0.88f, 0.77f);
            kComboBG.color = new Color(0.78f, 0.21f, 0.0f);            
        }
        else if(count >= 25)
        {
            kComboText.color = new Color(1.0f, 0.88f, 0.77f);
            kComboCount.color = new Color(1.0f, 0.88f, 0.77f);
            kComboBG.color = new Color(0.89f, 0.46f, 0.05f);
        }
        else if (count >= 20)
        {
            kComboText.color = new Color(0.33f, 0.23f, 0.0f);
            kComboCount.color = new Color(0.33f, 0.23f, 0.0f);
            kComboBG.color = new Color(1.0f, 0.82f, 0.15f);
        }
        else if (count >= 15)
        {
            kComboText.color = new Color(1.0f, 0.79f, 0.99f);
            kComboCount.color = new Color(1.0f, 0.79f, 0.99f);
            kComboBG.color = new Color(0.73f, 0.1f, 0.85f);
        }
        else if (count >= 10)
        {
            kComboText.color = new Color(0.0f, 0.23f, 0.34f);
            kComboCount.color = new Color(0.0f, 0.23f, 0.34f);
            kComboBG.color = new Color(0.11f, 0.7f, 0.86f);
        }
        else if (count >= 5)
        {
            kComboText.color = new Color(0.85f, 1.0f, 0.85f);
            kComboCount.color = new Color(0.85f, 1.0f, 0.85f);
            kComboBG.color = new Color(0.39f, 0.69f, 0.13f);
        }
        else
        {
            kComboText.color = new Color(0.92f, 0.92f, 0.92f);
            kComboCount.color = new Color(0.92f, 0.92f, 0.92f);
            kComboBG.color = new Color(0.6f, 0.6f, 0.6f);
        }


        if (count == 30)
        {
            GameSupport.TweenerPlay(kComboFixed);
            kComboAni.Play();
        }
        else if (count == 25)
        {
            GameSupport.TweenerPlay(kComboFixed);
            kComboAni.Play();
        }
        else if (count == 20)
        {
            GameSupport.TweenerPlay(kComboFixed);
            kComboAni.Play();
        }
        else if (count == 15)
        {
            GameSupport.TweenerPlay(kComboFixed);
            kComboAni.Play();
        }
        else if (count == 10)
        {
            GameSupport.TweenerPlay(kComboFixed);
            kComboAni.Play();
        }
        else if (count == 5)
        {
            GameSupport.TweenerPlay(kComboFixed);
            kComboAni.Play();
        }*/
    }

    public void ShowHUD(string text, string[] sprNames, bool useSupporterTex, float hideTime = 0.0f)
    {
        StopCoroutine("UpdateHideHUD");
        kHUD.gameObject.SetActive(true);

        kHUDLabel.color = new Color(kHUDLabel.color.r, kHUDLabel.color.g, kHUDLabel.color.b, 1.0f);
        kHUDLabel.textlocalize = text;

        for (int i = 0; i < kHUDSprs.Length; i++)
        {
            kHUDSprs[i].gameObject.SetActive(false);
        }

        HUDRootSupporter.SetActive(false);

        if (!useSupporterTex)
        {
            if(sprNames != null)
            { 
                for (int i = 0; i < sprNames.Length; i++)
                {
                    if (i >= kHUDSprs.Length)
                    {
                        break;
                    }

                    kHUDSprs[i].spriteName = sprNames[i];
                    kHUDSprs[i].gameObject.SetActive(true);
                }
            }

            HUDGrid.Reposition();
        }
        else
        {
            CharData charData = GameInfo.Instance.GetMainChar();
            CardData cardData = charData.GetEquipCard(charData.EquipCard[0]);
            Texture tex = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", string.Format("Icon/Item/{0}.png", cardData.TableData.Icon));

            HUDTexSupporter.mainTexture = tex;
            HUDRootSupporter.SetActive(true);
        }

        if(hideTime > 0.0f)
        {
            StartCoroutine("UpdateHideHUD", hideTime);
        }
    }

    private IEnumerator UpdateHideHUD(float duration)
    {
        float t = 0.0f;
        float alpha = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime / duration;
            alpha = 1.0f - EasingFunction.EaseInExpo(0.0f, 1.0f, t);

            kHUDLabel.color = new Color(kHUDLabel.color.r, kHUDLabel.color.g, kHUDLabel.color.b, alpha);

            yield return null;
        }

        HideHUD(true);
    }

    public void HideHUD(bool check = false)
    {
        if(check && kHUD.gameObject.activeSelf)
        {
            return;
        }

        StopCoroutine("UpdateHideHUD");
        kHUD.gameObject.SetActive(false);
    }

    public void PlayConnectCommonEffect(int commonEffectId)
    {
        //ConnectCommonEffect find = mListConnectCommonEff.Find(x => x.CommonEffectId == commonEffectId);
        ConnectCommonEffect find = null;
        for (int i = 0; i < mListConnectCommonEff.Count; i++)
        {
            if (mListConnectCommonEff[i].CommonEffectId == commonEffectId)
            {
                find = mListConnectCommonEff[i];
                break;
            }
        }

        if (find == null)
            return;

        find.Play();
    }

    private void HideComboCount10n()
    {
        if (comboCount10n)
            comboCount10n.gameObject.SetActive(false);

        if (comboCountShadow10n)
            comboCountShadow10n.gameObject.SetActive(false);
    }

    public void HideCombo()
    {
        kComboFixed.gameObject.SetActive(false);
        Utility.StopCoroutine(this, ref m_crComboTime);
    }

    private IEnumerator UpdateComboTime()
    {
        if (KComboGauge == null)
            yield break;

        while(m_comboRemainTime > 0.0f)
        {
            if (!pauseUpdateCombo)
            {
                m_comboRemainTime -= Time.deltaTime;
                KComboGauge.fillAmount = m_comboRemainTime / GameInfo.Instance.BattleConfig.ComboClearTime;
            }

            yield return null;
        }
    }

    public void ShowDamage()
    {
        GameSupport.TweenerPlay(kDamage);
    }

    public void ShowBossWarning(float autoHideDelay = 0.0f)
    {
        GameSupport.TweenerPlay(kBossWarning);

        if (SoundManager.Instance.GetSnd("BossWarning") == null)
            SoundManager.Instance.AddAudioClip("BossWarning", "Sound/Fx/snd_boss_battle_siren_01.wav", FSaveData.Instance.GetSEVolume());

        SoundManager.Instance.PlaySnd(SoundManager.eSoundType.FX, "BossWarning", FSaveData.Instance.GetSEVolume());

        if (autoHideDelay > 0.0f)
            Invoke("HideBossWarning", autoHideDelay);
    }

    public void HideBossWarning()
    {
        kBossWarning.gameObject.SetActive(false);
    }

	/*
    public void ShowEvade(bool show)
    {
        if (show)
            GameSupport.TweenerPlay(kMissScreen);
        else
            kMissScreen.SetActive(false);
    }
	*/

    public float ShowSupporterSkill(bool show, int grade, Texture texture )
    {
        float duration = 0.0f;

        if (show)
        {
            if (m_aniSupporterSkill == null)
                m_aniSupporterSkill = kSupporterSkill.GetComponentInChildren<Animation>();

            FAniPlay faniplay = kSupporterSkill.GetComponentInChildren<FAniPlay>();

            //if (m_aniSupporterSkill)
            //    duration = m_aniSupporterSkill.clip.length;

            duration = 1.0f;
            for (int i = 0; i < kSuppoterEffectList.Count; i++)
                kSuppoterEffectList[i].gameObject.SetActive(false);

            if (grade == (int)eGRADE.GRADE_UR)
            {
                //for (int i = 0; i < kSuppoterEffectList.Count; i++)
                //    kSuppoterEffectList[i].gameObject.SetActive(true);
                //m_aniSupporterSkill.Play("SupporterSkill_ur");
                duration = faniplay.Play(2);
                kSuppoterEffectList[2].gameObject.SetActive(true);
                kSuppoterEffectList[2].Play();
            }
            else if (grade == (int)eGRADE.GRADE_SR)
            {
                //m_aniSupporterSkill.Play("SupporterSkill_sr");
                duration = faniplay.Play(1);
                kSuppoterEffectList[1].gameObject.SetActive(true);
                kSuppoterEffectList[1].Play();
            }
            else
            {
                duration = faniplay.Play(0);
                //m_aniSupporterSkill.Play("SupporterSkill_n");
                kSuppoterEffectList[0].gameObject.SetActive(true);
                kSuppoterEffectList[0].Play();
            }

            //kSupporterSkill.SetActive(false);

            if (texture != null)
            {
                kSupporterTex.mainTexture = texture;
                for (int i = 0; i < kSupporterTexList.Count; i++)
                    kSupporterTexList[i].mainTexture = texture;
            }

            GameSupport.TweenerPlay(kSupporterSkill);
        }
        else
        {
            kSupporterSkill.SetActive(false);
        }

        return duration;
    }
    
    /*
    void Play(GameObject o)
    {
        o.SetActive(true);
        var list = o.GetComponentsInChildren<UITweener>();
        for (int i = 0; i < list.Length; i++)
        {
            list[i].ResetToBeginning();
            list[i].PlayForward();
        }
    }
    */

    float GetColorValue(float num)
    {
        float value = 0.0039f * num;
        return value;
    }

    public void ShowSkillName(string[] names)
    {
        if (names == null || names.Length <= 0 || !gameObject.activeSelf)
            return;

        m_qShowSkillName.Enqueue(names);

        if (m_qShowSkillName.Count == 1)
            DoShowSkillName();
    }

    private void DoShowSkillName()
    {
        string[] names = m_qShowSkillName.Peek();
        for (int i = 0; i < names.Length; i++)
        {
            aniSkills[i].gameObject.SetActive(true);
            m_lbSkillNames[i].textlocalize = names[i];
        }

        StartCoroutine(HideSkillName(names.Length, aniSkills[0][ANI_SKILL_NAME_IN].length + 1.0f));
    }

	private IEnumerator HideSkillName( int count, float delay ) {
		float time = 0.0f;

		while ( time < delay ) {
			time += Time.fixedDeltaTime;

            if ( m_qShowSkillName.Count > 1 ) {
                time = delay;
            }

			yield return mWaitForFixedUpdate;
		}

        for ( int i = 0; i < count; i++ ) {
            if ( i >= aniSkills.Length ) {
                break;
			}

            aniSkills[i].Play( ANI_SKILL_NAME_OUT );
        }

        if ( aniSkills.Length > 0 ) {
            yield return new WaitForSeconds( aniSkills[0][ANI_SKILL_NAME_OUT].length / ( m_qShowSkillName.Count * 2 ) );
        }

        if ( m_qShowSkillName.Count > 0 ) {
            m_qShowSkillName.Dequeue();
        }

        for ( int i = 0; i < aniSkills.Length; i++ ) {
            if ( i >= aniSkills.Length ) {
                break;
            }

            aniSkills[i].gameObject.SetActive( false );
        }

        if ( m_qShowSkillName.Count > 0 ) {
            DoShowSkillName();
        }
	}

	public void ClearSkillNameQueue()
    {
        m_qShowSkillName.Clear();
    }

    public void ShowWpnSkillName(string name)
    {
        mlbWpnSkillName.text = name;

        AniWpnSkill.gameObject.SetActive(true);
        AniWpnSkill.Play("[In]SkillName");

        Invoke("HideWpnSkillName", 1.5f);
    }

    private void HideWpnSkillName()
    {
        AniWpnSkill.Play("[OUT]SkillName");
    }
}