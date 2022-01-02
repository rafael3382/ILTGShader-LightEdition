using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
	// Token: 0x0200029E RID: 670
	public class GameLogSink : ILogSink
	{
		// Token: 0x0600150A RID: 5386 RVA: 0x0009F188 File Offset: 0x0009D388
		public GameLogSink()
		{
			try
			{
				if (GameLogSink.m_stream != null)
				{
					throw new InvalidOperationException("GameLogSink already created.");
				}
				string path = Storage.CombinePaths(new string[]
				{
					"app:/Bugs",
					"Game.log"
				});
				Storage.CreateDirectory("app:/Bugs");
				GameLogSink.m_stream = Storage.OpenFile(path, OpenFileMode.CreateOrOpen);
				if (GameLogSink.m_stream.Length > 10485760L)
				{
					GameLogSink.m_stream.Dispose();
					GameLogSink.m_stream = Storage.OpenFile(path, OpenFileMode.Create);
				}
				GameLogSink.m_stream.Position = GameLogSink.m_stream.Length;
				GameLogSink.m_writer = new StreamWriter(GameLogSink.m_stream);
			}
			catch (Exception ex)
			{
				Engine.Log.Error("Error creating GameLogSink. Reason: {0}", new object[]
				{
					ex.Message
				});
			}
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x0009F258 File Offset: 0x0009D458
		public static string GetRecentLog(int bytesCount)
		{
			if (GameLogSink.m_stream == null)
			{
				return string.Empty;
			}
			Stream stream = GameLogSink.m_stream;
			string result;
			lock (stream)
			{
				try
				{
					GameLogSink.m_stream.Position = MathUtils.Max(GameLogSink.m_stream.Position - (long)bytesCount, 0L);
					result = new StreamReader(GameLogSink.m_stream).ReadToEnd();
				}
				finally
				{
					GameLogSink.m_stream.Position = GameLogSink.m_stream.Length;
				}
			}
			return result;
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x0009F2F0 File Offset: 0x0009D4F0
		public static List<string> GetRecentLogLines(int bytesCount)
		{
			if (GameLogSink.m_stream == null)
			{
				return new List<string>();
			}
			Stream stream = GameLogSink.m_stream;
			List<string> result;
			lock (stream)
			{
				try
				{
					GameLogSink.m_stream.Position = MathUtils.Max(GameLogSink.m_stream.Position - (long)bytesCount, 0L);
					StreamReader streamReader = new StreamReader(GameLogSink.m_stream);
					List<string> list = new List<string>();
					for (;;)
					{
						string text = streamReader.ReadLine();
						if (text == null)
						{
							break;
						}
						list.Add(text);
					}
					result = list;
				}
				finally
				{
					GameLogSink.m_stream.Position = GameLogSink.m_stream.Length;
				}
			}
			return result;
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x0009F3A4 File Offset: 0x0009D5A4
		public void Log(LogType type, string message)
		{
			if (GameLogSink.m_stream != null)
			{
				Stream stream = GameLogSink.m_stream;
				lock (stream)
				{
					string value;
					switch (type)
					{
					case LogType.Debug:
						value = "DEBUG: ";
						break;
					case LogType.Verbose:
						value = "INFO: ";
						break;
					case LogType.Information:
						value = "INFO: ";
						break;
					case LogType.Warning:
						value = "WARNING: ";
						break;
					case LogType.Error:
						value = "ERROR: ";
						break;
					default:
						value = string.Empty;
						break;
					}
					GameLogSink.m_writer.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
					GameLogSink.m_writer.Write(" ");
					GameLogSink.m_writer.Write(value);
					GameLogSink.m_writer.WriteLine(message);
					GameLogSink.m_writer.Flush();
				}
			}
		}

		// Token: 0x04000DB8 RID: 3512
		public static Stream m_stream;

		// Token: 0x04000DB9 RID: 3513
		public static StreamWriter m_writer;
	}
}
