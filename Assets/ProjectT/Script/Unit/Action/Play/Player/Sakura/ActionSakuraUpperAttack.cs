
public class ActionSakuraUpperAttack : ActionUpperAttack
{
    protected override eAnimation GetCurAni()
    {
        return eAnimation.UpperAttack01;
    }

    protected override void ToUpperJump()
    {
        ActionParamUpperJump paramUpperJump = null;
        paramUpperJump = new ActionParamUpperJump(m_owner.cmptJump.m_jumpPower, ActionParamUpperJump.eCheckFallingType.IsFfalling, true, this);

        SetNextAction(eActionCommand.UpperJump, paramUpperJump);
    }
}
