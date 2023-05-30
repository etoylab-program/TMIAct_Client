
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterEmilyDrone : ActionSupporterSkillBase
{
    private static int MAX_DRONE_COUNT = 2;

    private List<DroneUnit.sDroneData>  mListDrone  = new List<DroneUnit.sDroneData>();
    private ActionParamFromBO           mParam      = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterEmilyDrone;

        cancelActionCommand = new eActionCommand[1];
        cancelActionCommand[0] = eActionCommand.SupporterEmilyDrone;

        for (int i = 0; i < MAX_DRONE_COUNT; i++)
        {
            DroneUnit.sDroneData droneData = new DroneUnit.sDroneData();
            droneData.Drone = ResourceMgr.Instance.CreateFromAssetBundle<DroneUnit>("unit", "Unit/NPC/Emily_Drone/Emily_Drone.prefab");
            if (droneData.Drone == null)
            {
                continue;
            }

            droneData.Index = mListDrone.Count;
            droneData.IsInit = false;
            droneData.CrAppear = null;

            droneData.Drone.Deactivate();
            mListDrone.Add(droneData);
        }
    }

    public override void OnStart(IActionBaseParam param)
    {
        DroneUnit.sDroneData find = mListDrone.Find(x => !x.Drone.IsActivate());
        if (find == null)
        {
            return;
        }

        base.OnStart(param);
        mParam = param as ActionParamFromBO;

        if (!find.IsInit)
        {
            find.Drone.Init(0, eCharacterType.Other, "");
            find.Drone.AddAIController("Emily_Drone");

            find.IsInit = true;
        }

        find.Drone.SetDroneUnit(DroneUnit.eDroneType.BySupporter, m_owner, find.Index, mParam.battleOptionData.duration, mParam.battleOptionData.tick);
        find.Drone.SetDroneAttackPower(m_owner.attackPower * mParam.battleOptionData.value);

        Utility.StopCoroutine(this, ref find.CrAppear);
        find.CrAppear = StartCoroutine(find.Drone.AppearForSupporter());
    }
}
