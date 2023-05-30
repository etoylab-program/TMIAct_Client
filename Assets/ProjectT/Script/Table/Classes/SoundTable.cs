using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundTable : ScriptableObject
{	

	// Excel Sheet : Sound
	//
	public List<Sound.Param> Sounds = new List<Sound.Param> ();

	[System.SerializableAttribute]
	public class Sound
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public int Type;
			public string Name;
			public string Path;
			public float Volume;
		}
	}

	public Sound.Param FindSound(int KeyValue)
	{
		return Sounds.Find(x => x.ID == KeyValue);
	}

	public Sound.Param FindSound(System.Predicate<Sound.Param> match)
	{
		return Sounds.Find(match);
	}

	public List<Sound.Param> FindAllSound(System.Predicate<Sound.Param> match)
	{
		return Sounds.FindAll(match);
	}

	// Excel Sheet : VoiceRndCount
	//
	public List<VoiceRndCount.Param> VoiceRndCounts = new List<VoiceRndCount.Param> ();

	[System.SerializableAttribute]
	public class VoiceRndCount
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public int ID;
			public int Value;
			public int Count;
		}
	}

	public VoiceRndCount.Param FindVoiceRndCount(int KeyValue)
	{
		return VoiceRndCounts.Find(x => x.Group == KeyValue);
	}

	public VoiceRndCount.Param FindVoiceRndCount(System.Predicate<VoiceRndCount.Param> match)
	{
		return VoiceRndCounts.Find(match);
	}

	public List<VoiceRndCount.Param> FindAllVoiceRndCount(System.Predicate<VoiceRndCount.Param> match)
	{
		return VoiceRndCounts.FindAll(match);
	}

	// Excel Sheet : VoiceArenaType
	//
	public List<VoiceArenaType.Param> VoiceArenaTypes = new List<VoiceArenaType.Param> ();

	[System.SerializableAttribute]
	public class VoiceArenaType
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int CharID;
			public int Type;
			public int RndCnt;
			public int SameCnt;
			public string ConnectChar;
		}
	}

	public VoiceArenaType.Param FindVoiceArenaType(int KeyValue)
	{
		return VoiceArenaTypes.Find(x => x.CharID == KeyValue);
	}

	public VoiceArenaType.Param FindVoiceArenaType(System.Predicate<VoiceArenaType.Param> match)
	{
		return VoiceArenaTypes.Find(match);
	}

	public List<VoiceArenaType.Param> FindAllVoiceArenaType(System.Predicate<VoiceArenaType.Param> match)
	{
		return VoiceArenaTypes.FindAll(match);
	}

}
