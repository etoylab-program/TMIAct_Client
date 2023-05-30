
public class ActionWeaponSkillBase : ActionBase {
	protected Player                mOwnerPlayer    = null;
	protected UnitBuffStats.sInfo   mBuffStatsInfo  = null;
	protected eAnimation            mCurAni         = eAnimation.None;


	public override void OnStart( IActionBaseParam param ) {
		base.OnStart( param );

		mOwnerPlayer = m_owner as Player;
		if( mOwnerPlayer ) {
			mOwnerPlayer.UseSp( mOwnerPlayer.boWeapon.GetUseWeaponSkillSp() );

			if( !SkipShowNames && !mOwnerPlayer.IsHelper ) {
				World.Instance.UIPlay.m_screenEffect.ShowWpnSkillName( FLocalizeString.Instance.GetText( mOwnerPlayer.boWeapon.data.TableData.Name ) );
			}

			if ( mOwnerPlayer.Guardian ) {
				mOwnerPlayer.Guardian.actionSystem.CancelCurrentAction();
				mOwnerPlayer.Guardian.CommandAction( actionCommand, param );
			}
		}
	}

	public override float GetAtkRange() {
		AniEvent.sEvent evt = m_owner.aniEvent.GetFirstAttackEvent( mCurAni );
		if( evt == null ) {
			Debug.LogError( mCurAni.ToString() + "공격 이벤트가 없네??" );
			return 0.0f;
		}
		else if( evt.visionRange <= 0.0f ) {
			Debug.LogError( mCurAni.ToString() + "Vistion Range가 0이네??" );
		}

		return evt.visionRange;
	}
}
