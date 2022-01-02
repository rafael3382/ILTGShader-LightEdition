using System;
using System.IO;
using System.Text.RegularExpressions;
using Engine;

namespace Game
{
	// Token: 0x02000132 RID: 306
	public class ShaderCodeManager
	{
		// Token: 0x060005C0 RID: 1472 RVA: 0x00020B24 File Offset: 0x0001ED24
		public static string GetFast(string fname)
		{
			string result = string.Empty;
			string[] array = fname.Split(new char[]
			{
				'.'
			});
			if (array.Length > 1)
			{
				result = ModsManager.GetInPakOrStorageFile<string>(array[0], "." + array[1]);
			}
			return result;
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00020B66 File Offset: 0x0001ED66
		public static string Get(string fname)
		{
			return ShaderCodeManager.GetIncludeText(string.Empty, fname, false);
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00020B74 File Offset: 0x0001ED74
		public static string GetExternal(string fname)
		{
			return ShaderCodeManager.GetIncludeText(string.Empty, fname, true);
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00020B84 File Offset: 0x0001ED84
		public static string GetIncludeText(string shaderText, string includefname, bool external)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			try
			{
				if (external)
				{
					string text3 = "app:/";
					text2 = new StreamReader(Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						text3,
						includefname
					}), OpenFileMode.Read)).ReadToEnd();
				}
				else if (includefname.Contains(".txt"))
				{
					includefname = includefname.Split(new char[]
					{
						'.'
					})[0];
					text2 = ContentManager.Get<string>(includefname, null);
				}
				else
				{
					text2 = ShaderCodeManager.GetFast(includefname);
				}
				if (text2 == string.Empty)
				{
					return string.Empty;
				}
				text2 = text2.Replace("\n", "$");
				string[] array = text2.Split(new char[]
				{
					'$'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Contains("#include"))
					{
						string includefname2 = new Regex("\"[^\"]*\"").Match(array[i]).Value.Replace("\"", "");
						text += ShaderCodeManager.GetIncludeText(shaderText, includefname2, external);
					}
					else
					{
						text = text + array[i].Replace("highp", "") + "\n";
					}
				}
				shaderText += text;
			}
			catch
			{
			}
			return shaderText;
		}
	}
}
