using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class GameTable_Importer : AssetPostprocessor
{
	private static readonly string filePath = "Assets/ProjectT/Table/Game.xlsx";
	private static readonly string exportPath = "Assets/Resources.AssetBundle/System/Table/Game.asset";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets)
		{
			if (!filePath.Equals (asset))
				continue;
				
			GameTable data = (GameTable)AssetDatabase.LoadAssetAtPath (exportPath, typeof(GameTable));
			if (data == null) {
				data = ScriptableObject.CreateInstance<GameTable> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				//data.hideFlags = HideFlags.NotEditable;
			}
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read))
			{
				IWorkbook book = new XSSFWorkbook (stream);
				
				
				/////////////////////////////////////////////
				// Import Sheet - Achieve
				{
					data.Achieves.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Achieve");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Achieve");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Achieve.Param p = new GameTable.Achieve.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.GroupOrder = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.AchieveKind = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.AchieveType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.AchieveIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.AchieveValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.RewardAchievePoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Achieves.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - AchieveEvent
				{
					data.AchieveEvents.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("AchieveEvent");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "AchieveEvent");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.AchieveEvent.Param p = new GameTable.AchieveEvent.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Image = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.BGTaxture = (cell == null ? "" : cell.StringCellValue);

							data.AchieveEvents.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - AchieveEventData
				{
					data.AchieveEventDatas.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("AchieveEventData");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "AchieveEventData");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.AchieveEventData.Param p = new GameTable.AchieveEventData.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.AchieveGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.GroupOrder = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.AchieveType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.AchieveIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.AchieveValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.RewardType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.RewardIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.AchieveEventDatas.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ArenaGrade
				{
					data.ArenaGrades.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ArenaGrade");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ArenaGrade");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.ArenaGrade.Param p = new GameTable.ArenaGrade.Param ();


							cell = row.GetCell(0);
							p.GradeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Tier = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.MatchPrice = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ReqScore = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ArenaGrades.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ArenaReward
				{
					data.ArenaRewards.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ArenaReward");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ArenaReward");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.ArenaReward.Param p = new GameTable.ArenaReward.Param ();


							cell = row.GetCell(0);
							p.RewardOrder = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RewardType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ArenaRewards.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ArenaTower
				{
					data.ArenaTowers.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ArenaTower");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ArenaTower");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.ArenaTower.Param p = new GameTable.ArenaTower.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.BGImg = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Scene = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.LevelPB = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.ClearCondition = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.MissionDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.ConditionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.BGM = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.LoadingTip = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.RewardType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.RewardIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ArenaTowers.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - AwakeSkill
				{
					data.AwakeSkills.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("AwakeSkill");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "AwakeSkill");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.AwakeSkill.Param p = new GameTable.AwakeSkill.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Etc = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.ItemReqListID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.MaxLevel = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.SptAddBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.SptAddBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.SptSvrOptType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.SptSvrOptValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.SptSvrOptIncValue = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.AwakeSkills.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - BadgeOpt
				{
					data.BadgeOpts.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("BadgeOpt");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "BadgeOpt");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.BadgeOpt.Param p = new GameTable.BadgeOpt.Param ();


							cell = row.GetCell(0);
							p.OptionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.EffectType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.IncEffectValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.BattlePowerRate = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.BadgeOpts.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Buff
				{
					data.Buffs.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Buff");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Buff");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Buff.Param p = new GameTable.Buff.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.UseType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Type = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Condition = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Value = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Buffs.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - BingoEvent
				{
					data.BingoEvents.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("BingoEvent");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "BingoEvent");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.BingoEvent.Param p = new GameTable.BingoEvent.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Image = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.BGTaxture = (cell == null ? "" : cell.StringCellValue);

							data.BingoEvents.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - BingoEventData
				{
					data.BingoEventDatas.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("BingoEventData");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "BingoEventData");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.BingoEventData.Param p = new GameTable.BingoEventData.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ItemID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ItemID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ItemID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.ItemID4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ItemID5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ItemID6 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ItemID7 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.ItemID8 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.ItemID9 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.ItemID10 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.ItemID11 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.ItemID12 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.ItemID13 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.ItemID14 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.ItemID15 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.ItemID16 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.ItemCount1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.ItemCount2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.ItemCount3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.ItemCount4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.ItemCount5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.ItemCount6 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.ItemCount7 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.ItemCount8 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(26);
							p.ItemCount9 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(27);
							p.ItemCount10 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(28);
							p.ItemCount11 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(29);
							p.ItemCount12 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(30);
							p.ItemCount13 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(31);
							p.ItemCount14 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(32);
							p.ItemCount15 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(33);
							p.ItemCount16 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(34);
							p.Clear1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(35);
							p.Clear2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(36);
							p.Clear3 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(37);
							p.Clear4 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(38);
							p.Clear5 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(39);
							p.Clear6 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(40);
							p.Clear7 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(41);
							p.Clear8 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(42);
							p.Clear9 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(43);
							p.Clear10 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(44);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(45);
							p.OpenCost = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.BingoEventDatas.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Card
				{
					data.Cards.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Card");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Card");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Card.Param p = new GameTable.Card.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Greetings = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.BaseStatSet = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.SellPrice = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.SellMPoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.HP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.DEF = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.IncHP = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(13);
							p.IncDEF = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(14);
							p.LevelUpGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.Exp = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.FavorGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.SkillEffectName = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.SkillEffectDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.SptBOWorkType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.SptAddBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.SptAddBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.CoolTime = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.MainSkillEffectName = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.MainSkillEffectDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.SptMainBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(26);
							p.SptMainBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(27);
							p.SptSvrOptType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(28);
							p.SptSvrOptValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(29);
							p.SptSvrOptIncValue = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(30);
							p.SptSvrMainOptType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(31);
							p.SptSvrMainOptValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(32);
							p.SptSvrMainOptIncValue = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(33);
							p.ScenarioGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(34);
							p.WakeReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(35);
							p.EnchantGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(36);
							p.Decomposition = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(37);
							p.AcquisitionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(38);
							p.SpriteIcon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(39);
							p.Changeable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(40);
							p.Tradeable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(41);
							p.Selectable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(42);
							p.Decomposable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(43);
							p.PreVisible = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Cards.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CardDispatchSlot
				{
					data.CardDispatchSlots.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CardDispatchSlot");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CardDispatchSlot");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CardDispatchSlot.Param p = new GameTable.CardDispatchSlot.Param ();


							cell = row.GetCell(0);
							p.Index = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.NeedRank = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.InitGrade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.OpenGoods = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.OpenValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.ChangeGoods = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ChangeValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.GradeRate1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.GradeRate2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.GradeRate3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.GradeRate4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.GradeRate5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.CardDispatchSlots.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CardDispatchMission
				{
					data.CardDispatchMissions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CardDispatchMission");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CardDispatchMission");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CardDispatchMission.Param p = new GameTable.CardDispatchMission.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Time = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.NeedURCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.SocketType1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.SocketType2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.SocketType3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.SocketType4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.SocketType5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.RewardType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.RewardIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.CardDispatchMissions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CardFormation
				{
					data.CardFormations.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CardFormation");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CardFormation");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CardFormation.Param p = new GameTable.CardFormation.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.CardID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.CardID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.CardID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.GetHP = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(7);
							p.LevelHP = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(8);
							p.FavorHP = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(9);
							p.FormationBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.FormationBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.OptionKind = (cell == null ? "" : cell.StringCellValue);

							data.CardFormations.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Character
				{
					data.Characters.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Character");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Character");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Character.Param p = new GameTable.Character.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Model = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.WakeReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.MonType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.CharBOSetList = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.MoveSpeed = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(10);
							p.HP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.ATK = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.DEF = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.CRI = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.IncHP = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(15);
							p.IncATK = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(16);
							p.IncDEF = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(17);
							p.IncCRI = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(18);
							p.InitSelect = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.InitWeapon = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.InitCostume = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.InitAddStageID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.InitSkillSlot1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.InitSkillSlot2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.InitSkillSlot3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.InitSkillSlot4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(26);
							p.StartDrt = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(27);
							p.WinDrt_01 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(28);
							p.WinDrt_02 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(29);
							p.GroggyDrt_02 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(30);
							p.ResurrectionDrt_02 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(31);
							p.DieDrt_02 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(32);
							p.USkillDrt_01 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(33);
							p.FxSndArmorHit = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(34);
							p.FxSndArmorBreak = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(35);
							p.EffTarget = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(36);
							p.EffShield = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(37);
							p.EffShieldBreak = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(38);
							p.EffShieldAttack = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(39);
							p.Hit_01 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(40);
							p.Hit_02 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(41);
							p.Hit_03 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(42);
							p.Hit_04 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(43);
							p.ScenarioGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(44);
							p.BuyDrt = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(45);
							p.GradeUpDrt = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(46);
							p.Face = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(47);
							p.SpriteIcon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(48);
							p.AI = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(49);
							p.PreferenceStep1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(50);
							p.PreferenceStep2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(51);
							p.PreferenceLevelGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(52);
							p.PreferenceBuff = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(53);
							p.TrainingRoom = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Characters.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CharacterSkillPassive
				{
					data.CharacterSkillPassives.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CharacterSkillPassive");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CharacterSkillPassive");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CharacterSkillPassive.Param p = new GameTable.CharacterSkillPassive.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.CharacterID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Slot = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Atlus = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.UpIcon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.SkillMovie = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.CommandIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.MaxLevel = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.NextID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.ParentsID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.CondType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.CondValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.ItemReqListID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.Effect = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(19);
							p.SkillAction = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(20);
							p.ReplacedAction = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(21);
							p.Value1 = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(22);
							p.Value2 = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(23);
							p.Value3 = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(24);
							p.IncValue1 = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(25);
							p.IncValue2 = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(26);
							p.CharAddBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(27);
							p.CharAddBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(28);
							p.AddBOSetActionIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(29);
							p.VoiceID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(30);
							p.CoolTime = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(31);
							p.LastAtkSuperArmor = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.CharacterSkillPassives.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ChatStamp
				{
					data.ChatStamps.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ChatStamp");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ChatStamp");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.ChatStamp.Param p = new GameTable.ChatStamp.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.StoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ChatStamps.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CircleCheck
				{
					data.CircleChecks.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CircleCheck");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CircleCheck");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CircleCheck.Param p = new GameTable.CircleCheck.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.CircleCheckCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.SendMailTypeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.NextCircleCheckGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.CircleChecks.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CircleGold
				{
					data.CircleGolds.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CircleGold");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CircleGold");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CircleGold.Param p = new GameTable.CircleGold.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.CircleWorkValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.CircleGolds.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - CircleMark
				{
					data.CircleMarks.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("CircleMark");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "CircleMark");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.CircleMark.Param p = new GameTable.CircleMark.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Marktype = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.SubIcon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.StoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.CircleMarks.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Costume
				{
					data.Costumes.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Costume");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Costume");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Costume.Param p = new GameTable.Costume.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.CharacterID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Model = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.HairModel = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.SubHairChange = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ColorCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.HP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.ATK = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.DEF = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.CRI = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.StoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.OrderNum = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.PreVisible = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.LobbyOnly = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(17);
							p.InGameOnly = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(18);
							p.UseDyeing = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.BaseColor1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(20);
							p.BaseColor2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(21);
							p.BaseColor3 = (cell == null ? "" : cell.StringCellValue);

							data.Costumes.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - DailyMission
				{
					data.DailyMissions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("DailyMission");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "DailyMission");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.DailyMission.Param p = new GameTable.DailyMission.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Day = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.MissionType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.MissionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.DailyMissions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - DailyMissionSet
				{
					data.DailyMissionSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("DailyMissionSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "DailyMissionSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.DailyMissionSet.Param p = new GameTable.DailyMissionSet.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.EventTarget = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.DailyMissionSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Enchant
				{
					data.Enchants.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Enchant");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Enchant");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Enchant.Param p = new GameTable.Enchant.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Level = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.IncreaseValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Prob = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ItemReqListID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Enchants.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - EventExchangeReward
				{
					data.EventExchangeRewards.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("EventExchangeReward");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "EventExchangeReward");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.EventExchangeReward.Param p = new GameTable.EventExchangeReward.Param ();


							cell = row.GetCell(0);
							p.EventID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RewardStep = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ReqItemID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ReqItemCnt1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ReqItemID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.ReqItemCnt2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ReqItemID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ReqItemCnt3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.ExchangeCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.IndexID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.EventExchangeRewards.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - EventResetReward
				{
					data.EventResetRewards.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("EventResetReward");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "EventResetReward");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.EventResetReward.Param p = new GameTable.EventResetReward.Param ();


							cell = row.GetCell(0);
							p.EventID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RewardStep = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.RewardRate = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.RewardCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ResetFlag = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.EventResetRewards.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - EventSet
				{
					data.EventSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("EventSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "EventSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.EventSet.Param p = new GameTable.EventSet.Param ();


							cell = row.GetCell(0);
							p.EventID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.EventType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.PlayOpenTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.PlayCloseTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.EventItemID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.EventItemID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.EventItemID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.EventItemID4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.EventItemID5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.EventItemID6 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.MainBGSpr = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(15);
							p.BannerBGSprNOW = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(16);
							p.BannerBGSprPAST = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(17);
							p.StageBG = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(18);
							p.EventRuleBG = (cell == null ? "" : cell.StringCellValue);

							data.EventSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Facility
				{
					data.Facilitys.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Facility");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Facility");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Facility.Param p = new GameTable.Facility.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.EffectDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Model = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.EffectType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.EffectValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.IncEffectValue = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(9);
							p.Time = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.RewardCardFavor = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.ParentsID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.LevelUpItemReq = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.MaxLevel = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.FacilityOpenUserRank = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.SlotOpenFacilityLv = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.Accelerate = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Facilitys.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - FacilityItemCombine
				{
					data.FacilityItemCombines.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("FacilityItemCombine");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "FacilityItemCombine");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.FacilityItemCombine.Param p = new GameTable.FacilityItemCombine.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ReqFacilityLv = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Time = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.RewardCardFavor = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ItemID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ItemCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ItemReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.FacilityItemCombines.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - FacilityOperationRoom
				{
					data.FacilityOperationRooms.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("FacilityOperationRoom");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "FacilityOperationRoom");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.FacilityOperationRoom.Param p = new GameTable.FacilityOperationRoom.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ParticipantCount = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ProductValueMin = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ProductValueMax = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.FacilityOperationRooms.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - FacilityTrade
				{
					data.FacilityTrades.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("FacilityTrade");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "FacilityTrade");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.FacilityTrade.Param p = new GameTable.FacilityTrade.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.MaterialGrade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.MaterialCount = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.SuccessProb = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.TradeGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.FacilityTrades.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - FacilityTradeAddon
				{
					data.FacilityTradeAddons.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("FacilityTradeAddon");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "FacilityTradeAddon");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.FacilityTradeAddon.Param p = new GameTable.FacilityTradeAddon.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.AddType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.AddFuncType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.AddFuncValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.AddItemID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.AddItemCount = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.UseGrade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.FacilityTradeAddons.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Gem
				{
					data.Gems.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Gem");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Gem");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Gem.Param p = new GameTable.Gem.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.MainType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.SubType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.SellPrice = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.HP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.ATK = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.DEF = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.CRI = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.IncHP = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(13);
							p.IncATK = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(14);
							p.IncDEF = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(15);
							p.IncCRI = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(16);
							p.LevelUpGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.Exp = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.WakeReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.RandOptGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.OptResetReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.EvReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.EvolutionResult = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.AcquisitionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Gems.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - GemSetType
				{
					data.GemSetTypes.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("GemSetType");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "GemSetType");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.GemSetType.Param p = new GameTable.GemSetType.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);

							data.GemSetTypes.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - GemRandOpt
				{
					data.GemRandOpts.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("GemRandOpt");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "GemRandOpt");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.GemRandOpt.Param p = new GameTable.GemRandOpt.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.EffectType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Min = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Max = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.Value = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(8);
							p.RndStep = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.GemRandOpts.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - InfluenceInfo
				{
					data.InfluenceInfos.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("InfluenceInfo");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "InfluenceInfo");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.InfluenceInfo.Param p = new GameTable.InfluenceInfo.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.InfluenceInfos.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - InfluenceMission
				{
					data.InfluenceMissions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("InfluenceMission");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "InfluenceMission");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.InfluenceMission.Param p = new GameTable.InfluenceMission.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.MissionType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.MissionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.InfluenceMissions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - InfluenceMissionSet
				{
					data.InfluenceMissionSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("InfluenceMissionSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "InfluenceMissionSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.InfluenceMissionSet.Param p = new GameTable.InfluenceMissionSet.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.RewardTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.EventItemID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.InfluenceMissionSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - InfluenceRank
				{
					data.InfluenceRanks.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("InfluenceRank");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "InfluenceRank");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.InfluenceRank.Param p = new GameTable.InfluenceRank.Param ();


							cell = row.GetCell(0);
							p.RewardOrder = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RewardType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.InfluenceRanks.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Item
				{
					data.Items.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Item");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Item");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Item.Param p = new GameTable.Item.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.SubType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.SellPrice = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.Value = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.AcquisitionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.CashExchange = (float)(cell == null ? 0 : cell.NumericCellValue);

							data.Items.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ItemReqList
				{
					data.ItemReqLists.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ItemReqList");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ItemReqList");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.ItemReqList.Param p = new GameTable.ItemReqList.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Level = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ItemID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Count1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ItemID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Count2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ItemID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.Count3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ItemID4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.Count4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.Gold = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.GoodsValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.LimitLevel = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ItemReqLists.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LevelUp
				{
					data.LevelUps.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LevelUp");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LevelUp");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.LevelUp.Param p = new GameTable.LevelUp.Param ();


							cell = row.GetCell(0);
							p.Group = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Level = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Exp = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Value1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LevelUps.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LobbyAnimation
				{
					data.LobbyAnimations.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LobbyAnimation");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LobbyAnimation");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.LobbyAnimation.Param p = new GameTable.LobbyAnimation.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Character = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.Animation = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.Face = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.LockType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LobbyAnimations.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LobbyTheme
				{
					data.LobbyThemes.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LobbyTheme");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LobbyTheme");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.LobbyTheme.Param p = new GameTable.LobbyTheme.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Prefab = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Bgm = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.FreeSelect = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LobbyThemes.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LoginBonus
				{
					data.LoginBonuss.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LoginBonus");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LoginBonus");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.LoginBonus.Param p = new GameTable.LoginBonus.Param ();


							cell = row.GetCell(0);
							p.LoginGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.LoginCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.SendMailTypeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.BGimg = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.NextGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LoginBonuss.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LoginBonusMonthly
				{
					data.LoginBonusMonthlys.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LoginBonusMonthly");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LoginBonusMonthly");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.LoginBonusMonthly.Param p = new GameTable.LoginBonusMonthly.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.DayCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.SendMailTypeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LoginBonusMonthlys.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - LoginEvent
				{
					data.LoginEvents.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("LoginEvent");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "LoginEvent");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.LoginEvent.Param p = new GameTable.LoginEvent.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.EventTarget = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.SendMailTypeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.AbsentReward = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.LoginEvents.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - MonthlyFee
				{
					data.MonthlyFees.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("MonthlyFee");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "MonthlyFee");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.MonthlyFee.Param p = new GameTable.MonthlyFee.Param ();


							cell = row.GetCell(0);
							p.MonthlyFeeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RewardID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.D_RewardID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.SendMailTypeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.MonthlyFees.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - NPCUserData
				{
					data.NPCUserDatas.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("NPCUserData");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "NPCUserData");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.NPCUserData.Param p = new GameTable.NPCUserData.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.MarkID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.UserRank = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.NickName = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Score = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Tier = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.TeamPower = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.NPCUserDatas.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - PassSet
				{
					data.PassSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("PassSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "PassSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.PassSet.Param p = new GameTable.PassSet.Param ();


							cell = row.GetCell(0);
							p.PassID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StartTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.EndTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.N_RewardID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.SendMailTypeID_N = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.S_RewardID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.SendMailTypeID_S = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.PassStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.PassSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - PassMission
				{
					data.PassMissions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("PassMission");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "PassMission");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.PassMission.Param p = new GameTable.PassMission.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.PassID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ActiveTime = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.MissionType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.MissionIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.MissionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.RewardPoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.PassMissions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RaidStore
				{
					data.RaidStores.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RaidStore");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RaidStore");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RaidStore.Param p = new GameTable.RaidStore.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.StoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Prob = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RaidStores.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Random
				{
					data.Randoms.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Random");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Random");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Random.Param p = new GameTable.Random.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Prob = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.Value = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Randoms.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RankUPReward
				{
					data.RankUPRewards.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RankUPReward");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RankUPReward");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RankUPReward.Param p = new GameTable.RankUPReward.Param ();


							cell = row.GetCell(0);
							p.RewardRank = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.SendMailTypeID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.MessageString = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RankUPRewards.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RoomAction
				{
					data.RoomActions.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RoomAction");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RoomAction");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RoomAction.Param p = new GameTable.RoomAction.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.CharacterID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Action = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Weight = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(6);
							p.Action2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.Weight2 = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(8);
							p.StoreRoomID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RoomActions.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RoomFigure
				{
					data.RoomFigures.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RoomFigure");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RoomFigure");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RoomFigure.Param p = new GameTable.RoomFigure.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.IconBuy = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Model = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.Platform = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ContentsType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ContentsIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.CharacterID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.StoreRoomID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RoomFigures.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RoomFunc
				{
					data.RoomFuncs.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RoomFunc");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RoomFunc");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RoomFunc.Param p = new GameTable.RoomFunc.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.RoomTheme = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Function = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.StoreRoomID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RoomFuncs.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RoomTheme
				{
					data.RoomThemes.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RoomTheme");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RoomTheme");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RoomTheme.Param p = new GameTable.RoomTheme.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.Scene = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Bgm = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.MaxChar = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.InitFunc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.PreVisible = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.StoreRoomID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RoomThemes.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - RotationGacha
				{
					data.RotationGachas.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("RotationGacha");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "RotationGacha");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.RotationGacha.Param p = new GameTable.RotationGacha.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Banner = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.Image = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.StoreID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.StoreID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.StoreID3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.StoreID4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.OpenCash = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.RotationGachas.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - ServerMergeSet
				{
					data.ServerMergeSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("ServerMergeSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "ServerMergeSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.ServerMergeSet.Param p = new GameTable.ServerMergeSet.Param ();


							cell = row.GetCell(0);
							p.MaxCount = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.ServerMergeSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - SecretQuestBOSet
				{
					data.SecretQuestBOSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("SecretQuestBOSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "SecretQuestBOSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.SecretQuestBOSet.Param p = new GameTable.SecretQuestBOSet.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.StageBOSet = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.SecretQuestBOSets.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - SecretQuestLevel
				{
					data.SecretQuestLevels.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("SecretQuestLevel");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "SecretQuestLevel");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.SecretQuestLevel.Param p = new GameTable.SecretQuestLevel.Param ();


							cell = row.GetCell(0);
							p.GroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.No = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Scene = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.LevelPB = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.ClearCondition = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.MissionDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ConditionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.BGM = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.AmbienceSound = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.LoadingTip = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.Monster1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.Monster2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.Monster3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.Monster4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.Monster5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.SecretQuestLevels.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Stage
				{
					data.Stages.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Stage");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Stage");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Stage.Param p = new GameTable.Stage.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.StageType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.TypeValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.Difficulty = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.Chapter = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.Section = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.LimitStage = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.NextStage = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.Ticket = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.TicketMultiple = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.Scene = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(14);
							p.LevelPB = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(15);
							p.ClearCondition = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.MissionDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.ConditionValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.RewardValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.RewardGold = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.RewardEXP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.RewardCardFavor = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.RewardCharEXP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(23);
							p.RewardSkillPoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(24);
							p.N_DropID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.N_DropMinCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(26);
							p.N_DropMaxCnt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(27);
							p.Luck_DropID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(28);
							p.Condi_Type = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(29);
							p.Condi_Value = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(30);
							p.Condi_DropID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(31);
							p.Mission_00 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(32);
							p.Mission_01 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(33);
							p.Mission_02 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(34);
							p.BGM = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(35);
							p.AmbienceSound = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(36);
							p.LoadingTip = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(37);
							p.Monster1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(38);
							p.Monster2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(39);
							p.Monster3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(40);
							p.Monster4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(41);
							p.Monster5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(42);
							p.StageBOSet = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(43);
							p.ScenarioID_BeforeStart = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(44);
							p.ScenarioDrt_Start = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(45);
							p.ShowPlayerStartDrt = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(46);
							p.ScenarioID_AfterStart = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(47);
							p.ScenarioID_BeforeBossAppear = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(48);
							p.ScenarioDrt_BossAppear = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(49);
							p.ScenarioDrt_BossDie = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(50);
							p.ScenarioDrt_EndMission = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(51);
							p.ScenarioID_EndMission = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(52);
							p.ScenarioDrt_AfterEndMission = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(53);
							p.ContinueType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(54);
							p.ContinueCost = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(55);
							p.UseFastQuestTicket = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(56);
							p.PlayerMode = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Stages.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Store
				{
					data.Stores.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Store");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Store");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Store.Param p = new GameTable.Store.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Etc = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.SaleType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.BuyNotStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.NeedBuyStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.BuyType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.PurchaseType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.PurchaseIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.PurchaseValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.BonusGoodsType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.BonusGoodsValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.Value1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.Value2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.Value3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.AOS_ID = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(18);
							p.IOS_ID = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(19);
							p.ConnectStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.NeedDesirePoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.GetableDP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.SpecialType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Stores.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - StoreRoom
				{
					data.StoreRooms.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("StoreRoom");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "StoreRoom");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.StoreRoom.Param p = new GameTable.StoreRoom.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Etc = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.ProductType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.ProductIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.ProductValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.PurchaseType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.PurchaseValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.NeedRoomPoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.StoreRooms.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - UserMark
				{
					data.UserMarks.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("UserMark");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "UserMark");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.UserMark.Param p = new GameTable.UserMark.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.GetDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.ConType = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.ConIndex = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(7);
							p.ConValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(8);
							p.OrderNum = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.PreVisible = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.AniEnable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.UserMarks.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - UnexpectedPackage
				{
					data.UnexpectedPackages.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("UnexpectedPackage");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "UnexpectedPackage");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.UnexpectedPackage.Param p = new GameTable.UnexpectedPackage.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.UnexpectedType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.RepeatValue = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Value1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(4);
							p.Value2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(5);
							p.ConnectStoreID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(6);
							p.PackageBuyTime = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.UnexpectedPackages.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - Weapon
				{
					data.Weapons.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("Weapon");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "Weapon");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.Weapon.Param p = new GameTable.Weapon.Param ();


							cell = row.GetCell(0);
							p.ID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.Name = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(2);
							p.Desc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(3);
							p.Icon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.ModelR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.ModelL = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.SubModelR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.SubModelL = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.Sub2ModelR = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(9);
							p.Sub2ModelL = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(10);
							p.AddedUnitWeapon = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(11);
							p.CharacterID = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(12);
							p.CharacterBaseID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.WpnDepotSetFlag = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(14);
							p.Grade = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.BaseStatSet = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(16);
							p.SellPrice = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(17);
							p.SellMPoint = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(18);
							p.LevelUpGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(19);
							p.Exp = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(20);
							p.ATK = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(21);
							p.CRI = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(22);
							p.IncATK = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(23);
							p.IncCRI = (float)(cell == null ? 0 : cell.NumericCellValue);
							cell = row.GetCell(24);
							p.SkillEffectName = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(25);
							p.SkillEffectDesc = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(26);
							p.WpnBOWorkType = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(27);
							p.WpnBOWorkTypeValue = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(28);
							p.WpnBOActivate = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(29);
							p.WpnAddBOSetID1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(30);
							p.WpnAddBOSetID2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(31);
							p.UseSP = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(32);
							p.WakeReqGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(33);
							p.EnchantGroup = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(34);
							p.Decomposition = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(35);
							p.AcquisitionID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(36);
							p.Tradeable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(37);
							p.Selectable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(38);
							p.Decomposable = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(39);
							p.PreVisible = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.Weapons.Add(p);

						}
					}
				}

				/////////////////////////////////////////////
				// Import Sheet - WeeklyMissionSet
				{
					data.WeeklyMissionSets.Clear();	// clear sheet

					ISheet sheet = book.GetSheet("WeeklyMissionSet");
					if(sheet == null)
					{
						Debug.LogError("ExcelImporter Error: sheet not found," + "WeeklyMissionSet");
					}
					else
					{
						for (int i=1; i<= sheet.LastRowNum; i++)
						{
							IRow row = sheet.GetRow (i);
							ICell cell = null;
						
							GameTable.WeeklyMissionSet.Param p = new GameTable.WeeklyMissionSet.Param ();


							cell = row.GetCell(0);
							p.SetID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(1);
							p.WMCon0 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(2);
							p.WMCon1 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(3);
							p.WMCon2 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(4);
							p.WMCon3 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(5);
							p.WMCon4 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(6);
							p.WMCon5 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(7);
							p.WMCon6 = (cell == null ? "" : cell.StringCellValue);
							cell = row.GetCell(8);
							p.WMCnt0 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(9);
							p.WMCnt1 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(10);
							p.WMCnt2 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(11);
							p.WMCnt3 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(12);
							p.WMCnt4 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(13);
							p.WMCnt5 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(14);
							p.WMCnt6 = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());
							cell = row.GetCell(15);
							p.RewardGroupID = int.Parse((cell == null ? 0 : cell.NumericCellValue).ToString());

							data.WeeklyMissionSets.Add(p);

						}
					}
				}

			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
