using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000154 RID: 340
	public class JsonModelReader
	{
		// Token: 0x0600075A RID: 1882 RVA: 0x00029230 File Offset: 0x00027430
		static JsonModelReader()
		{
			JsonModelReader.FacesDic.Add("north", new List<Vector3>
			{
				Vector3.UnitX,
				Vector3.Zero,
				Vector3.UnitY,
				new Vector3(1f, 1f, 0f)
			});
			JsonModelReader.FacesDic.Add("south", new List<Vector3>
			{
				new Vector3(1f, 0f, 1f),
				Vector3.UnitZ,
				new Vector3(0f, 1f, 1f),
				new Vector3(1f, 1f, 1f)
			});
			JsonModelReader.FacesDic.Add("east", new List<Vector3>
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 1f),
				new Vector3(1f, 1f, 1f),
				new Vector3(1f, 1f, 0f)
			});
			JsonModelReader.FacesDic.Add("west", new List<Vector3>
			{
				Vector3.Zero,
				Vector3.UnitZ,
				new Vector3(0f, 1f, 1f),
				Vector3.UnitY
			});
			JsonModelReader.FacesDic.Add("up", new List<Vector3>
			{
				Vector3.UnitY,
				new Vector3(0f, 1f, 1f),
				Vector3.One,
				new Vector3(1f, 1f, 0f)
			});
			JsonModelReader.FacesDic.Add("down", new List<Vector3>
			{
				Vector3.Zero,
				Vector3.UnitZ,
				new Vector3(1f, 0f, 1f),
				new Vector3(1f, 0f, 0f)
			});
			JsonModelReader.NormalDic.Add("north", new Vector3(0f, 0f, -1f));
			JsonModelReader.NormalDic.Add("south", new Vector3(0f, 0f, 1f));
			JsonModelReader.NormalDic.Add("east", new Vector3(1f, 0f, 0f));
			JsonModelReader.NormalDic.Add("west", new Vector3(-1f, 0f, 0f));
			JsonModelReader.NormalDic.Add("up", new Vector3(0f, 1f, 0f));
			JsonModelReader.NormalDic.Add("down", new Vector3(0f, -1f, 0f));
			JsonModelReader.FacedirecDic.Add("north", new List<int>
			{
				0,
				2,
				1,
				0,
				3,
				2
			});
			JsonModelReader.FacedirecDic.Add("west", new List<int>
			{
				0,
				2,
				1,
				0,
				3,
				2
			});
			JsonModelReader.FacedirecDic.Add("up", new List<int>
			{
				0,
				2,
				1,
				0,
				3,
				2
			});
			JsonModelReader.FacedirecDic.Add("south", new List<int>
			{
				0,
				1,
				2,
				0,
				2,
				3
			});
			JsonModelReader.FacedirecDic.Add("east", new List<int>
			{
				0,
				1,
				2,
				0,
				2,
				3
			});
			JsonModelReader.FacedirecDic.Add("down", new List<int>
			{
				0,
				1,
				2,
				0,
				2,
				3
			});
			JsonModelReader.TextureRotate.Add(0f, new List<int>
			{
				0,
				3,
				2,
				3,
				2,
				1,
				0,
				1
			});
			JsonModelReader.TextureRotate.Add(90f, new List<int>
			{
				0,
				1,
				0,
				3,
				2,
				3,
				2,
				1
			});
			JsonModelReader.TextureRotate.Add(180f, new List<int>
			{
				2,
				1,
				0,
				1,
				0,
				3,
				2,
				3
			});
			JsonModelReader.TextureRotate.Add(270f, new List<int>
			{
				2,
				3,
				2,
				1,
				0,
				1,
				0,
				3
			});
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00029820 File Offset: 0x00027A20
		public static float ObjConvertFloat(object obj)
		{
			if (obj is double)
			{
				return (float)((double)obj);
			}
			if (obj is long)
			{
				return (float)((long)obj);
			}
			throw new Exception("错误的数据转换，不能将" + obj.GetType().Name + "转换为float");
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x0002986C File Offset: 0x00027A6C
		public static JsonModel Load(Stream stream)
		{
			Dictionary<string, ObjModelReader.ObjMesh> dictionary = new Dictionary<string, ObjModelReader.ObjMesh>();
			Vector3 firstPersonOffset = Vector3.One;
			Vector3 firstPersonRotation = Vector3.Zero;
			Vector3 firstPersonScale = Vector3.One;
			Vector3 inHandOffset = Vector3.One;
			Vector3 inHandRotation = Vector3.Zero;
			Vector3 inHandScale = Vector3.One;
			string text = string.Empty;
			try
			{
				object obj = SimpleJson.DeserializeObject(new StreamReader(stream).ReadToEnd());
				if (obj is JsonObject)
				{
					JsonObject jsonObject = obj as JsonObject;
					Vector2 zero = Vector2.Zero;
					Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
					object obj2;
					if (jsonObject.TryGetValue("display", out obj2))
					{
						object obj3;
						if ((obj2 as JsonObject).TryGetValue("thirdperson_righthand", out obj3))
						{
							JsonObject jsonObject2 = obj3 as JsonObject;
							object obj4;
							if (jsonObject2.TryGetValue("rotation", out obj4))
							{
								JsonArray jsonArray = obj4 as JsonArray;
								inHandRotation = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray[0]), JsonModelReader.ObjConvertFloat(jsonArray[1]), JsonModelReader.ObjConvertFloat(jsonArray[2]));
							}
							object obj5;
							if (jsonObject2.TryGetValue("translation", out obj5))
							{
								JsonArray jsonArray2 = obj5 as JsonArray;
								inHandOffset = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray2[0]), JsonModelReader.ObjConvertFloat(jsonArray2[1]), JsonModelReader.ObjConvertFloat(jsonArray2[2]));
							}
							object obj6;
							if (jsonObject2.TryGetValue("scale", out obj6))
							{
								JsonArray jsonArray3 = obj6 as JsonArray;
								inHandScale = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray3[0]), JsonModelReader.ObjConvertFloat(jsonArray3[1]), JsonModelReader.ObjConvertFloat(jsonArray3[2]));
							}
						}
						object obj7;
						if ((obj2 as JsonObject).TryGetValue("firstperson_righthand", out obj7))
						{
							JsonObject jsonObject3 = obj7 as JsonObject;
							object obj8;
							if (jsonObject3.TryGetValue("rotation", out obj8))
							{
								JsonArray jsonArray4 = obj8 as JsonArray;
								firstPersonRotation = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray4[0]), JsonModelReader.ObjConvertFloat(jsonArray4[1]), JsonModelReader.ObjConvertFloat(jsonArray4[2]));
							}
							object obj9;
							if (jsonObject3.TryGetValue("translation", out obj9))
							{
								JsonArray jsonArray5 = obj9 as JsonArray;
								firstPersonOffset = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray5[0]), JsonModelReader.ObjConvertFloat(jsonArray5[1]), JsonModelReader.ObjConvertFloat(jsonArray5[2]));
							}
							object obj10;
							if (jsonObject3.TryGetValue("scale", out obj10))
							{
								JsonArray jsonArray6 = obj10 as JsonArray;
								firstPersonScale = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray6[0]), JsonModelReader.ObjConvertFloat(jsonArray6[1]), JsonModelReader.ObjConvertFloat(jsonArray6[2]));
							}
						}
					}
					object obj11;
					if (jsonObject.TryGetValue("parent", out obj11))
					{
						text = (obj11 as string);
					}
					object obj12;
					if (jsonObject.TryGetValue("textures", out obj12))
					{
						foreach (KeyValuePair<string, object> keyValuePair in (obj12 as JsonObject))
						{
							dictionary2.Add(keyValuePair.Key, (string)keyValuePair.Value);
						}
					}
					object obj13;
					if (jsonObject.TryGetValue("texture_size", out obj13))
					{
						JsonArray jsonArray7 = obj13 as JsonArray;
						new Vector2(JsonModelReader.ObjConvertFloat(jsonArray7[0]), JsonModelReader.ObjConvertFloat(jsonArray7[1]));
					}
					object obj14;
					if (jsonObject.TryGetValue("elements", out obj14))
					{
						JsonArray jsonArray8 = obj14 as JsonArray;
						for (int i = 0; i < jsonArray8.Count; i++)
						{
							JsonObject jsonObject4 = jsonArray8[i] as JsonObject;
							JsonArray jsonArray9 = jsonObject4["from"] as JsonArray;
							JsonArray jsonArray10 = jsonObject4["to"] as JsonArray;
							string text2 = "undefined";
							object obj15;
							if (jsonObject4.TryGetValue("name", out obj15))
							{
								text2 = (obj15 as string);
							}
							ObjModelReader.ObjMesh objMesh;
							if (!dictionary.TryGetValue(text2, out objMesh))
							{
								objMesh = new ObjModelReader.ObjMesh(text2);
								objMesh.ElementIndex = i;
								dictionary.Add(text2, objMesh);
							}
							object obj16;
							if (jsonObject4.TryGetValue("rotation", out obj16))
							{
								JsonObject jsonObject5 = obj16 as JsonObject;
								object obj17 = jsonObject5["origin"];
								JsonModelReader.ObjConvertFloat(jsonObject5["angle"]);
							}
							Vector3 vector = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray9[0]), JsonModelReader.ObjConvertFloat(jsonArray9[1]), JsonModelReader.ObjConvertFloat(jsonArray9[2]));
							Vector3 vector2 = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray10[0]), JsonModelReader.ObjConvertFloat(jsonArray10[1]), JsonModelReader.ObjConvertFloat(jsonArray10[2]));
							Matrix m = Matrix.CreateScale(vector2.X - vector.X, vector2.Y - vector.Y, vector2.Z - vector.Z) * Matrix.CreateTranslation(vector.X, vector.Y, vector.Z) * Matrix.CreateScale(0.0625f);
							object obj18;
							if (jsonObject4.TryGetValue("faces", out obj18))
							{
								foreach (KeyValuePair<string, object> keyValuePair2 in (obj18 as JsonObject))
								{
									ObjModelReader.ObjMesh objMesh2 = new ObjModelReader.ObjMesh(keyValuePair2.Key);
									List<Vector3> list = JsonModelReader.FacesDic[keyValuePair2.Key];
									JsonObject jsonObject6 = keyValuePair2.Value as JsonObject;
									float key = 0f;
									string key2 = keyValuePair2.Key;
									float[] array = new float[4];
									List<Vector2> list2 = new List<Vector2>();
									object obj19;
									if (jsonObject6.TryGetValue("rotation", out obj19))
									{
										key = JsonModelReader.ObjConvertFloat(obj19);
									}
									object obj20;
									if (jsonObject6.TryGetValue("uv", out obj20))
									{
										JsonArray jsonArray11 = obj20 as JsonArray;
										for (int j = 0; j < jsonArray11.Count; j++)
										{
											array[j] = JsonModelReader.ObjConvertFloat(jsonArray11[j]) / 16f;
										}
										new Vector2(array[2] - array[0], array[3] - array[1]) / 2f + new Vector2(array[0], array[1]);
										list2.Add(new Vector2(array[JsonModelReader.TextureRotate[key][0]], array[JsonModelReader.TextureRotate[key][1]]));
										list2.Add(new Vector2(array[JsonModelReader.TextureRotate[key][2]], array[JsonModelReader.TextureRotate[key][3]]));
										list2.Add(new Vector2(array[JsonModelReader.TextureRotate[key][4]], array[JsonModelReader.TextureRotate[key][5]]));
										list2.Add(new Vector2(array[JsonModelReader.TextureRotate[key][6]], array[JsonModelReader.TextureRotate[key][7]]));
									}
									object obj21;
									if (jsonObject6.TryGetValue("texture", out obj21))
									{
										string text3 = obj21 as string;
										string texturePath;
										if (dictionary2.TryGetValue(text3.Substring(1), out texturePath))
										{
											objMesh2.TexturePath = texturePath;
										}
									}
									ObjModelReader.ObjPosition[] array2 = new ObjModelReader.ObjPosition[3];
									ObjModelReader.ObjTexCood[] array3 = new ObjModelReader.ObjTexCood[3];
									new ObjModelReader.ObjNormal[3];
									int index = JsonModelReader.FacedirecDic[key2][0];
									int index2 = JsonModelReader.FacedirecDic[key2][1];
									int index3 = JsonModelReader.FacedirecDic[key2][2];
									Vector3 vector3 = Vector3.Transform(list[index], m);
									Vector3 vector4 = Vector3.Transform(list[index2], m);
									Vector3 vector5 = Vector3.Transform(list[index3], m);
									array2[0] = new ObjModelReader.ObjPosition(vector3.X, vector3.Y, vector3.Z);
									array2[1] = new ObjModelReader.ObjPosition(vector4.X, vector4.Y, vector4.Z);
									array2[2] = new ObjModelReader.ObjPosition(vector5.X, vector5.Y, vector5.Z);
									Vector2 vector6 = list2[index];
									Vector2 vector7 = list2[index2];
									Vector2 vector8 = list2[index3];
									array3[0] = new ObjModelReader.ObjTexCood(vector6.X, vector6.Y);
									array3[1] = new ObjModelReader.ObjTexCood(vector7.X, vector7.Y);
									array3[2] = new ObjModelReader.ObjTexCood(vector8.X, vector8.Y);
									int count = objMesh2.Vertices.Count;
									objMesh2.Indices.Add((ushort)count++);
									objMesh2.Indices.Add((ushort)count++);
									objMesh2.Indices.Add((ushort)count++);
									objMesh2.Vertices.Add(new ObjModelReader.ObjVertex
									{
										position = array2[0],
										objNormal = new ObjModelReader.ObjNormal(0f, 0f, 0f),
										texCood = array3[0]
									});
									objMesh2.Vertices.Add(new ObjModelReader.ObjVertex
									{
										position = array2[1],
										objNormal = new ObjModelReader.ObjNormal(0f, 0f, 0f),
										texCood = array3[1]
									});
									objMesh2.Vertices.Add(new ObjModelReader.ObjVertex
									{
										position = array2[2],
										objNormal = new ObjModelReader.ObjNormal(0f, 0f, 0f),
										texCood = array3[2]
									});
									index = JsonModelReader.FacedirecDic[key2][3];
									index2 = JsonModelReader.FacedirecDic[key2][4];
									index3 = JsonModelReader.FacedirecDic[key2][5];
									vector3 = Vector3.Transform(list[index], m);
									vector4 = Vector3.Transform(list[index2], m);
									vector5 = Vector3.Transform(list[index3], m);
									array2[0] = new ObjModelReader.ObjPosition(vector3.X, vector3.Y, vector3.Z);
									array2[1] = new ObjModelReader.ObjPosition(vector4.X, vector4.Y, vector4.Z);
									array2[2] = new ObjModelReader.ObjPosition(vector5.X, vector5.Y, vector5.Z);
									vector6 = list2[index];
									vector7 = list2[index2];
									vector8 = list2[index3];
									array3[0] = new ObjModelReader.ObjTexCood(vector6.X, vector6.Y);
									array3[1] = new ObjModelReader.ObjTexCood(vector7.X, vector7.Y);
									array3[2] = new ObjModelReader.ObjTexCood(vector8.X, vector8.Y);
									objMesh2.Indices.Add((ushort)count++);
									objMesh2.Indices.Add((ushort)count++);
									objMesh2.Indices.Add((ushort)count++);
									objMesh2.Vertices.Add(new ObjModelReader.ObjVertex
									{
										position = array2[0],
										objNormal = new ObjModelReader.ObjNormal(0f, 0f, 0f),
										texCood = array3[0]
									});
									objMesh2.Vertices.Add(new ObjModelReader.ObjVertex
									{
										position = array2[1],
										objNormal = new ObjModelReader.ObjNormal(0f, 0f, 0f),
										texCood = array3[1]
									});
									objMesh2.Vertices.Add(new ObjModelReader.ObjVertex
									{
										position = array2[2],
										objNormal = new ObjModelReader.ObjNormal(0f, 0f, 0f),
										texCood = array3[2]
									});
									objMesh.ChildMeshes.Add(objMesh2);
								}
							}
						}
					}
					List<ObjModelReader.ObjMesh> list3 = new List<ObjModelReader.ObjMesh>();
					foreach (KeyValuePair<string, ObjModelReader.ObjMesh> keyValuePair3 in dictionary)
					{
						list3.Add(keyValuePair3.Value);
					}
					object obj22;
					if (jsonObject.TryGetValue("groups", out obj22))
					{
						Dictionary<string, ObjModelReader.ObjMesh> dictionary3 = new Dictionary<string, ObjModelReader.ObjMesh>();
						JsonArray jsonArray12 = obj22 as JsonArray;
						for (int k = 0; k < jsonArray12.Count; k++)
						{
							JsonObject jsonObject7 = jsonArray12[k] as JsonObject;
							string text4 = k.ToString();
							object obj23;
							if (jsonObject7.TryGetValue("name", out obj23))
							{
								text4 = (string)obj23;
							}
							ObjModelReader.ObjMesh objMesh3 = new ObjModelReader.ObjMesh(text4);
							object obj24;
							if (jsonObject7.TryGetValue("oringin", out obj24))
							{
								JsonArray jsonArray13 = obj24 as JsonArray;
								Vector3 vector9 = new Vector3(JsonModelReader.ObjConvertFloat(jsonArray13[0]), JsonModelReader.ObjConvertFloat(jsonArray13[1]), JsonModelReader.ObjConvertFloat(jsonArray13[2])) / 16f;
								objMesh3.MeshMatrix = new Matrix?(Matrix.CreateTranslation(vector9.X, vector9.Y, vector9.Z));
							}
							object obj25;
							if (jsonObject7.TryGetValue("children", out obj25))
							{
								JsonArray jsonArray14 = obj25 as JsonArray;
								for (int l = 0; l < jsonArray14.Count; l++)
								{
									int eleindex = (int)JsonModelReader.ObjConvertFloat(jsonArray14[l]);
									objMesh3.ChildMeshes.Add(list3.Find((ObjModelReader.ObjMesh xp) => xp.ElementIndex == eleindex));
								}
							}
							dictionary3.Add(text4, objMesh3);
						}
						JsonModel jsonModel = ObjModelReader.ObjMeshesToModel<JsonModel>(dictionary3);
						if (!string.IsNullOrEmpty(text))
						{
							try
							{
								jsonModel.ParentModel = ContentManager.Get<JsonModel>(text, null);
							}
							catch
							{
							}
						}
						jsonModel.InHandScale = inHandScale;
						jsonModel.InHandOffset = inHandOffset;
						jsonModel.InHandRotation = inHandRotation;
						jsonModel.FirstPersonOffset = firstPersonOffset;
						jsonModel.FirstPersonScale = firstPersonScale;
						jsonModel.FirstPersonRotation = firstPersonRotation;
						return jsonModel;
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
			JsonModel jsonModel2 = ObjModelReader.ObjMeshesToModel<JsonModel>(dictionary);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					jsonModel2.ParentModel = ContentManager.Get<JsonModel>(text, null);
				}
				catch
				{
				}
			}
			jsonModel2.InHandScale = inHandScale;
			jsonModel2.InHandOffset = inHandOffset;
			jsonModel2.InHandRotation = inHandRotation;
			jsonModel2.FirstPersonOffset = firstPersonOffset;
			jsonModel2.FirstPersonScale = firstPersonScale;
			jsonModel2.FirstPersonRotation = firstPersonRotation;
			return jsonModel2;
		}

		// Token: 0x0400032D RID: 813
		public static Dictionary<string, List<Vector3>> FacesDic = new Dictionary<string, List<Vector3>>();

		// Token: 0x0400032E RID: 814
		public static Dictionary<string, Vector3> NormalDic = new Dictionary<string, Vector3>();

		// Token: 0x0400032F RID: 815
		public static Dictionary<string, List<int>> FacedirecDic = new Dictionary<string, List<int>>();

		// Token: 0x04000330 RID: 816
		public static Dictionary<float, List<int>> TextureRotate = new Dictionary<float, List<int>>();
	}
}
