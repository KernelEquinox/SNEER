using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace WpfXnaRenderControl.Properties
{
	// Token: 0x02000003 RID: 3
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class Resources
	{
		// Token: 0x06000004 RID: 4 RVA: 0x00002075 File Offset: 0x00000275
		internal Resources()
		{
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002080 File Offset: 0x00000280
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(Resources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("WpfXnaRenderControl.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = resourceManager;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000006 RID: 6 RVA: 0x000020BF File Offset: 0x000002BF
		// (set) Token: 0x06000007 RID: 7 RVA: 0x000020C6 File Offset: 0x000002C6
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

		// Token: 0x04000002 RID: 2
		private static ResourceManager resourceMan;

		// Token: 0x04000003 RID: 3
		private static CultureInfo resourceCulture;
	}
}
