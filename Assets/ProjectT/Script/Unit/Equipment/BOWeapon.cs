
using System.Collections.Generic;


public class BOWeapon : BattleOption {
	public WeaponData data { get { return mData; } }

	private WeaponData	mData			= null;
	private bool		mbHasNoSkill	= false;


	public BOWeapon( long uId, Unit owner ) : base( owner ) {
		mToExecuteType = eToExecuteType.Weapon;
		mbHasNoSkill = true;

		mData = GameInfo.Instance.GetWeaponData( uId );
		if( mData == null ) {
			Debug.LogError( string.Format( "{0}번 무기가 없습니다.", uId ) );
			return;
		}

		//if( mData.TableData.WpnBOActivate > 0 && mData.TableData.WpnBOActivate != owner.tableId ) {
		//	return;
		//}

		if( mData.TableData.WpnBOActivate <= 0 || mData.TableData.WpnBOActivate == owner.tableId ) {
			mListBattleOptionSet.Clear();

			GameClientTable.BattleOptionSet.Param param = null;
			if( mData.TableData.WpnAddBOSetID1 > 0 ) {
				param = GameInfo.Instance.GameClientTable.FindBattleOptionSet( mData.TableData.WpnAddBOSetID1 );
				if( param == null ) {
					Debug.LogError( string.Format( "{0}번 배틀옵션셋 데이터가 없습니다.", mData.TableData.WpnAddBOSetID1 ) );
					return;
				}

				mListBattleOptionSet.Add( param );
			}

			if( mData.TableData.WpnAddBOSetID2 > 0 ) {
				param = GameInfo.Instance.GameClientTable.FindBattleOptionSet( mData.TableData.WpnAddBOSetID2 );
				if( param == null ) {
					Debug.LogError( string.Format( "{0}번 배틀옵션셋 데이터가 없습니다.", mData.TableData.WpnAddBOSetID2 ) );
					return;
				}

				mListBattleOptionSet.Add( param );
			}
		}

		Dictionary<int, int> dicGemSetType = new Dictionary<int, int>();
		foreach (long gemUid in mData.SlotGemUID)
		{
			GemData gemData = GameInfo.Instance.GetGemData(gemUid);
			if (gemData == null)
			{
				continue;
			}

			GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemData.SetOptID);
			if (gemSetTypeParam == null)
			{
				continue;
			}

			if (dicGemSetType.ContainsKey(gemData.SetOptID))
			{
				++dicGemSetType[gemData.SetOptID];
			}
			else
			{
				dicGemSetType.Add(gemData.SetOptID, 1);
			}
		}

		foreach (KeyValuePair<int, int> gemSetType in dicGemSetType)
		{
			List<GameClientTable.GemSetOpt.Param> gemSetOptParamList = GameInfo.Instance.GameClientTable.FindAllGemSetOpt(x => x.GroupID == gemSetType.Key);
			foreach (GameClientTable.GemSetOpt.Param gemSetOptParam in gemSetOptParamList)
			{
				if (gemSetType.Value < gemSetOptParam.SetCount)
				{
					continue;
				}

				GameClientTable.BattleOptionSet.Param battleOptionSetParam = GameInfo.Instance.GameClientTable.FindBattleOptionSet(gemSetOptParam.GemBOSetID1);
				if (battleOptionSetParam != null)
				{
					mListBattleOptionSet.Add(battleOptionSetParam);
				}

				battleOptionSetParam = GameInfo.Instance.GameClientTable.FindBattleOptionSet(gemSetOptParam.GemBOSetID2);
				if (battleOptionSetParam != null)
				{
					mListBattleOptionSet.Add(battleOptionSetParam);
				}
			}
		}

		Parse( mData.SkillLv );
		mbHasNoSkill = false;
	}

	public BOWeapon( WeaponData weaponData, Unit owner ) : base( owner ) {
		mToExecuteType = eToExecuteType.Weapon;
		mbHasNoSkill = true;

		mData = weaponData;
		if( mData == null ) {
			Debug.LogError( string.Format( "{0}번 무기가 없습니다.", weaponData.TableID ) );
			return;
		}

		if( mData.TableData.WpnBOActivate > 0 && mData.TableData.WpnBOActivate != owner.tableId ) {
			return;
		}

		mListBattleOptionSet.Clear();

		GameClientTable.BattleOptionSet.Param param = null;
		if( mData.TableData.WpnAddBOSetID1 > 0 ) {
			param = GameInfo.Instance.GameClientTable.FindBattleOptionSet( mData.TableData.WpnAddBOSetID1 );
			if( param == null ) {
				Debug.LogError( string.Format( "{0}번 배틀옵션셋 데이터가 없습니다.", mData.TableData.WpnAddBOSetID1 ) );
				return;
			}

			mListBattleOptionSet.Add( param );
		}

		if( mData.TableData.WpnAddBOSetID2 > 0 ) {
			param = GameInfo.Instance.GameClientTable.FindBattleOptionSet( mData.TableData.WpnAddBOSetID2 );
			if( param == null ) {
				Debug.LogError( string.Format( "{0}번 배틀옵션셋 데이터가 없습니다.", mData.TableData.WpnAddBOSetID2 ) );
				return;
			}

			mListBattleOptionSet.Add( param );
		}

		Dictionary<int, int> dicGemSetType = new Dictionary<int, int>();
		foreach(long gemUid in mData.SlotGemUID)
        {
			GemData gemData = GameInfo.Instance.GetGemData(gemUid);
			if (gemData == null)
            {
				continue;
            }

			GameTable.GemSetType.Param gemSetTypeParam = GameInfo.Instance.GameTable.FindGemSetType(gemData.SetOptID);
			if (gemSetTypeParam == null)
            {
				continue;
            }

			if (dicGemSetType.ContainsKey(gemData.SetOptID))
            {
				++dicGemSetType[gemData.SetOptID];
			}
			else
            {
				dicGemSetType.Add(gemData.SetOptID, 1);
			}
		}

		foreach (KeyValuePair<int, int> gemSetType in dicGemSetType)
        {
			List<GameClientTable.GemSetOpt.Param> gemSetOptParamList = GameInfo.Instance.GameClientTable.FindAllGemSetOpt(x => x.GroupID == gemSetType.Key);
			foreach (GameClientTable.GemSetOpt.Param gemSetOptParam in gemSetOptParamList)
            {
				if (gemSetType.Value < gemSetOptParam.SetCount)
                {
					continue;
                }

				GameClientTable.BattleOptionSet.Param battleOptionSetParam = GameInfo.Instance.GameClientTable.FindBattleOptionSet(gemSetOptParam.GemBOSetID1);
				if (battleOptionSetParam != null)
                {
					mListBattleOptionSet.Add(battleOptionSetParam);
				}

				battleOptionSetParam = GameInfo.Instance.GameClientTable.FindBattleOptionSet(gemSetOptParam.GemBOSetID2);
				if (battleOptionSetParam != null)
				{
					mListBattleOptionSet.Add(battleOptionSetParam);
				}
			}
        }

		Parse( mData.SkillLv );
		mbHasNoSkill = false;
	}

	public override bool HasActiveSkill() {
		if( mbHasNoSkill ) {
			return false;
		}

		if( mData.TableData.WpnBOWorkType == (int)eSkillType.Active ) {
			return true;
		}

		if( mData.TableData.WpnBOWorkType == (int)eSkillType.ConditionalActive_HasAction ) {
			eActionCommand actionCommand = Utility.GetActionCommandByString(mData.TableData.WpnBOWorkTypeValue);
			return mOwner.actionSystem.HasAction( actionCommand );
		}

		return false;
	}

	public float GetUseWeaponSkillSp() {
		if( mbHasNoSkill ) {
			return 0.0f;
		}

		if( mData.TableData.WpnBOActivate > 0 && mData.TableData.WpnBOActivate != mOwner.tableId ) {
			return 0.0f;
		}

		return mData.TableData.UseSP;
	}
}
