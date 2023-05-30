using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class CommandCheckList_Importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/ProjectT/Table/CommandCheckList.xlsx";
    private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/CommandCheckList.asset";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (!filePath.Equals(asset))
                continue;

            CommandCheckListTable data = (CommandCheckListTable)AssetDatabase.LoadAssetAtPath(exportPath, typeof(CommandCheckListTable));
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<CommandCheckListTable>();
                AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
                //data.hideFlags = HideFlags.NotEditable;
            }

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook book = new XSSFWorkbook(stream);


                /////////////////////////////////////////////
                // Import Sheet - String
                {
                    data.Keywords.Clear();   // clear sheet

                    ISheet sheet = book.GetSheet("Keyword");
                    if (sheet == null)
                    {
                        Debug.LogError("ExcelImporter Error: sheet not found," + "String");
                    }
                    else
                    {
                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            ICell cell = null;

                            CommandCheckListTable.Keyword.Param p = new CommandCheckListTable.Keyword.Param();


                            cell = row.GetCell(0);
                            try
                            {
                                p.keyword = (cell == null ? "" : cell.StringCellValue);

                            }
                            catch(System.InvalidOperationException e)
                            {
                                int temp = (int)(cell == null ? 0 : cell.NumericCellValue);
                                p.keyword = temp.ToString();
                            }

                            data.Keywords.Add(p);

                        }
                    }
                }

            }

            ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
            EditorUtility.SetDirty(obj);
        }
    }
}
