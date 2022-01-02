using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x0200013A RID: 314
	public static class BlocksTexturesManager
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00021B05 File Offset: 0x0001FD05
		// (set) Token: 0x060005E9 RID: 1513 RVA: 0x00021B0C File Offset: 0x0001FD0C
		public static Texture2D DefaultBlocksTexture { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060005EA RID: 1514 RVA: 0x00021B14 File Offset: 0x0001FD14
		public static ReadOnlyList<string> BlockTexturesNames
		{
			get
			{
				return new ReadOnlyList<string>(BlocksTexturesManager.m_blockTextureNames);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00021B20 File Offset: 0x0001FD20
		public static string BlockTexturesDirectoryName
		{
			get
			{
				return "app:/TexturePacks";
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060005EC RID: 1516 RVA: 0x00021B28 File Offset: 0x0001FD28
		// (remove) Token: 0x060005ED RID: 1517 RVA: 0x00021B5C File Offset: 0x0001FD5C
		public static event Action<string> BlocksTextureDeleted;

		// Token: 0x060005EE RID: 1518 RVA: 0x00021B8F File Offset: 0x0001FD8F
		public static void Initialize()
		{
			Storage.CreateDirectory(BlocksTexturesManager.BlockTexturesDirectoryName);
			BlocksTexturesManager.DefaultBlocksTexture = ContentManager.Get<Texture2D>("Textures/Blocks", null);
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x00021BAB File Offset: 0x0001FDAB
		public static bool IsBuiltIn(string name)
		{
			return string.IsNullOrEmpty(name);
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00021BB3 File Offset: 0x0001FDB3
		public static string GetFileName(string name)
		{
			if (BlocksTexturesManager.IsBuiltIn(name))
			{
				return null;
			}
			return Storage.CombinePaths(new string[]
			{
				BlocksTexturesManager.BlockTexturesDirectoryName,
				name
			});
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00021BD6 File Offset: 0x0001FDD6
		public static string GetDisplayName(string name)
		{
			if (BlocksTexturesManager.IsBuiltIn(name))
			{
				return "Survivalcraft";
			}
			return Storage.GetFileNameWithoutExtension(name);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00021BEC File Offset: 0x0001FDEC
		public static DateTime GetCreationDate(string name)
		{
			try
			{
				if (!BlocksTexturesManager.IsBuiltIn(name))
				{
					return Storage.GetFileLastWriteTime(BlocksTexturesManager.GetFileName(name));
				}
			}
			catch
			{
			}
			return new DateTime(2000, 1, 1);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00021C34 File Offset: 0x0001FE34
		public static Texture2D LoadTexture(string name)
		{
			Texture2D texture2D = null;
			if (!BlocksTexturesManager.IsBuiltIn(name))
			{
				try
				{
					using (Stream stream = Storage.OpenFile(BlocksTexturesManager.GetFileName(name), OpenFileMode.Read))
					{
						BlocksTexturesManager.ValidateBlocksTexture(stream);
						stream.Position = 0L;
						texture2D = Texture2D.Load(stream, false, 1);
					}
				}
				catch (Exception ex)
				{
					Log.Warning(string.Concat(new string[]
					{
						"Could not load blocks texture \"",
						name,
						"\". Reason: ",
						ex.Message,
						"."
					}));
				}
			}
			if (texture2D == null)
			{
				texture2D = BlocksTexturesManager.DefaultBlocksTexture;
			}
			return texture2D;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x00021CDC File Offset: 0x0001FEDC
		public static string ImportBlocksTexture(string name, Stream stream)
		{
			Exception ex = ExternalContentManager.VerifyExternalContentName(name);
			if (ex != null)
			{
				throw ex;
			}
			if (Storage.GetExtension(name) != ".scbtex")
			{
				name += ".scbtex";
			}
			BlocksTexturesManager.ValidateBlocksTexture(stream);
			stream.Position = 0L;
			string result;
			using (Stream stream2 = Storage.OpenFile(BlocksTexturesManager.GetFileName(name), OpenFileMode.Create))
			{
				stream.CopyTo(stream2);
				result = name;
			}
			return result;
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x00021D58 File Offset: 0x0001FF58
		public static void DeleteBlocksTexture(string name)
		{
			try
			{
				string fileName = BlocksTexturesManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					Storage.DeleteFile(fileName);
					Action<string> blocksTextureDeleted = BlocksTexturesManager.BlocksTextureDeleted;
					if (blocksTextureDeleted != null)
					{
						blocksTextureDeleted(name);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Unable to delete blocks texture \"" + name + "\"", e);
			}
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00021DB8 File Offset: 0x0001FFB8
		public static void UpdateBlocksTexturesList()
		{
			BlocksTexturesManager.m_blockTextureNames.Clear();
			BlocksTexturesManager.m_blockTextureNames.Add(string.Empty);
			foreach (string item in Storage.ListFileNames(BlocksTexturesManager.BlockTexturesDirectoryName))
			{
				BlocksTexturesManager.m_blockTextureNames.Add(item);
			}
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00021E28 File Offset: 0x00020028
		public static void ValidateBlocksTexture(Stream stream)
		{
			Image image = Image.Load(stream);
			if (image.Width > 65535 || image.Height > 65535)
			{
				throw new InvalidOperationException(string.Format("Blocks texture is larger than 65535x65535 pixels (size={0}x{1})", image.Width, image.Height));
			}
			if (!MathUtils.IsPowerOf2((long)image.Width) || !MathUtils.IsPowerOf2((long)image.Height))
			{
				throw new InvalidOperationException(string.Format("Blocks texture does not have power-of-two size (size={0}x{1})", image.Width, image.Height));
			}
		}

		// Token: 0x040002A6 RID: 678
		public static List<string> m_blockTextureNames = new List<string>();
	}
}
