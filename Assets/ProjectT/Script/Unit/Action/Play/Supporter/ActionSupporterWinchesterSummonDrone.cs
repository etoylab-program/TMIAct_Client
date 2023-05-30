
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionSupporterWinchesterSummonDrone : ActionSupporterSkillBase
{
    private static int MAX_DRONE_COUNT = 4;

    private List<DroneUnit.sDroneData>  mListDrone  = new List<DroneUnit.sDroneData>();
    private ActionParamFromBO           mParam      = null;


    public override void Init(int tableId, List<GameTable.CharacterSkillPassive.Param> listAddCharSkillParam)
    {
        base.Init(tableId, listAddCharSkillParam);
        actionCommand = eActionCommand.SupporterWinchesterSummonDrone;

        for (int i = 0; i < MAX_DRONE_COUNT; i++)
        {
            DroneUnit.sDroneData droneData = new DroneUnit.sDroneData();
            droneData.Drone = ResourceMgr.Instance.CreateFromAssetBundle<DroneUnit>("unit", "Unit/NPC/Aina_Drone/Aina_Drone.prefab");
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
        base.OnStart(param);
        mParam = param as ActionParamFromBO;

        for (int i = 0; i < mListDrone.Count; i++)
        {
            DroneUnit.sDroneData data = mListDrone[i];
            if (!data.IsInit)
            {
                data.Drone.Init(0, eCharacterType.Other, "");
                data.Drone.AddAIController("Emily_Drone");

                data.IsInit = true;
            }

            data.Drone.SetDroneUnit(DroneUnit.eDroneType.BySupporter, m_owner, data.Index, mParam.battleOptionData.duration, mParam.battleOptionData.tick);
            data.Drone.SetDroneAttackPower(m_owner.attackPower * mParam.battleOptionData.value);
            //data.Drone.SetTweenToPos(new Vector3(0.0f, 0.05f, 0.0f));

            Utility.StopCoroutine(this, ref data.CrAppear);
            data.CrAppear = StartCoroutine(data.Drone.AppearForSupporter());
        }
    }
}
