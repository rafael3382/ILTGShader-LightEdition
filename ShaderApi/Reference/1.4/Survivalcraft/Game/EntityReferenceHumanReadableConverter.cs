using System;
using Engine.Serialization;

namespace Game
{
	// Token: 0x02000285 RID: 645
	[HumanReadableConverter(typeof(EntityReference))]
	public class EntityReferenceHumanReadableConverter : IHumanReadableConverter
	{
		// Token: 0x0600147E RID: 5246 RVA: 0x00099668 File Offset: 0x00097868
		public string ConvertToString(object value)
		{
			return ((EntityReference)value).ReferenceString;
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x00099683 File Offset: 0x00097883
		public object ConvertFromString(Type type, string data)
		{
			return EntityReference.FromReferenceString(data);
		}
	}
}
