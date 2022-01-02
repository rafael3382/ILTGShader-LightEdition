using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000158 RID: 344
	public class ObjModelReader
	{
		// Token: 0x06000796 RID: 1942 RVA: 0x0002A910 File Offset: 0x00028B10
		static ObjModelReader()
		{
			ObjModelReader.FaceMap.Add(4, new List<int>
			{
				0,
				2,
				1
			});
			ObjModelReader.FaceMap.Add(5, new List<int>
			{
				0,
				2,
				1
			});
			ObjModelReader.FaceMap.Add(2, new List<int>
			{
				0,
				2,
				1
			});
			ObjModelReader.FaceMap.Add(3, new List<int>
			{
				0,
				2,
				1
			});
			ObjModelReader.FaceMap.Add(0, new List<int>
			{
				0,
				2,
				1
			});
			ObjModelReader.FaceMap.Add(1, new List<int>
			{
				0,
				2,
				1
			});
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x0002AA08 File Offset: 0x00028C08
		public static ObjModel Load(Stream stream)
		{
			Dictionary<string, ObjModelReader.ObjMesh> dictionary = new Dictionary<string, ObjModelReader.ObjMesh>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			List<ObjModelReader.ObjPosition> list = new List<ObjModelReader.ObjPosition>();
			List<ObjModelReader.ObjTexCood> list2 = new List<ObjModelReader.ObjTexCood>();
			List<ObjModelReader.ObjNormal> list3 = new List<ObjModelReader.ObjNormal>();
			try
			{
				StreamReader streamReader = new StreamReader(stream);
				ObjModelReader.ObjMesh objMesh = null;
				string text = null;
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().Split(new char[]
					{
						'\t',
						' '
					}, StringSplitOptions.None);
					string text2 = array[0];
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
					if (num <= 1328799683U)
					{
						if (num != 990293175U)
						{
							if (num != 1128908517U)
							{
								if (num == 1328799683U)
								{
									if (text2 == "usemtl")
									{
										if (dictionary2.TryGetValue(array[1], out text))
										{
										}
									}
								}
							}
							else if (text2 == "vn")
							{
								list3.Add(new ObjModelReader.ObjNormal(array[1], array[2], array[3]));
							}
						}
						else if (text2 == "mtllib")
						{
							dictionary2 = ContentManager.Get<MtllibStruct>(array[1], null).TexturePaths;
						}
					}
					else if (num <= 3809224601U)
					{
						if (num != 1498016135U)
						{
							if (num == 3809224601U)
							{
								if (text2 == "f")
								{
									if (string.IsNullOrEmpty(text))
									{
										text = "Textures/NoneTexture";
									}
									objMesh.TexturePath = text;
									if (array.Length - 1 != 3)
									{
										throw new Exception("模型必须为三角面");
									}
									int num2 = 0;
									int count = objMesh.Vertices.Count;
									while (++num2 < array.Length)
									{
										string[] array2 = array[num2].Split(new char[]
										{
											'/'
										}, StringSplitOptions.None);
										if (array2.Length != 3)
										{
											throw new Exception("面参数错误");
										}
										int num3 = int.Parse(array2[0]);
										int num4 = int.Parse(array2[1]);
										int num5 = int.Parse(array2[2]);
										ObjModelReader.ObjPosition position = list[num3 - 1];
										ObjModelReader.ObjTexCood texCood = list2[num4 - 1];
										ObjModelReader.ObjNormal objNormal = list3[num5 - 1];
										int key = CellFace.Vector3ToFace(new Vector3(objNormal.x, objNormal.y, objNormal.z), 5);
										objMesh.Indices.Add((ushort)(count + ObjModelReader.FaceMap[key][num2 - 1]));
										objMesh.Vertices.Add(new ObjModelReader.ObjVertex
										{
											position = position,
											objNormal = objNormal,
											texCood = texCood
										});
									}
								}
							}
						}
						else if (text2 == "vt")
						{
							list2.Add(new ObjModelReader.ObjTexCood(array[1], array[2]));
						}
					}
					else if (num != 3926667934U)
					{
						if (num == 4077666505U)
						{
							if (text2 == "v")
							{
								list.Add(new ObjModelReader.ObjPosition(array[1], array[2], array[3]));
							}
						}
					}
					else if (text2 == "o")
					{
						ObjModelReader.ObjMesh objMesh2;
						if (dictionary.TryGetValue(array[1], out objMesh2))
						{
							objMesh = objMesh2;
						}
						else
						{
							objMesh = new ObjModelReader.ObjMesh(array[1]);
							dictionary.Add(array[1], objMesh);
						}
					}
				}
			}
			finally
			{
				if (stream != null)
				{
					((IDisposable)stream).Dispose();
				}
			}
			return ObjModelReader.ObjMeshesToModel<ObjModel>(dictionary);
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0002ADA4 File Offset: 0x00028FA4
		public static void AppendMesh(Model model, ModelBone rootBone, string texturepath, ObjModelReader.ObjMesh objMesh)
		{
			ModelBone modelBone = model.NewBone(objMesh.MeshName, (objMesh.MeshMatrix != null) ? objMesh.MeshMatrix.Value : Matrix.Identity, rootBone);
			if (objMesh.Vertices.Count > 0)
			{
				ModelMesh modelMesh = model.NewMesh(objMesh.MeshName, modelBone, objMesh.CalculateBoundingBox());
				VertexBuffer vertexBuffer = new VertexBuffer(new VertexDeclaration(new VertexElement[]
				{
					new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
					new VertexElement(12, VertexElementFormat.Vector3, VertexElementSemantic.Normal),
					new VertexElement(24, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate)
				}), objMesh.Vertices.Count);
				MemoryStream memoryStream = new MemoryStream();
				MemoryStream memoryStream2 = new MemoryStream();
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2);
				for (int i = 0; i < objMesh.Vertices.Count; i++)
				{
					ObjModelReader.ObjVertex objVertex = objMesh.Vertices[i];
					binaryWriter.Write(objVertex.position.x);
					binaryWriter.Write(objVertex.position.y);
					binaryWriter.Write(objVertex.position.z);
					binaryWriter.Write(objVertex.objNormal.x);
					binaryWriter.Write(objVertex.objNormal.y);
					binaryWriter.Write(objVertex.objNormal.z);
					binaryWriter.Write(objVertex.texCood.tx);
					binaryWriter.Write(objVertex.texCood.ty);
				}
				for (int j = 0; j < objMesh.Indices.Count; j++)
				{
					binaryWriter2.Write(objMesh.Indices[j]);
				}
				byte[] tag = memoryStream.ToArray();
				byte[] tag2 = memoryStream2.ToArray();
				memoryStream.Close();
				memoryStream2.Close();
				vertexBuffer.SetData<ObjModelReader.ObjVertex>(objMesh.Vertices.Array, 0, objMesh.Vertices.Count, 0);
				vertexBuffer.Tag = tag;
				IndexBuffer indexBuffer = new IndexBuffer(IndexFormat.SixteenBits, objMesh.Indices.Count);
				indexBuffer.SetData<ushort>(objMesh.Indices.Array, 0, objMesh.Indices.Count, 0);
				indexBuffer.Tag = tag2;
				modelMesh.NewMeshPart(vertexBuffer, indexBuffer, 0, objMesh.Indices.Count, objMesh.CalculateBoundingBox()).TexturePath = objMesh.TexturePath;
				model.AddMesh(modelMesh);
			}
			foreach (ObjModelReader.ObjMesh objMesh2 in objMesh.ChildMeshes)
			{
				ObjModelReader.AppendMesh(model, modelBone, objMesh2.TexturePath, objMesh2);
			}
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0002B054 File Offset: 0x00029254
		public static T ObjMeshesToModel<T>(Dictionary<string, ObjModelReader.ObjMesh> Meshes) where T : class
		{
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsSubclassOf(typeof(Model)))
			{
				throw new Exception("不能将" + typeFromHandle.Name + "转换为Model类型");
			}
			object obj = Activator.CreateInstance(typeFromHandle);
			Model model = obj as Model;
			ModelBone rootBone = model.NewBone("Object", Matrix.Identity, null);
			foreach (KeyValuePair<string, ObjModelReader.ObjMesh> keyValuePair in Meshes)
			{
				ObjModelReader.AppendMesh(model, rootBone, keyValuePair.Key, keyValuePair.Value);
			}
			return obj as T;
		}

		// Token: 0x0400033B RID: 827
		public static Dictionary<int, List<int>> FaceMap = new Dictionary<int, List<int>>();

		// Token: 0x0200044A RID: 1098
		public struct ObjPosition
		{
			// Token: 0x06001FC8 RID: 8136 RVA: 0x000E642B File Offset: 0x000E462B
			public ObjPosition(string x_, string y_, string z_)
			{
				this.x = float.Parse(x_);
				this.y = float.Parse(y_);
				this.z = float.Parse(z_);
			}

			// Token: 0x06001FC9 RID: 8137 RVA: 0x000E6451 File Offset: 0x000E4651
			public ObjPosition(float x_, float y_, float z_)
			{
				this.x = x_;
				this.y = y_;
				this.z = z_;
			}

			// Token: 0x040015D3 RID: 5587
			public float x;

			// Token: 0x040015D4 RID: 5588
			public float y;

			// Token: 0x040015D5 RID: 5589
			public float z;
		}

		// Token: 0x0200044B RID: 1099
		public struct ObjVertex
		{
			// Token: 0x040015D6 RID: 5590
			public ObjModelReader.ObjPosition position;

			// Token: 0x040015D7 RID: 5591
			public ObjModelReader.ObjNormal objNormal;

			// Token: 0x040015D8 RID: 5592
			public ObjModelReader.ObjTexCood texCood;
		}

		// Token: 0x0200044C RID: 1100
		public struct ObjNormal
		{
			// Token: 0x06001FCA RID: 8138 RVA: 0x000E6468 File Offset: 0x000E4668
			public ObjNormal(string x_, string y_, string z_)
			{
				this.x = float.Parse(x_);
				this.y = float.Parse(y_);
				this.z = float.Parse(z_);
			}

			// Token: 0x06001FCB RID: 8139 RVA: 0x000E648E File Offset: 0x000E468E
			public ObjNormal(float x_, float y_, float z_)
			{
				this.x = x_;
				this.y = y_;
				this.z = z_;
			}

			// Token: 0x040015D9 RID: 5593
			public float x;

			// Token: 0x040015DA RID: 5594
			public float y;

			// Token: 0x040015DB RID: 5595
			public float z;
		}

		// Token: 0x0200044D RID: 1101
		public struct ObjTexCood
		{
			// Token: 0x06001FCC RID: 8140 RVA: 0x000E64A5 File Offset: 0x000E46A5
			public ObjTexCood(string tx_, string ty_)
			{
				this.tx = float.Parse(tx_);
				this.ty = float.Parse(ty_);
			}

			// Token: 0x06001FCD RID: 8141 RVA: 0x000E64BF File Offset: 0x000E46BF
			public ObjTexCood(float tx_, float ty_)
			{
				this.tx = tx_;
				this.ty = ty_;
			}

			// Token: 0x040015DC RID: 5596
			public float tx;

			// Token: 0x040015DD RID: 5597
			public float ty;
		}

		// Token: 0x0200044E RID: 1102
		public class ObjMesh
		{
			// Token: 0x06001FCE RID: 8142 RVA: 0x000E64CF File Offset: 0x000E46CF
			public ObjMesh(string meshname)
			{
				this.MeshName = meshname;
			}

			// Token: 0x06001FCF RID: 8143 RVA: 0x000E650C File Offset: 0x000E470C
			public BoundingBox CalculateBoundingBox()
			{
				List<Vector3> list = new List<Vector3>();
				for (int i = 0; i < this.Vertices.Count; i++)
				{
					list.Add(new Vector3(this.Vertices[i].position.x, this.Vertices[i].position.y, this.Vertices[i].position.z));
				}
				return new BoundingBox(list);
			}

			// Token: 0x040015DE RID: 5598
			public int ElementIndex;

			// Token: 0x040015DF RID: 5599
			public DynamicArray<ObjModelReader.ObjVertex> Vertices = new DynamicArray<ObjModelReader.ObjVertex>();

			// Token: 0x040015E0 RID: 5600
			public DynamicArray<ushort> Indices = new DynamicArray<ushort>();

			// Token: 0x040015E1 RID: 5601
			public string TexturePath = "Textures/NoneTexture";

			// Token: 0x040015E2 RID: 5602
			public string MeshName;

			// Token: 0x040015E3 RID: 5603
			public Matrix? MeshMatrix;

			// Token: 0x040015E4 RID: 5604
			public List<ObjModelReader.ObjMesh> ChildMeshes = new List<ObjModelReader.ObjMesh>();
		}
	}
}
