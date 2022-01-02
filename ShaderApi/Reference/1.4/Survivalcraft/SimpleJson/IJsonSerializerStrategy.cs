using System;
using System.CodeDom.Compiler;

namespace SimpleJson
{
	// Token: 0x02000005 RID: 5
	[GeneratedCode("simple-json", "1.0.0")]
	internal interface IJsonSerializerStrategy
	{
		// Token: 0x06000037 RID: 55
		bool TrySerializeNonPrimitiveObject(object input, out object output);

		// Token: 0x06000038 RID: 56
		object DeserializeObject(object value, Type type);
	}
}
