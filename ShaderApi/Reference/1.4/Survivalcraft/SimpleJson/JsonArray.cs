using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000006 RID: 6
	[GeneratedCode("simple-json", "1.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonArray : List<object>
	{
		// Token: 0x06000039 RID: 57 RVA: 0x0000573B File Offset: 0x0000393B
		public JsonArray()
		{
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00005743 File Offset: 0x00003943
		public JsonArray(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600003B RID: 59 RVA: 0x0000574C File Offset: 0x0000394C
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this) ?? string.Empty;
		}
	}
}
