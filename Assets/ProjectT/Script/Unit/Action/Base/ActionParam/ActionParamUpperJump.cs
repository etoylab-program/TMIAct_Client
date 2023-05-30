
using UnityEngine;


public class ActionParamUpperJump : IActionBaseParam
{
    public enum eCheckFallingType
    {
        Animation = 0,
        IsFfalling,
    }


    public float                    jumpPower           { get; private set; }
    public eCheckFallingType        checkFallingType    { get; private set; }
    public bool                     hideMeshOnJumping   { get; private set; }
    public ActionSelectSkillBase    ExecuteAction       { get; private set; } // UpperJump 액션을 실행시킨 액션
	public bool						SlowFalling			{ get; private set; } = false;
	public float					SlowFallingDuration { get; private set; } = 0.0f;
    

    public ActionParamUpperJump(float jumpPower, eCheckFallingType checkFallingType, bool hideMeshOnJumping, ActionSelectSkillBase executeAction)
    {
        this.jumpPower = jumpPower;
        this.checkFallingType = checkFallingType;
        this.hideMeshOnJumping = hideMeshOnJumping;
        ExecuteAction = executeAction;
    }

	public ActionParamUpperJump(float jumpPower, eCheckFallingType checkFallingType, bool hideMeshOnJumping, ActionSelectSkillBase executeAction,
								bool slowFalling, float slowFallingDuration)
	{
		this.jumpPower = jumpPower;
		this.checkFallingType = checkFallingType;
		this.hideMeshOnJumping = hideMeshOnJumping;
		ExecuteAction = executeAction;
		SlowFalling = slowFalling;
		SlowFallingDuration = slowFallingDuration;
	}
}
