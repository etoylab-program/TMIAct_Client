
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScenarioParam
{
    public int Group;
    public string BundlePath;
    public int Num;
    public string Type;
    public float Pos;
    public string Value1;
    public string Value2;
    public string Value3;
    public string Value4;
    public string Value5;
    public string Voice;
    public float Time;
    public int Next;
    public string TremIndex;
    public string TremLogOnly;
}

public class TremParam
{
    public int Group;
    public string Title;
    public string Desc;
}


public class ScenarioMgr :  FSingleton<ScenarioMgr>
{
	public static eLANGUAGE Language { get; private set; }

	public List<ScenarioParam>	Scenarios		{ get; private set; } = new List<ScenarioParam>();
	public List<TremParam>		ScenarioTrems	{ get; private set; } = new List<TremParam>();

	private ScenarioInfoTable					mScenarioInfoTable		= null;
	private Scenario_TremTable					mScenarioTremTable		= null;
	private ScenarioTable						mScenarioTable			= null;
	private List<ScenarioTable.Scenario.Param>	mListScenario			= new List<ScenarioTable.Scenario.Param>();
	private string								mCurScenarioTableName	= "";
	private System.Action						OnEnd					= null;


    public void Open( int group, System.Action endCallback = null)
    {
        Language = FLocalizeString.Language;

        InitScenarioScene(group);

        if (Scenarios.Count == 0)
            return;

		if ( AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage ) {
			//World.Instance.InGameCamera.MainCamera.enabled = false;
			GameUIManager.Instance.ShowUI( "GameOffPopup", false );
		}
        
        int stageTableId = -1;
        if( Scenarios[0].Type == "SHOW")
        {
            if( Scenarios[0].Pos == 0 )
            {
                UIValue.Instance.SetValue(UIValue.EParamType.StroyID, group);
                if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
                {
                    LobbyUIManager.Instance.ShowUI("StroyPopup", false);
                }
                else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
                {
                    if(World.Instance.StageInfoType == World.eStageInfoType.New)
                    {
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("WatchStory", new Firebase.Analytics.Parameter("StageId", stageTableId),
                                                                                    new Firebase.Analytics.Parameter("StoryId", group));
                    }

                    GameUIManager.Instance.ShowUI("StroyPopup", false);
                }
            }
            else if (Scenarios[0].Pos == 1)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.StroyID, group);
                if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
                {
                    var popup = LobbyUIManager.Instance.GetUI<UIStoryCommunicationPopup>("StoryCommunicationPopup");
                    if( popup.gameObject.activeSelf == false )
                        LobbyUIManager.Instance.ShowUI("StoryCommunicationPopup", true);
                }
                else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
                {
                    if (World.Instance.StageInfoType == World.eStageInfoType.New)
                    {
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("WatchCommunication", new Firebase.Analytics.Parameter("StageId", stageTableId),
                                                                                            new Firebase.Analytics.Parameter("StoryId", group));
                    }

                    var popup = GameUIManager.Instance.GetUI<UIStoryCommunicationPopup>("StoryCommunicationPopup");
                    if (popup.gameObject.activeSelf == false)
                        GameUIManager.Instance.ShowUI("StoryCommunicationPopup", true);
                }
            }
            else if(Scenarios[0].Pos == 2)
            {
                UIValue.Instance.SetValue(UIValue.EParamType.ScenarioGroupID, group);
                if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
                {
                    UIValue.Instance.SetValue(UIValue.EParamType.SkipCinemaPictureMode, true);
                    LobbyUIManager.Instance.ShowUI("BookCardCinemaPopup", true);
                }
                else if(AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
                {
                    UIBookCardCinemaPopup popup = GameUIManager.Instance.GetUI<UIBookCardCinemaPopup>("BookCardCinemaPopup");
                    if (!popup.gameObject.activeSelf)
                        GameUIManager.Instance.ShowUI("BookCardCinemaPopup", true);
                }
                
            }
        }

        if (endCallback != null)
            OnEnd = endCallback;
    }
    
    public void InitScenarioScene( int group )
    {
        Language = FLocalizeString.Language;

        Scenarios.Clear();
        ScenarioTrems.Clear();

	//#if SCENARIO_TABLE_SPLIT
		if (!LoadScenarioByGroupId(group, false))
		{
			return;
		}
		/*
	#else
		if (mScenarioTable == null)
		{
			mScenarioTable = (ScenarioTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Scenario.asset");
		}

		mListScenario.Clear();
		for (int i = 0; i < mScenarioTable.Scenarios.Count; i++)
		{
			if (mScenarioTable.Scenarios[i].Group == group)
			{
				mListScenario.Add(mScenarioTable.Scenarios[i]);
			}
		}
	#endif
	*/

		for (int i = 0; i < mListScenario.Count; i++)
        {
            var data = new ScenarioParam();
            data.Group = mListScenario[i].Group;
            data.BundlePath = mListScenario[i].BundlePath;
            data.Num = mListScenario[i].Num;
            data.Type = mListScenario[i].Type;
            data.Pos = mListScenario[i].Pos;
            data.Value1 = mListScenario[i].Value1;
            data.Value2 = mListScenario[i].Value2;
            data.Value3 = mListScenario[i].Value3;
            data.Value4 = mListScenario[i].Value4;
            data.Value5 = mListScenario[i].Value5;
            data.Voice = mListScenario[i].Voice;
            data.Time = mListScenario[i].Time;
            data.Next = mListScenario[i].Next;
            data.TremIndex = mListScenario[i].TremIndex;
            data.TremLogOnly = mListScenario[i].TremLogOnly;

            try
            {
                if (mListScenario[i].Type == "CHAR_TALK" || mListScenario[i].Type == "CHAR_TALK_S")
                {
                    ScenarioTable.String.Param stringdata = null;
                    long id = long.Parse(data.Value1);
                    for (int j = 0; j < mScenarioTable.Strings.Count; j++)
                    {
                        if (mScenarioTable.Strings[j].ID == id)
                        {
                            stringdata = mScenarioTable.Strings[j];
                            break;
                        }
                    }

                    if (stringdata != null)
                    {
                        if (Language == eLANGUAGE.KOR)
                        {
                            data.Value1 = stringdata.Name_KOR;
                            data.Value2 = stringdata.Text_KOR;
                        }
                        else if (Language == eLANGUAGE.JPN)
                        {
                            data.Value1 = stringdata.Name_JPN;
                            data.Value2 = stringdata.Text_JPN;
                        }
                        else if (Language == eLANGUAGE.ENG)
                        {
                            data.Value1 = stringdata.Name_ENG;
                            data.Value2 = stringdata.Text_ENG;
                        }
                        else if (Language == eLANGUAGE.CHS)
                        {
                            data.Value1 = stringdata.Name_CHS;
                            data.Value2 = stringdata.Text_CHS;
                        }
                        else if (Language == eLANGUAGE.CHT)
                        {
                            data.Value1 = stringdata.Name_CHT;
                            data.Value2 = stringdata.Text_CHT;
                        }
                        else if (Language == eLANGUAGE.ESP)
                        {
                            data.Value1 = stringdata.Name_ESP;
                            data.Value2 = stringdata.Text_ESP;
                        }
                    }
                }
            }
            catch(System.FormatException e)
            {
                Debug.LogError(e);
            }

            Scenarios.Add(data);
        }

        for (int i = 0; i < Scenarios.Count; i++)
        {
            if (!string.IsNullOrEmpty(Scenarios[i].TremIndex))
            {
                Scenarios[i].TremIndex = Scenarios[i].TremIndex.Replace(" ", "");
				string[] trems = Utility.Split(Scenarios[i].TremIndex, ','); //Scenarios[i].TremIndex.Split(',');
				for (int j = 0; j < trems.Length; j++)
                {
                    var data = ScenarioTrems.Find(x => x.Group == int.Parse(trems[j]));
                    if (data == null)
                    {
					//#if SCENARIO_TABLE_SPLIT
						Scenario_TremTable.Trem.Param paramTrem = mScenarioTremTable.FindTrem(int.Parse(trems[j]));
						/*
					#else
						var paramTrem = mScenarioTable.FindTrem(int.Parse(trems[j]));
					#endif
					*/

						TremParam tremData = new TremParam();
                        tremData.Group = paramTrem.Group;

                        if (Language == eLANGUAGE.KOR)
                        {
                            tremData.Title = paramTrem.Title_KOR;
                            tremData.Desc = paramTrem.Desc_KOR;
                        }
                        else if (Language == eLANGUAGE.JPN)
                        {
                            tremData.Title = paramTrem.Title_JPN;
                            tremData.Desc = paramTrem.Desc_JPN;
                        }
                        else if (Language == eLANGUAGE.ENG)
                        {
                            tremData.Title = paramTrem.Title_ENG;
                            tremData.Desc = paramTrem.Desc_ENG;
                        }
                        else if (Language == eLANGUAGE.CHS)
                        {
                            tremData.Title = paramTrem.Title_CHS;
                            tremData.Desc = paramTrem.Desc_CHS;
                        }
                        else if (Language == eLANGUAGE.CHT)
                        {
                            tremData.Title = paramTrem.Title_CHT;
                            tremData.Desc = paramTrem.Desc_CHT;
                        }
                        else if (Language == eLANGUAGE.ESP)
                        {
                            tremData.Title = paramTrem.Title_ESP;
                            tremData.Desc = paramTrem.Desc_ESP;
                        }
                        
                        ScenarioTrems.Add(tremData);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(Scenarios[i].TremLogOnly))
            {
                Scenarios[i].TremLogOnly = Scenarios[i].TremLogOnly.Replace(" ", "");
				string[] trems = Utility.Split(Scenarios[i].TremLogOnly, ','); //Scenarios[i].TremLogOnly.Split(',');
				for (int j = 0; j < trems.Length; j++)
                {
                    //var data = ScenarioTrems.Find(x => x.Group == int.Parse(trems[j]));
                    TremParam data = null;
                    int compareGroup = int.Parse(trems[j]);
                    for (int k = 0; k < ScenarioTrems.Count; k++)
                    {
                        if(ScenarioTrems[k].Group == compareGroup)
                        {
                            data = ScenarioTrems[k];
                            break;
                        }
                    }

                    if (data == null)
                    {
					//#if SCENARIO_TABLE_SPLIT
						Scenario_TremTable.Trem.Param paramTrem = null;
						int compareGroup2 = int.Parse(trems[j]);
						for (int k = 0; k < mScenarioTremTable.Trems.Count; k++)
						{
							if (mScenarioTremTable.Trems[k].Group == compareGroup2)
							{
								paramTrem = mScenarioTremTable.Trems[k];
								break;
							}
						}
						/*
					#else
						var paramTrem = mScenarioTable.FindTrem(int.Parse(trems[j]));

						int compareGroup2 = int.Parse(trems[j]);                        
                        for (int k = 0; k < mScenarioTable.Trems.Count; k++)
                        {
                            if (mScenarioTable.Trems[k].Group == compareGroup2)
                            {
                                paramTrem = mScenarioTable.Trems[k];
                                break;
                            }
                        }
					#endif
					*/

						TremParam tremData = new TremParam();
                        tremData.Group = paramTrem.Group;
                        if (Language == eLANGUAGE.KOR)
                        {
                            tremData.Title = paramTrem.Title_KOR;
                            tremData.Desc = paramTrem.Desc_KOR;
                        }
                        else if (Language == eLANGUAGE.JPN)
                        {
                            tremData.Title = paramTrem.Title_JPN;
                            tremData.Desc = paramTrem.Desc_JPN;
                        }
                        else if (Language == eLANGUAGE.ENG)
                        {
                            tremData.Title = paramTrem.Title_ENG;
                            tremData.Desc = paramTrem.Desc_ENG;
                        }
                        else if (Language == eLANGUAGE.CHS)
                        {
                            tremData.Title = paramTrem.Title_CHS;
                            tremData.Desc = paramTrem.Desc_CHS;
                        }
                        else if (Language == eLANGUAGE.CHT)
                        {
                            tremData.Title = paramTrem.Title_CHT;
                            tremData.Desc = paramTrem.Desc_CHT;
                        }
                        else if (Language == eLANGUAGE.ESP)
                        {
                            tremData.Title = paramTrem.Title_ESP;
                            tremData.Desc = paramTrem.Desc_ESP;
                        }

                        ScenarioTrems.Add(tremData);

                    }
                }
            }
        }
    }

    public ScenarioParam GetScenarioParam( int index )
    {
        if (0 <= index && Scenarios.Count > index)
            return Scenarios[index];

        return null;
    }

    public TremParam GetScenarioTremParam(int tableID)
    {
        TremParam data = ScenarioTrems.Find(x => x.Group == tableID);
        if (data != null)
            return data;

        return null;
    }

    public string GetText(int groupId, int num)
    {
        Language = FLocalizeString.Language;

	//#if SCENARIO_TABLE_SPLIT
		if (!LoadScenarioByGroupId(groupId, true))
		{
			return string.Empty;

		}
		/*
	#else
		if (mScenarioTable == null)
		{
			mScenarioTable = (ScenarioTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Scenario.asset");
		}

        if (mScenarioTable == null)
        {
            Debug.LogError("시나리오 데이터가 없습니다.");
            return string.Empty;
        }
	#endif
	*/

		string text = null;

		ScenarioTable.Scenario.Param param = mScenarioTable.FindScenario(x => x.Group == groupId && x.Num == num);
        if (param != null)
        {
            if (param.Type == "CHAR_TALK" || param.Type == "CHAR_TALK_S")
            {
                var stringdata = mScenarioTable.FindString(x => x.ID == long.Parse(param.Value1));
                if (stringdata != null)
                {
                    if (Language == eLANGUAGE.KOR)
                        text = stringdata.Text_KOR;
                    else if (Language == eLANGUAGE.JPN)
                        text = stringdata.Text_JPN;
                    else if (Language == eLANGUAGE.ENG)
                        text = stringdata.Text_ENG;
                    else if (Language == eLANGUAGE.CHS)
                        text = stringdata.Text_CHS;
                    else if (Language == eLANGUAGE.CHT)
                        text = stringdata.Text_CHT;
                    else if (Language == eLANGUAGE.ESP)
                        text = stringdata.Text_ESP;
                }
            }
            
        }

        return text;
    }

    public void OnPopupEnd()
    {
        if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Lobby)
        {

        }
        else if (AppMgr.Instance.SceneType == AppMgr.eSceneType.Stage)
        {
            World.Instance.ShowGamePlayUI(1);
        }

        OnEnd?.Invoke();
        InitOnEndAction();
    }

	public void InitOnEndAction()
	{
		OnEnd = null;
	}

	private bool LoadScenarioByGroupId(int groupId, bool loadTableOnly)
	{
		if (mScenarioInfoTable == null)
		{
			mScenarioInfoTable = (ScenarioInfoTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/ScenarioInfo.asset");
		}

		if (mScenarioTremTable == null)
		{
			mScenarioTremTable = (Scenario_TremTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/Scenario_Trem.asset");
		}

		string scenarioTableName = "";
		for(int i = 0; i < mScenarioInfoTable.Infos.Count; ++i)
		{
			if(groupId >= mScenarioInfoTable.Infos[i].MinGroupId && groupId <= mScenarioInfoTable.Infos[i].MaxGroupId)
			{
				scenarioTableName = mScenarioInfoTable.Infos[i].Name;
				break;
			}
		}

		if(string.IsNullOrEmpty(scenarioTableName))
		{
			return false;
		}

		if (mCurScenarioTableName.CompareTo(scenarioTableName) != 0)
		{
			mScenarioTable = (ScenarioTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/" + scenarioTableName + ".asset");
			if (mScenarioTable == null)
			{
				Debug.LogError(scenarioTableName + " 테이블에 " + groupId + "번 시나리오가 없습니다.");
				return false;
			}

			mCurScenarioTableName = scenarioTableName;
		}

		if (loadTableOnly)
		{
			return true;
		}

		mListScenario.Clear();
		for (int i = 0; i < mScenarioTable.Scenarios.Count; i++)
		{
			if (mScenarioTable.Scenarios[i].Group == groupId)
			{
				mListScenario.Add(mScenarioTable.Scenarios[i]);
			}
		}

		return true;
	}
}
