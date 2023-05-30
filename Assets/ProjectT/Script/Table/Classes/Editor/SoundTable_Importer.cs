using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class SoundTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/Sound.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/Sound.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			SoundTable data = (SoundTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(SoundTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<SoundTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - Sound
				{
					data.Sounds.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Sound");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Sound");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							SoundTable.Sound.Param p = new SoundTable.Sound.Param ();


							cell = row.GetCell(0);
							p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(1);
							p.Type = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(2);
							p.Name = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Path = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Volume = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.Sounds.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - VoiceRndCount
				{
					data.VoiceRndCounts.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("VoiceRndCount");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "VoiceRndCount");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							SoundTable.VoiceRndCount.Param p = new SoundTable.VoiceRndCount.Param ();


							cell = row.GetCell(0);
							p.Group = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(1);
							p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(2);
							p.Value = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(3);
							p.Count = (int)(cell == null ? 0 : cell.NumericCellValue);

							data.VoiceRndCounts.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - VoiceArenaType
				{
					data.VoiceArenaTypes.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("VoiceArenaType");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "VoiceArenaType");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							SoundTable.VoiceArenaType.Param p = new SoundTable.VoiceArenaType.Param ();


							cell = row.GetCell(0);
							p.CharID = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(1);
							p.Type = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(2);
							p.RndCnt = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(3);
							p.SameCnt = (int)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(4);
							p.ConnectChar = (cell == null ? "" : cell.StringCellValue);

							data.VoiceArenaTypes.Add(p);

						}
					}
				}

			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
