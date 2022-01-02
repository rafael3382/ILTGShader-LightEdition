using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Game.Properties
{
	// Token: 0x020003AE RID: 942
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06001D50 RID: 7504 RVA: 0x000E0691 File Offset: 0x000DE891
		internal Resources()
		{
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06001D51 RID: 7505 RVA: 0x000E0699 File Offset: 0x000DE899
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Game.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06001D52 RID: 7506 RVA: 0x000E06C5 File Offset: 0x000DE8C5
		// (set) Token: 0x06001D53 RID: 7507 RVA: 0x000E06CC File Offset: 0x000DE8CC
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x040013C2 RID: 5058
		public static ResourceManager resourceMan;

		// Token: 0x040013C3 RID: 5059
		public static CultureInfo resourceCulture;
	}
}
