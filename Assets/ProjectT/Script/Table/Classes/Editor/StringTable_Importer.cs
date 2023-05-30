using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StringTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/String.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/String.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			StringTable data = (StringTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(StringTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<StringTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - String
				{
					data.Strings.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("String");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "String");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							StringTable.String.Param p = new StringTable.String.Param ();


							cell = row.GetCell(0);
							p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(1);
							p.Text_KOR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Text_JPN = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Text_ENG = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Text_CHS = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Text_CHT = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Text_ESP = (cell == null ? "" : cell.StringCellValue);

							data.Strings.Add(p);

						}
					}
				}

			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
