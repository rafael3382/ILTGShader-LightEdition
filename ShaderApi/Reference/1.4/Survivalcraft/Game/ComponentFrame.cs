using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200020A RID: 522
	public class ComponentFrame : Component
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000F51 RID: 3921 RVA: 0x000720DE File Offset: 0x000702DE
		// (set) Token: 0x06000F52 RID: 3922 RVA: 0x000720E6 File Offset: 0x000702E6
		public Vector3 Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				if (value != this.m_position)
				{
					this.m_cachedMatrixValid = false;
					this.m_position = value;
					Action<ComponentFrame> positionChanged = this.PositionChanged;
					if (positionChanged == null)
					{
						return;
					}
					positionChanged(this);
				}
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000F53 RID: 3923 RVA: 0x00072115 File Offset: 0x00070315
		// (set) Token: 0x06000F54 RID: 3924 RVA: 0x0007211D File Offset: 0x0007031D
		public Quaternion Rotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				value = Quaternion.Normalize(value);
				if (value != this.m_rotation)
				{
					this.m_cachedMatrixValid = false;
					this.m_rotation = value;
					Action<ComponentFrame> rotationChanged = this.RotationChanged;
					if (rotationChanged == null)
					{
						return;
					}
					rotationChanged(this);
				}
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000F55 RID: 3925 RVA: 0x00072154 File Offset: 0x00070354
		public Matrix Matrix
		{
			get
			{
				if (!this.m_cachedMatrixValid)
				{
					this.m_cachedMatrix = Matrix.CreateFromQuaternion(this.Rotation);
					this.m_cachedMatrix.Translation = this.Position;
				}
				return this.m_cachedMatrix;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000F56 RID: 3926 RVA: 0x00072186 File Offset: 0x00070386
		// (set) Token: 0x06000F57 RID: 3927 RVA: 0x0007218E File Offset: 0x0007038E
		public virtual Action<ComponentFrame> PositionChanged { get; set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000F58 RID: 3928 RVA: 0x00072197 File Offset: 0x00070397
		// (set) Token: 0x06000F59 RID: 3929 RVA: 0x0007219F File Offset: 0x0007039F
		public virtual Action<ComponentFrame> RotationChanged { get; set; }

		// Token: 0x06000F5A RID: 3930 RVA: 0x000721A8 File Offset: 0x000703A8
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.Position = valuesDictionary.GetValue<Vector3>("Position");
			this.Rotation = valuesDictionary.GetValue<Quaternion>("Rotation");
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x000721CC File Offset: 0x000703CC
		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<Vector3>("Position", this.Position);
			valuesDictionary.SetValue<Quaternion>("Rotation", this.Rotation);
		}

		// Token: 0x040008D2 RID: 2258
		public Vector3 m_position;

		// Token: 0x040008D3 RID: 2259
		public Quaternion m_rotation;

		// Token: 0x040008D4 RID: 2260
		public bool m_cachedMatrixValid;

		// Token: 0x040008D5 RID: 2261
		public Matrix m_cachedMatrix;
	}
}
