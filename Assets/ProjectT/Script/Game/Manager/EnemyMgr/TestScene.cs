
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public struct sTestPlayerData {
	public int					TableId;
	public int					Grade;
	public int					Level;
	public int[]				SkillTableIds;
	public int					Skill2ndStatsLevel;
	public sTestSupporterData[]	SupporterDatas;
	public sTestWeaponData[]	WeaponDatas;
	public sTestCostumeData		CostumeData;
	public Vector3				StartPos;
	public Vector3				StartRot;
}

[System.Serializable]
public struct sTestSupporterData {
	public int Id;
	public int Level;
	public int SkillLv;
	public int Wake;
}

[System.Serializable]
public struct sTestWeaponData {
	public int Id;
	public int SkinTID;
	public int Level;
	public int SkillLevel;
	public int Wake;
}

[System.Serializable]
public struct sTestCostumeData {
	public int Id;
	public int Color;
}

[System.Serializable]
public struct sTestHelperData {
	public sTestPlayerData	Data;
	public float			LifeTime;
}


public class TestScene : BaseEnemyMgr {
	[Header( "[Option]" )]
	[SerializeField] private bool				_ShowDistance			= true;
	[SerializeField] private bool				_InvinciblePlayer		= false;
	[SerializeField] private bool				_EnableDamage			= false;
	[SerializeField] private bool				_EasyEvade				= false;
	[SerializeField] private bool				_AutoTargeting			= true;
	[SerializeField] private bool				_AlwaysCritical			= false;
	[SerializeField] private bool				_TurnCameraToTarget		= false;

	[Header( "[Player]" )]
	[SerializeField] private sTestPlayerData	_PlayerData;
	[SerializeField] private int[]				_PlayerBOSetIds;

	[Header( "[Helper]" )]
	[SerializeField] private bool				_UseHelper				= false;
	[SerializeField] private sTestHelperData[]	_HelperDatas;
	[SerializeField] private bool				_UseHelperSkill			= false;
	[SerializeField] private bool				_UseHelperWSkill		= false;
	[SerializeField] private bool				_UseHelperUSkill		= false;

	[Header( "[EnemyMgr]" )]
	[SerializeField] private Transform[]		_EdgePoints				= null;
	[SerializeField] private Vector3			_BaseSpawnPos			= Vector3.zero;
	[SerializeField] private Vector3			_BaseSpawnRot			= Vector3.zero;
	[SerializeField] private int				_SpawnDistance			= 1;

	[Header( "[Enemy]" )]
	[SerializeField] private int[]				_EnemyTableId			= null;
	[SerializeField] private int				_EnemyStageBOSetGroupId	= 0;


	public bool IsInvinciblePlayer	{ get { return _InvinciblePlayer; } }
	public bool IsEnableDamage		{ get { return _EnableDamage; } }
	public bool IsEasyEavde			{ get { return _EasyEvade; } }
	public bool IsAlwaysCritical	{ get { return _AlwaysCritical; } }

	private GameTable		mGameTable			= null;
	private GameClientTable	mGameClientTable	= null;
	private Player			mPlayer				= null;
	private List<Player>	mListHelper			= new List<Player>();


	public override void Init() {
		FSaveData.Instance.AutoTargeting = _AutoTargeting;
		FSaveData.Instance.AutoTargetingSkill = _AutoTargeting;
		FSaveData.Instance.TurnCameraToTarget = _TurnCameraToTarget;
		FSaveData.Instance.SkipDirector = GameInfo.Instance.GameConfig.TestSkipDirector;

		for ( int i = 0; i < envObjects.Length; i++ ) {
			envObjects[i].Init( -1, eCharacterType.Other, null );
			envObjects[i].Activate();
		}

		SetPlayer();
		SetEnemies();

		Active();
	}

	public override List<Unit> GetActiveEnvObjects() {
		List<Unit> list = new List<Unit>();

		for ( int i = 0; i < envObjects.Length; i++ ) {
			if ( envObjects[i] == null || !envObjects[i].IsActivate() || envObjects[i].curHp <= 0.0f ) {
				continue;
			}

			list.Add( envObjects[i] );
		}

		return list;
	}

	public override void AddStageBOSet( int stageBOSetGroupId ) {
		if ( stageBOSetGroupId <= 0 ) {
			return;
		}

		List<GameClientTable.StageBOSet.Param> list = GameInfo.Instance.GameClientTable.FindAllStageBOSet( x => x.Group == stageBOSetGroupId );
		if ( list == null || list.Count <= 0 ) {
			return;
		}

		for ( int i = 0; i < list.Count; i++ ) {
			string str = list[i].StageBOSetList.Replace( " ", "" );

			string[] split = str.Split( ',' );
			for ( int j = 0; j < split.Length; j++ ) {
				int id = Utility.SafeIntParse( split[j] );

				for ( int k = 0; k < m_listEnemy.Count; k++ ) {
					Enemy enemy = m_listEnemy[k] as Enemy;
					if ( enemy == null ) {
						continue;
					}

					enemy.AddBOSet( id );
				}
			}
		}
	}

	public override Transform[] GetEdgePoints() {
		return _EdgePoints;
	}

	public override void Active() {
		base.Active();
		mPlayer.OnMissionStart();

		if ( mListHelper.Count > 0 ) {
			for ( int i = 0; i < mListHelper.Count; i++ ) {
				mListHelper[i].OnMissionStart();
				mListHelper[i].StartBT();
			}
		}

		GameUIManager.Instance.HideUI( "GameOffPopup" );
	}

#if UNITY_EDITOR
	protected override void OnDrawGizmos() {
		base.OnDrawGizmos();

		if ( _ShowDistance == false || mPlayer == null ) {
			return;
		}

		Vector3 playerPos = mPlayer.transform.position;

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine( playerPos, playerPos + ( mPlayer.transform.forward * 3.0f ) );

		Gizmos.color = Color.red;
		Gizmos.DrawLine( Camera.main.transform.position, playerPos );

		if ( m_listEnemy.Count > 0 ) {
			for ( int i = 0; i < m_listEnemy.Count; i++ ) {
				Enemy enemy = m_listEnemy[i] as Enemy;
				if ( enemy == null || enemy.MainCollider == null ) {
					continue;
				}

				DrawEnemyInfo( enemy );

				if ( enemy.child ) {
					DrawEnemyInfo( enemy.child );
				}
			}
		}

		if ( m_listEnemy.Count > 0 ) {
			Gizmos.color = Color.white;

			for ( int i = 0; i < m_listEnemy.Count; i++ ) {
				Enemy enemy = m_listEnemy[i] as Enemy;
				if ( enemy == null || enemy.mainTarget == null ) {
					continue;
				}

				Vector3 from = enemy.mainTarget.transform.position;
				Vector3 to = enemy.MainCollider.GetCenterPos();

				Gizmos.DrawLine( from, to );

				UnityEditor.Handles.Label( Vector3.Lerp( from, to, 0.5f ), Vector3.Distance( from, to ).ToString() );

				for ( int j = 0; j < enemy.ListCollider.Count; j++ ) {
					to = enemy.ListCollider[j].GetCenterPos();

					Gizmos.DrawLine( from, to );
					UnityEditor.Handles.Label( Vector3.Lerp( from, to, 0.5f ), Vector3.Distance( from, to ).ToString() );
				}
			}
		}
	}
#endif

	private void Awake() {
		maxMonsterCountInSpawnGroup = _EnemyTableId.Length;

		if ( AppMgr.Instance.configData.m_EditorTableLoadType == Config.eEditorTableLoadType.Editor ) {
			mGameTable = (GameTable)ResourceMgr.Instance.LoadFromAssetBundle( "system", "System/Table/Game.asset" );
			mGameClientTable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle( "system", "System/Table/GameClientEditor.asset" );
		}
		else {
			mGameTable = (GameTable)ResourceMgr.Instance.LoadFromAssetBundle( "system", "System/Table/Game.asset" );
			mGameClientTable = (GameClientTable)ResourceMgr.Instance.LoadFromAssetBundle( "system", "System/Table/GameClient.asset" );
		}
	}

	private void SetPlayer() {
		CharData charData = Create_PlayerDataOrNull( _PlayerData, false );

		Utility.AddTestPlayerSkill( charData, _PlayerData.SkillTableIds, _PlayerData.Skill2ndStatsLevel );
		Utility.AddTestPlayerSupporter( charData, _PlayerData.SupporterDatas );
		Utility.AddTestPlayerWeapon( charData, _PlayerData.WeaponDatas );
		Utility.AddTestPlayerCostume( charData, _PlayerData.CostumeData );

		mPlayer = GameSupport.CreatePlayer( charData, false, false, false );

		mPlayer.SetInitialPosition( _PlayerData.StartPos, Quaternion.Euler( _PlayerData.StartRot ) );
		//mPlayer.SetAction( true );
		mPlayer.AddDirector();

		GameInfo.Instance.UserData.MainCharUID = mPlayer.charData.CUID;

		World.Instance.InGameCamera.SetPlayer( mPlayer );
		World.Instance.InGameCamera.SetInitialDefaultMode();

#if UNITY_EDITOR
		World.Instance.SetPlayerInTestScene( mPlayer );
#endif

		// 플레이어 배틀옵션
		for ( int i = 0; i < _PlayerBOSetIds.Length; ++i ) {
			mPlayer.AddBOCharBattleOptionSet( _PlayerBOSetIds[i], 1 );

			for ( int j = 0; j < mListHelper.Count; j++ ) {
				mListHelper[j].AddBOCharBattleOptionSet( _PlayerBOSetIds[i], 1 );
			}
		}

		mPlayer.OnGameStart();
		mPlayer.ClearEnemyNavigators();

		SetHelper();

		World.Instance.ListPlayer.Clear();
		World.Instance.ListPlayer.Add( mPlayer );
		World.Instance.ListPlayer.AddRange( mListHelper );

		World.Instance.TestHelperType = World.eTestHelperType.MANUAL;
		World.Instance.UIPlay.ShowRaidUI( true );
	}

	private CharData Create_PlayerDataOrNull( sTestPlayerData data, bool isHelper ) {
		GameTable.Character.Param param = mGameTable.FindCharacter( data.TableId );
		if ( param == null ) {
			Debug.LogError( data.TableId + "번 캐릭터가 없습니다." );
			return null;
		}

		CharData charData = GameInfo.Instance.GetCharDataByTableID( data.TableId );
		if ( charData == null ) {
			NetLocalSvr.Instance.AddChar( data.TableId );
			charData = GameInfo.Instance.GetCharDataByTableID( data.TableId );
		}

		charData.Grade = Mathf.Max( 1, data.Grade );
		charData.Level = Mathf.Max( data.Level );

		return charData;
	}

	private void SetHelper() {
		if ( !_UseHelper || _HelperDatas == null || _HelperDatas.Length <= 0 ) {
			return;
		}

		mListHelper.Clear();

		Vector3 basePos = mPlayer.transform.position - ( mPlayer.transform.forward * 1.0f );
		int evenNumCount = 1;
		int oddNumCount = 1;

		for ( int i = 0; i < _HelperDatas.Length; i++ ) {
			CharData charData = Create_PlayerDataOrNull( _HelperDatas[i].Data, true );
			if ( charData == null ) {
				continue;
			}

			Utility.AddTestPlayerSkill( charData, _HelperDatas[i].Data.SkillTableIds, _HelperDatas[i].Data.Skill2ndStatsLevel );
			Utility.AddTestPlayerSupporter( charData, _HelperDatas[i].Data.SupporterDatas );
			Utility.AddTestPlayerWeapon( charData, _HelperDatas[i].Data.WeaponDatas );
			Utility.AddTestPlayerCostume( charData, _HelperDatas[i].Data.CostumeData );

			Player helper = GameSupport.CreatePlayer( charData, false, false, true );

			helper.SetInitialPosition( _HelperDatas[i].Data.StartPos, Quaternion.Euler( _HelperDatas[i].Data.StartRot ) );
			helper.SetAction( true );
			helper.SetLifeTime( _HelperDatas[i].LifeTime );
			helper.OnGameStart();

			if ( i % 2 == 0 ) {
				helper.SetInitialPosition( basePos - ( mPlayer.transform.right * ( 1.0f * evenNumCount++ ) ), mPlayer.transform.rotation );
			}
			else {
				helper.SetInitialPosition( basePos + ( mPlayer.transform.right * ( 1.0f * oddNumCount++ ) ), mPlayer.transform.rotation );
			}

			helper.AutoSupporterSkill = _UseHelperSkill;
			helper.AutoWeaponSkill = _UseHelperWSkill;
			helper.AutoUltimateSkill = _UseHelperUSkill;

			mListHelper.Add( helper );
			World.Instance.UIPlay.SetSubCharUnit( i, helper );
		}
	}

	private Enemy CreateEnemy( int tableId ) {
		GameClientTable.Monster.Param param = mGameClientTable.FindMonster( tableId );
		if ( param == null ) {
			Debug.LogError( tableId + "번 몬스터가 없습니다." );
			return null;
		}

		string strModel = Utility.AppendString( "Unit/", param.ModelPb, ".prefab" );

		Enemy enemy = ResourceMgr.Instance.CreateFromAssetBundle<Enemy>( "unit", strModel );
		enemy.Init( tableId, eCharacterType.Monster, null );

		if ( enemy.grade == Unit.eGrade.Boss ) {
			EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_ENEMY_BOSS_APPEAR, enemy );
			enemy.actionSystem.CancelCurrentAction();
		}

		if ( param.Child > 0 ) {
			Enemy child = CreateEnemy( param.Child );
			child.SetParent( enemy );

			enemy.SetChild( child );
		}

		enemy.CreateMonsterClone( enemy.CloneCount );
		return enemy;
	}

	private void SetEnemies() {
		if ( _EnemyTableId.Length <= 0 ) {
			return;
		}

		m_listEnemy.Clear();
		for ( int i = 0; i < _EnemyTableId.Length; i++ ) {
			Enemy enemy = CreateEnemy( _EnemyTableId[i] );
			m_listEnemy.Add( enemy );
		}

		AddStageBOSet( _EnemyStageBOSetGroupId );
		maxMonsterCountInSpawnGroup = m_listEnemy.Count;

		for ( int i = 0; i < m_listEnemy.Count; i++ ) {
			int value = Mathf.Min( 7, m_listEnemy.Count );

			float x = (float)Random.Range( (int)( -_SpawnDistance * value ), (int)( _SpawnDistance * value ) );
			float z = (float)Random.Range( (int)( -_SpawnDistance * value ), (int)( _SpawnDistance * value ) );

			m_listEnemy[i].SetInitialPosition( new Vector3( _BaseSpawnPos.x + x, _BaseSpawnPos.y, _BaseSpawnPos.z + z ), Quaternion.Euler( _BaseSpawnRot ) );

			Enemy enemy = m_listEnemy[i] as Enemy;
			if ( enemy && !enemy.IsSummonEnemy ) {
				enemy.Activate();
				enemy.name += "_" + i.ToString();
				enemy.ExecuteBattleOption( BattleOption.eBOTimingType.GameStart, 0, null ); // 테스트씬에선 Appear액션 안해줘서 따로 호출

				mPlayer.SetEnemyNavigatorTarget( i, enemy );
			}
		}

		BattleAreaManager testBattleAreaManager = Object.FindObjectOfType<BattleAreaManager>();
		if ( testBattleAreaManager ) {
			testBattleAreaManager.Init();
		}
	}

#if UNITY_EDITOR
	private void Update() {
		if ( Input.GetKeyDown( KeyCode.R ) ) {
			for ( int i = 0; i < m_listEnemy.Count; i++ ) {
				if ( m_listEnemy[i].IsActivate() )
					continue;

				m_listEnemy[i].Activate();
				if ( m_listEnemy[i].grade == Unit.eGrade.Boss ) {
					EventMgr.Instance.SendEvent( eEventSubject.World, eEventType.EVENT_GAME_ENEMY_BOSS_APPEAR, m_listEnemy[i] );
					m_listEnemy[i].actionSystem.CancelCurrentAction();
				}
			}
		}
	}

	private void DrawEnemyInfo( Unit enemy ) {
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine( enemy.GetCenterPos(), enemy.GetCenterPos() + ( enemy.transform.forward * enemy.MainCollider.radius * 2.0f ) );
	}
#endif
}
