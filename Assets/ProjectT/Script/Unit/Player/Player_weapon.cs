
using System;
using System.Collections;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;


public partial class Player : Unit {
	public bool		IsWeaponChangeCoolingTime	{ get; private set; } = false;
	public float	WeaponChangeCoolingTime		{ get; private set; } = 0.0f;

	private Coroutine mCr = null;


	public void InitWeaponChange() {
		IsWeaponChangeCoolingTime = false;
		WeaponChangeCoolingTime = 0.0f;
	}

	private void StartWeaponChange( int beforeWeaponIndex ) {
		Utility.StopCoroutine( World.Instance, ref mCr );
		mCr = World.Instance.StartCoroutine( UpdateWeaponChange() );

		World.Instance.UIPlay.ChangeWeapon( this, mBOWeapons[beforeWeaponIndex].data );
	}

	private IEnumerator UpdateWeaponChange() {
		IsWeaponChangeCoolingTime = true;
		WeaponChangeCoolingTime = GameInfo.Instance.BattleConfig.WeaponChangeTime;

		while( WeaponChangeCoolingTime > 0.0f ) {
			WeaponChangeCoolingTime -= Time.deltaTime;
			yield return null;
		}

		InitWeaponChange();
	}
}
