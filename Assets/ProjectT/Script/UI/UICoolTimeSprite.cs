
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICoolTimeSprite : MonoBehaviour
{
    public UITexture    SkillTex;
    public UISprite     SkillUpSpr;
    public UISprite     SprCoolTime;
    public UILabel      LbCoolTime;

    private ActionSelectSkillBase   mActionSelectedSkill    = null;
    private float                   mRemainTime             = 0.0f;


    public void Init(ActionSelectSkillBase actionSelectedSkill, string atlas, string icon, string upicon )
    {
        //if (SkillTex)
        //{
        //    SkillTex.mainTexture = null;
        //    SkillUpSpr.gameObject.SetActive(false);
        //    if (!string.IsNullOrEmpty(icon))
        //    {
        //        SkillTex.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Skill/" + icon + ".png") as Texture2D;
        //    }
        //}

        if (!string.IsNullOrEmpty(icon))
        {
            SkillUpSpr.gameObject.SetActive(true);
            GameSupport.SetSkillSprite(ref SkillUpSpr, atlas, icon);
        }

        SprCoolTime.fillAmount = 0.0f;
        LbCoolTime.text = "";

        mActionSelectedSkill = actionSelectedSkill;
        mRemainTime = 0.0f;
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

	private void Update() {
		if( mActionSelectedSkill == null || mActionSelectedSkill.PossibleToUse ) {
			SprCoolTime.fillAmount = 0.0f;
			LbCoolTime.text = "";

			return;
		}

		mRemainTime = ( mActionSelectedSkill.GetMaxCoolTime() - mActionSelectedSkill.CoolTime );
		if( !mActionSelectedSkill.PassingCoolTime ) {
			SprCoolTime.fillAmount = 1.0f;
			LbCoolTime.text = "";
		}
		else {
			SprCoolTime.fillAmount = mRemainTime / mActionSelectedSkill.GetMaxCoolTime();

#if UNITY_EDITOR
			LbCoolTime.text = mRemainTime.ToString( "F1" );
#else
            LbCoolTime.text = mRemainTime.ToString("F0");
#endif
		}
	}
}
