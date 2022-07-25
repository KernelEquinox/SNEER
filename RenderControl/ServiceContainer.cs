using System;
using System.Collections.Generic;

namespace WpfXnaRenderControl
{
	// Token: 0x02000006 RID: 6
	public class ServiceContainer : IServiceProvider
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002759 File Offset: 0x00000959
		public void AddService<T>(T service)
		{
			this._services.Add(typeof(T), service);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002778 File Offset: 0x00000978
		public object GetService(Type serviceType)
		{
			object result;
			this._services.TryGetValue(serviceType, out result);
			return result;
		}

		// Token: 0x0400000E RID: 14
		private Dictionary<Type, object> _services = new Dictionary<Type, object>();
	}
}
