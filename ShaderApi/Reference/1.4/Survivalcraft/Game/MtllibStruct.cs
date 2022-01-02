using System;
using System.Collections.Generic;
using System.IO;

namespace Game
{
	// Token: 0x02000157 RID: 343
	public class MtllibStruct
	{
		// Token: 0x06000794 RID: 1940 RVA: 0x0002A848 File Offset: 0x00028A48
		public static MtllibStruct Load(Stream stream)
		{
			MtllibStruct mtllibStruct = new MtllibStruct();
			try
			{
				StreamReader streamReader = new StreamReader(stream);
				string text = null;
				while (!streamReader.EndOfStream)
				{
					string[] array = streamReader.ReadLine().Split(new char[]
					{
						'\t',
						' '
					}, StringSplitOptions.None);
					string a = array[0];
					if (!(a == "newmtl"))
					{
						if (a == "map_Kd")
						{
							if (string.IsNullOrEmpty(text))
							{
								throw new Exception("请先newmtl");
							}
							mtllibStruct.TexturePaths.Add(text, array[1]);
						}
					}
					else
					{
						text = array[1];
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
			return mtllibStruct;
		}

		// Token: 0x0400033A RID: 826
		public Dictionary<string, string> TexturePaths = new Dictionary<string, string>();
	}
}
