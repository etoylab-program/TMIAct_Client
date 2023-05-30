
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterSkillBase : ActionBase
{
    protected ActionParamFromBO mParamFromBO = null;
    protected Player            mOwnerPlayer = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);

        mOwnerPlayer = m_owner as Player;
        TableId = GetInstanceID();
    }

    public override void OnStart(IActionBaseParam param)
    {
        isPlaying = true;

        m_param = param;

        m_endUpdate = false;
        m_checkTime = 0.0f;

        isCancel = false;

        OnStartCallback?.Invoke();
    }
}
