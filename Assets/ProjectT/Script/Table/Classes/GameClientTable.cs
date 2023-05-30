using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameClientTable : ScriptableObject
{	

	// Excel Sheet : Acquisition
	//
	public List<Acquisition.Param> Acquisitions = new List<Acquisition.Param> ();

	[System.SerializableAttribute]
	public class Acquisition
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int Num;
			public int Desc;
			public string Type;
			public string Value1;
			public string Value2;
			public string Value3;
		}
	}

	public Acquisition.Param FindAcquisition(int KeyValue)
	{
		return Acquisitions.Find(x => x.GroupID == KeyValue);
	}

	public Acquisition.Param FindAcquisition(System.Predicate<Acquisition.Param> match)
	{
		return Acquisitions.Find(match);
	}

	public List<Acquisition.Param> FindAllAcquisition(System.Predicate<Acquisition.Param> match)
	{
		return Acquisitions.FindAll(match);
	}

	// Excel Sheet : BattleOption
	//
	public List<BattleOption.Param> BattleOptions = new List<BattleOption.Param> ();

	[System.SerializableAttribute]
	public class BattleOption
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string CheckTimingType;
			public string StartCondType;
			public string BOPreCondType;
			public string BOCondType;
			public float CondValue;
			public string BOActionCondType;
			public string BOAtkCondType;
			public string BOTarType;
			public float BOTarValue;
			public string BOBuffType;
			public string BOFuncType;
			public string BOAddCallTiming;
			public int BOAddBOSetID;
			public int BOBuffIconType;
			public int BuffIconFlash;
			public int Repeat;
			public float RepeatDelay;
			public int UseOnceAndIgnoreFunc;
		}
	}

	public BattleOption.Param FindBattleOption(int KeyValue)
	{
		return BattleOptions.Find(x => x.ID == KeyValue);
	}

	public BattleOption.Param FindBattleOption(System.Predicate<BattleOption.Param> match)
	{
		return BattleOptions.Find(match);
	}

	public List<BattleOption.Param> FindAllBattleOption(System.Predicate<BattleOption.Param> match)
	{
		return BattleOptions.FindAll(match);
	}

	// Excel Sheet : BattleOptionSet
	//
	public List<BattleOptionSet.Param> BattleOptionSets = new List<BattleOptionSet.Param> ();

	[System.SerializableAttribute]
	public class BattleOptionSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Desc;
			public int BattleOptionID;
			public float BOStartDelay;
			public float BOCoolTime;
			public int BORandomStart;
			public int BOFuncValue;
			public int BOFuncValue2;
			public int BOFuncValue3;
			public float BOFuncIncValue;
			public float BOBuffDurTime;
			public float BOBuffTurnTime;
			public int BOBuffStackValue;
			public int BOReferenceSetID;
			public string BOAction;
			public int BOIndependentActionSystem;
			public string BOProjectile;
			public int BOEffectType;
			public int BONoDelayStartEffId;
			public int BOStartEffectId;
			public int BOEndEffectId;
			public int BOEffectIndex;
			public int BOEffectIndex2;
			public int UseOnce;
			public string MinionIds;
		}
	}

	public BattleOptionSet.Param FindBattleOptionSet(int KeyValue)
	{
		return BattleOptionSets.Find(x => x.ID == KeyValue);
	}

	public BattleOptionSet.Param FindBattleOptionSet(System.Predicate<BattleOptionSet.Param> match)
	{
		return BattleOptionSets.Find(match);
	}

	public List<BattleOptionSet.Param> FindAllBattleOptionSet(System.Predicate<BattleOptionSet.Param> match)
	{
		return BattleOptionSets.FindAll(match);
	}

	// Excel Sheet : Book
	//
	public List<Book.Param> Books = new List<Book.Param> ();

	[System.SerializableAttribute]
	public class Book
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public int Num;
			public int ItemID;
			public int Desc;
			public int AcquisitionID;
		}
	}

	public Book.Param FindBook(int KeyValue)
	{
		return Books.Find(x => x.Group == KeyValue);
	}

	public Book.Param FindBook(System.Predicate<Book.Param> match)
	{
		return Books.Find(match);
	}

	public List<Book.Param> FindAllBook(System.Predicate<Book.Param> match)
	{
		return Books.FindAll(match);
	}

	// Excel Sheet : BookMonster
	//
	public List<BookMonster.Param> BookMonsters = new List<BookMonster.Param> ();

	[System.SerializableAttribute]
	public class BookMonster
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int MonType;
			public int Grade;
			public string Icon;
			public int RoomFigureID;
			public float Scale;
			public float Height;
		}
	}

	public BookMonster.Param FindBookMonster(int KeyValue)
	{
		return BookMonsters.Find(x => x.ID == KeyValue);
	}

	public BookMonster.Param FindBookMonster(System.Predicate<BookMonster.Param> match)
	{
		return BookMonsters.Find(match);
	}

	public List<BookMonster.Param> FindAllBookMonster(System.Predicate<BookMonster.Param> match)
	{
		return BookMonsters.FindAll(match);
	}

	// Excel Sheet : Chapter
	//
	public List<Chapter.Param> Chapters = new List<Chapter.Param> ();

	[System.SerializableAttribute]
	public class Chapter
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Bg;
		}
	}

	public Chapter.Param FindChapter(int KeyValue)
	{
		return Chapters.Find(x => x.ID == KeyValue);
	}

	public Chapter.Param FindChapter(System.Predicate<Chapter.Param> match)
	{
		return Chapters.Find(match);
	}

	public List<Chapter.Param> FindAllChapter(System.Predicate<Chapter.Param> match)
	{
		return Chapters.FindAll(match);
	}

	// Excel Sheet : ChatWords
	//
	public List<ChatWords.Param> ChatWordss = new List<ChatWords.Param> ();

	[System.SerializableAttribute]
	public class ChatWords
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Words;
		}
	}

	public ChatWords.Param FindChatWords(int KeyValue)
	{
		return ChatWordss.Find(x => x.ID == KeyValue);
	}

	public ChatWords.Param FindChatWords(System.Predicate<ChatWords.Param> match)
	{
		return ChatWordss.Find(match);
	}

	public List<ChatWords.Param> FindAllChatWords(System.Predicate<ChatWords.Param> match)
	{
		return ChatWordss.FindAll(match);
	}

	// Excel Sheet : CommonEffect
	//
	public List<CommonEffect.Param> CommonEffects = new List<CommonEffect.Param> ();

	[System.SerializableAttribute]
	public class CommonEffect
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Type;
			public int UnitType;
			public int UnitTableId;
			public string EffectPrefab;
			public string Attach;
			public int Follow;
			public string Sound;
			public int MixerIndex;
			public string CameraAni;
			public float PosX;
			public float PosY;
			public float PosZ;
			public float RotX;
			public float RotY;
			public float RotZ;
		}
	}

	public CommonEffect.Param FindCommonEffect(int KeyValue)
	{
		return CommonEffects.Find(x => x.ID == KeyValue);
	}

	public CommonEffect.Param FindCommonEffect(System.Predicate<CommonEffect.Param> match)
	{
		return CommonEffects.Find(match);
	}

	public List<CommonEffect.Param> FindAllCommonEffect(System.Predicate<CommonEffect.Param> match)
	{
		return CommonEffects.FindAll(match);
	}

	// Excel Sheet : DropItem
	//
	public List<DropItem.Param> DropItems = new List<DropItem.Param> ();

	[System.SerializableAttribute]
	public class DropItem
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Type;
			public int ItemAddBOSetID1;
			public string ModelPb;
		}
	}

	public DropItem.Param FindDropItem(int KeyValue)
	{
		return DropItems.Find(x => x.ID == KeyValue);
	}

	public DropItem.Param FindDropItem(System.Predicate<DropItem.Param> match)
	{
		return DropItems.Find(match);
	}

	public List<DropItem.Param> FindAllDropItem(System.Predicate<DropItem.Param> match)
	{
		return DropItems.FindAll(match);
	}

	// Excel Sheet : EventPage
	//
	public List<EventPage.Param> EventPages = new List<EventPage.Param> ();

	[System.SerializableAttribute]
	public class EventPage
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Type;
			public int TypeValue;
			public int Name;
			public string TabIcon;
		}
	}

	public EventPage.Param FindEventPage(int KeyValue)
	{
		return EventPages.Find(x => x.ID == KeyValue);
	}

	public EventPage.Param FindEventPage(System.Predicate<EventPage.Param> match)
	{
		return EventPages.Find(match);
	}

	public List<EventPage.Param> FindAllEventPage(System.Predicate<EventPage.Param> match)
	{
		return EventPages.FindAll(match);
	}

	// Excel Sheet : FacilityTradeHelp
	//
	public List<FacilityTradeHelp.Param> FacilityTradeHelps = new List<FacilityTradeHelp.Param> ();

	[System.SerializableAttribute]
	public class FacilityTradeHelp
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Desc;
			public string Icon;
			public int Count;
		}
	}

	public FacilityTradeHelp.Param FindFacilityTradeHelp(int KeyValue)
	{
		return FacilityTradeHelps.Find(x => x.ID == KeyValue);
	}

	public FacilityTradeHelp.Param FindFacilityTradeHelp(System.Predicate<FacilityTradeHelp.Param> match)
	{
		return FacilityTradeHelps.Find(match);
	}

	public List<FacilityTradeHelp.Param> FindAllFacilityTradeHelp(System.Predicate<FacilityTradeHelp.Param> match)
	{
		return FacilityTradeHelps.FindAll(match);
	}

	// Excel Sheet : GachaTab
	//
	public List<GachaTab.Param> GachaTabs = new List<GachaTab.Param> ();

	[System.SerializableAttribute]
	public class GachaTab
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Type;
			public int Category;
			public string TabIcon;
			public string BGTaxture;
			public int StoreID1;
			public int StoreID2;
			public int Name;
		}
	}

	public GachaTab.Param FindGachaTab(int KeyValue)
	{
		return GachaTabs.Find(x => x.ID == KeyValue);
	}

	public GachaTab.Param FindGachaTab(System.Predicate<GachaTab.Param> match)
	{
		return GachaTabs.Find(match);
	}

	public List<GachaTab.Param> FindAllGachaTab(System.Predicate<GachaTab.Param> match)
	{
		return GachaTabs.FindAll(match);
	}

	// Excel Sheet : GemSetOpt
	//
	public List<GemSetOpt.Param> GemSetOpts = new List<GemSetOpt.Param> ();

	[System.SerializableAttribute]
	public class GemSetOpt
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int SetCount;
			public int Name;
			public int Desc;
			public int GemBOSetID1;
			public int GemBOSetID2;
		}
	}

	public GemSetOpt.Param FindGemSetOpt(int KeyValue)
	{
		return GemSetOpts.Find(x => x.GroupID == KeyValue);
	}

	public GemSetOpt.Param FindGemSetOpt(System.Predicate<GemSetOpt.Param> match)
	{
		return GemSetOpts.Find(match);
	}

	public List<GemSetOpt.Param> FindAllGemSetOpt(System.Predicate<GemSetOpt.Param> match)
	{
		return GemSetOpts.FindAll(match);
	}

	// Excel Sheet : GoodsIcon
	//
	public List<GoodsIcon.Param> GoodsIcons = new List<GoodsIcon.Param> ();

	[System.SerializableAttribute]
	public class GoodsIcon
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Etc;
			public string Icon;
		}
	}

	public GoodsIcon.Param FindGoodsIcon(int KeyValue)
	{
		return GoodsIcons.Find(x => x.ID == KeyValue);
	}

	public GoodsIcon.Param FindGoodsIcon(System.Predicate<GoodsIcon.Param> match)
	{
		return GoodsIcons.Find(match);
	}

	public List<GoodsIcon.Param> FindAllGoodsIcon(System.Predicate<GoodsIcon.Param> match)
	{
		return GoodsIcons.FindAll(match);
	}

	// Excel Sheet : HelpBuffDebuff
	//
	public List<HelpBuffDebuff.Param> HelpBuffDebuffs = new List<HelpBuffDebuff.Param> ();

	[System.SerializableAttribute]
	public class HelpBuffDebuff
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Desc;
			public string Icon;
			public int BuffType;
		}
	}

	public HelpBuffDebuff.Param FindHelpBuffDebuff(int KeyValue)
	{
		return HelpBuffDebuffs.Find(x => x.ID == KeyValue);
	}

	public HelpBuffDebuff.Param FindHelpBuffDebuff(System.Predicate<HelpBuffDebuff.Param> match)
	{
		return HelpBuffDebuffs.Find(match);
	}

	public List<HelpBuffDebuff.Param> FindAllHelpBuffDebuff(System.Predicate<HelpBuffDebuff.Param> match)
	{
		return HelpBuffDebuffs.FindAll(match);
	}

	// Excel Sheet : HelpCharInfo
	//
	public List<HelpCharInfo.Param> HelpCharInfos = new List<HelpCharInfo.Param> ();

	[System.SerializableAttribute]
	public class HelpCharInfo
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int CharID;
			public string StrongID;
			public string WeakID;
		}
	}

	public HelpCharInfo.Param FindHelpCharInfo(int KeyValue)
	{
		return HelpCharInfos.Find(x => x.CharID == KeyValue);
	}

	public HelpCharInfo.Param FindHelpCharInfo(System.Predicate<HelpCharInfo.Param> match)
	{
		return HelpCharInfos.Find(match);
	}

	public List<HelpCharInfo.Param> FindAllHelpCharInfo(System.Predicate<HelpCharInfo.Param> match)
	{
		return HelpCharInfos.FindAll(match);
	}

	// Excel Sheet : HelpEnemyInfo
	//
	public List<HelpEnemyInfo.Param> HelpEnemyInfos = new List<HelpEnemyInfo.Param> ();

	[System.SerializableAttribute]
	public class HelpEnemyInfo
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int StageType;
			public int StageID;
			public int Name;
			public int Desc;
			public int MonType;
			public int Grade;
			public string Icon;
		}
	}

	public HelpEnemyInfo.Param FindHelpEnemyInfo(int KeyValue)
	{
		return HelpEnemyInfos.Find(x => x.ID == KeyValue);
	}

	public HelpEnemyInfo.Param FindHelpEnemyInfo(System.Predicate<HelpEnemyInfo.Param> match)
	{
		return HelpEnemyInfos.Find(match);
	}

	public List<HelpEnemyInfo.Param> FindAllHelpEnemyInfo(System.Predicate<HelpEnemyInfo.Param> match)
	{
		return HelpEnemyInfos.FindAll(match);
	}

	// Excel Sheet : HowToBeStrong
	//
	public List<HowToBeStrong.Param> HowToBeStrongs = new List<HowToBeStrong.Param> ();

	[System.SerializableAttribute]
	public class HowToBeStrong
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public int Index;
			public int Name;
			public string Atlas;
			public string Icon;
			public int Detail;
			public int Desc;
			public int BtnText;
			public int ItemID1;
			public int ItemID2;
			public int ItemID3;
			public int ItemID4;
			public int ItemID5;
		}
	}

	public HowToBeStrong.Param FindHowToBeStrong(int KeyValue)
	{
		return HowToBeStrongs.Find(x => x.Group == KeyValue);
	}

	public HowToBeStrong.Param FindHowToBeStrong(System.Predicate<HowToBeStrong.Param> match)
	{
		return HowToBeStrongs.Find(match);
	}

	public List<HowToBeStrong.Param> FindAllHowToBeStrong(System.Predicate<HowToBeStrong.Param> match)
	{
		return HowToBeStrongs.FindAll(match);
	}

	// Excel Sheet : LoadingTip
	//
	public List<LoadingTip.Param> LoadingTips = new List<LoadingTip.Param> ();

	[System.SerializableAttribute]
	public class LoadingTip
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int Name;
			public int Desc;
		}
	}

	public LoadingTip.Param FindLoadingTip(int KeyValue)
	{
		return LoadingTips.Find(x => x.GroupID == KeyValue);
	}

	public LoadingTip.Param FindLoadingTip(System.Predicate<LoadingTip.Param> match)
	{
		return LoadingTips.Find(match);
	}

	public List<LoadingTip.Param> FindAllLoadingTip(System.Predicate<LoadingTip.Param> match)
	{
		return LoadingTips.FindAll(match);
	}

	// Excel Sheet : LoginBonusMonthlyDisplay
	//
	public List<LoginBonusMonthlyDisplay.Param> LoginBonusMonthlyDisplays = new List<LoginBonusMonthlyDisplay.Param> ();

	[System.SerializableAttribute]
	public class LoginBonusMonthlyDisplay
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Banner;
			public int Name;
			public int Day;
		}
	}

	public LoginBonusMonthlyDisplay.Param FindLoginBonusMonthlyDisplay(int KeyValue)
	{
		return LoginBonusMonthlyDisplays.Find(x => x.ID == KeyValue);
	}

	public LoginBonusMonthlyDisplay.Param FindLoginBonusMonthlyDisplay(System.Predicate<LoginBonusMonthlyDisplay.Param> match)
	{
		return LoginBonusMonthlyDisplays.Find(match);
	}

	public List<LoginBonusMonthlyDisplay.Param> FindAllLoginBonusMonthlyDisplay(System.Predicate<LoginBonusMonthlyDisplay.Param> match)
	{
		return LoginBonusMonthlyDisplays.FindAll(match);
	}

	// Excel Sheet : Menu
	//
	public List<Menu.Param> Menus = new List<Menu.Param> ();

	[System.SerializableAttribute]
	public class Menu
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public string Icon;
			public int PreVisible;
		}
	}

	public Menu.Param FindMenu(int KeyValue)
	{
		return Menus.Find(x => x.ID == KeyValue);
	}

	public Menu.Param FindMenu(System.Predicate<Menu.Param> match)
	{
		return Menus.Find(match);
	}

	public List<Menu.Param> FindAllMenu(System.Predicate<Menu.Param> match)
	{
		return Menus.FindAll(match);
	}

	// Excel Sheet : MiniGameShoot
	//
	public List<MiniGameShoot.Param> MiniGameShoots = new List<MiniGameShoot.Param> ();

	[System.SerializableAttribute]
	public class MiniGameShoot
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public float Time;
			public float MoveSpeed;
			public string ModelPb;
			public int Score;
			public string TagetPattern;
			public int EffectType;
			public int ThrowAniType;
			public string Hitsnd;
		}
	}

	public MiniGameShoot.Param FindMiniGameShoot(int KeyValue)
	{
		return MiniGameShoots.Find(x => x.ID == KeyValue);
	}

	public MiniGameShoot.Param FindMiniGameShoot(System.Predicate<MiniGameShoot.Param> match)
	{
		return MiniGameShoots.Find(match);
	}

	public List<MiniGameShoot.Param> FindAllMiniGameShoot(System.Predicate<MiniGameShoot.Param> match)
	{
		return MiniGameShoots.FindAll(match);
	}

	// Excel Sheet : Monster
	//
	public List<Monster.Param> Monsters = new List<Monster.Param> ();

	[System.SerializableAttribute]
	public class Monster
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public int Level;
			public int EnemyType;
			public int HPShow;
			public int MonType;
			public int Grade;
			public string MonBOSetList;
			public string BossAppear;
			public int BattlePower;
			public string BaseStatSet;
			public int MaxHP;
			public float AttackPower;
			public int DefenceRate;
			public int CriticalRate;
			public int Shield;
			public int ShieldBreakEffect;
			public int AttackSight;
			public float MoveSpeed;
			public float BackwardSpeed;
			public float HitRecovery;
			public float AttackDelay;
			public float AttackProb;
			public int DefaultSuperArmor;
			public int DropObjID;
			public int AllDropItemID;
			public int AllDropItemValue;
			public int RandomDropItemID;
			public int RandomItemValue;
			public string AI;
			public string ModelPb;
			public int Platform;
			public string Texture;
			public float Scale;
			public int RimlightColor_R;
			public int RimlightColor_G;
			public int RimlightColor_B;
			public int RimlightValue;
			public string Portrait;
			public string AttachEffect;
			public float PushWeight;
			public bool Immune_KnockBack;
			public bool Immune_Fly;
			public bool Immune_Down;
			public bool Immune_Pulling;
			public string FxSndArmorHit;
			public string FxSndArmorBreak;
			public string Hit_01;
			public string Hit_02;
			public string Hit_03;
			public string Hit_04;
			public string HitSnd;
			public string EffShield;
			public string EffShieldBreak;
			public string EffShieldAttack;
			public int Child;
			public string EffBossHit;
			public string DieDrt;
			public int ChangeId;
			public bool FixedCamera;
			public int KnockBack_Type;
			public string MinionId;
		}
	}

	public Monster.Param FindMonster(int KeyValue)
	{
		return Monsters.Find(x => x.ID == KeyValue);
	}

	public Monster.Param FindMonster(System.Predicate<Monster.Param> match)
	{
		return Monsters.Find(match);
	}

	public List<Monster.Param> FindAllMonster(System.Predicate<Monster.Param> match)
	{
		return Monsters.FindAll(match);
	}

	// Excel Sheet : NameColor
	//
	public List<NameColor.Param> NameColors = new List<NameColor.Param> ();

	[System.SerializableAttribute]
	public class NameColor
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string RGBColor;
		}
	}

	public NameColor.Param FindNameColor(int KeyValue)
	{
		return NameColors.Find(x => x.ID == KeyValue);
	}

	public NameColor.Param FindNameColor(System.Predicate<NameColor.Param> match)
	{
		return NameColors.Find(match);
	}

	public List<NameColor.Param> FindAllNameColor(System.Predicate<NameColor.Param> match)
	{
		return NameColors.FindAll(match);
	}

	// Excel Sheet : NPCCharRnd
	//
	public List<NPCCharRnd.Param> NPCCharRnds = new List<NPCCharRnd.Param> ();

	[System.SerializableAttribute]
	public class NPCCharRnd
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string MarkID;
			public string UserRank;
			public string NickName;
			public string Score;
			public int Grade;
			public int Tier;
			public string BadgeCnt;
			public int CharCnt;
			public int CharGrade;
			public string CharLv;
			public int WpnGrade;
			public string WpnLv;
			public int WpnSLv;
			public int WpnWake;
			public int SptGrade;
			public string SptLv;
			public int SptSLv;
			public int SptWake;
		}
	}

	public NPCCharRnd.Param FindNPCCharRnd(int KeyValue)
	{
		return NPCCharRnds.Find(x => x.ID == KeyValue);
	}

	public NPCCharRnd.Param FindNPCCharRnd(System.Predicate<NPCCharRnd.Param> match)
	{
		return NPCCharRnds.Find(match);
	}

	public List<NPCCharRnd.Param> FindAllNPCCharRnd(System.Predicate<NPCCharRnd.Param> match)
	{
		return NPCCharRnds.FindAll(match);
	}

	// Excel Sheet : RaidAddStat
	//
	public List<RaidAddStat.Param> RaidAddStats = new List<RaidAddStat.Param> ();

	[System.SerializableAttribute]
	public class RaidAddStat
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int RaidStep;
			public int AddDef;
			public float AddSpd;
			public float AddCriRate;
			public float AddCriDmg;
			public float AddCriReg;
			public float AddCriDef;
			public float AddPenetrate;
		}
	}

	public RaidAddStat.Param FindRaidAddStat(int KeyValue)
	{
		return RaidAddStats.Find(x => x.RaidStep == KeyValue);
	}

	public RaidAddStat.Param FindRaidAddStat(System.Predicate<RaidAddStat.Param> match)
	{
		return RaidAddStats.Find(match);
	}

	public List<RaidAddStat.Param> FindAllRaidAddStat(System.Predicate<RaidAddStat.Param> match)
	{
		return RaidAddStats.FindAll(match);
	}

	// Excel Sheet : RaidBOSet
	//
	public List<RaidBOSet.Param> RaidBOSets = new List<RaidBOSet.Param> ();

	[System.SerializableAttribute]
	public class RaidBOSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int RaidStep;
			public int Count;
			public int StageBOSet;
		}
	}

	public RaidBOSet.Param FindRaidBOSet(int KeyValue)
	{
		return RaidBOSets.Find(x => x.RaidStep == KeyValue);
	}

	public RaidBOSet.Param FindRaidBOSet(System.Predicate<RaidBOSet.Param> match)
	{
		return RaidBOSets.Find(match);
	}

	public List<RaidBOSet.Param> FindAllRaidBOSet(System.Predicate<RaidBOSet.Param> match)
	{
		return RaidBOSets.FindAll(match);
	}

	// Excel Sheet : RaidInfo
	//
	public List<RaidInfo.Param> RaidInfos = new List<RaidInfo.Param> ();

	[System.SerializableAttribute]
	public class RaidInfo
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Name;
			public string RaidBg;
			public string StageBg;
			public string LevelPBList;
		}
	}

	public RaidInfo.Param FindRaidInfo(int KeyValue)
	{
		return RaidInfos.Find(x => x.ID == KeyValue);
	}

	public RaidInfo.Param FindRaidInfo(System.Predicate<RaidInfo.Param> match)
	{
		return RaidInfos.Find(match);
	}

	public List<RaidInfo.Param> FindAllRaidInfo(System.Predicate<RaidInfo.Param> match)
	{
		return RaidInfos.FindAll(match);
	}

	// Excel Sheet : RandomDrop
	//
	public List<RandomDrop.Param> RandomDrops = new List<RandomDrop.Param> ();

	[System.SerializableAttribute]
	public class RandomDrop
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int DropItemIndex;
			public int DropItemValueMin;
			public int DropItemValueMax;
			public int Prob;
		}
	}

	public RandomDrop.Param FindRandomDrop(int KeyValue)
	{
		return RandomDrops.Find(x => x.GroupID == KeyValue);
	}

	public RandomDrop.Param FindRandomDrop(System.Predicate<RandomDrop.Param> match)
	{
		return RandomDrops.Find(match);
	}

	public List<RandomDrop.Param> FindAllRandomDrop(System.Predicate<RandomDrop.Param> match)
	{
		return RandomDrops.FindAll(match);
	}

	// Excel Sheet : StageMission
	//
	public List<StageMission.Param> StageMissions = new List<StageMission.Param> ();

	[System.SerializableAttribute]
	public class StageMission
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Desc;
			public string MissionCondition;
			public string ConditionCompareType;
			public int ConditionValue;
		}
	}

	public StageMission.Param FindStageMission(int KeyValue)
	{
		return StageMissions.Find(x => x.ID == KeyValue);
	}

	public StageMission.Param FindStageMission(System.Predicate<StageMission.Param> match)
	{
		return StageMissions.Find(match);
	}

	public List<StageMission.Param> FindAllStageMission(System.Predicate<StageMission.Param> match)
	{
		return StageMissions.FindAll(match);
	}

	// Excel Sheet : StageBOSet
	//
	public List<StageBOSet.Param> StageBOSets = new List<StageBOSet.Param> ();

	[System.SerializableAttribute]
	public class StageBOSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public int Desc;
			public string Icon;
			public string StageBOSetList;
		}
	}

	public StageBOSet.Param FindStageBOSet(int KeyValue)
	{
		return StageBOSets.Find(x => x.Group == KeyValue);
	}

	public StageBOSet.Param FindStageBOSet(System.Predicate<StageBOSet.Param> match)
	{
		return StageBOSets.Find(match);
	}

	public List<StageBOSet.Param> FindAllStageBOSet(System.Predicate<StageBOSet.Param> match)
	{
		return StageBOSets.FindAll(match);
	}

	// Excel Sheet : StoreDisplayGoods
	//
	public List<StoreDisplayGoods.Param> StoreDisplayGoodss = new List<StoreDisplayGoods.Param> ();

	[System.SerializableAttribute]
	public class StoreDisplayGoods
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Etc;
			public int StoreID;
			public int ShowHaveStoreID;
			public int HideHaveStoreID;
			public string HideConType;
			public int HideConValue;
			public int OriginalPriceID;
			public string CmpstBnfts;
			public int PanelType;
			public string Type;
			public int Category;
			public int SubCategory;
			public int LimitType;
			public int DrtType;
			public int Name;
			public int Description;
			public int Count;
			public int PackageUIType;
			public int AdvancedPackage;
			public int PackageLabel;
			public string Icon;
			public string MarkIcon;
			public bool IconLocalize;
			public int CharacterID;
			public int ShowButton;
		}
	}

	public StoreDisplayGoods.Param FindStoreDisplayGoods(int KeyValue)
	{
		return StoreDisplayGoodss.Find(x => x.ID == KeyValue);
	}

	public StoreDisplayGoods.Param FindStoreDisplayGoods(System.Predicate<StoreDisplayGoods.Param> match)
	{
		return StoreDisplayGoodss.Find(match);
	}

	public List<StoreDisplayGoods.Param> FindAllStoreDisplayGoods(System.Predicate<StoreDisplayGoods.Param> match)
	{
		return StoreDisplayGoodss.FindAll(match);
	}

	// Excel Sheet : Tutorial
	//
	public List<Tutorial.Param> Tutorials = new List<Tutorial.Param> ();

	[System.SerializableAttribute]
	public class Tutorial
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int StateID;
			public int Step;
			public int Desc;
			public int SendPacket;
			public int NextID;
			public int NextType;
			public float NextTime;
			public float TimeScale;
			public bool Mask;
			public string Popup;
			public string PopupObject;
			public int MaskWidth;
			public int MaskHeight;
			public int MaskIndex;
			public bool CharShow;
			public int CharPosX;
			public int DescPosX;
			public int DescPosY;
			public int DescPivot;
			public int HandStyle;
			public int HandX;
			public int HandY;
			public int HandMX;
			public int HandMY;
			public float HandSpeed;
			public float HandAngleZ;
			public string HUDSprite;
			public int HUDUseSupporterTex;
		}
	}

	public Tutorial.Param FindTutorial(int KeyValue)
	{
		return Tutorials.Find(x => x.ID == KeyValue);
	}

	public Tutorial.Param FindTutorial(System.Predicate<Tutorial.Param> match)
	{
		return Tutorials.Find(match);
	}

	public List<Tutorial.Param> FindAllTutorial(System.Predicate<Tutorial.Param> match)
	{
		return Tutorials.FindAll(match);
	}

	// Excel Sheet : WpnDepotSet
	//
	public List<WpnDepotSet.Param> WpnDepotSets = new List<WpnDepotSet.Param> ();

	[System.SerializableAttribute]
	public class WpnDepotSet
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int GroupID;
			public int GroupOrder;
			public int Name;
			public int ReqCnt;
			public float BonusATK;
		}
	}

	public WpnDepotSet.Param FindWpnDepotSet(int KeyValue)
	{
		return WpnDepotSets.Find(x => x.GroupID == KeyValue);
	}

	public WpnDepotSet.Param FindWpnDepotSet(System.Predicate<WpnDepotSet.Param> match)
	{
		return WpnDepotSets.Find(match);
	}

	public List<WpnDepotSet.Param> FindAllWpnDepotSet(System.Predicate<WpnDepotSet.Param> match)
	{
		return WpnDepotSets.FindAll(match);
	}

}
