

public class ActionParamMaikaAttackDuringAttack : IActionBaseParam
{
	public int AttackIndex { get; private set; } = 0;


    public void SetAttackIndex(int attackIndex)
	{
		AttackIndex = attackIndex;
	}
}
