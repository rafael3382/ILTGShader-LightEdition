using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000151 RID: 337
	public static class WebManager
	{
		// Token: 0x06000721 RID: 1825
		[DllImport("wininet.dll")]
		public static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

		// Token: 0x06000722 RID: 1826 RVA: 0x00027580 File Offset: 0x00025780
		public static bool IsInternetConnectionAvailable()
		{
			try
			{
				int num;
				return WebManager.InternetGetConnectedState(out num, 0);
			}
			catch (Exception e)
			{
				Log.Warning(ExceptionManager.MakeFullErrorMessage("Could not check internet connection availability.", e));
			}
			return true;
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x000275C0 File Offset: 0x000257C0
		public static void Get(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			WebManager.<>c__DisplayClass3_0 CS$<>8__locals1 = new WebManager.<>c__DisplayClass3_0();
			CS$<>8__locals1.progress = progress;
			CS$<>8__locals1.parameters = parameters;
			CS$<>8__locals1.address = address;
			CS$<>8__locals1.headers = headers;
			CS$<>8__locals1.success = success;
			CS$<>8__locals1.failure = failure;
			CS$<>8__locals1.targetStream = null;
			CS$<>8__locals1.e = null;
			Task.Run(delegate()
			{
				WebManager.<>c__DisplayClass3_0.<<Get>b__0>d <<Get>b__0>d;
				<<Get>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Get>b__0>d.<>4__this = CS$<>8__locals1;
				<<Get>b__0>d.<>1__state = -1;
				<<Get>b__0>d.<>t__builder.Start<WebManager.<>c__DisplayClass3_0.<<Get>b__0>d>(ref <<Get>b__0>d);
				return <<Get>b__0>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x0002761D File Offset: 0x0002581D
		public static void Put(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			WebManager.PutOrPost(false, address, parameters, headers, data, progress, success, failure);
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x0002762F File Offset: 0x0002582F
		public static void Post(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			WebManager.PutOrPost(true, address, parameters, headers, data, progress, success, failure);
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00027644 File Offset: 0x00025844
		public static string UrlParametersToString(Dictionary<string, string> values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = string.Empty;
			foreach (KeyValuePair<string, string> keyValuePair in values)
			{
				stringBuilder.Append(value);
				value = "&";
				stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Key));
				stringBuilder.Append('=');
				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Value));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x000276E8 File Offset: 0x000258E8
		public static byte[] UrlParametersToBytes(Dictionary<string, string> values)
		{
			return Encoding.UTF8.GetBytes(WebManager.UrlParametersToString(values));
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x000276FA File Offset: 0x000258FA
		public static MemoryStream UrlParametersToStream(Dictionary<string, string> values)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(WebManager.UrlParametersToString(values)));
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x00027714 File Offset: 0x00025914
		public static Dictionary<string, string> UrlParametersFromString(string s)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = s.Split(new char[]
			{
				'&'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = Uri.UnescapeDataString(array[i]).Split(new char[]
				{
					'='
				});
				if (array2.Length == 2)
				{
					dictionary[array2[0]] = array2[1];
				}
			}
			return dictionary;
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x00027774 File Offset: 0x00025974
		public static Dictionary<string, string> UrlParametersFromBytes(byte[] bytes)
		{
			return WebManager.UrlParametersFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0002778A File Offset: 0x0002598A
		public static object JsonFromString(string s)
		{
			return SimpleJson.DeserializeObject(s);
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00027792 File Offset: 0x00025992
		public static object JsonFromBytes(byte[] bytes)
		{
			return WebManager.JsonFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x000277A8 File Offset: 0x000259A8
		public static void PutOrPost(bool isPost, string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			WebManager.<>c__DisplayClass13_0 CS$<>8__locals1 = new WebManager.<>c__DisplayClass13_0();
			CS$<>8__locals1.headers = headers;
			CS$<>8__locals1.parameters = parameters;
			CS$<>8__locals1.address = address;
			CS$<>8__locals1.data = data;
			CS$<>8__locals1.progress = progress;
			CS$<>8__locals1.isPost = isPost;
			CS$<>8__locals1.success = success;
			CS$<>8__locals1.failure = failure;
			CS$<>8__locals1.responseData = null;
			CS$<>8__locals1.e = null;
			Task.Run(delegate()
			{
				WebManager.<>c__DisplayClass13_0.<<PutOrPost>b__0>d <<PutOrPost>b__0>d;
				<<PutOrPost>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<PutOrPost>b__0>d.<>4__this = CS$<>8__locals1;
				<<PutOrPost>b__0>d.<>1__state = -1;
				<<PutOrPost>b__0>d.<>t__builder.Start<WebManager.<>c__DisplayClass13_0.<<PutOrPost>b__0>d>(ref <<PutOrPost>b__0>d);
				return <<PutOrPost>b__0>d.<>t__builder.Task;
			});
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x00027818 File Offset: 0x00025A18
		public static Task VerifyResponse(HttpResponseMessage message)
		{
			WebManager.<VerifyResponse>d__14 <VerifyResponse>d__;
			<VerifyResponse>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<VerifyResponse>d__.message = message;
			<VerifyResponse>d__.<>1__state = -1;
			<VerifyResponse>d__.<>t__builder.Start<WebManager.<VerifyResponse>d__14>(ref <VerifyResponse>d__);
			return <VerifyResponse>d__.<>t__builder.Task;
		}

		// Token: 0x02000444 RID: 1092
		public class ProgressHttpContent : HttpContent
		{
			// Token: 0x06001FB1 RID: 8113 RVA: 0x000E60B1 File Offset: 0x000E42B1
			public ProgressHttpContent(Stream sourceStream, CancellableProgress progress)
			{
				this.m_sourceStream = sourceStream;
				this.m_progress = (progress ?? new CancellableProgress());
			}

			// Token: 0x06001FB2 RID: 8114 RVA: 0x000E60D0 File Offset: 0x000E42D0
			protected override bool TryComputeLength(out long length)
			{
				length = this.m_sourceStream.Length;
				return true;
			}

			// Token: 0x06001FB3 RID: 8115 RVA: 0x000E60E0 File Offset: 0x000E42E0
			protected override Task SerializeToStreamAsync(Stream targetStream, TransportContext context)
			{
				WebManager.ProgressHttpContent.<SerializeToStreamAsync>d__4 <SerializeToStreamAsync>d__;
				<SerializeToStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
				<SerializeToStreamAsync>d__.<>4__this = this;
				<SerializeToStreamAsync>d__.targetStream = targetStream;
				<SerializeToStreamAsync>d__.<>1__state = -1;
				<SerializeToStreamAsync>d__.<>t__builder.Start<WebManager.ProgressHttpContent.<SerializeToStreamAsync>d__4>(ref <SerializeToStreamAsync>d__);
				return <SerializeToStreamAsync>d__.<>t__builder.Task;
			}

			// Token: 0x040015B2 RID: 5554
			public Stream m_sourceStream;

			// Token: 0x040015B3 RID: 5555
			public CancellableProgress m_progress;
		}
	}
}
