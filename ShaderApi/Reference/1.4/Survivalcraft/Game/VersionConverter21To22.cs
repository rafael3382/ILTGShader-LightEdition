using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000356 RID: 854
	public class VersionConverter21To22 : VersionConverter
	{
		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06001905 RID: 6405 RVA: 0x000C385C File Offset: 0x000C1A5C
		public override string SourceVersion
		{
			get
			{
				return "2.1";
			}
		}

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001906 RID: 6406 RVA: 0x000C3863 File Offset: 0x000C1A63
		public override string TargetVersion
		{
			get
			{
				return "2.2";
			}
		}

		// Token: 0x06001907 RID: 6407 RVA: 0x000C386C File Offset: 0x000C1A6C
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			string empty = string.Empty;
			foreach (XElement xelement in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "GameInfo"
			select e)
			{
				foreach (XElement node in xelement.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "TerrainGenerationMode"))
				{
					if (XmlUtils.GetAttributeValue<string>(node, "Value", "") == "Flat")
					{
						XmlUtils.SetAttributeValue(node, "Value", "FlatContinent");
					}
				}
			}
			foreach (XElement xelement2 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Pickables"
			select e)
			{
				foreach (XElement xelement3 in xelement2.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Pickables"))
				{
					foreach (XElement xelement4 in xelement3.Elements("Values"))
					{
						foreach (XElement node2 in xelement4.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node2, "Value"));
							XmlUtils.SetAttributeValue(node2, "Value", num);
						}
					}
				}
			}
			foreach (XElement xelement5 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Projectiles"
			select e)
			{
				foreach (XElement xelement6 in xelement5.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Projectiles"))
				{
					foreach (XElement xelement7 in xelement6.Elements("Values"))
					{
						foreach (XElement node3 in xelement7.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num2 = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node3, "Value"));
							XmlUtils.SetAttributeValue(node3, "Value", num2);
						}
					}
				}
			}
			foreach (XElement xelement8 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CollapsingBlockBehavior"
			select e)
			{
				foreach (XElement xelement9 in xelement8.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CollapsingBlocks"))
				{
					foreach (XElement xelement10 in xelement9.Elements("Values"))
					{
						foreach (XElement node4 in xelement10.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num3 = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node4, "Value"));
							XmlUtils.SetAttributeValue(node4, "Value", num3);
						}
					}
				}
			}
			foreach (XElement xelement11 in projectNode.Element("Entities").Elements())
			{
				foreach (XElement xelement12 in from e in xelement11.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Clothing"
				select e)
				{
					foreach (XElement xelement13 in xelement12.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Clothes"))
					{
						foreach (XElement node5 in xelement13.Elements())
						{
							string attributeValue = XmlUtils.GetAttributeValue<string>(node5, "Value");
							int[] array = HumanReadableConverter.ValuesListFromString<int>(';', attributeValue);
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = VersionConverter21To22.ConvertValue(array[i]);
							}
							string value = HumanReadableConverter.ValuesListToString<int>(';', array);
							XmlUtils.SetAttributeValue(node5, "Value", value);
						}
					}
				}
			}
			string[] inventoryNames = new string[]
			{
				"Inventory",
				"CreativeInventory",
				"CraftingTable",
				"Chest",
				"Furnace",
				"Dispenser"
			};
			Func<XElement, bool> <>9__13;
			foreach (XElement xelement14 in projectNode.Element("Entities").Elements())
			{
				IEnumerable<XElement> source = xelement14.Elements("Values");
				Func<XElement, bool> predicate;
				if ((predicate = <>9__13) == null)
				{
					predicate = (<>9__13 = ((XElement e) => inventoryNames.Contains(XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty))));
				}
				foreach (XElement xelement15 in source.Where(predicate))
				{
					foreach (XElement xelement16 in from e in xelement15.Elements("Values")
					where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Slots"
					select e)
					{
						foreach (XElement xelement17 in xelement16.Elements())
						{
							foreach (XElement node6 in xelement17.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Contents"))
							{
								int num4 = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node6, "Value"));
								XmlUtils.SetAttributeValue(node6, "Value", num4);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001908 RID: 6408 RVA: 0x000C4364 File Offset: 0x000C2564
		public override void ConvertWorld(string directoryName)
		{
			try
			{
				this.ConvertChunks(directoryName);
				this.ConvertProject(directoryName);
				foreach (string text in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".new"
				select f)
				{
					string sourcePath = Storage.CombinePaths(new string[]
					{
						directoryName,
						text
					});
					string destinationPath = Storage.CombinePaths(new string[]
					{
						directoryName,
						Storage.GetFileNameWithoutExtension(text)
					});
					Storage.MoveFile(sourcePath, destinationPath);
				}
				foreach (string text2 in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".old"
				select f)
				{
					Storage.DeleteFile(Storage.CombinePaths(new string[]
					{
						directoryName,
						text2
					}));
				}
			}
			catch (Exception ex)
			{
				foreach (string text3 in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".old"
				select f)
				{
					string sourcePath2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						text3
					});
					string destinationPath2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						Storage.GetFileNameWithoutExtension(text3)
					});
					Storage.MoveFile(sourcePath2, destinationPath2);
				}
				foreach (string text4 in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".new"
				select f)
				{
					Storage.DeleteFile(Storage.CombinePaths(new string[]
					{
						directoryName,
						text4
					}));
				}
				throw ex;
			}
		}

		// Token: 0x06001909 RID: 6409 RVA: 0x000C45A0 File Offset: 0x000C27A0
		public void ConvertProject(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			string path2 = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml.new"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path2, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}

		// Token: 0x0600190A RID: 6410 RVA: 0x000C4640 File Offset: 0x000C2840
		public void ConvertChunks(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			});
			string path2 = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32h.dat.new"
			});
			long num = 2L * Storage.GetFileSize(path) + 52428800L;
			if (Storage.FreeSpace < num)
			{
				throw new InvalidOperationException(string.Format("Not enough free space to convert world. {0}MB required.", num / 1024L / 1024L));
			}
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				using (Stream stream2 = Storage.OpenFile(path2, OpenFileMode.Create))
				{
					byte[] array = new byte[131072];
					byte[] array2 = new byte[262144];
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer22.WriteTOCEntry(stream2, 0, 0, -1);
					}
					int num2 = 0;
					for (;;)
					{
						stream.Position = (long)(12 * num2);
						int cx;
						int cz;
						int num3;
						TerrainSerializer129.ReadTOCEntry(stream, out cx, out cz, out num3);
						if (num3 < 0)
						{
							break;
						}
						stream2.Position = (long)(12 * num2);
						TerrainSerializer22.WriteTOCEntry(stream2, cx, cz, num2);
						stream.Position = 786444L + 132112L * (long)num3;
						stream2.Position = stream2.Length;
						TerrainSerializer129.ReadChunkHeader(stream);
						TerrainSerializer22.WriteChunkHeader(stream2, cx, cz);
						stream.Read(array, 0, 131072);
						int num4 = 0;
						int num5 = 0;
						for (int j = 0; j < 16; j++)
						{
							for (int k = 0; k < 16; k++)
							{
								for (int l = 0; l < 256; l++)
								{
									int num6;
									if (l <= 127)
									{
										num6 = VersionConverter21To22.ConvertValue((int)array[4 * num4] | (int)array[4 * num4 + 1] << 8 | (int)array[4 * num4 + 2] << 16 | (int)array[4 * num4 + 3] << 24);
										num4++;
									}
									else
									{
										num6 = 0;
									}
									array2[4 * num5] = (byte)num6;
									array2[4 * num5 + 1] = (byte)(num6 >> 8);
									array2[4 * num5 + 2] = (byte)(num6 >> 16);
									array2[4 * num5 + 3] = (byte)(num6 >> 24);
									num5++;
								}
							}
						}
						stream2.Write(array2, 0, 262144);
						stream.Read(array, 0, 1024);
						stream2.Write(array, 0, 1024);
						num2++;
					}
				}
			}
			Storage.MoveFile(Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			}), Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat.old"
			}));
		}

		// Token: 0x0600190B RID: 6411 RVA: 0x000C490C File Offset: 0x000C2B0C
		public static int ConvertValue(int value)
		{
			int num = value & 1023;
			int num2 = value >> 10 & 15;
			int num3 = value >> 14;
			VersionConverter21To22.ConvertContentsLightData(ref num, ref num2, ref num3);
			return num | num2 << 10 | num3 << 14;
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x000C4944 File Offset: 0x000C2B44
		public static void ConvertContentsLightData(ref int contents, ref int light, ref int data)
		{
			if (contents == 30)
			{
				contents = 29;
			}
			if (contents == 34)
			{
				contents = 29;
			}
			if (contents == 32)
			{
				contents = 29;
			}
			if (contents == 35)
			{
				contents = 29;
			}
			if (contents == 33)
			{
				contents = 29;
			}
			if (contents == 170)
			{
				contents = 169;
			}
			if (contents == 122)
			{
				contents = 122;
			}
			if (contents == 123)
			{
				contents = 123;
			}
		}
	}
}
