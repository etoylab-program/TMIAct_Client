using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScenarioTable : ScriptableObject
{	

	// Excel Sheet : Scenario
	//
	public List<Scenario.Param> Scenarios = new List<Scenario.Param> ();

	[System.SerializableAttribute]
	public class Scenario
	{
		[System.SerializableAttribute]
		public class Param
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
	}

	public Scenario.Param FindScenario(int KeyValue)
	{
		return Scenarios.Find(x => x.Group == KeyValue);
	}

	public Scenario.Param FindScenario(System.Predicate<Scenario.Param> match)
	{
		return Scenarios.Find(match);
	}

	public List<Scenario.Param> FindAllScenario(System.Predicate<Scenario.Param> match)
	{
		return Scenarios.FindAll(match);
	}

	// Excel Sheet : Trem
	//
	public List<Trem.Param> Trems = new List<Trem.Param> ();

	[System.SerializableAttribute]
	public class Trem
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int Group;
			public string Title_KOR;
			public string Desc_KOR;
			public string Title_JPN;
			public string Desc_JPN;
			public string Title_ENG;
			public string Desc_ENG;
			public string Title_CHS;
			public string Desc_CHS;
			public string Title_CHT;
			public string Desc_CHT;
			public string Title_ESP;
			public string Desc_ESP;
		}
	}

	public Trem.Param FindTrem(int KeyValue)
	{
		return Trems.Find(x => x.Group == KeyValue);
	}

	public Trem.Param FindTrem(System.Predicate<Trem.Param> match)
	{
		return Trems.Find(match);
	}

	public List<Trem.Param> FindAllTrem(System.Predicate<Trem.Param> match)
	{
		return Trems.FindAll(match);
	}

	// Excel Sheet : String
	//
	public List<String.Param> Strings = new List<String.Param> ();

	[System.SerializableAttribute]
	public class String
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public long ID;
			public string Name_KOR;
			public string Text_KOR;
			public string Name_JPN;
			public string Text_JPN;
			public string Name_ENG;
			public string Text_ENG;
			public string Name_CHS;
			public string Text_CHS;
			public string Name_CHT;
			public string Text_CHT;
			public string Name_ESP;
			public string Text_ESP;
		}
	}

	public String.Param FindString(long KeyValue)
	{
		return Strings.Find(x => x.ID == KeyValue);
	}

	public String.Param FindString(System.Predicate<String.Param> match)
	{
		return Strings.Find(match);
	}

	public List<String.Param> FindAllString(System.Predicate<String.Param> match)
	{
		return Strings.FindAll(match);
	}

}
