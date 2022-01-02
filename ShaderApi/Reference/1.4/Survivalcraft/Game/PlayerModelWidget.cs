using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000399 RID: 921
	public class PlayerModelWidget : CanvasWidget
	{
		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x000D87A5 File Offset: 0x000D69A5
		// (set) Token: 0x06001BB1 RID: 7089 RVA: 0x000D87AD File Offset: 0x000D69AD
		public CharacterSkinsCache CharacterSkinsCache
		{
			get
			{
				return this.m_characterSkinsCache;
			}
			set
			{
				if (value != null)
				{
					this.m_publicCharacterSkinsCache.Clear();
					this.m_characterSkinsCache = value;
					return;
				}
				this.m_characterSkinsCache = this.m_publicCharacterSkinsCache;
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06001BB2 RID: 7090 RVA: 0x000D87D1 File Offset: 0x000D69D1
		// (set) Token: 0x06001BB3 RID: 7091 RVA: 0x000D87D9 File Offset: 0x000D69D9
		public PlayerModelWidget.Shot CameraShot { get; set; }

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001BB4 RID: 7092 RVA: 0x000D87E2 File Offset: 0x000D69E2
		// (set) Token: 0x06001BB5 RID: 7093 RVA: 0x000D87EA File Offset: 0x000D69EA
		public int AnimateHeadSeed { get; set; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001BB6 RID: 7094 RVA: 0x000D87F3 File Offset: 0x000D69F3
		// (set) Token: 0x06001BB7 RID: 7095 RVA: 0x000D87FB File Offset: 0x000D69FB
		public int AnimateHandsSeed { get; set; }

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001BB8 RID: 7096 RVA: 0x000D8804 File Offset: 0x000D6A04
		// (set) Token: 0x06001BB9 RID: 7097 RVA: 0x000D880C File Offset: 0x000D6A0C
		public bool OuterClothing { get; set; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001BBA RID: 7098 RVA: 0x000D8815 File Offset: 0x000D6A15
		// (set) Token: 0x06001BBB RID: 7099 RVA: 0x000D881D File Offset: 0x000D6A1D
		public PlayerClass PlayerClass { get; set; }

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06001BBC RID: 7100 RVA: 0x000D8826 File Offset: 0x000D6A26
		// (set) Token: 0x06001BBD RID: 7101 RVA: 0x000D882E File Offset: 0x000D6A2E
		public string CharacterSkinName { get; set; }

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06001BBE RID: 7102 RVA: 0x000D8837 File Offset: 0x000D6A37
		// (set) Token: 0x06001BBF RID: 7103 RVA: 0x000D883F File Offset: 0x000D6A3F
		public Texture2D CharacterSkinTexture { get; set; }

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x000D8848 File Offset: 0x000D6A48
		// (set) Token: 0x06001BC1 RID: 7105 RVA: 0x000D8850 File Offset: 0x000D6A50
		public Texture2D OuterClothingTexture { get; set; }

		// Token: 0x06001BC2 RID: 7106 RVA: 0x000D885C File Offset: 0x000D6A5C
		public PlayerModelWidget()
		{
			this.m_modelWidget = new ModelWidget
			{
				UseAlphaThreshold = true,
				IsPerspective = true
			};
			this.Children.Add(this.m_modelWidget);
			this.IsHitTestVisible = false;
			this.m_publicCharacterSkinsCache = new CharacterSkinsCache();
			this.m_characterSkinsCache = this.m_publicCharacterSkinsCache;
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x000D88B8 File Offset: 0x000D6AB8
		public override void Update()
		{
			if (base.Input.Press != null)
			{
				if (this.m_lastDrag != null)
				{
					this.m_rotation += 0.01f * (base.Input.Press.Value.X - this.m_lastDrag.Value.X);
					this.m_lastDrag = new Vector2?(base.Input.Press.Value);
					base.Input.Clear();
				}
				else if (base.HitTestGlobal(base.Input.Press.Value, null) == this)
				{
					this.m_lastDrag = new Vector2?(base.Input.Press.Value);
				}
			}
			else
			{
				this.m_lastDrag = null;
				this.m_rotation = MathUtils.NormalizeAngle(this.m_rotation);
				if (MathUtils.Abs(this.m_rotation) > 0.01f)
				{
					this.m_rotation *= MathUtils.PowSign(0.1f, Time.FrameDuration);
				}
				else
				{
					this.m_rotation = 0f;
				}
			}
			this.m_modelWidget.ModelMatrix = ((this.m_rotation != 0f) ? Matrix.CreateRotationY(this.m_rotation) : Matrix.Identity);
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x000D8A14 File Offset: 0x000D6C14
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			this.m_modelWidget.Model = (this.OuterClothing ? CharacterSkinsManager.GetOuterClothingModel(this.PlayerClass) : CharacterSkinsManager.GetPlayerModel(this.PlayerClass));
			if (this.CameraShot == PlayerModelWidget.Shot.Body)
			{
				this.m_modelWidget.ViewPosition = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 1.46f, -3.2f) : new Vector3(0f, 1.39f, -3.04f));
				this.m_modelWidget.ViewTarget = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 0.9f, 0f) : new Vector3(0f, 0.86f, 0f));
				this.m_modelWidget.ViewFov = 0.57f;
			}
			else
			{
				if (this.CameraShot != PlayerModelWidget.Shot.Bust)
				{
					throw new InvalidOperationException("Unknown shot.");
				}
				this.m_modelWidget.ViewPosition = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 1.5f, -1.05f) : new Vector3(0f, 1.43f, -1f));
				this.m_modelWidget.ViewTarget = ((this.PlayerClass == PlayerClass.Male) ? new Vector3(0f, 1.5f, 0f) : new Vector3(0f, 1.43f, 0f));
				this.m_modelWidget.ViewFov = 0.57f;
			}
			this.m_modelWidget.TextureOverride = (this.OuterClothing ? this.OuterClothingTexture : ((this.CharacterSkinName != null) ? this.CharacterSkinsCache.GetTexture(this.CharacterSkinName) : this.CharacterSkinTexture));
			if (this.AnimateHeadSeed != 0)
			{
				int num = (this.AnimateHeadSeed < 0) ? this.GetHashCode() : this.AnimateHeadSeed;
				float num2 = (float)MathUtils.Remainder(Time.FrameStartTime + 1000.0 * (double)num, 10000.0);
				Vector2 vector = default(Vector2);
				vector.X = MathUtils.Lerp(-0.75f, 0.75f, SimplexNoise.OctavedNoise(num2 + 100f, 0.2f, 1, 2f, 0.5f, false));
				vector.Y = MathUtils.Lerp(-0.5f, 0.5f, SimplexNoise.OctavedNoise(num2 + 200f, 0.17f, 1, 2f, 0.5f, false));
				Matrix value = Matrix.CreateRotationX(vector.Y) * Matrix.CreateRotationZ(vector.X);
				this.m_modelWidget.SetBoneTransform(this.m_modelWidget.Model.FindBone("Head", true).Index, new Matrix?(value));
			}
			if (!this.OuterClothing && this.AnimateHandsSeed != 0)
			{
				int num3 = (this.AnimateHandsSeed < 0) ? this.GetHashCode() : this.AnimateHandsSeed;
				float num4 = (float)MathUtils.Remainder(Time.FrameStartTime + 1000.0 * (double)num3, 10000.0);
				Vector2 vector2 = default(Vector2);
				vector2.X = MathUtils.Lerp(0.2f, 0f, SimplexNoise.OctavedNoise(num4 + 100f, 0.7f, 1, 2f, 0.5f, false));
				vector2.Y = MathUtils.Lerp(-0.3f, 0.3f, SimplexNoise.OctavedNoise(num4 + 200f, 0.7f, 1, 2f, 0.5f, false));
				Vector2 vector3 = default(Vector2);
				vector3.X = MathUtils.Lerp(-0.2f, 0f, SimplexNoise.OctavedNoise(num4 + 300f, 0.7f, 1, 2f, 0.5f, false));
				vector3.Y = MathUtils.Lerp(-0.3f, 0.3f, SimplexNoise.OctavedNoise(num4 + 400f, 0.7f, 1, 2f, 0.5f, false));
				Matrix value2 = Matrix.CreateRotationX(vector2.Y) * Matrix.CreateRotationY(vector2.X);
				Matrix value3 = Matrix.CreateRotationX(vector3.Y) * Matrix.CreateRotationY(vector3.X);
				this.m_modelWidget.SetBoneTransform(this.m_modelWidget.Model.FindBone("Hand1", true).Index, new Matrix?(value2));
				this.m_modelWidget.SetBoneTransform(this.m_modelWidget.Model.FindBone("Hand2", true).Index, new Matrix?(value3));
			}
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x000D8E8D File Offset: 0x000D708D
		public override void UpdateCeases()
		{
			if (base.RootWidget == null)
			{
				if (this.m_publicCharacterSkinsCache.ContainsTexture(this.m_modelWidget.TextureOverride))
				{
					this.m_modelWidget.TextureOverride = null;
				}
				this.m_publicCharacterSkinsCache.Clear();
			}
			base.UpdateCeases();
		}

		// Token: 0x040012C9 RID: 4809
		public ModelWidget m_modelWidget;

		// Token: 0x040012CA RID: 4810
		public CharacterSkinsCache m_publicCharacterSkinsCache;

		// Token: 0x040012CB RID: 4811
		public CharacterSkinsCache m_characterSkinsCache;

		// Token: 0x040012CC RID: 4812
		public Vector2? m_lastDrag;

		// Token: 0x040012CD RID: 4813
		public float m_rotation;

		// Token: 0x02000588 RID: 1416
		public enum Shot
		{
			// Token: 0x040019F1 RID: 6641
			Body,
			// Token: 0x040019F2 RID: 6642
			Bust
		}
	}
}
