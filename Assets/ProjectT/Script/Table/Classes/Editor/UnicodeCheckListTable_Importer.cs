using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class UnicodeCheckListTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/UnicodeCheckList.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/UnicodeCheckList.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			UnicodeCheckListTable data = (UnicodeCheckListTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(UnicodeCheckListTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<UnicodeCheckListTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - Info
				{
					data.Infos.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Info");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Info");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							UnicodeCheckListTable.Info.Param p = new UnicodeCheckListTable.Info.Param ();


							cell = row.GetCell(0);
							p.Min = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Max = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Infos.Add(p);

						}
					}
				}

			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
