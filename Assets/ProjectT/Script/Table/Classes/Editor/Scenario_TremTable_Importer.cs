using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class Scenario_TremTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/Scenario_Trem.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/Scenario_Trem.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			Scenario_TremTable data = (Scenario_TremTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Scenario_TremTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Scenario_TremTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - Trem
				{
					data.Trems.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Trem");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Trem");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							Scenario_TremTable.Trem.Param p = new Scenario_TremTable.Trem.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Title_KOR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Desc_KOR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Title_JPN = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Desc_JPN = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Title_ENG = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Desc_ENG = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.Title_CHS = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.Desc_CHS = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.Title_CHT = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.Desc_CHT = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.Title_ESP = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(12);
							p.Desc_ESP = (cell == null ? "" : cell.StringCellValue);

							data.Trems.Add(p);

						}
					}
				}

			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
