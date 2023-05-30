using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTable : ScriptableObject
{	

	// Excel Sheet : Achieve
	//
	public List<Achieve.Param> Achieves = new List<Achieve.Param> ();

	[System.SerializableAttribute]
	public class Achieve
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int GroupOrder;
			public int Name;
			public int Desc;
			public int AchieveKind;
			public string AchieveType;
			public int AchieveIndex;
			public int AchieveValue;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
			public int RewardAchievePoint;
		}
	}

	public Achieve.Param FindAchieve(int KeyValue)
	{
		return Achieves.Find(x => x.GroupID == KeyValue);
	}

	public Achieve.Param FindAchieve(System.Predicate<Achieve.Param> match)
	{
		return Achieves.Find(match);
	}

	public List<Achieve.Param> FindAllAchieve(System.Predicate<Achieve.Param> match)
	{
		return Achieves.FindAll(match);
	}

	// Excel Sheet : AchieveEvent
	//
	public List<AchieveEvent.Param> AchieveEvents = new List<AchieveEvent.Param> ();

	[System.SerializableAttribute]
	public class AchieveEvent
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string StartTime;
			public string EndTime;
			public int Name;
			public string Image;
			public string BGTaxture;
		}
	}

	public AchieveEvent.Param FindAchieveEvent(int KeyValue)
	{
		return AchieveEvents.Find(x => x.ID == KeyValue);
	}

	public AchieveEvent.Param FindAchieveEvent(System.Predicate<AchieveEvent.Param> match)
	{
		return AchieveEvents.Find(match);
	}

	public List<AchieveEvent.Param> FindAllAchieveEvent(System.Predicate<AchieveEvent.Param> match)
	{
		return AchieveEvents.FindAll(match);
	}

	// Excel Sheet : AchieveEventData
	//
	public List<AchieveEventData.Param> AchieveEventDatas = new List<AchieveEventData.Param> ();

	[System.SerializableAttribute]
	public class AchieveEventData
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int AchieveGroup;
			public int GroupOrder;
			public int Name;
			public int Desc;
			public string AchieveType;
			public int AchieveIndex;
			public int AchieveValue;
			public int RewardType;
			public int RewardIndex;
			public int RewardValue;
		}
	}

	public AchieveEventData.Param FindAchieveEventData(int KeyValue)
	{
		return AchieveEventDatas.Find(x => x.GroupID == KeyValue);
	}

	public AchieveEventData.Param FindAchieveEventData(System.Predicate<AchieveEventData.Param> match)
	{
		return AchieveEventDatas.Find(match);
	}

	public List<AchieveEventData.Param> FindAllAchieveEventData(System.Predicate<AchieveEventData.Param> match)
	{
		return AchieveEventDatas.FindAll(match);
	}

	// Excel Sheet : ArenaGrade
	//
	public List<ArenaGrade.Param> ArenaGrades = new List<ArenaGrade.Param> ();

	[System.SerializableAttribute]
	public class ArenaGrade
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GradeID;
			public int Name;
			public string Icon;
			public int Grade;
			public int Tier;
			public int MatchPrice;
			public int ReqScore;
		}
	}

	public ArenaGrade.Param FindArenaGrade(int KeyValue)
	{
		return ArenaGrades.Find(x => x.GradeID == KeyValue);
	}

	public ArenaGrade.Param FindArenaGrade(System.Predicate<ArenaGrade.Param> match)
	{
		return ArenaGrades.Find(match);
	}

	public List<ArenaGrade.Param> FindAllArenaGrade(System.Predicate<ArenaGrade.Param> match)
	{
		return ArenaGrades.FindAll(match);
	}

	// Excel Sheet : ArenaReward
	//
	public List<ArenaReward.Param> ArenaRewards = new List<ArenaReward.Param> ();

	[System.SerializableAttribute]
	public class ArenaReward
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int RewardOrder;
			public int RewardType;
			public int RewardValue;
			public int RewardGroupID;
		}
	}

	public ArenaReward.Param FindArenaReward(int KeyValue)
	{
		return ArenaRewards.Find(x => x.RewardOrder == KeyValue);
	}

	public ArenaReward.Param FindArenaReward(System.Predicate<ArenaReward.Param> match)
	{
		return ArenaRewards.Find(match);
	}

	public List<ArenaReward.Param> FindAllArenaReward(System.Predicate<ArenaReward.Param> match)
	{
		return ArenaRewards.FindAll(match);
	}

	// Excel Sheet : ArenaTower
	//
	public List<ArenaTower.Param> ArenaTowers = new List<ArenaTower.Param> ();

	[System.SerializableAttribute]
	public class ArenaTower
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string BGImg;
			public int Name;
			public int Desc;
			public int Type;
			public string Scene;
			public string LevelPB;
			public int ClearCondition;
			public int MissionDesc;
			public int ConditionValue;
			public string BGM;
			public int LoadingTip;
			public int RewardType;
			public int RewardIndex;
			public int RewardValue;
		}
	}

	public ArenaTower.Param FindArenaTower(int KeyValue)
	{
		return ArenaTowers.Find(x => x.ID == KeyValue);
	}

	public ArenaTower.Param FindArenaTower(System.Predicate<ArenaTower.Param> match)
	{
		return ArenaTowers.Find(match);
	}

	public List<ArenaTower.Param> FindAllArenaTower(System.Predicate<ArenaTower.Param> match)
	{
		return ArenaTowers.FindAll(match);
	}

	// Excel Sheet : AwakeSkill
	//
	public List<AwakeSkill.Param> AwakeSkills = new List<AwakeSkill.Param> ();

	[System.SerializableAttribute]
	public class AwakeSkill
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Etc;
			public int Name;
			public int Desc;
			public string Icon;
			public int ItemReqListID;
			public int MaxLevel;
			public int SptAddBOSetID1;
			public int SptAddBOSetID2;
			public string SptSvrOptType;
			public int SptSvrOptValue;
			public float SptSvrOptIncValue;
		}
	}

	public AwakeSkill.Param FindAwakeSkill(int KeyValue)
	{
		return AwakeSkills.Find(x => x.ID == KeyValue);
	}

	public AwakeSkill.Param FindAwakeSkill(System.Predicate<AwakeSkill.Param> match)
	{
		return AwakeSkills.Find(match);
	}

	public List<AwakeSkill.Param> FindAllAwakeSkill(System.Predicate<AwakeSkill.Param> match)
	{
		return AwakeSkills.FindAll(match);
	}

	// Excel Sheet : BadgeOpt
	//
	public List<BadgeOpt.Param> BadgeOpts = new List<BadgeOpt.Param> ();

	[System.SerializableAttribute]
	public class BadgeOpt
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int OptionID;
			public int Name;
			public int Desc;
			public string Icon;
			public string EffectType;
			public int IncEffectValue;
			public float BattlePowerRate;
		}
	}

	public BadgeOpt.Param FindBadgeOpt(int KeyValue)
	{
		return BadgeOpts.Find(x => x.OptionID == KeyValue);
	}

	public BadgeOpt.Param FindBadgeOpt(System.Predicate<BadgeOpt.Param> match)
	{
		return BadgeOpts.Find(match);
	}

	public List<BadgeOpt.Param> FindAllBadgeOpt(System.Predicate<BadgeOpt.Param> match)
	{
		return BadgeOpts.FindAll(match);
	}

	// Excel Sheet : Buff
	//
	public List<Buff.Param> Buffs = new List<Buff.Param> ();

	[System.SerializableAttribute]
	public class Buff
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int UseType;
			public string Type;
			public int Condition;
			public int Value;
		}
	}

	public Buff.Param FindBuff(int KeyValue)
	{
		return Buffs.Find(x => x.ID == KeyValue);
	}

	public Buff.Param FindBuff(System.Predicate<Buff.Param> match)
	{
		return Buffs.Find(match);
	}

	public List<Buff.Param> FindAllBuff(System.Predicate<Buff.Param> match)
	{
		return Buffs.FindAll(match);
	}

	// Excel Sheet : BingoEvent
	//
	public List<BingoEvent.Param> BingoEvents = new List<BingoEvent.Param> ();

	[System.SerializableAttribute]
	public class BingoEvent
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string StartTime;
			public string EndTime;
			public int Name;
			public string Image;
			public string BGTaxture;
		}
	}

	public BingoEvent.Param FindBingoEvent(int KeyValue)
	{
		return BingoEvents.Find(x => x.ID == KeyValue);
	}

	public BingoEvent.Param FindBingoEvent(System.Predicate<BingoEvent.Param> match)
	{
		return BingoEvents.Find(match);
	}

	public List<BingoEvent.Param> FindAllBingoEvent(System.Predicate<BingoEvent.Param> match)
	{
		return BingoEvents.FindAll(match);
	}

	// Excel Sheet : BingoEventData
	//
	public List<BingoEventData.Param> BingoEventDatas = new List<BingoEventData.Param> ();

	[System.SerializableAttribute]
	public class BingoEventData
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int No;
			public int ItemID1;
			public int ItemID2;
			public int ItemID3;
			public int ItemID4;
			public int ItemID5;
			public int ItemID6;
			public int ItemID7;
			public int ItemID8;
			public int ItemID9;
			public int ItemID10;
			public int ItemID11;
			public int ItemID12;
			public int ItemID13;
			public int ItemID14;
			public int ItemID15;
			public int ItemID16;
			public int ItemCount1;
			public int ItemCount2;
			public int ItemCount3;
			public int ItemCount4;
			public int ItemCount5;
			public int ItemCount6;
			public int ItemCount7;
			public int ItemCount8;
			public int ItemCount9;
			public int ItemCount10;
			public int ItemCount11;
			public int ItemCount12;
			public int ItemCount13;
			public int ItemCount14;
			public int ItemCount15;
			public int ItemCount16;
			public string Clear1;
			public string Clear2;
			public string Clear3;
			public string Clear4;
			public string Clear5;
			public string Clear6;
			public string Clear7;
			public string Clear8;
			public string Clear9;
			public string Clear10;
			public int RewardGroupID;
			public int OpenCost;
		}
	}

	public BingoEventData.Param FindBingoEventData(int KeyValue)
	{
		return BingoEventDatas.Find(x => x.GroupID == KeyValue);
	}

	public BingoEventData.Param FindBingoEventData(System.Predicate<BingoEventData.Param> match)
	{
		return BingoEventDatas.Find(match);
	}

	public List<BingoEventData.Param> FindAllBingoEventData(System.Predicate<BingoEventData.Param> match)
	{
		return BingoEventDatas.FindAll(match);
	}

	// Excel Sheet : Card
	//
	public List<Card.Param> Cards = new List<Card.Param> ();

	[System.SerializableAttribute]
	public class Card
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public int Greetings;
			public string Icon;
			public int Type;
			public int Grade;
			public int BaseStatSet;
			public int SellPrice;
			public int SellMPoint;
			public int HP;
			public int DEF;
			public float IncHP;
			public float IncDEF;
			public int LevelUpGroup;
			public int Exp;
			public int FavorGroup;
			public int SkillEffectName;
			public int SkillEffectDesc;
			public int SptBOWorkType;
			public int SptAddBOSetID1;
			public int SptAddBOSetID2;
			public int CoolTime;
			public int MainSkillEffectName;
			public int MainSkillEffectDesc;
			public int SptMainBOSetID1;
			public int SptMainBOSetID2;
			public string SptSvrOptType;
			public int SptSvrOptValue;
			public float SptSvrOptIncValue;
			public string SptSvrMainOptType;
			public int SptSvrMainOptValue;
			public float SptSvrMainOptIncValue;
			public int ScenarioGroupID;
			public int WakeReqGroup;
			public int EnchantGroup;
			public int Decomposition;
			public int AcquisitionID;
			public string SpriteIcon;
			public int Changeable;
			public int Tradeable;
			public int Selectable;
			public int Decomposable;
			public int PreVisible;
		}
	}

	public Card.Param FindCard(int KeyValue)
	{
		return Cards.Find(x => x.ID == KeyValue);
	}

	public Card.Param FindCard(System.Predicate<Card.Param> match)
	{
		return Cards.Find(match);
	}

	public List<Card.Param> FindAllCard(System.Predicate<Card.Param> match)
	{
		return Cards.FindAll(match);
	}

	// Excel Sheet : CardDispatchSlot
	//
	public List<CardDispatchSlot.Param> CardDispatchSlots = new List<CardDispatchSlot.Param> ();

	[System.SerializableAttribute]
	public class CardDispatchSlot
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Index;
			public int NeedRank;
			public int InitGrade;
			public int OpenGoods;
			public int OpenValue;
			public int ChangeGoods;
			public int ChangeValue;
			public int GradeRate1;
			public int GradeRate2;
			public int GradeRate3;
			public int GradeRate4;
			public int GradeRate5;
		}
	}

	public CardDispatchSlot.Param FindCardDispatchSlot(int KeyValue)
	{
		return CardDispatchSlots.Find(x => x.Index == KeyValue);
	}

	public CardDispatchSlot.Param FindCardDispatchSlot(System.Predicate<CardDispatchSlot.Param> match)
	{
		return CardDispatchSlots.Find(match);
	}

	public List<CardDispatchSlot.Param> FindAllCardDispatchSlot(System.Predicate<CardDispatchSlot.Param> match)
	{
		return CardDispatchSlots.FindAll(match);
	}

	// Excel Sheet : CardDispatchMission
	//
	public List<CardDispatchMission.Param> CardDispatchMissions = new List<CardDispatchMission.Param> ();

	[System.SerializableAttribute]
	public class CardDispatchMission
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Grade;
			public int Time;
			public int NeedURCnt;
			public int SocketType1;
			public int SocketType2;
			public int SocketType3;
			public int SocketType4;
			public int SocketType5;
			public int RewardType;
			public int RewardIndex;
			public int RewardValue;
		}
	}

	public CardDispatchMission.Param FindCardDispatchMission(int KeyValue)
	{
		return CardDispatchMissions.Find(x => x.ID == KeyValue);
	}

	public CardDispatchMission.Param FindCardDispatchMission(System.Predicate<CardDispatchMission.Param> match)
	{
		return CardDispatchMissions.Find(match);
	}

	public List<CardDispatchMission.Param> FindAllCardDispatchMission(System.Predicate<CardDispatchMission.Param> match)
	{
		return CardDispatchMissions.FindAll(match);
	}

	// Excel Sheet : CardFormation
	//
	public List<CardFormation.Param> CardFormations = new List<CardFormation.Param> ();

	[System.SerializableAttribute]
	public class CardFormation
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public int CardID1;
			public int CardID2;
			public int CardID3;
			public float GetHP;
			public float LevelHP;
			public float FavorHP;
			public int FormationBOSetID1;
			public int FormationBOSetID2;
			public string OptionKind;
		}
	}

	public CardFormation.Param FindCardFormation(int KeyValue)
	{
		return CardFormations.Find(x => x.ID == KeyValue);
	}

	public CardFormation.Param FindCardFormation(System.Predicate<CardFormation.Param> match)
	{
		return CardFormations.Find(match);
	}

	public List<CardFormation.Param> FindAllCardFormation(System.Predicate<CardFormation.Param> match)
	{
		return CardFormations.FindAll(match);
	}

	// Excel Sheet : Character
	//
	public List<Character.Param> Characters = new List<Character.Param> ();

	[System.SerializableAttribute]
	public class Character
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Icon;
			public string Model;
			public int WakeReqGroup;
			public int Type;
			public int MonType;
			public string CharBOSetList;
			public float MoveSpeed;
			public int HP;
			public int ATK;
			public int DEF;
			public int CRI;
			public float IncHP;
			public float IncATK;
			public float IncDEF;
			public float IncCRI;
			public int InitSelect;
			public int InitWeapon;
			public int InitCostume;
			public int InitAddStageID;
			public int InitSkillSlot1;
			public int InitSkillSlot2;
			public int InitSkillSlot3;
			public int InitSkillSlot4;
			public string StartDrt;
			public string WinDrt_01;
			public string WinDrt_02;
			public string GroggyDrt_02;
			public string ResurrectionDrt_02;
			public string DieDrt_02;
			public string USkillDrt_01;
			public string FxSndArmorHit;
			public string FxSndArmorBreak;
			public string EffTarget;
			public string EffShield;
			public string EffShieldBreak;
			public string EffShieldAttack;
			public string Hit_01;
			public string Hit_02;
			public string Hit_03;
			public string Hit_04;
			public int ScenarioGroupID;
			public string BuyDrt;
			public string GradeUpDrt;
			public string Face;
			public string SpriteIcon;
			public string AI;
			public string PreferenceStep1;
			public string PreferenceStep2;
			public int PreferenceLevelGroup;
			public int PreferenceBuff;
			public int TrainingRoom;
		}
	}

	public Character.Param FindCharacter(int KeyValue)
	{
		return Characters.Find(x => x.ID == KeyValue);
	}

	public Character.Param FindCharacter(System.Predicate<Character.Param> match)
	{
		return Characters.Find(match);
	}

	public List<Character.Param> FindAllCharacter(System.Predicate<Character.Param> match)
	{
		return Characters.FindAll(match);
	}

	// Excel Sheet : CharacterSkillPassive
	//
	public List<CharacterSkillPassive.Param> CharacterSkillPassives = new List<CharacterSkillPassive.Param> ();

	[System.SerializableAttribute]
	public class CharacterSkillPassive
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int CharacterID;
			public int Name;
			public int Desc;
			public int Slot;
			public string Atlus;
			public string Icon;
			public string UpIcon;
			public string SkillMovie;
			public int CommandIndex;
			public int Type;
			public int MaxLevel;
			public int NextID;
			public int ParentsID;
			public int CondType;
			public int CondValue;
			public int GroupID;
			public int ItemReqListID;
			public string Effect;
			public string SkillAction;
			public string ReplacedAction;
			public float Value1;
			public float Value2;
			public float Value3;
			public float IncValue1;
			public float IncValue2;
			public int CharAddBOSetID1;
			public int CharAddBOSetID2;
			public int AddBOSetActionIndex;
			public int VoiceID;
			public float CoolTime;
			public int LastAtkSuperArmor;
		}
	}

	public CharacterSkillPassive.Param FindCharacterSkillPassive(int KeyValue)
	{
		return CharacterSkillPassives.Find(x => x.ID == KeyValue);
	}

	public CharacterSkillPassive.Param FindCharacterSkillPassive(System.Predicate<CharacterSkillPassive.Param> match)
	{
		return CharacterSkillPassives.Find(match);
	}

	public List<CharacterSkillPassive.Param> FindAllCharacterSkillPassive(System.Predicate<CharacterSkillPassive.Param> match)
	{
		return CharacterSkillPassives.FindAll(match);
	}

	// Excel Sheet : ChatStamp
	//
	public List<ChatStamp.Param> ChatStamps = new List<ChatStamp.Param> ();

	[System.SerializableAttribute]
	public class ChatStamp
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Icon;
			public int StoreID;
		}
	}

	public ChatStamp.Param FindChatStamp(int KeyValue)
	{
		return ChatStamps.Find(x => x.ID == KeyValue);
	}

	public ChatStamp.Param FindChatStamp(System.Predicate<ChatStamp.Param> match)
	{
		return ChatStamps.Find(match);
	}

	public List<ChatStamp.Param> FindAllChatStamp(System.Predicate<ChatStamp.Param> match)
	{
		return ChatStamps.FindAll(match);
	}

	// Excel Sheet : CircleCheck
	//
	public List<CircleCheck.Param> CircleChecks = new List<CircleCheck.Param> ();

	[System.SerializableAttribute]
	public class CircleCheck
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int CircleCheckCnt;
			public int RewardGroupID;
			public int SendMailTypeID;
			public int NextCircleCheckGroupID;
		}
	}

	public CircleCheck.Param FindCircleCheck(int KeyValue)
	{
		return CircleChecks.Find(x => x.GroupID == KeyValue);
	}

	public CircleCheck.Param FindCircleCheck(System.Predicate<CircleCheck.Param> match)
	{
		return CircleChecks.Find(match);
	}

	public List<CircleCheck.Param> FindAllCircleCheck(System.Predicate<CircleCheck.Param> match)
	{
		return CircleChecks.FindAll(match);
	}

	// Excel Sheet : CircleGold
	//
	public List<CircleGold.Param> CircleGolds = new List<CircleGold.Param> ();

	[System.SerializableAttribute]
	public class CircleGold
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int CircleWorkValue;
			public int RewardValue;
		}
	}

	public CircleGold.Param FindCircleGold(int KeyValue)
	{
		return CircleGolds.Find(x => x.ID == KeyValue);
	}

	public CircleGold.Param FindCircleGold(System.Predicate<CircleGold.Param> match)
	{
		return CircleGolds.Find(match);
	}

	public List<CircleGold.Param> FindAllCircleGold(System.Predicate<CircleGold.Param> match)
	{
		return CircleGolds.FindAll(match);
	}

	// Excel Sheet : CircleMark
	//
	public List<CircleMark.Param> CircleMarks = new List<CircleMark.Param> ();

	[System.SerializableAttribute]
	public class CircleMark
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Marktype;
			public string Icon;
			public string SubIcon;
			public int StoreID;
		}
	}

	public CircleMark.Param FindCircleMark(int KeyValue)
	{
		return CircleMarks.Find(x => x.ID == KeyValue);
	}

	public CircleMark.Param FindCircleMark(System.Predicate<CircleMark.Param> match)
	{
		return CircleMarks.Find(match);
	}

	public List<CircleMark.Param> FindAllCircleMark(System.Predicate<CircleMark.Param> match)
	{
		return CircleMarks.FindAll(match);
	}

	// Excel Sheet : Costume
	//
	public List<Costume.Param> Costumes = new List<Costume.Param> ();

	[System.SerializableAttribute]
	public class Costume
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int CharacterID;
			public int Name;
			public string Icon;
			public string Model;
			public string HairModel;
			public int SubHairChange;
			public int ColorCnt;
			public int Grade;
			public int HP;
			public int ATK;
			public int DEF;
			public int CRI;
			public int StoreID;
			public int OrderNum;
			public int PreVisible;
			public string LobbyOnly;
			public string InGameOnly;
			public int UseDyeing;
			public string BaseColor1;
			public string BaseColor2;
			public string BaseColor3;
		}
	}

	public Costume.Param FindCostume(int KeyValue)
	{
		return Costumes.Find(x => x.ID == KeyValue);
	}

	public Costume.Param FindCostume(System.Predicate<Costume.Param> match)
	{
		return Costumes.Find(match);
	}

	public List<Costume.Param> FindAllCostume(System.Predicate<Costume.Param> match)
	{
		return Costumes.FindAll(match);
	}

	// Excel Sheet : DailyMission
	//
	public List<DailyMission.Param> DailyMissions = new List<DailyMission.Param> ();

	[System.SerializableAttribute]
	public class DailyMission
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int Day;
			public int No;
			public int Desc;
			public string MissionType;
			public int MissionValue;
			public int RewardGroupID;
		}
	}

	public DailyMission.Param FindDailyMission(int KeyValue)
	{
		return DailyMissions.Find(x => x.GroupID == KeyValue);
	}

	public DailyMission.Param FindDailyMission(System.Predicate<DailyMission.Param> match)
	{
		return DailyMissions.Find(match);
	}

	public List<DailyMission.Param> FindAllDailyMission(System.Predicate<DailyMission.Param> match)
	{
		return DailyMissions.FindAll(match);
	}

	// Excel Sheet : DailyMissionSet
	//
	public List<DailyMissionSet.Param> DailyMissionSets = new List<DailyMissionSet.Param> ();

	[System.SerializableAttribute]
	public class DailyMissionSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string StartTime;
			public string EndTime;
			public int EventTarget;
			public int RewardGroupID;
		}
	}

	public DailyMissionSet.Param FindDailyMissionSet(int KeyValue)
	{
		return DailyMissionSets.Find(x => x.ID == KeyValue);
	}

	public DailyMissionSet.Param FindDailyMissionSet(System.Predicate<DailyMissionSet.Param> match)
	{
		return DailyMissionSets.Find(match);
	}

	public List<DailyMissionSet.Param> FindAllDailyMissionSet(System.Predicate<DailyMissionSet.Param> match)
	{
		return DailyMissionSets.FindAll(match);
	}

	// Excel Sheet : Enchant
	//
	public List<Enchant.Param> Enchants = new List<Enchant.Param> ();

	[System.SerializableAttribute]
	public class Enchant
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int Level;
			public int IncreaseValue;
			public int Prob;
			public int ItemReqListID;
		}
	}

	public Enchant.Param FindEnchant(int KeyValue)
	{
		return Enchants.Find(x => x.GroupID == KeyValue);
	}

	public Enchant.Param FindEnchant(System.Predicate<Enchant.Param> match)
	{
		return Enchants.Find(match);
	}

	public List<Enchant.Param> FindAllEnchant(System.Predicate<Enchant.Param> match)
	{
		return Enchants.FindAll(match);
	}

	// Excel Sheet : EventExchangeReward
	//
	public List<EventExchangeReward.Param> EventExchangeRewards = new List<EventExchangeReward.Param> ();

	[System.SerializableAttribute]
	public class EventExchangeReward
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int EventID;
			public int RewardStep;
			public int ReqItemID1;
			public int ReqItemCnt1;
			public int ReqItemID2;
			public int ReqItemCnt2;
			public int ReqItemID3;
			public int ReqItemCnt3;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
			public int ExchangeCnt;
			public int IndexID;
		}
	}

	public EventExchangeReward.Param FindEventExchangeReward(int KeyValue)
	{
		return EventExchangeRewards.Find(x => x.EventID == KeyValue);
	}

	public EventExchangeReward.Param FindEventExchangeReward(System.Predicate<EventExchangeReward.Param> match)
	{
		return EventExchangeRewards.Find(match);
	}

	public List<EventExchangeReward.Param> FindAllEventExchangeReward(System.Predicate<EventExchangeReward.Param> match)
	{
		return EventExchangeRewards.FindAll(match);
	}

	// Excel Sheet : EventResetReward
	//
	public List<EventResetReward.Param> EventResetRewards = new List<EventResetReward.Param> ();

	[System.SerializableAttribute]
	public class EventResetReward
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int EventID;
			public int RewardStep;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
			public int RewardRate;
			public int RewardCnt;
			public int ResetFlag;
		}
	}

	public EventResetReward.Param FindEventResetReward(int KeyValue)
	{
		return EventResetRewards.Find(x => x.EventID == KeyValue);
	}

	public EventResetReward.Param FindEventResetReward(System.Predicate<EventResetReward.Param> match)
	{
		return EventResetRewards.Find(match);
	}

	public List<EventResetReward.Param> FindAllEventResetReward(System.Predicate<EventResetReward.Param> match)
	{
		return EventResetRewards.FindAll(match);
	}

	// Excel Sheet : EventSet
	//
	public List<EventSet.Param> EventSets = new List<EventSet.Param> ();

	[System.SerializableAttribute]
	public class EventSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int EventID;
			public int EventType;
			public string StartTime;
			public string EndTime;
			public string PlayOpenTime;
			public string PlayCloseTime;
			public int EventItemID1;
			public int EventItemID2;
			public int EventItemID3;
			public int EventItemID4;
			public int EventItemID5;
			public int EventItemID6;
			public int Name;
			public int Desc;
			public string MainBGSpr;
			public string BannerBGSprNOW;
			public string BannerBGSprPAST;
			public string StageBG;
			public string EventRuleBG;
		}
	}

	public EventSet.Param FindEventSet(int KeyValue)
	{
		return EventSets.Find(x => x.EventID == KeyValue);
	}

	public EventSet.Param FindEventSet(System.Predicate<EventSet.Param> match)
	{
		return EventSets.Find(match);
	}

	public List<EventSet.Param> FindAllEventSet(System.Predicate<EventSet.Param> match)
	{
		return EventSets.FindAll(match);
	}

	// Excel Sheet : Facility
	//
	public List<Facility.Param> Facilitys = new List<Facility.Param> ();

	[System.SerializableAttribute]
	public class Facility
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public int EffectDesc;
			public string Icon;
			public string Model;
			public string EffectType;
			public int EffectValue;
			public float IncEffectValue;
			public int Time;
			public int RewardCardFavor;
			public int ParentsID;
			public int LevelUpItemReq;
			public int MaxLevel;
			public int FacilityOpenUserRank;
			public int SlotOpenFacilityLv;
			public int Type;
			public int Accelerate;
		}
	}

	public Facility.Param FindFacility(int KeyValue)
	{
		return Facilitys.Find(x => x.ID == KeyValue);
	}

	public Facility.Param FindFacility(System.Predicate<Facility.Param> match)
	{
		return Facilitys.Find(match);
	}

	public List<Facility.Param> FindAllFacility(System.Predicate<Facility.Param> match)
	{
		return Facilitys.FindAll(match);
	}

	// Excel Sheet : FacilityItemCombine
	//
	public List<FacilityItemCombine.Param> FacilityItemCombines = new List<FacilityItemCombine.Param> ();

	[System.SerializableAttribute]
	public class FacilityItemCombine
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public int ReqFacilityLv;
			public int Time;
			public int RewardCardFavor;
			public int ItemID;
			public int ItemCnt;
			public int ItemReqGroup;
		}
	}

	public FacilityItemCombine.Param FindFacilityItemCombine(int KeyValue)
	{
		return FacilityItemCombines.Find(x => x.ID == KeyValue);
	}

	public FacilityItemCombine.Param FindFacilityItemCombine(System.Predicate<FacilityItemCombine.Param> match)
	{
		return FacilityItemCombines.Find(match);
	}

	public List<FacilityItemCombine.Param> FindAllFacilityItemCombine(System.Predicate<FacilityItemCombine.Param> match)
	{
		return FacilityItemCombines.FindAll(match);
	}

	// Excel Sheet : FacilityOperationRoom
	//
	public List<FacilityOperationRoom.Param> FacilityOperationRooms = new List<FacilityOperationRoom.Param> ();

	[System.SerializableAttribute]
	public class FacilityOperationRoom
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int Name;
			public int Desc;
			public int ParticipantCount;
			public int ProductType;
			public int ProductIndex;
			public int ProductValueMin;
			public int ProductValueMax;
		}
	}

	public FacilityOperationRoom.Param FindFacilityOperationRoom(int KeyValue)
	{
		return FacilityOperationRooms.Find(x => x.GroupID == KeyValue);
	}

	public FacilityOperationRoom.Param FindFacilityOperationRoom(System.Predicate<FacilityOperationRoom.Param> match)
	{
		return FacilityOperationRooms.Find(match);
	}

	public List<FacilityOperationRoom.Param> FindAllFacilityOperationRoom(System.Predicate<FacilityOperationRoom.Param> match)
	{
		return FacilityOperationRooms.FindAll(match);
	}

	// Excel Sheet : FacilityTrade
	//
	public List<FacilityTrade.Param> FacilityTrades = new List<FacilityTrade.Param> ();

	[System.SerializableAttribute]
	public class FacilityTrade
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int MaterialGrade;
			public int MaterialCount;
			public int SuccessProb;
			public int TradeGroup;
		}
	}

	public FacilityTrade.Param FindFacilityTrade(int KeyValue)
	{
		return FacilityTrades.Find(x => x.ID == KeyValue);
	}

	public FacilityTrade.Param FindFacilityTrade(System.Predicate<FacilityTrade.Param> match)
	{
		return FacilityTrades.Find(match);
	}

	public List<FacilityTrade.Param> FindAllFacilityTrade(System.Predicate<FacilityTrade.Param> match)
	{
		return FacilityTrades.FindAll(match);
	}

	// Excel Sheet : FacilityTradeAddon
	//
	public List<FacilityTradeAddon.Param> FacilityTradeAddons = new List<FacilityTradeAddon.Param> ();

	[System.SerializableAttribute]
	public class FacilityTradeAddon
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int AddType;
			public int AddFuncType;
			public int AddFuncValue;
			public int Name;
			public int Desc;
			public int AddItemID;
			public int AddItemCount;
			public int UseGrade;
		}
	}

	public FacilityTradeAddon.Param FindFacilityTradeAddon(int KeyValue)
	{
		return FacilityTradeAddons.Find(x => x.ID == KeyValue);
	}

	public FacilityTradeAddon.Param FindFacilityTradeAddon(System.Predicate<FacilityTradeAddon.Param> match)
	{
		return FacilityTradeAddons.Find(match);
	}

	public List<FacilityTradeAddon.Param> FindAllFacilityTradeAddon(System.Predicate<FacilityTradeAddon.Param> match)
	{
		return FacilityTradeAddons.FindAll(match);
	}

	// Excel Sheet : Gem
	//
	public List<Gem.Param> Gems = new List<Gem.Param> ();

	[System.SerializableAttribute]
	public class Gem
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Icon;
			public int MainType;
			public int SubType;
			public int Grade;
			public int SellPrice;
			public int HP;
			public int ATK;
			public int DEF;
			public int CRI;
			public float IncHP;
			public float IncATK;
			public float IncDEF;
			public float IncCRI;
			public int LevelUpGroup;
			public int Exp;
			public int WakeReqGroup;
			public int RandOptGroup;
			public int OptResetReqGroup;
			public int EvReqGroup;
			public int EvolutionResult;
			public int AcquisitionID;
		}
	}

	public Gem.Param FindGem(int KeyValue)
	{
		return Gems.Find(x => x.ID == KeyValue);
	}

	public Gem.Param FindGem(System.Predicate<Gem.Param> match)
	{
		return Gems.Find(match);
	}

	public List<Gem.Param> FindAllGem(System.Predicate<Gem.Param> match)
	{
		return Gems.FindAll(match);
	}

	// Excel Sheet : GemSetType
	//
	public List<GemSetType.Param> GemSetTypes = new List<GemSetType.Param> ();

	[System.SerializableAttribute]
	public class GemSetType
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public string Icon;
		}
	}

	public GemSetType.Param FindGemSetType(int KeyValue)
	{
		return GemSetTypes.Find(x => x.ID == KeyValue);
	}

	public GemSetType.Param FindGemSetType(System.Predicate<GemSetType.Param> match)
	{
		return GemSetTypes.Find(match);
	}

	public List<GemSetType.Param> FindAllGemSetType(System.Predicate<GemSetType.Param> match)
	{
		return GemSetTypes.FindAll(match);
	}

	// Excel Sheet : GemRandOpt
	//
	public List<GemRandOpt.Param> GemRandOpts = new List<GemRandOpt.Param> ();

	[System.SerializableAttribute]
	public class GemRandOpt
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int ID;
			public int Name;
			public int Desc;
			public string EffectType;
			public int Min;
			public int Max;
			public float Value;
			public int RndStep;
		}
	}

	public GemRandOpt.Param FindGemRandOpt(int KeyValue)
	{
		return GemRandOpts.Find(x => x.GroupID == KeyValue);
	}

	public GemRandOpt.Param FindGemRandOpt(System.Predicate<GemRandOpt.Param> match)
	{
		return GemRandOpts.Find(match);
	}

	public List<GemRandOpt.Param> FindAllGemRandOpt(System.Predicate<GemRandOpt.Param> match)
	{
		return GemRandOpts.FindAll(match);
	}

	// Excel Sheet : InfluenceInfo
	//
	public List<InfluenceInfo.Param> InfluenceInfos = new List<InfluenceInfo.Param> ();

	[System.SerializableAttribute]
	public class InfluenceInfo
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int No;
			public int Name;
			public int Desc;
			public string Icon;
			public int RewardGroupID;
		}
	}

	public InfluenceInfo.Param FindInfluenceInfo(int KeyValue)
	{
		return InfluenceInfos.Find(x => x.GroupID == KeyValue);
	}

	public InfluenceInfo.Param FindInfluenceInfo(System.Predicate<InfluenceInfo.Param> match)
	{
		return InfluenceInfos.Find(match);
	}

	public List<InfluenceInfo.Param> FindAllInfluenceInfo(System.Predicate<InfluenceInfo.Param> match)
	{
		return InfluenceInfos.FindAll(match);
	}

	// Excel Sheet : InfluenceMission
	//
	public List<InfluenceMission.Param> InfluenceMissions = new List<InfluenceMission.Param> ();

	[System.SerializableAttribute]
	public class InfluenceMission
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int No;
			public int Desc;
			public string MissionType;
			public int MissionValue;
			public int RewardGroupID;
		}
	}

	public InfluenceMission.Param FindInfluenceMission(int KeyValue)
	{
		return InfluenceMissions.Find(x => x.GroupID == KeyValue);
	}

	public InfluenceMission.Param FindInfluenceMission(System.Predicate<InfluenceMission.Param> match)
	{
		return InfluenceMissions.Find(match);
	}

	public List<InfluenceMission.Param> FindAllInfluenceMission(System.Predicate<InfluenceMission.Param> match)
	{
		return InfluenceMissions.FindAll(match);
	}

	// Excel Sheet : InfluenceMissionSet
	//
	public List<InfluenceMissionSet.Param> InfluenceMissionSets = new List<InfluenceMissionSet.Param> ();

	[System.SerializableAttribute]
	public class InfluenceMissionSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string StartTime;
			public string EndTime;
			public string RewardTime;
			public int RewardGroupID;
			public int EventItemID;
		}
	}

	public InfluenceMissionSet.Param FindInfluenceMissionSet(int KeyValue)
	{
		return InfluenceMissionSets.Find(x => x.ID == KeyValue);
	}

	public InfluenceMissionSet.Param FindInfluenceMissionSet(System.Predicate<InfluenceMissionSet.Param> match)
	{
		return InfluenceMissionSets.Find(match);
	}

	public List<InfluenceMissionSet.Param> FindAllInfluenceMissionSet(System.Predicate<InfluenceMissionSet.Param> match)
	{
		return InfluenceMissionSets.FindAll(match);
	}

	// Excel Sheet : InfluenceRank
	//
	public List<InfluenceRank.Param> InfluenceRanks = new List<InfluenceRank.Param> ();

	[System.SerializableAttribute]
	public class InfluenceRank
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int RewardOrder;
			public int RewardType;
			public int RewardValue;
			public int RewardGroupID;
		}
	}

	public InfluenceRank.Param FindInfluenceRank(int KeyValue)
	{
		return InfluenceRanks.Find(x => x.RewardOrder == KeyValue);
	}

	public InfluenceRank.Param FindInfluenceRank(System.Predicate<InfluenceRank.Param> match)
	{
		return InfluenceRanks.Find(match);
	}

	public List<InfluenceRank.Param> FindAllInfluenceRank(System.Predicate<InfluenceRank.Param> match)
	{
		return InfluenceRanks.FindAll(match);
	}

	// Excel Sheet : Item
	//
	public List<Item.Param> Items = new List<Item.Param> ();

	[System.SerializableAttribute]
	public class Item
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Icon;
			public int Type;
			public int SubType;
			public int Grade;
			public int SellPrice;
			public int Value;
			public int AcquisitionID;
			public float CashExchange;
		}
	}

	public Item.Param FindItem(int KeyValue)
	{
		return Items.Find(x => x.ID == KeyValue);
	}

	public Item.Param FindItem(System.Predicate<Item.Param> match)
	{
		return Items.Find(match);
	}

	public List<Item.Param> FindAllItem(System.Predicate<Item.Param> match)
	{
		return Items.FindAll(match);
	}

	// Excel Sheet : ItemReqList
	//
	public List<ItemReqList.Param> ItemReqLists = new List<ItemReqList.Param> ();

	[System.SerializableAttribute]
	public class ItemReqList
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public int Level;
			public int ItemID1;
			public int Count1;
			public int ItemID2;
			public int Count2;
			public int ItemID3;
			public int Count3;
			public int ItemID4;
			public int Count4;
			public int Gold;
			public int GoodsValue;
			public int LimitLevel;
		}
	}

	public ItemReqList.Param FindItemReqList(int KeyValue)
	{
		return ItemReqLists.Find(x => x.Group == KeyValue);
	}

	public ItemReqList.Param FindItemReqList(System.Predicate<ItemReqList.Param> match)
	{
		return ItemReqLists.Find(match);
	}

	public List<ItemReqList.Param> FindAllItemReqList(System.Predicate<ItemReqList.Param> match)
	{
		return ItemReqLists.FindAll(match);
	}

	// Excel Sheet : LevelUp
	//
	public List<LevelUp.Param> LevelUps = new List<LevelUp.Param> ();

	[System.SerializableAttribute]
	public class LevelUp
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public int Level;
			public int Exp;
			public int Value1;
		}
	}

	public LevelUp.Param FindLevelUp(int KeyValue)
	{
		return LevelUps.Find(x => x.Group == KeyValue);
	}

	public LevelUp.Param FindLevelUp(System.Predicate<LevelUp.Param> match)
	{
		return LevelUps.Find(match);
	}

	public List<LevelUp.Param> FindAllLevelUp(System.Predicate<LevelUp.Param> match)
	{
		return LevelUps.FindAll(match);
	}

	// Excel Sheet : LobbyAnimation
	//
	public List<LobbyAnimation.Param> LobbyAnimations = new List<LobbyAnimation.Param> ();

	[System.SerializableAttribute]
	public class LobbyAnimation
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Character;
			public int No;
			public int Name;
			public int Desc;
			public string Icon;
			public string Animation;
			public string Face;
			public int LockType;
		}
	}

	public LobbyAnimation.Param FindLobbyAnimation(int KeyValue)
	{
		return LobbyAnimations.Find(x => x.ID == KeyValue);
	}

	public LobbyAnimation.Param FindLobbyAnimation(System.Predicate<LobbyAnimation.Param> match)
	{
		return LobbyAnimations.Find(match);
	}

	public List<LobbyAnimation.Param> FindAllLobbyAnimation(System.Predicate<LobbyAnimation.Param> match)
	{
		return LobbyAnimations.FindAll(match);
	}

	// Excel Sheet : LobbyTheme
	//
	public List<LobbyTheme.Param> LobbyThemes = new List<LobbyTheme.Param> ();

	[System.SerializableAttribute]
	public class LobbyTheme
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public string Icon;
			public string Prefab;
			public int Bgm;
			public int FreeSelect;
		}
	}

	public LobbyTheme.Param FindLobbyTheme(int KeyValue)
	{
		return LobbyThemes.Find(x => x.ID == KeyValue);
	}

	public LobbyTheme.Param FindLobbyTheme(System.Predicate<LobbyTheme.Param> match)
	{
		return LobbyThemes.Find(match);
	}

	public List<LobbyTheme.Param> FindAllLobbyTheme(System.Predicate<LobbyTheme.Param> match)
	{
		return LobbyThemes.FindAll(match);
	}

	// Excel Sheet : LoginBonus
	//
	public List<LoginBonus.Param> LoginBonuss = new List<LoginBonus.Param> ();

	[System.SerializableAttribute]
	public class LoginBonus
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int LoginGroupID;
			public int LoginCnt;
			public int RewardGroupID;
			public int SendMailTypeID;
			public string BGimg;
			public int NextGroupID;
		}
	}

	public LoginBonus.Param FindLoginBonus(int KeyValue)
	{
		return LoginBonuss.Find(x => x.LoginGroupID == KeyValue);
	}

	public LoginBonus.Param FindLoginBonus(System.Predicate<LoginBonus.Param> match)
	{
		return LoginBonuss.Find(match);
	}

	public List<LoginBonus.Param> FindAllLoginBonus(System.Predicate<LoginBonus.Param> match)
	{
		return LoginBonuss.FindAll(match);
	}

	// Excel Sheet : LoginBonusMonthly
	//
	public List<LoginBonusMonthly.Param> LoginBonusMonthlys = new List<LoginBonusMonthly.Param> ();

	[System.SerializableAttribute]
	public class LoginBonusMonthly
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int DayCnt;
			public int Grade;
			public int SendMailTypeID;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
		}
	}

	public LoginBonusMonthly.Param FindLoginBonusMonthly(int KeyValue)
	{
		return LoginBonusMonthlys.Find(x => x.GroupID == KeyValue);
	}

	public LoginBonusMonthly.Param FindLoginBonusMonthly(System.Predicate<LoginBonusMonthly.Param> match)
	{
		return LoginBonusMonthlys.Find(match);
	}

	public List<LoginBonusMonthly.Param> FindAllLoginBonusMonthly(System.Predicate<LoginBonusMonthly.Param> match)
	{
		return LoginBonusMonthlys.FindAll(match);
	}

	// Excel Sheet : LoginEvent
	//
	public List<LoginEvent.Param> LoginEvents = new List<LoginEvent.Param> ();

	[System.SerializableAttribute]
	public class LoginEvent
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string StartTime;
			public string EndTime;
			public int EventTarget;
			public int SendMailTypeID;
			public int RewardGroupID;
			public int AbsentReward;
		}
	}

	public LoginEvent.Param FindLoginEvent(int KeyValue)
	{
		return LoginEvents.Find(x => x.ID == KeyValue);
	}

	public LoginEvent.Param FindLoginEvent(System.Predicate<LoginEvent.Param> match)
	{
		return LoginEvents.Find(match);
	}

	public List<LoginEvent.Param> FindAllLoginEvent(System.Predicate<LoginEvent.Param> match)
	{
		return LoginEvents.FindAll(match);
	}

	// Excel Sheet : MonthlyFee
	//
	public List<MonthlyFee.Param> MonthlyFees = new List<MonthlyFee.Param> ();

	[System.SerializableAttribute]
	public class MonthlyFee
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int MonthlyFeeID;
			public int RewardID;
			public int D_RewardID;
			public int SendMailTypeID;
		}
	}

	public MonthlyFee.Param FindMonthlyFee(int KeyValue)
	{
		return MonthlyFees.Find(x => x.MonthlyFeeID == KeyValue);
	}

	public MonthlyFee.Param FindMonthlyFee(System.Predicate<MonthlyFee.Param> match)
	{
		return MonthlyFees.Find(match);
	}

	public List<MonthlyFee.Param> FindAllMonthlyFee(System.Predicate<MonthlyFee.Param> match)
	{
		return MonthlyFees.FindAll(match);
	}

	// Excel Sheet : NPCUserData
	//
	public List<NPCUserData.Param> NPCUserDatas = new List<NPCUserData.Param> ();

	[System.SerializableAttribute]
	public class NPCUserData
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int MarkID;
			public int UserRank;
			public int NickName;
			public int Score;
			public int Grade;
			public int Tier;
			public int TeamPower;
		}
	}

	public NPCUserData.Param FindNPCUserData(int KeyValue)
	{
		return NPCUserDatas.Find(x => x.ID == KeyValue);
	}

	public NPCUserData.Param FindNPCUserData(System.Predicate<NPCUserData.Param> match)
	{
		return NPCUserDatas.Find(match);
	}

	public List<NPCUserData.Param> FindAllNPCUserData(System.Predicate<NPCUserData.Param> match)
	{
		return NPCUserDatas.FindAll(match);
	}

	// Excel Sheet : PassSet
	//
	public List<PassSet.Param> PassSets = new List<PassSet.Param> ();

	[System.SerializableAttribute]
	public class PassSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int PassID;
			public string StartTime;
			public string EndTime;
			public int Type;
			public int N_RewardID;
			public int SendMailTypeID_N;
			public int S_RewardID;
			public int SendMailTypeID_S;
			public int PassStoreID;
		}
	}

	public PassSet.Param FindPassSet(int KeyValue)
	{
		return PassSets.Find(x => x.PassID == KeyValue);
	}

	public PassSet.Param FindPassSet(System.Predicate<PassSet.Param> match)
	{
		return PassSets.Find(match);
	}

	public List<PassSet.Param> FindAllPassSet(System.Predicate<PassSet.Param> match)
	{
		return PassSets.FindAll(match);
	}

	// Excel Sheet : PassMission
	//
	public List<PassMission.Param> PassMissions = new List<PassMission.Param> ();

	[System.SerializableAttribute]
	public class PassMission
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int PassID;
			public string ActiveTime;
			public int Desc;
			public string MissionType;
			public int MissionIndex;
			public int MissionValue;
			public int RewardPoint;
		}
	}

	public PassMission.Param FindPassMission(int KeyValue)
	{
		return PassMissions.Find(x => x.ID == KeyValue);
	}

	public PassMission.Param FindPassMission(System.Predicate<PassMission.Param> match)
	{
		return PassMissions.Find(match);
	}

	public List<PassMission.Param> FindAllPassMission(System.Predicate<PassMission.Param> match)
	{
		return PassMissions.FindAll(match);
	}

	// Excel Sheet : RaidStore
	//
	public List<RaidStore.Param> RaidStores = new List<RaidStore.Param> ();

	[System.SerializableAttribute]
	public class RaidStore
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int No;
			public int StoreID;
			public int Prob;
		}
	}

	public RaidStore.Param FindRaidStore(int KeyValue)
	{
		return RaidStores.Find(x => x.GroupID == KeyValue);
	}

	public RaidStore.Param FindRaidStore(System.Predicate<RaidStore.Param> match)
	{
		return RaidStores.Find(match);
	}

	public List<RaidStore.Param> FindAllRaidStore(System.Predicate<RaidStore.Param> match)
	{
		return RaidStores.FindAll(match);
	}

	// Excel Sheet : Random
	//
	public List<Random.Param> Randoms = new List<Random.Param> ();

	[System.SerializableAttribute]
	public class Random
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
			public int Prob;
			public int Value;
		}
	}

	public Random.Param FindRandom(int KeyValue)
	{
		return Randoms.Find(x => x.GroupID == KeyValue);
	}

	public Random.Param FindRandom(System.Predicate<Random.Param> match)
	{
		return Randoms.Find(match);
	}

	public List<Random.Param> FindAllRandom(System.Predicate<Random.Param> match)
	{
		return Randoms.FindAll(match);
	}

	// Excel Sheet : RankUPReward
	//
	public List<RankUPReward.Param> RankUPRewards = new List<RankUPReward.Param> ();

	[System.SerializableAttribute]
	public class RankUPReward
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int RewardRank;
			public int RewardGroupID;
			public int SendMailTypeID;
			public int MessageString;
		}
	}

	public RankUPReward.Param FindRankUPReward(int KeyValue)
	{
		return RankUPRewards.Find(x => x.RewardRank == KeyValue);
	}

	public RankUPReward.Param FindRankUPReward(System.Predicate<RankUPReward.Param> match)
	{
		return RankUPRewards.Find(match);
	}

	public List<RankUPReward.Param> FindAllRankUPReward(System.Predicate<RankUPReward.Param> match)
	{
		return RankUPRewards.FindAll(match);
	}

	// Excel Sheet : RoomAction
	//
	public List<RoomAction.Param> RoomActions = new List<RoomAction.Param> ();

	[System.SerializableAttribute]
	public class RoomAction
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int CharacterID;
			public int Type;
			public string Action;
			public float Weight;
			public string Action2;
			public float Weight2;
			public int StoreRoomID;
		}
	}

	public RoomAction.Param FindRoomAction(int KeyValue)
	{
		return RoomActions.Find(x => x.ID == KeyValue);
	}

	public RoomAction.Param FindRoomAction(System.Predicate<RoomAction.Param> match)
	{
		return RoomActions.Find(match);
	}

	public List<RoomAction.Param> FindAllRoomAction(System.Predicate<RoomAction.Param> match)
	{
		return RoomActions.FindAll(match);
	}

	// Excel Sheet : RoomFigure
	//
	public List<RoomFigure.Param> RoomFigures = new List<RoomFigure.Param> ();

	[System.SerializableAttribute]
	public class RoomFigure
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public string Icon;
			public string IconBuy;
			public string Model;
			public int Platform;
			public int ContentsType;
			public int ContentsIndex;
			public int CharacterID;
			public int StoreRoomID;
		}
	}

	public RoomFigure.Param FindRoomFigure(int KeyValue)
	{
		return RoomFigures.Find(x => x.ID == KeyValue);
	}

	public RoomFigure.Param FindRoomFigure(System.Predicate<RoomFigure.Param> match)
	{
		return RoomFigures.Find(match);
	}

	public List<RoomFigure.Param> FindAllRoomFigure(System.Predicate<RoomFigure.Param> match)
	{
		return RoomFigures.FindAll(match);
	}

	// Excel Sheet : RoomFunc
	//
	public List<RoomFunc.Param> RoomFuncs = new List<RoomFunc.Param> ();

	[System.SerializableAttribute]
	public class RoomFunc
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int RoomTheme;
			public int Name;
			public string Icon;
			public string Function;
			public int GroupID;
			public int StoreRoomID;
		}
	}

	public RoomFunc.Param FindRoomFunc(int KeyValue)
	{
		return RoomFuncs.Find(x => x.ID == KeyValue);
	}

	public RoomFunc.Param FindRoomFunc(System.Predicate<RoomFunc.Param> match)
	{
		return RoomFuncs.Find(match);
	}

	public List<RoomFunc.Param> FindAllRoomFunc(System.Predicate<RoomFunc.Param> match)
	{
		return RoomFuncs.FindAll(match);
	}

	// Excel Sheet : RoomTheme
	//
	public List<RoomTheme.Param> RoomThemes = new List<RoomTheme.Param> ();

	[System.SerializableAttribute]
	public class RoomTheme
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public string Icon;
			public string Scene;
			public int Bgm;
			public int MaxChar;
			public int InitFunc;
			public int PreVisible;
			public int StoreRoomID;
		}
	}

	public RoomTheme.Param FindRoomTheme(int KeyValue)
	{
		return RoomThemes.Find(x => x.ID == KeyValue);
	}

	public RoomTheme.Param FindRoomTheme(System.Predicate<RoomTheme.Param> match)
	{
		return RoomThemes.Find(match);
	}

	public List<RoomTheme.Param> FindAllRoomTheme(System.Predicate<RoomTheme.Param> match)
	{
		return RoomThemes.FindAll(match);
	}

	// Excel Sheet : RotationGacha
	//
	public List<RotationGacha.Param> RotationGachas = new List<RotationGacha.Param> ();

	[System.SerializableAttribute]
	public class RotationGacha
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Banner;
			public string Image;
			public int StoreID1;
			public int StoreID2;
			public int StoreID3;
			public int StoreID4;
			public int OpenCash;
		}
	}

	public RotationGacha.Param FindRotationGacha(int KeyValue)
	{
		return RotationGachas.Find(x => x.ID == KeyValue);
	}

	public RotationGacha.Param FindRotationGacha(System.Predicate<RotationGacha.Param> match)
	{
		return RotationGachas.Find(match);
	}

	public List<RotationGacha.Param> FindAllRotationGacha(System.Predicate<RotationGacha.Param> match)
	{
		return RotationGachas.FindAll(match);
	}

	// Excel Sheet : ServerMergeSet
	//
	public List<ServerMergeSet.Param> ServerMergeSets = new List<ServerMergeSet.Param> ();

	[System.SerializableAttribute]
	public class ServerMergeSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int MaxCount;
		}
	}

	public ServerMergeSet.Param FindServerMergeSet(int KeyValue)
	{
		return ServerMergeSets.Find(x => x.MaxCount == KeyValue);
	}

	public ServerMergeSet.Param FindServerMergeSet(System.Predicate<ServerMergeSet.Param> match)
	{
		return ServerMergeSets.Find(match);
	}

	public List<ServerMergeSet.Param> FindAllServerMergeSet(System.Predicate<ServerMergeSet.Param> match)
	{
		return ServerMergeSets.FindAll(match);
	}

	// Excel Sheet : SecretQuestBOSet
	//
	public List<SecretQuestBOSet.Param> SecretQuestBOSets = new List<SecretQuestBOSet.Param> ();

	[System.SerializableAttribute]
	public class SecretQuestBOSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int StageBOSet;
		}
	}

	public SecretQuestBOSet.Param FindSecretQuestBOSet(int KeyValue)
	{
		return SecretQuestBOSets.Find(x => x.GroupID == KeyValue);
	}

	public SecretQuestBOSet.Param FindSecretQuestBOSet(System.Predicate<SecretQuestBOSet.Param> match)
	{
		return SecretQuestBOSets.Find(match);
	}

	public List<SecretQuestBOSet.Param> FindAllSecretQuestBOSet(System.Predicate<SecretQuestBOSet.Param> match)
	{
		return SecretQuestBOSets.FindAll(match);
	}

	// Excel Sheet : SecretQuestLevel
	//
	public List<SecretQuestLevel.Param> SecretQuestLevels = new List<SecretQuestLevel.Param> ();

	[System.SerializableAttribute]
	public class SecretQuestLevel
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int No;
			public int Type;
			public string Scene;
			public string LevelPB;
			public int ClearCondition;
			public int MissionDesc;
			public int ConditionValue;
			public string BGM;
			public string AmbienceSound;
			public int LoadingTip;
			public int Monster1;
			public int Monster2;
			public int Monster3;
			public int Monster4;
			public int Monster5;
		}
	}

	public SecretQuestLevel.Param FindSecretQuestLevel(int KeyValue)
	{
		return SecretQuestLevels.Find(x => x.GroupID == KeyValue);
	}

	public SecretQuestLevel.Param FindSecretQuestLevel(System.Predicate<SecretQuestLevel.Param> match)
	{
		return SecretQuestLevels.Find(match);
	}

	public List<SecretQuestLevel.Param> FindAllSecretQuestLevel(System.Predicate<SecretQuestLevel.Param> match)
	{
		return SecretQuestLevels.FindAll(match);
	}

	// Excel Sheet : Stage
	//
	public List<Stage.Param> Stages = new List<Stage.Param> ();

	[System.SerializableAttribute]
	public class Stage
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Icon;
			public int Name;
			public int Desc;
			public int StageType;
			public int TypeValue;
			public int Difficulty;
			public int Chapter;
			public int Section;
			public int LimitStage;
			public int NextStage;
			public int Ticket;
			public int TicketMultiple;
			public string Scene;
			public string LevelPB;
			public int ClearCondition;
			public int MissionDesc;
			public int ConditionValue;
			public int RewardValue;
			public int RewardGold;
			public int RewardEXP;
			public int RewardCardFavor;
			public int RewardCharEXP;
			public int RewardSkillPoint;
			public int N_DropID;
			public int N_DropMinCnt;
			public int N_DropMaxCnt;
			public int Luck_DropID;
			public int Condi_Type;
			public int Condi_Value;
			public int Condi_DropID;
			public int Mission_00;
			public int Mission_01;
			public int Mission_02;
			public string BGM;
			public string AmbienceSound;
			public int LoadingTip;
			public int Monster1;
			public int Monster2;
			public int Monster3;
			public int Monster4;
			public int Monster5;
			public int StageBOSet;
			public int ScenarioID_BeforeStart;
			public string ScenarioDrt_Start;
			public int ShowPlayerStartDrt;
			public int ScenarioID_AfterStart;
			public int ScenarioID_BeforeBossAppear;
			public string ScenarioDrt_BossAppear;
			public string ScenarioDrt_BossDie;
			public string ScenarioDrt_EndMission;
			public int ScenarioID_EndMission;
			public string ScenarioDrt_AfterEndMission;
			public int ContinueType;
			public int ContinueCost;
			public int UseFastQuestTicket;
			public int PlayerMode;
		}
	}

	public Stage.Param FindStage(int KeyValue)
	{
		return Stages.Find(x => x.ID == KeyValue);
	}

	public Stage.Param FindStage(System.Predicate<Stage.Param> match)
	{
		return Stages.Find(match);
	}

	public List<Stage.Param> FindAllStage(System.Predicate<Stage.Param> match)
	{
		return Stages.FindAll(match);
	}

	// Excel Sheet : Store
	//
	public List<Store.Param> Stores = new List<Store.Param> ();

	[System.SerializableAttribute]
	public class Store
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Etc;
			public int SaleType;
			public int BuyNotStoreID;
			public int NeedBuyStoreID;
			public int BuyType;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
			public int PurchaseType;
			public int PurchaseIndex;
			public int PurchaseValue;
			public int BonusGoodsType;
			public int BonusGoodsValue;
			public int Value1;
			public int Value2;
			public int Value3;
			public string AOS_ID;
			public string IOS_ID;
			public int ConnectStoreID;
			public int NeedDesirePoint;
			public int GetableDP;
			public int SpecialType;
		}
	}

	public Store.Param FindStore(int KeyValue)
	{
		return Stores.Find(x => x.ID == KeyValue);
	}

	public Store.Param FindStore(System.Predicate<Store.Param> match)
	{
		return Stores.Find(match);
	}

	public List<Store.Param> FindAllStore(System.Predicate<Store.Param> match)
	{
		return Stores.FindAll(match);
	}

	// Excel Sheet : StoreRoom
	//
	public List<StoreRoom.Param> StoreRooms = new List<StoreRoom.Param> ();

	[System.SerializableAttribute]
	public class StoreRoom
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Etc;
			public int ProductType;
			public int ProductIndex;
			public int ProductValue;
			public int PurchaseType;
			public int PurchaseValue;
			public int NeedRoomPoint;
		}
	}

	public StoreRoom.Param FindStoreRoom(int KeyValue)
	{
		return StoreRooms.Find(x => x.ID == KeyValue);
	}

	public StoreRoom.Param FindStoreRoom(System.Predicate<StoreRoom.Param> match)
	{
		return StoreRooms.Find(match);
	}

	public List<StoreRoom.Param> FindAllStoreRoom(System.Predicate<StoreRoom.Param> match)
	{
		return StoreRooms.FindAll(match);
	}

	// Excel Sheet : UserMark
	//
	public List<UserMark.Param> UserMarks = new List<UserMark.Param> ();

	[System.SerializableAttribute]
	public class UserMark
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public int GetDesc;
			public string Icon;
			public string ConType;
			public int ConIndex;
			public int ConValue;
			public int OrderNum;
			public int PreVisible;
			public int AniEnable;
		}
	}

	public UserMark.Param FindUserMark(int KeyValue)
	{
		return UserMarks.Find(x => x.ID == KeyValue);
	}

	public UserMark.Param FindUserMark(System.Predicate<UserMark.Param> match)
	{
		return UserMarks.Find(match);
	}

	public List<UserMark.Param> FindAllUserMark(System.Predicate<UserMark.Param> match)
	{
		return UserMarks.FindAll(match);
	}

	// Excel Sheet : UnexpectedPackage
	//
	public List<UnexpectedPackage.Param> UnexpectedPackages = new List<UnexpectedPackage.Param> ();

	[System.SerializableAttribute]
	public class UnexpectedPackage
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int UnexpectedType;
			public int RepeatValue;
			public int Value1;
			public int Value2;
			public int ConnectStoreID;
			public int PackageBuyTime;
		}
	}

	public UnexpectedPackage.Param FindUnexpectedPackage(int KeyValue)
	{
		return UnexpectedPackages.Find(x => x.ID == KeyValue);
	}

	public UnexpectedPackage.Param FindUnexpectedPackage(System.Predicate<UnexpectedPackage.Param> match)
	{
		return UnexpectedPackages.Find(match);
	}

	public List<UnexpectedPackage.Param> FindAllUnexpectedPackage(System.Predicate<UnexpectedPackage.Param> match)
	{
		return UnexpectedPackages.FindAll(match);
	}

	// Excel Sheet : Weapon
	//
	public List<Weapon.Param> Weapons = new List<Weapon.Param> ();

	[System.SerializableAttribute]
	public class Weapon
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Icon;
			public string ModelR;
			public string ModelL;
			public string SubModelR;
			public string SubModelL;
			public string Sub2ModelR;
			public string Sub2ModelL;
			public string AddedUnitWeapon;
			public string CharacterID;
			public int CharacterBaseID;
			public string WpnDepotSetFlag;
			public int Grade;
			public int BaseStatSet;
			public int SellPrice;
			public int SellMPoint;
			public int LevelUpGroup;
			public int Exp;
			public int ATK;
			public int CRI;
			public float IncATK;
			public float IncCRI;
			public int SkillEffectName;
			public int SkillEffectDesc;
			public int WpnBOWorkType;
			public string WpnBOWorkTypeValue;
			public int WpnBOActivate;
			public int WpnAddBOSetID1;
			public int WpnAddBOSetID2;
			public int UseSP;
			public int WakeReqGroup;
			public int EnchantGroup;
			public int Decomposition;
			public int AcquisitionID;
			public int Tradeable;
			public int Selectable;
			public int Decomposable;
			public int PreVisible;
		}
	}

	public Weapon.Param FindWeapon(int KeyValue)
	{
		return Weapons.Find(x => x.ID == KeyValue);
	}

	public Weapon.Param FindWeapon(System.Predicate<Weapon.Param> match)
	{
		return Weapons.Find(match);
	}

	public List<Weapon.Param> FindAllWeapon(System.Predicate<Weapon.Param> match)
	{
		return Weapons.FindAll(match);
	}

	// Excel Sheet : WeeklyMissionSet
	//
	public List<WeeklyMissionSet.Param> WeeklyMissionSets = new List<WeeklyMissionSet.Param> ();

	[System.SerializableAttribute]
	public class WeeklyMissionSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int SetID;
			public string WMCon0;
			public string WMCon1;
			public string WMCon2;
			public string WMCon3;
			public string WMCon4;
			public string WMCon5;
			public string WMCon6;
			public int WMCnt0;
			public int WMCnt1;
			public int WMCnt2;
			public int WMCnt3;
			public int WMCnt4;
			public int WMCnt5;
			public int WMCnt6;
			public int RewardGroupID;
		}
	}

	public WeeklyMissionSet.Param FindWeeklyMissionSet(int KeyValue)
	{
		return WeeklyMissionSets.Find(x => x.SetID == KeyValue);
	}

	public WeeklyMissionSet.Param FindWeeklyMissionSet(System.Predicate<WeeklyMissionSet.Param> match)
	{
		return WeeklyMissionSets.Find(match);
	}

	public List<WeeklyMissionSet.Param> FindAllWeeklyMissionSet(System.Predicate<WeeklyMissionSet.Param> match)
	{
		return WeeklyMissionSets.FindAll(match);
	}

}
