
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class WorldPVP : World
{
    private void CreateTestPlayerTeamData()
    {
        mListTeamChar.Clear();
        GameClientTable.NPCCharRnd.Param npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.ID == PlayerNpcCharRndTableID);

        List<int> listCharId = new List<int>();
        if (RandomCharacters)
        {
            int randID = UnityEngine.Random.Range(1, GameInfo.Instance.GameClientTable.NPCCharRnds.Count + 1);
            npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.ID == randID);

            List<GameTable.Character.Param> listAllCharacter = new List<GameTable.Character.Param>();
            listAllCharacter.AddRange(GameInfo.Instance.GameTable.Characters);

            for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
                listCharId.Add(0);

            for (int i = 0; i < npcRnd.CharCnt; i++)
            {
                int randIndex = Random.Range(0, listAllCharacter.Count);
                listCharId[(int)eArenaTeamSlotPos.LAST_POS - i] = listAllCharacter[randIndex].ID;

                listAllCharacter.RemoveAt(randIndex);
            }
        }
        else
        {
            for(int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
            {
                listCharId.Add(PlayerTeam[i].TableId);
            }
        }

        for (int i = 0; i < listCharId.Count; i++)
        {
            if (listCharId[i] <= 0)
            {
                mListTeamChar.Add(null);
            }
            else
            {
                TeamCharData teamCharData = CreateTestTeamCharData(listCharId[i], PlayerTeam.Length <= 0 ? null : PlayerTeam[i], npcRnd);
                mListTeamChar.Add(teamCharData.CharData);
            }
        }
    }

    private void CreateTestOpponentTeamData()
    {
        GameInfo.Instance.MatchTeam.SetUserNickName( "김혁" );
        GameInfo.Instance.MatchTeam.UserLv = 99;
        GameInfo.Instance.MatchTeam.charlist.Clear();

        GameClientTable.NPCCharRnd.Param npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.ID == OpponentNpcCharRndTableID);

        List<int> listCharId = new List<int>();
        if (RandomCharacters)
        {
            int randID = UnityEngine.Random.Range(1, GameInfo.Instance.GameClientTable.NPCCharRnds.Count + 1);
            npcRnd = GameInfo.Instance.GameClientTable.FindNPCCharRnd(x => x.ID == randID);

            List<GameTable.Character.Param> listAllCharacter = new List<GameTable.Character.Param>();
            listAllCharacter.AddRange(GameInfo.Instance.GameTable.Characters);

            for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
                listCharId.Add(0);

            for (int i = 0; i < npcRnd.CharCnt; i++)
            {
                int randIndex = Random.Range(0, listAllCharacter.Count);
                listCharId[(int)eArenaTeamSlotPos.LAST_POS - i] = listAllCharacter[randIndex].ID;

                listAllCharacter.RemoveAt(randIndex);
            }
        }
        else
        {
            for (int i = 0; i < (int)eArenaTeamSlotPos._MAX_; i++)
            {
                listCharId.Add(OpponentTeam[i].TableId);
            }
        }

        for (int i = 0; i < listCharId.Count; i++)
        {
            if (listCharId[i] <= 0)
            {
                GameInfo.Instance.MatchTeam.charlist.Add(null);
            }
            else
            {
                TeamCharData teamCharData = CreateTestTeamCharData(listCharId[i], OpponentTeam.Length <= 0 ? null : OpponentTeam[i], npcRnd);
                GameInfo.Instance.MatchTeam.charlist.Add(teamCharData);
            }
        }
    }

	private TeamCharData CreateTestTeamCharData( int charTableId, sTestPVPCharData testPVPCharData, GameClientTable.NPCCharRnd.Param npcRnd ) {
		TeamCharData teamCharData = GameSupport.CreateArenaEnemyTeamCharData( charTableId, npcRnd );
		int charId = charTableId * 1000;

		// 무기
		int weaponId = teamCharData.MainWeaponData.TableID;

		if( !RandomCharacters ) {
			weaponId = charId + testPVPCharData.WeaponId;
			teamCharData.MainWeaponData = GameSupport.CreateArenaDummyWeaponData( charId, npcRnd, ref weaponId );
		}

		//무기 추가 PVP씬에서 실행했을때만..
		long itemUID = NetLocalSvr.Instance.AddWeapon( weaponId );
		if( itemUID != -1 ) {
			teamCharData.CharData.EquipWeaponUID = itemUID;
			WeaponData weapondata = GameInfo.Instance.GetWeaponData( itemUID );
			weapondata.Level = teamCharData.MainWeaponData.Level;
			weapondata.SkillLv = teamCharData.MainWeaponData.SkillLv;
			weapondata.Wake = teamCharData.MainWeaponData.Wake;

			teamCharData.MainWeaponData = weapondata;
		}

		// 장착 스킬
		//if (!RandomCharacters)
		{
			for( int i = 0; i < teamCharData.CharData.EquipSkill.Length; i++ ) {
				teamCharData.CharData.EquipSkill[i] = 0;
			}

			List<int> listSkillId = new List<int>();

			if( testPVPCharData.SkillIds.Length > 0 ) {
				listSkillId.AddRange( testPVPCharData.SkillIds );
			}

			teamCharData.CharData.PassvieList.Clear();
			teamCharData.CharData.PassvieList.Add( new PassiveData( charId + 301, 1 ) ); // 오의
			teamCharData.CharData.EquipSkill[0] = charId + 301;

			if( listSkillId.Count > 0 ) {
				for( int i = 0; i < listSkillId.Count; i++ ) {
					teamCharData.CharData.PassvieList.Add( new PassiveData( listSkillId[i], 1 ) );

					if( i < (int)eCOUNT.SKILLSLOT - 1 ) {
						teamCharData.CharData.EquipSkill[i + 1] = listSkillId[i];
					}
				}
			}
			else {
				Debug.LogError( charTableId + "번 캐릭터 스킬 장착을 안함." );
			}
		}

		// 장착 서포터
		//if (!RandomCharacters)
		{
			for( int i = 0; i < teamCharData.CharData.EquipCard.Length; i++ ) {
				teamCharData.CharData.EquipCard[i] = 0;
			}

			List<int> listSupporterId = new List<int>();

			if( testPVPCharData.SupporterIds.Length > 0 ) {
				listSupporterId.AddRange( testPVPCharData.SupporterIds );
			}

			teamCharData.CardList.Clear();
			for( int i = 0; i < listSupporterId.Count; i++ ) {
				if( i >= (int)eCOUNT.CARDSLOT ) {
					continue;
				}

				itemUID = NetLocalSvr.Instance.AddCard( listSupporterId[i] );
				if( itemUID <= 0 ) {
					continue;
				}

				teamCharData.CharData.EquipCard[i] = itemUID;

				CardData cardData = GameInfo.Instance.GetCardData(itemUID);
				cardData.Level = 1;
				cardData.SkillLv = 1;
				cardData.Wake = 1;

				teamCharData.CardList.Add( cardData );
			}
		}

		return teamCharData;
	}
}
