using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class String_ENGTable : ScriptableObject
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
			public string Text;
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
