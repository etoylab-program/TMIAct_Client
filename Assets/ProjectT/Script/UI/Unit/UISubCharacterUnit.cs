
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UISubCharacterUnit : FUnit
{
	[SerializeField] private UITexture			_CharTex;
	[SerializeField] private UISprite			_CoolTimeSpr;
	[SerializeField] private UILabel			_CoolTimeLabel;
	[SerializeField] private UISprite			_HpSpr;
	[SerializeField] private UIPlayerSpBubble[]	_SpBubbles;
	[SerializeField] private ParticleSystem     _MaxSpEff;

	public Player	CurPlayer	{ get; private set; } = null;
	public UIButton	Btn			{ get; private set; } = null;

	private int					mIndex				= -1;
	private WaitForFixedUpdate  mWaitForFixedUpdate = new WaitForFixedUpdate();
	private Coroutine			mCr					= null;
	private float               mSpPerBubble        = 0.0f;
	private bool                mCheckMaxSpEff		= false;


	public void GetButtonComponent() {
		if( Btn ) {
			return;
		}

		Btn = GetComponent<UIButton>();
	}

	public void Set( int index, Player player ) {
		mIndex = index;
		CurPlayer = player;

		string icon = player.charData.TableData.Icon;
		string costume = player.charData.TableData.InitCostume.ToString();

		_CharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Char/MainSlot/MainSlot_" + icon + "_" + costume + ".png" );
		_CoolTimeSpr.fillAmount = 0.0f;
		_CoolTimeLabel.textlocalize = "";

		mSpPerBubble = GameInfo.Instance.BattleConfig.USUseSP / _SpBubbles.Length;
		for( int i = 0; i < _SpBubbles.Length; i++ ) {
			_SpBubbles[i].Init();
		}
	}

	public void Set( Player player ) {
		CurPlayer = player;

		string icon = player.charData.TableData.Icon;
		string costume = player.charData.TableData.InitCostume.ToString();

		_CharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle( "icon", "Icon/Char/MainSlot/MainSlot_" + icon + "_" + costume + ".png" );
		_CoolTimeSpr.fillAmount = 0.0f;
		_CoolTimeLabel.textlocalize = "";

		mSpPerBubble = GameInfo.Instance.BattleConfig.USUseSP / _SpBubbles.Length;
		for( int i = 0; i < _SpBubbles.Length; i++ ) {
			_SpBubbles[i].Init();
		}
	}

	public void Disable() {
		Utility.StopCoroutine( World.Instance, ref mCr );

		_CoolTimeSpr.fillAmount = 1.0f;
		_CoolTimeLabel.textlocalize = "";
	}

	public bool IsCurPlayerDead() {
		return CurPlayer == null || CurPlayer.curHp <= 0.0f;
	}

	public void StartCoolTime() {
		if( IsCurPlayerDead() ) {
			return;
		}

		Utility.StopCoroutine( World.Instance, ref mCr );
		mCr = World.Instance.StartCoroutine( UpdateCoolTime() );
	}

	public void InitPlayerSp() {
		if( IsCurPlayerDead() ) {
			return;
		}

		for( int i = 0; i < _SpBubbles.Length; i++ ) {
			_SpBubbles[i].Init();
		}

		mSpPerBubble = GameInfo.Instance.BattleConfig.USUseSP / _SpBubbles.Length;
		AddPlayerSp();
	}

	public void AddPlayerSp() {
		if( IsCurPlayerDead() ) {
			return;
		}

		int onCount = (int)(CurPlayer.curSp / mSpPerBubble);
		if( onCount < _SpBubbles.Length ) {
			_SpBubbles[onCount].Fill( ( CurPlayer.curSp / mSpPerBubble ) - onCount );
		}

		if( onCount >= 1 ) {
			float useWeaponSkillSp = CurPlayer.boWeapon == null ? 0.0f : CurPlayer.boWeapon.GetUseWeaponSkillSp();

			for( int i = 0; i < onCount; i++ ) {
				bool possibleSkill = useWeaponSkillSp > 0.0f && CurPlayer.curSp >= useWeaponSkillSp;

				if( possibleSkill && !_SpBubbles[i].OnPossibleSkill ) {
					for( int j = 0; j < onCount - 1; j++ ) {
						_SpBubbles[j].psPossibleSkill.Play();
					}
				}

				_SpBubbles[i].On( possibleSkill, CurPlayer.curSp >= GameInfo.Instance.BattleConfig.USUseSP );
			}
		}

		_MaxSpEff.gameObject.SetActive( CurPlayer.curSp >= GameInfo.Instance.BattleConfig.USUseSP );
	}

	public void SubPlayerSp() {
		if( IsCurPlayerDead() ) {
			return;
		}

		for( int i = 0; i < _SpBubbles.Length; i++ ) {
			_SpBubbles[i].Off();
		}

		if( CurPlayer.curSp < GameInfo.Instance.BattleConfig.USUseSP ) {
			_MaxSpEff.gameObject.SetActive( false );

			float useWeaponSkillSp = CurPlayer.boWeapon == null ? 0.0f : CurPlayer.boWeapon.GetUseWeaponSkillSp();
			int onCount = (int)(CurPlayer.curSp / mSpPerBubble);

			for( int i = 0; i < onCount; i++ ) {
				bool possibleSkill = useWeaponSkillSp > 0.0f && CurPlayer.curSp >= useWeaponSkillSp;

				if( i > 0 && possibleSkill && !_SpBubbles[i].OnPossibleSkill ) {
					for( int j = 0; j < onCount - 1; j++ ) {
						_SpBubbles[j].psPossibleSkill.Play();
					}
				}

				_SpBubbles[i].On( possibleSkill, CurPlayer.curSp >= GameInfo.Instance.BattleConfig.USUseSP );
			}

			if( onCount < _SpBubbles.Length ) {
				_SpBubbles[onCount].Fill( ( CurPlayer.curSp / mSpPerBubble ) - onCount );
			}
		}
	}

	public void DecreasePlayerSp() {
		if( IsCurPlayerDead() ) {
			return;
		}

		if( CurPlayer.curSp < GameInfo.Instance.BattleConfig.USUseSP ) {
			_MaxSpEff.gameObject.SetActive( false );

			float useWeaponSkillSp = CurPlayer.boWeapon == null ? 0.0f : CurPlayer.boWeapon.GetUseWeaponSkillSp();
			bool possibleSkill = useWeaponSkillSp > 0.0f && CurPlayer.curSp >= useWeaponSkillSp;

			if( !possibleSkill ) {
				for( int i = 0; i < _SpBubbles.Length; i++ ) {
					_SpBubbles[i].psPossibleSkill.Stop();
				}
			}

			int onCount = (int)(CurPlayer.curSp / mSpPerBubble);
			for( int i = onCount + 1; i < _SpBubbles.Length; i++ ) {
				_SpBubbles[i].Off();
			}

			if( onCount < _SpBubbles.Length ) {
				_SpBubbles[onCount].Off();
				_SpBubbles[onCount].Fill( ( CurPlayer.curSp / mSpPerBubble ) - onCount, false );
			}
		}
	}

	public void OnBtnChangeChar() {
		if( World.Instance.IsEndGame || World.Instance.IsPause || Director.IsPlaying || IsCurPlayerDead() || _CoolTimeSpr.fillAmount > 0.0f ) {
			return;
		}

		if( World.Instance.Player.Input && World.Instance.Player.Input.isPause ) {
			return;
		}

		Player player = CurPlayer;
		Set( mIndex, World.Instance.Player );

		World.Instance.ChangePlayableCharacter( player );
		World.Instance.UIPlay.StartCoolTimeAllSubCharBtn();
	}

	private void Update() {
		if( CurPlayer == null ) {
			return;
		}

		_HpSpr.fillAmount = CurPlayer.curHp / CurPlayer.maxHp;

		if( World.Instance.UIPlay.widgets[1].alpha < 1.0f ) {
			if( _MaxSpEff.gameObject.activeSelf ) {
				mCheckMaxSpEff = true;
				_MaxSpEff.gameObject.SetActive( false );
			}
		}
		else {
			if( mCheckMaxSpEff ) {
				_MaxSpEff.gameObject.SetActive( true );
				mCheckMaxSpEff = false;
			}
		}
	}

	private IEnumerator UpdateCoolTime() {
		_CoolTimeSpr.fillAmount = 1.0f;

		float remainTime = GameInfo.Instance.GameConfig.TeamChangeCoolDown;
		float checkTime = 0.0f;
		int countdown = 0;
		int beforeCountdown = 0;

		while( checkTime < remainTime ) {
			checkTime += Time.fixedDeltaTime;
			
			countdown = (int)( remainTime - checkTime ) + 1;
			_CoolTimeSpr.fillAmount = ( 1.0f - ( checkTime / remainTime ) );

			if( countdown != beforeCountdown ) {
				_CoolTimeLabel.textlocalize = countdown.ToString();
				beforeCountdown = countdown;
			}

			yield return mWaitForFixedUpdate;
		}

		_CoolTimeSpr.fillAmount = 0.0f;
		_CoolTimeLabel.textlocalize = "";
	}
}
