using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StringTable : ScriptableObject
{	

	// Excel Sheet : String
	//
	public List<String.Param> Strings = new List<String.Param> ();

	[System.SerializableAttribute]
	public class String
	{
		[System.SerializableAttribute]
		public class Param
		{
			
			public int ID;
			public string Text_KOR;
			public string Text_JPN;
			public string Text_ENG;
			public string Text_CHS;
			public string Text_CHT;
			public string Text_ESP;
		}
	}

	public String.Param FindString(int KeyValue)
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
