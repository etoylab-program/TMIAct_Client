
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionFeliciaUpperAttack : ActionUpperAttack
{
	protected override void ToUpperJump()
	{
		ActionParamUpperJump paramUpperJump = new ActionParamUpperJump(m_owner.cmptJump.m_jumpPower, ActionParamUpperJump.eCheckFallingType.Animation, 
																	   false, this, true, 0.5f);
		SetNextAction(eActionCommand.UpperJump, paramUpperJump);
	}
}
