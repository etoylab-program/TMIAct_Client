using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Scenario_JPNTable : ScriptableObject
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
			public string Title;
			public string Desc;
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

}
