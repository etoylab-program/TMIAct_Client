
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldTraining : World
{
    public TrainingroomManager TrainingroomManager { get; private set; } = null;


    public override void Init(int tableId, eSTAGETYPE stageType = eSTAGETYPE.STAGE_NONE)
    {
        base.Init(tableId, stageType);

        AppMgr.Instance.CustomInput.ShowCursor(false);

        GameUIManager.Instance.HideUI("BikeRaceModePanel", false);
        GameUIManager.Instance.ShowUI("SkillTrainingPanel", false);

        Log.Show("#####");
        Player = GameSupport.CreatePlayer(GameSupport.GetGameSeleteCharUID());
        InGameCamera.SetPlayer(Player);

        if (!Player.HasDirector("Start"))
            Player.AddDirector();

        TrainingroomManager = GetComponent<TrainingroomManager>();
        if (TrainingroomManager != null)
        {
            EnemyMgr = TrainingroomManager;

            TrainingroomManager.Init(Player);
            InGameCamera.SetInitialDefaultMode();
        }

        if ( Player.Guardian ) {
            Utility.SetLayer( Player.Guardian.gameObject, (int)eLayer.Player, true );
        }

        Player.PlayDirector("Start", null);
        ListPlayer.Add( Player );

        PostInit();
        GameUIManager.Instance.HideUI( "GameOffPopup", false );
    }
}
