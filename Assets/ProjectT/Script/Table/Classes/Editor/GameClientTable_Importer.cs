using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class GameClientTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/GameClient.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/GameClient.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			GameClientTable data = (GameClientTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(GameClientTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<GameClientTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - Acquisition
				{
					data.Acquisitions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Acquisition");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Acquisition");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.Acquisition.Param p = new GameClientTable.Acquisition.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Num = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Type = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Value1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Value2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Value3 = (cell == null ? "" : cell.StringCellValue);

							data.Acquisitions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - BattleOption
				{
					data.BattleOptions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("BattleOption");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "BattleOption");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.BattleOption.Param p = new GameClientTable.BattleOption.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.CheckTimingType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.StartCondType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.BOPreCondType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.BOCondType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.CondValue = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(6);
							p.BOActionCondType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.BOAtkCondType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.BOTarType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.BOTarValue = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(10);
							p.BOBuffType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.BOFuncType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(12);
							p.BOAddCallTiming = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(13);
							p.BOAddBOSetID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.BOBuffIconType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.BuffIconFlash = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.Repeat = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.RepeatDelay = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(18);
							p.UseOnceAndIgnoreFunc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.BattleOptions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - BattleOptionSet
				{
					data.BattleOptionSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("BattleOptionSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "BattleOptionSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.BattleOptionSet.Param p = new GameClientTable.BattleOptionSet.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Desc = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.BattleOptionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.BOStartDelay = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(4);
							p.BOCoolTime = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(5);
							p.BORandomStart = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.BOFuncValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.BOFuncValue2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.BOFuncValue3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.BOFuncIncValue = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(10);
							p.BOBuffDurTime = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(11);
							p.BOBuffTurnTime = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(12);
							p.BOBuffStackValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.BOReferenceSetID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.BOAction = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(15);
							p.BOIndependentActionSystem = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.BOProjectile = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(17);
							p.BOEffectType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.BONoDelayStartEffId = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.BOStartEffectId = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.BOEndEffectId = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.BOEffectIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.BOEffectIndex2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.UseOnce = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.MinionIds = (cell == null ? "" : cell.StringCellValue);

							data.BattleOptionSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Book
				{
					data.Books.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Book");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Book");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.Book.Param p = new GameClientTable.Book.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Num = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ItemID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.AcquisitionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Books.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - BookMonster
				{
					data.BookMonsters.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("BookMonster");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "BookMonster");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.BookMonster.Param p = new GameClientTable.BookMonster.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.MonType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.RoomFigureID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Scale = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(7);
							p.Height = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.BookMonsters.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Chapter
				{
					data.Chapters.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Chapter");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Chapter");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.Chapter.Param p = new GameClientTable.Chapter.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Bg = (cell == null ? "" : cell.StringCellValue);

							data.Chapters.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ChatWords
				{
					data.ChatWordss.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ChatWords");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ChatWords");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.ChatWords.Param p = new GameClientTable.ChatWords.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Words = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ChatWordss.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CommonEffect
				{
					data.CommonEffects.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CommonEffect");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CommonEffect");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.CommonEffect.Param p = new GameClientTable.CommonEffect.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.UnitType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.UnitTableId = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.EffectPrefab = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Attach = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Follow = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.Sound = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.MixerIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.CameraAni = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.PosX = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(11);
							p.PosY = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(12);
							p.PosZ = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(13);
							p.RotX = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(14);
							p.RotY = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(15);
							p.RotZ = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.CommonEffects.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - DropItem
				{
					data.DropItems.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("DropItem");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "DropItem");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.DropItem.Param p = new GameClientTable.DropItem.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ItemAddBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ModelPb = (cell == null ? "" : cell.StringCellValue);

							data.DropItems.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - EventPage
				{
					data.EventPages.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("EventPage");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "EventPage");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.EventPage.Param p = new GameClientTable.EventPage.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.TypeValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.TabIcon = (cell == null ? "" : cell.StringCellValue);

							data.EventPages.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - FacilityTradeHelp
				{
					data.FacilityTradeHelps.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("FacilityTradeHelp");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "FacilityTradeHelp");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.FacilityTradeHelp.Param p = new GameClientTable.FacilityTradeHelp.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Count = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.FacilityTradeHelps.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - GachaTab
				{
					data.GachaTabs.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("GachaTab");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "GachaTab");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.GachaTab.Param p = new GameClientTable.GachaTab.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Type = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Category = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.TabIcon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.BGTaxture = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.StoreID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.StoreID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.GachaTabs.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - GemSetOpt
				{
					data.GemSetOpts.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("GemSetOpt");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "GemSetOpt");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.GemSetOpt.Param p = new GameClientTable.GemSetOpt.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.SetCount = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.GemBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.GemBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.GemSetOpts.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - GoodsIcon
				{
					data.GoodsIcons.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("GoodsIcon");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "GoodsIcon");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.GoodsIcon.Param p = new GameClientTable.GoodsIcon.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Etc = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);

							data.GoodsIcons.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - HelpBuffDebuff
				{
					data.HelpBuffDebuffs.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("HelpBuffDebuff");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "HelpBuffDebuff");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.HelpBuffDebuff.Param p = new GameClientTable.HelpBuffDebuff.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.BuffType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.HelpBuffDebuffs.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - HelpCharInfo
				{
					data.HelpCharInfos.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("HelpCharInfo");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "HelpCharInfo");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.HelpCharInfo.Param p = new GameClientTable.HelpCharInfo.Param ();


							cell = row.GetCell(0);
							p.CharID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StrongID = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.WeakID = (cell == null ? "" : cell.StringCellValue);

							data.HelpCharInfos.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - HelpEnemyInfo
				{
					data.HelpEnemyInfos.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("HelpEnemyInfo");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "HelpEnemyInfo");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.HelpEnemyInfo.Param p = new GameClientTable.HelpEnemyInfo.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StageType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.StageID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.MonType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.Icon = (cell == null ? "" : cell.StringCellValue);

							data.HelpEnemyInfos.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - HowToBeStrong
				{
					data.HowToBeStrongs.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("HowToBeStrong");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "HowToBeStrong");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.HowToBeStrong.Param p = new GameClientTable.HowToBeStrong.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Index = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Atlas = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Detail = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.BtnText = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ItemID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.ItemID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.ItemID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.ItemID4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.ItemID5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.HowToBeStrongs.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LoadingTip
				{
					data.LoadingTips.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LoadingTip");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LoadingTip");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.LoadingTip.Param p = new GameClientTable.LoadingTip.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LoadingTips.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LoginBonusMonthlyDisplay
				{
					data.LoginBonusMonthlyDisplays.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LoginBonusMonthlyDisplay");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LoginBonusMonthlyDisplay");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.LoginBonusMonthlyDisplay.Param p = new GameClientTable.LoginBonusMonthlyDisplay.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Banner = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Day = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LoginBonusMonthlyDisplays.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Menu
				{
					data.Menus.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Menu");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Menu");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.Menu.Param p = new GameClientTable.Menu.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.PreVisible = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Menus.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - MiniGameShoot
				{
					data.MiniGameShoots.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("MiniGameShoot");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "MiniGameShoot");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.MiniGameShoot.Param p = new GameClientTable.MiniGameShoot.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Time = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(2);
							p.MoveSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(3);
							p.ModelPb = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Score = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.TagetPattern = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.EffectType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ThrowAniType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.Hitsnd = (cell == null ? "" : cell.StringCellValue);

							data.MiniGameShoots.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Monster
				{
					data.Monsters.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Monster");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Monster");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.Monster.Param p = new GameClientTable.Monster.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Level = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.EnemyType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.HPShow = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.MonType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.MonBOSetList = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.BossAppear = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.BattlePower = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.BaseStatSet = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.MaxHP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.AttackPower = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(13);
							p.DefenceRate = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.CriticalRate = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.Shield = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.ShieldBreakEffect = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.AttackSight = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.MoveSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(19);
							p.BackwardSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(20);
							p.HitRecovery = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(21);
							p.AttackDelay = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(22);
							p.AttackProb = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(23);
							p.DefaultSuperArmor = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.DropObjID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.AllDropItemID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(26);
							p.AllDropItemValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(27);
							p.RandomDropItemID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(28);
							p.RandomItemValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(29);
							p.AI = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(30);
							p.ModelPb = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(31);
							p.Platform = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(32);
							p.Texture = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(33);
							p.Scale = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(34);
							p.RimlightColor_R = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(35);
							p.RimlightColor_G = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(36);
							p.RimlightColor_B = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(37);
							p.RimlightValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(38);
							p.Portrait = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(39);
							p.AttachEffect = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(40);
							p.PushWeight = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(41);
							p.Immune_KnockBack = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(42);
							p.Immune_Fly = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(43);
							p.Immune_Down = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(44);
							p.Immune_Pulling = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(45);
							p.FxSndArmorHit = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(46);
							p.FxSndArmorBreak = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(47);
							p.Hit_01 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(48);
							p.Hit_02 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(49);
							p.Hit_03 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(50);
							p.Hit_04 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(51);
							p.HitSnd = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(52);
							p.EffShield = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(53);
							p.EffShieldBreak = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(54);
							p.EffShieldAttack = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(55);
							p.Child = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(56);
							p.EffBossHit = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(57);
							p.DieDrt = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(58);
							p.ChangeId = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(59);
							p.FixedCamera = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(60);
							p.KnockBack_Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(61);
							p.MinionId = (cell == null ? "" : cell.StringCellValue);

							data.Monsters.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - NameColor
				{
					data.NameColors.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("NameColor");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "NameColor");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.NameColor.Param p = new GameClientTable.NameColor.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RGBColor = (cell == null ? "" : cell.StringCellValue);

							data.NameColors.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - NPCCharRnd
				{
					data.NPCCharRnds.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("NPCCharRnd");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "NPCCharRnd");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.NPCCharRnd.Param p = new GameClientTable.NPCCharRnd.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.MarkID = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.UserRank = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.NickName = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Score = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Tier = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.BadgeCnt = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.CharCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.CharGrade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.CharLv = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.WpnGrade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.WpnLv = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(13);
							p.WpnSLv = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.WpnWake = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.SptGrade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.SptLv = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(17);
							p.SptSLv = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.SptWake = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.NPCCharRnds.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RaidAddStat
				{
					data.RaidAddStats.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RaidAddStat");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RaidAddStat");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.RaidAddStat.Param p = new GameClientTable.RaidAddStat.Param ();


							cell = row.GetCell(0);
							p.RaidStep = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.AddDef = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.AddSpd = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(3);
							p.AddCriRate = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(4);
							p.AddCriDmg = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(5);
							p.AddCriReg = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(6);
							p.AddCriDef = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(7);
							p.AddPenetrate = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.RaidAddStats.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RaidBOSet
				{
					data.RaidBOSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RaidBOSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RaidBOSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.RaidBOSet.Param p = new GameClientTable.RaidBOSet.Param ();


							cell = row.GetCell(0);
							p.RaidStep = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Count = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.StageBOSet = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RaidBOSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RaidInfo
				{
					data.RaidInfos.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RaidInfo");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RaidInfo");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.RaidInfo.Param p = new GameClientTable.RaidInfo.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RaidBg = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.StageBg = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.LevelPBList = (cell == null ? "" : cell.StringCellValue);

							data.RaidInfos.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RandomDrop
				{
					data.RandomDrops.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RandomDrop");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RandomDrop");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.RandomDrop.Param p = new GameClientTable.RandomDrop.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.DropItemIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.DropItemValueMin = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.DropItemValueMax = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Prob = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RandomDrops.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - StageMission
				{
					data.StageMissions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("StageMission");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "StageMission");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.StageMission.Param p = new GameClientTable.StageMission.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.MissionCondition = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.ConditionCompareType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.ConditionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.StageMissions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - StageBOSet
				{
					data.StageBOSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("StageBOSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "StageBOSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.StageBOSet.Param p = new GameClientTable.StageBOSet.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.StageBOSetList = (cell == null ? "" : cell.StringCellValue);

							data.StageBOSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - StoreDisplayGoods
				{
					data.StoreDisplayGoodss.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("StoreDisplayGoods");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "StoreDisplayGoods");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.StoreDisplayGoods.Param p = new GameClientTable.StoreDisplayGoods.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Etc = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.StoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ShowHaveStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.HideHaveStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.HideConType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.HideConValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.OriginalPriceID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.CmpstBnfts = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.PanelType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.Type = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.Category = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.SubCategory = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.LimitType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.DrtType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.Description = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.Count = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.PackageUIType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.AdvancedPackage = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.PackageLabel = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(22);
							p.MarkIcon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(23);
							p.IconLocalize = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(24);
							p.CharacterID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.ShowButton = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.StoreDisplayGoodss.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Tutorial
				{
					data.Tutorials.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Tutorial");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Tutorial");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.Tutorial.Param p = new GameClientTable.Tutorial.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StateID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Step = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.SendPacket = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.NextID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.NextType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.NextTime = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(8);
							p.TimeScale = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(9);
							p.Mask = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(10);
							p.Popup = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.PopupObject = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(12);
							p.MaskWidth = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.MaskHeight = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.MaskIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.CharShow = (cell == null ? false : cell.BooleanCellValue);
							cell = row.GetCell(16);
							p.CharPosX = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.DescPosX = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.DescPosY = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.DescPivot = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.HandStyle = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.HandX = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.HandY = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.HandMX = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.HandMY = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.HandSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(26);
							p.HandAngleZ = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(27);
							p.HUDSprite = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(28);
							p.HUDUseSupporterTex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Tutorials.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - WpnDepotSet
				{
					data.WpnDepotSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("WpnDepotSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "WpnDepotSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameClientTable.WpnDepotSet.Param p = new GameClientTable.WpnDepotSet.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.GroupOrder = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ReqCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.BonusATK = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.WpnDepotSets.Add(p);

						}
					}
				}

			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
