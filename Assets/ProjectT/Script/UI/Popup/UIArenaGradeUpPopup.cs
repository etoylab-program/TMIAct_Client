
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIArenaGradeUpPopup : FComponent
{
    [Header("[Property]")]
    public UILabel  LbDesc;
    public UISprite SprBeforeGrade;
    public UISprite SprCurrentGrade;

    private WorldPVP mWorldPVP = null;


    public override void OnEnable()
    {
        base.OnEnable();
        InitComponent();
    }

	public override void InitComponent() {
		base.InitComponent();

		if ( mWorldPVP == null ) {
			mWorldPVP = World.Instance as WorldPVP;
		}

		SprBeforeGrade.spriteName = mWorldPVP.ParamBeforeGrade.Icon;
		SprCurrentGrade.spriteName = mWorldPVP.ParamCurrentGrade.Icon;

		LbDesc.textlocalize = string.Format( FLocalizeString.Instance.GetText( 3167 ), FLocalizeString.Instance.GetText( mWorldPVP.ParamCurrentGrade.Name ) );
	}

	private void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        if(AppMgr.Instance.CustomInput.GetButtonDown(BaseCustomInput.eKeyKind.Select))
        {
            OnClickClose();
            Invoke("SendToResultPanel", 0.1f);
        }
    }

    private void SendToResultPanel()
    {
        UIArenaResultPopup resultPanel = GameUIManager.Instance.GetUI<UIArenaResultPopup>("ArenaResultPopup");
        resultPanel.PossibleToInput = true;
    }
}
