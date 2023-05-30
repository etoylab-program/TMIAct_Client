
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class BattleAreaScenario : MonoBehaviour
{
    public int      ScenarioId      = -1;
    public string   DirectorPath    = null;

    private WorldStage  mWorldStage = null;
    private Director    mDirector   = null;


    private void Awake()
    {
        mWorldStage = World.Instance as WorldStage;

        BoxCollider boxCol = GetComponent<BoxCollider>();
        boxCol.isTrigger = true;

        if (!string.IsNullOrEmpty(DirectorPath))
        {
            //if (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos)
            //    DirectorPath += "_aos";

            mDirector = GameSupport.CreateDirector("cinematic_cutscene/" + DirectorPath);
            mDirector.Init(null);
            mDirector.SetCallbackOnEnd2(OnEnd);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (ScenarioId <= 0 && string.IsNullOrEmpty(DirectorPath))
        {
            Debug.LogError("시나리오 아이디나 디렉터 경로 중 하나는 값이 필요합니다.");
            return;
        }

        if (!col.CompareTag("Player"))
        {
            return;
        }

        if(ScenarioId > 0)
        {
            mWorldStage.ShowScenario(ScenarioId, OnEnd);
        }
        else if(mDirector)
        {
            mDirector.Play();
        }
    }

    private void OnEnd()
    {
        gameObject.SetActive(false);
    }
}
