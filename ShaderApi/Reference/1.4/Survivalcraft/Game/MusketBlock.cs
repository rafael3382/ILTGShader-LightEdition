using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000B8 RID: 184
	public class MusketBlock : Block
	{
		// Token: 0x06000393 RID: 915 RVA: 0x0001591C File Offset: 0x00013B1C
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Musket", null);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Musket", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Hammer", true).ParentBone);
			this.m_standaloneBlockMeshUnloaded = new BlockMesh();
			this.m_standaloneBlockMeshUnloaded.AppendModelMeshPart(model.FindMesh("Musket", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, Color.White);
			this.m_standaloneBlockMeshUnloaded.AppendModelMeshPart(model.FindMesh("Hammer", true).MeshParts[0], boneAbsoluteTransform2, false, false, false, false, Color.White);
			this.m_standaloneBlockMeshLoaded = new BlockMesh();
			this.m_standaloneBlockMeshLoaded.AppendModelMeshPart(model.FindMesh("Musket", true).MeshParts[0], boneAbsoluteTransform, false, false, false, false, Color.White);
			this.m_standaloneBlockMeshLoaded.AppendModelMeshPart(model.FindMesh("Hammer", true).MeshParts[0], Matrix.CreateRotationX(0.7f) * boneAbsoluteTransform2, false, false, false, false, Color.White);
			base.Initialize();
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00015A4A File Offset: 0x00013C4A
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00015A4C File Offset: 0x00013C4C
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if (MusketBlock.GetHammerState(Terrain.ExtractData(value)))
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshLoaded, color, 2f * size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshUnloaded, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00015A9C File Offset: 0x00013C9C
		public override bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			if (Terrain.ExtractContents(oldValue) != 212)
			{
				return true;
			}
			int data = Terrain.ExtractData(oldValue);
			return MusketBlock.SetHammerState(Terrain.ExtractData(newValue), true) != MusketBlock.SetHammerState(data, true);
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00015AD7 File Offset: 0x00013CD7
		public override int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 8 & 255;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00015AE8 File Offset: 0x00013CE8
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= -65281;
			num |= MathUtils.Clamp(damage, 0, 255) << 8;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00015B1C File Offset: 0x00013D1C
		public static MusketBlock.LoadState GetLoadState(int data)
		{
			return (MusketBlock.LoadState)(data & 3);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00015B21 File Offset: 0x00013D21
		public static int SetLoadState(int data, MusketBlock.LoadState loadState)
		{
			return (data & -4) | (int)(loadState & MusketBlock.LoadState.Loaded);
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00015B2B File Offset: 0x00013D2B
		public static bool GetHammerState(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00015B33 File Offset: 0x00013D33
		public static int SetHammerState(int data, bool state)
		{
			return (data & -5) | (state ? 1 : 0) << 2;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00015B44 File Offset: 0x00013D44
		public static BulletBlock.BulletType? GetBulletType(int data)
		{
			int num = data >> 4 & 15;
			if (num != 0)
			{
				return new BulletBlock.BulletType?((BulletBlock.BulletType)(num - 1));
			}
			return null;
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00015B70 File Offset: 0x00013D70
		public static int SetBulletType(int data, BulletBlock.BulletType? bulletType)
		{
			int num = (int)((bulletType != null) ? (bulletType.Value + 1) : BulletBlock.BulletType.MusketBall);
			return (data & -241) | (num & 15) << 4;
		}

		// Token: 0x040001A8 RID: 424
		public const int Index = 212;

		// Token: 0x040001A9 RID: 425
		public BlockMesh m_standaloneBlockMeshUnloaded;

		// Token: 0x040001AA RID: 426
		public BlockMesh m_standaloneBlockMeshLoaded;

		// Token: 0x020003FE RID: 1022
		public enum LoadState
		{
			// Token: 0x040014C5 RID: 5317
			Empty,
			// Token: 0x040014C6 RID: 5318
			Gunpowder,
			// Token: 0x040014C7 RID: 5319
			Wad,
			// Token: 0x040014C8 RID: 5320
			Loaded
		}
	}
}
