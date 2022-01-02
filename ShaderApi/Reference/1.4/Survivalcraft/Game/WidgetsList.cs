using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Game
{
	// Token: 0x020003AD RID: 941
	public class WidgetsList : IEnumerable<Widget>, IEnumerable
	{
		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06001D3B RID: 7483 RVA: 0x000E0319 File Offset: 0x000DE519
		public int Count
		{
			get
			{
				return this.m_widgets.Count;
			}
		}

		// Token: 0x1700050A RID: 1290
		public Widget this[int index]
		{
			get
			{
				return this.m_widgets[index];
			}
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x000E0334 File Offset: 0x000DE534
		public WidgetsList(ContainerWidget containerWidget)
		{
			this.m_containerWidget = containerWidget;
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x000E034E File Offset: 0x000DE54E
		public void Add(Widget widget)
		{
			this.Insert(this.Count, widget);
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x000E035D File Offset: 0x000DE55D
		public void Add(params Widget[] widgets)
		{
			this.AddRange(widgets);
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x000E0368 File Offset: 0x000DE568
		public void AddRange(IEnumerable<Widget> widgets)
		{
			foreach (Widget widget in widgets)
			{
				this.Add(widget);
			}
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x000E03B0 File Offset: 0x000DE5B0
		public void Insert(int index, Widget widget)
		{
			if (this.m_widgets.Contains(widget))
			{
				throw new InvalidOperationException("Child widget already present in container.");
			}
			if (index < 0 || index > this.m_widgets.Count)
			{
				throw new InvalidOperationException("Widget index out of range.");
			}
			widget.ChangeParent(this.m_containerWidget);
			this.m_widgets.Insert(index, widget);
			this.m_containerWidget.WidgetAdded(widget);
			this.m_version++;
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x000E0428 File Offset: 0x000DE628
		public void InsertBefore(Widget beforeWidget, Widget widget)
		{
			int num = this.m_widgets.IndexOf(beforeWidget);
			if (num < 0)
			{
				throw new InvalidOperationException("Widget not present in container.");
			}
			this.Insert(num, widget);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x000E045C File Offset: 0x000DE65C
		public void InsertAfter(Widget afterWidget, Widget widget)
		{
			int num = this.m_widgets.IndexOf(afterWidget);
			if (num < 0)
			{
				throw new InvalidOperationException("Widget not present in container.");
			}
			this.Insert(num + 1, widget);
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x000E0490 File Offset: 0x000DE690
		public void Remove(Widget widget)
		{
			int num = this.IndexOf(widget);
			if (num >= 0)
			{
				this.RemoveAt(num);
				return;
			}
			throw new InvalidOperationException("Child widget not present in container.");
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x000E04BC File Offset: 0x000DE6BC
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.m_widgets.Count)
			{
				throw new InvalidOperationException("Widget index out of range.");
			}
			Widget widget = this.m_widgets[index];
			widget.ChangeParent(null);
			this.m_widgets.RemoveAt(index);
			this.m_containerWidget.WidgetRemoved(widget);
			this.m_version--;
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x000E0520 File Offset: 0x000DE720
		public void Clear()
		{
			while (this.Count > 0)
			{
				this.RemoveAt(this.Count - 1);
			}
		}

		// Token: 0x06001D47 RID: 7495 RVA: 0x000E053B File Offset: 0x000DE73B
		public int IndexOf(Widget widget)
		{
			return this.m_widgets.IndexOf(widget);
		}

		// Token: 0x06001D48 RID: 7496 RVA: 0x000E0549 File Offset: 0x000DE749
		public bool Contains(Widget widget)
		{
			return this.m_widgets.Contains(widget);
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x000E0558 File Offset: 0x000DE758
		public Widget Find(string name, Type type, bool throwIfNotFound = true)
		{
			foreach (Widget widget in this.m_widgets)
			{
				if ((name == null || (widget.Name != null && widget.Name == name)) && (type == null || type == widget.GetType() || widget.GetType().GetTypeInfo().IsSubclassOf(type)))
				{
					return widget;
				}
				ContainerWidget containerWidget = widget as ContainerWidget;
				if (containerWidget != null)
				{
					Widget widget2 = containerWidget.Children.Find(name, type, false);
					if (widget2 != null)
					{
						return widget2;
					}
				}
			}
			if (throwIfNotFound)
			{
				throw new Exception(string.Format("Required widget \"{0}\" of type \"{1}\" not found.", name, type));
			}
			return null;
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x000E0628 File Offset: 0x000DE828
		public Widget Find(string name, bool throwIfNotFound = true)
		{
			return this.Find(name, null, throwIfNotFound);
		}

		// Token: 0x06001D4B RID: 7499 RVA: 0x000E0633 File Offset: 0x000DE833
		public T Find<T>(string name, bool throwIfNotFound = true) where T : class
		{
			return this.Find(name, typeof(T), throwIfNotFound) as T;
		}

		// Token: 0x06001D4C RID: 7500 RVA: 0x000E0651 File Offset: 0x000DE851
		public T Find<T>(bool throwIfNotFound = true) where T : class
		{
			return this.Find(null, typeof(T), throwIfNotFound) as T;
		}

		// Token: 0x06001D4D RID: 7501 RVA: 0x000E066F File Offset: 0x000DE86F
		public WidgetsList.Enumerator GetEnumerator()
		{
			return new WidgetsList.Enumerator(this);
		}

		// Token: 0x06001D4E RID: 7502 RVA: 0x000E0677 File Offset: 0x000DE877
		IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator()
		{
			return new WidgetsList.Enumerator(this);
		}

		// Token: 0x06001D4F RID: 7503 RVA: 0x000E0684 File Offset: 0x000DE884
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new WidgetsList.Enumerator(this);
		}

		// Token: 0x040013BF RID: 5055
		public ContainerWidget m_containerWidget;

		// Token: 0x040013C0 RID: 5056
		public List<Widget> m_widgets = new List<Widget>();

		// Token: 0x040013C1 RID: 5057
		public int m_version;

		// Token: 0x02000591 RID: 1425
		public struct Enumerator : IEnumerator<Widget>, IDisposable, IEnumerator
		{
			// Token: 0x17000599 RID: 1433
			// (get) Token: 0x0600235A RID: 9050 RVA: 0x000EF04E File Offset: 0x000ED24E
			public Widget Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x1700059A RID: 1434
			// (get) Token: 0x0600235B RID: 9051 RVA: 0x000EF056 File Offset: 0x000ED256
			object IEnumerator.Current
			{
				get
				{
					return this.m_current;
				}
			}

			// Token: 0x0600235C RID: 9052 RVA: 0x000EF05E File Offset: 0x000ED25E
			public Enumerator(WidgetsList collection)
			{
				this.m_collection = collection;
				this.m_current = null;
				this.m_index = 0;
				this.m_version = collection.m_version;
			}

			// Token: 0x0600235D RID: 9053 RVA: 0x000EF081 File Offset: 0x000ED281
			public void Dispose()
			{
			}

			// Token: 0x0600235E RID: 9054 RVA: 0x000EF084 File Offset: 0x000ED284
			public bool MoveNext()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("WidgetsList was modified, enumeration cannot continue.");
				}
				if (this.m_index < this.m_collection.m_widgets.Count)
				{
					this.m_current = this.m_collection.m_widgets[this.m_index];
					this.m_index++;
					return true;
				}
				this.m_current = null;
				return false;
			}

			// Token: 0x0600235F RID: 9055 RVA: 0x000EF0FB File Offset: 0x000ED2FB
			public void Reset()
			{
				if (this.m_collection.m_version != this.m_version)
				{
					throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
				}
				this.m_index = 0;
				this.m_current = null;
			}

			// Token: 0x04001A05 RID: 6661
			public WidgetsList m_collection;

			// Token: 0x04001A06 RID: 6662
			public Widget m_current;

			// Token: 0x04001A07 RID: 6663
			public int m_index;

			// Token: 0x04001A08 RID: 6664
			public int m_version;
		}
	}
}
