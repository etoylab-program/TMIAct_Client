using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class ScenarioTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/Scenario_Favor_C1.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/Scenario_Favor_C1.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			ScenarioTable data = (ScenarioTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScenarioTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<ScenarioTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - Scenario
				{
					data.Scenarios.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Scenario");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Scenario");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							ScenarioTable.Scenario.Param p = new ScenarioTable.Scenario.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.BundlePath = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Num = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Type = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Pos = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(5);
							p.Value1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Value2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.Value3 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.Value4 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.Value5 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.Voice = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.Time = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(12);
							p.Next = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.TremIndex = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(14);
							p.TremLogOnly = (cell == null ? "" : cell.StringCellValue);

							data.Scenarios.Add(p);

						}
					}
				}

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
						
							ScenarioTable.Trem.Param p = new ScenarioTable.Trem.Param ();


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
						
							ScenarioTable.String.Param p = new ScenarioTable.String.Param ();


							cell = row.GetCell(0);
							p.ID = long.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name_KOR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Text_KOR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Name_JPN = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Text_JPN = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Name_ENG = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Text_ENG = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.Name_CHS = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.Text_CHS = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.Name_CHT = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.Text_CHT = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.Name_ESP = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(12);
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
