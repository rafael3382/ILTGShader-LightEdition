using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x020003A9 RID: 937
	public class Widget : IDisposable
	{
		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06001C9C RID: 7324 RVA: 0x000DDE7E File Offset: 0x000DC07E
		// (set) Token: 0x06001C9D RID: 7325 RVA: 0x000DDE88 File Offset: 0x000DC088
		public WidgetInput WidgetsHierarchyInput
		{
			get
			{
				return this.m_widgetsHierarchyInput;
			}
			set
			{
				if (value == null)
				{
					if (this.m_widgetsHierarchyInput != null)
					{
						this.m_widgetsHierarchyInput.m_widget = null;
						this.m_widgetsHierarchyInput = null;
					}
					return;
				}
				if (value.m_widget != null && value.m_widget != this)
				{
					throw new InvalidOperationException("WidgetInput already assigned to another widget.");
				}
				value.m_widget = this;
				this.m_widgetsHierarchyInput = value;
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06001C9E RID: 7326 RVA: 0x000DDEE0 File Offset: 0x000DC0E0
		public WidgetInput Input
		{
			get
			{
				Widget widget = this;
				while (widget.WidgetsHierarchyInput == null)
				{
					widget = widget.ParentWidget;
					if (widget == null)
					{
						return WidgetInput.EmptyInput;
					}
				}
				return widget.WidgetsHierarchyInput;
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06001C9F RID: 7327 RVA: 0x000DDF0D File Offset: 0x000DC10D
		// (set) Token: 0x06001CA0 RID: 7328 RVA: 0x000DDF15 File Offset: 0x000DC115
		public Matrix LayoutTransform
		{
			get
			{
				return this.m_layoutTransform;
			}
			set
			{
				this.m_layoutTransform = value;
				this.m_isLayoutTransformIdentity = (value == Matrix.Identity);
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06001CA1 RID: 7329 RVA: 0x000DDF2F File Offset: 0x000DC12F
		// (set) Token: 0x06001CA2 RID: 7330 RVA: 0x000DDF37 File Offset: 0x000DC137
		public Matrix RenderTransform
		{
			get
			{
				return this.m_renderTransform;
			}
			set
			{
				this.m_renderTransform = value;
				this.m_isRenderTransformIdentity = (value == Matrix.Identity);
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06001CA3 RID: 7331 RVA: 0x000DDF51 File Offset: 0x000DC151
		public Matrix GlobalTransform
		{
			get
			{
				return this.m_globalTransform;
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06001CA4 RID: 7332 RVA: 0x000DDF5C File Offset: 0x000DC15C
		public float GlobalScale
		{
			get
			{
				if (this.m_globalScale == null)
				{
					this.m_globalScale = new float?(this.m_globalTransform.Right.Length());
				}
				return this.m_globalScale.Value;
			}
		}

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06001CA5 RID: 7333 RVA: 0x000DDF9F File Offset: 0x000DC19F
		public Matrix InvertedGlobalTransform
		{
			get
			{
				if (this.m_invertedGlobalTransform == null)
				{
					this.m_invertedGlobalTransform = new Matrix?(Matrix.Invert(this.m_globalTransform));
				}
				return this.m_invertedGlobalTransform.Value;
			}
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06001CA6 RID: 7334 RVA: 0x000DDFCF File Offset: 0x000DC1CF
		public BoundingRectangle GlobalBounds
		{
			get
			{
				return this.m_globalBounds;
			}
		}

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06001CA7 RID: 7335 RVA: 0x000DDFD7 File Offset: 0x000DC1D7
		// (set) Token: 0x06001CA8 RID: 7336 RVA: 0x000DDFDF File Offset: 0x000DC1DF
		public Color ColorTransform
		{
			get
			{
				return this.m_colorTransform;
			}
			set
			{
				this.m_colorTransform = value;
			}
		}

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001CA9 RID: 7337 RVA: 0x000DDFE8 File Offset: 0x000DC1E8
		public Color GlobalColorTransform
		{
			get
			{
				return this.m_globalColorTransform;
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06001CAA RID: 7338 RVA: 0x000DDFF0 File Offset: 0x000DC1F0
		// (set) Token: 0x06001CAB RID: 7339 RVA: 0x000DDFF8 File Offset: 0x000DC1F8
		public virtual string Name { get; set; }

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001CAC RID: 7340 RVA: 0x000DE001 File Offset: 0x000DC201
		// (set) Token: 0x06001CAD RID: 7341 RVA: 0x000DE009 File Offset: 0x000DC209
		public object Tag { get; set; }

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06001CAE RID: 7342 RVA: 0x000DE012 File Offset: 0x000DC212
		// (set) Token: 0x06001CAF RID: 7343 RVA: 0x000DE01A File Offset: 0x000DC21A
		public virtual bool IsVisible
		{
			get
			{
				return this.m_isVisible;
			}
			set
			{
				if (value != this.m_isVisible)
				{
					this.m_isVisible = value;
					if (!this.m_isVisible)
					{
						this.UpdateCeases();
					}
				}
			}
		}

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06001CB0 RID: 7344 RVA: 0x000DE03A File Offset: 0x000DC23A
		// (set) Token: 0x06001CB1 RID: 7345 RVA: 0x000DE042 File Offset: 0x000DC242
		public virtual bool IsEnabled
		{
			get
			{
				return this.m_isEnabled;
			}
			set
			{
				if (value != this.m_isEnabled)
				{
					this.m_isEnabled = value;
					if (!this.m_isEnabled)
					{
						this.UpdateCeases();
					}
				}
			}
		}

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001CB2 RID: 7346 RVA: 0x000DE062 File Offset: 0x000DC262
		// (set) Token: 0x06001CB3 RID: 7347 RVA: 0x000DE06A File Offset: 0x000DC26A
		public virtual bool IsHitTestVisible { get; set; }

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06001CB4 RID: 7348 RVA: 0x000DE073 File Offset: 0x000DC273
		public bool IsVisibleGlobal
		{
			get
			{
				return this.IsVisible && (this.ParentWidget == null || this.ParentWidget.IsVisibleGlobal);
			}
		}

		// Token: 0x170004DA RID: 1242
		// (get) Token: 0x06001CB5 RID: 7349 RVA: 0x000DE094 File Offset: 0x000DC294
		public bool IsEnabledGlobal
		{
			get
			{
				return this.IsEnabled && (this.ParentWidget == null || this.ParentWidget.IsEnabledGlobal);
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06001CB6 RID: 7350 RVA: 0x000DE0B5 File Offset: 0x000DC2B5
		// (set) Token: 0x06001CB7 RID: 7351 RVA: 0x000DE0BD File Offset: 0x000DC2BD
		public bool ClampToBounds { get; set; }

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06001CB8 RID: 7352 RVA: 0x000DE0C6 File Offset: 0x000DC2C6
		// (set) Token: 0x06001CB9 RID: 7353 RVA: 0x000DE0CE File Offset: 0x000DC2CE
		public virtual Vector2 Margin { get; set; }

		// Token: 0x170004DD RID: 1245
		// (get) Token: 0x06001CBA RID: 7354 RVA: 0x000DE0D7 File Offset: 0x000DC2D7
		// (set) Token: 0x06001CBB RID: 7355 RVA: 0x000DE0DF File Offset: 0x000DC2DF
		public virtual WidgetAlignment HorizontalAlignment { get; set; }

		// Token: 0x170004DE RID: 1246
		// (get) Token: 0x06001CBC RID: 7356 RVA: 0x000DE0E8 File Offset: 0x000DC2E8
		// (set) Token: 0x06001CBD RID: 7357 RVA: 0x000DE0F0 File Offset: 0x000DC2F0
		public virtual WidgetAlignment VerticalAlignment { get; set; }

		// Token: 0x170004DF RID: 1247
		// (get) Token: 0x06001CBE RID: 7358 RVA: 0x000DE0F9 File Offset: 0x000DC2F9
		public Vector2 ActualSize
		{
			get
			{
				return this.m_actualSize;
			}
		}

		// Token: 0x170004E0 RID: 1248
		// (get) Token: 0x06001CBF RID: 7359 RVA: 0x000DE101 File Offset: 0x000DC301
		// (set) Token: 0x06001CC0 RID: 7360 RVA: 0x000DE109 File Offset: 0x000DC309
		public Vector2 DesiredSize
		{
			get
			{
				return this.m_desiredSize;
			}
			set
			{
				this.m_desiredSize = value;
			}
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x06001CC1 RID: 7361 RVA: 0x000DE112 File Offset: 0x000DC312
		public Vector2 ParentDesiredSize
		{
			get
			{
				return this.m_parentDesiredSize;
			}
		}

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x06001CC2 RID: 7362 RVA: 0x000DE11A File Offset: 0x000DC31A
		// (set) Token: 0x06001CC3 RID: 7363 RVA: 0x000DE122 File Offset: 0x000DC322
		public bool IsUpdateEnabled { get; set; } = true;

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x06001CC4 RID: 7364 RVA: 0x000DE12B File Offset: 0x000DC32B
		// (set) Token: 0x06001CC5 RID: 7365 RVA: 0x000DE133 File Offset: 0x000DC333
		public bool IsDrawEnabled { get; set; } = true;

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06001CC6 RID: 7366 RVA: 0x000DE13C File Offset: 0x000DC33C
		// (set) Token: 0x06001CC7 RID: 7367 RVA: 0x000DE144 File Offset: 0x000DC344
		public bool IsDrawRequired { get; set; }

		// Token: 0x170004E5 RID: 1253
		// (get) Token: 0x06001CC8 RID: 7368 RVA: 0x000DE14D File Offset: 0x000DC34D
		// (set) Token: 0x06001CC9 RID: 7369 RVA: 0x000DE155 File Offset: 0x000DC355
		public bool IsOverdrawRequired { get; set; }

		// Token: 0x170004E6 RID: 1254
		// (set) Token: 0x06001CCA RID: 7370 RVA: 0x000DE15E File Offset: 0x000DC35E
		public XElement Style
		{
			set
			{
				this.LoadContents(null, value);
			}
		}

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06001CCB RID: 7371 RVA: 0x000DE168 File Offset: 0x000DC368
		// (set) Token: 0x06001CCC RID: 7372 RVA: 0x000DE170 File Offset: 0x000DC370
		public ContainerWidget ParentWidget { get; set; }

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06001CCD RID: 7373 RVA: 0x000DE179 File Offset: 0x000DC379
		public Widget RootWidget
		{
			get
			{
				if (this.ParentWidget == null)
				{
					return this;
				}
				return this.ParentWidget.RootWidget;
			}
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x000DE190 File Offset: 0x000DC390
		public Widget()
		{
			this.IsVisible = true;
			this.IsHitTestVisible = true;
			this.IsEnabled = true;
			this.DesiredSize = new Vector2(float.PositiveInfinity);
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x000DE210 File Offset: 0x000DC410
		public static Widget LoadWidget(object eventsTarget, XElement node, ContainerWidget parentWidget)
		{
			if (node.Name.LocalName.Contains("."))
			{
				throw new NotImplementedException("Node property specification not implemented.");
			}
			Widget widget = Activator.CreateInstance(Widget.FindTypeFromXmlName(node.Name.LocalName, node.Name.NamespaceName)) as Widget;
			if (widget == null)
			{
				throw new Exception("Type \"" + node.Name.LocalName + "\" is not a Widget.");
			}
			if (parentWidget != null)
			{
				parentWidget.Children.Add(widget);
			}
			widget.LoadContents(eventsTarget, node);
			return widget;
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000DE2A0 File Offset: 0x000DC4A0
		public void LoadContents(object eventsTarget, XElement node)
		{
			this.LoadProperties(eventsTarget, node);
			this.LoadChildren(eventsTarget, node);
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x000DE2B4 File Offset: 0x000DC4B4
		public void LoadProperties(object eventsTarget, XElement node)
		{
			IEnumerable<PropertyInfo> runtimeProperties = base.GetType().GetRuntimeProperties();
			using (IEnumerator<XAttribute> enumerator = node.Attributes().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XAttribute attribute = enumerator.Current;
					if (!attribute.IsNamespaceDeclaration && !attribute.Name.LocalName.StartsWith("_"))
					{
						if (attribute.Name.LocalName.Contains('.'))
						{
							string[] array = attribute.Name.LocalName.Split(new char[]
							{
								'.'
							});
							if (array.Length != 2)
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Attached property reference must have form \"TypeName.PropertyName\", property \"",
									attribute.Name.LocalName,
									"\" in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							Type type = Widget.FindTypeFromXmlName(array[0], (attribute.Name.NamespaceName != string.Empty) ? attribute.Name.NamespaceName : node.Name.NamespaceName);
							string setterName = "Set" + array[1];
							MethodInfo methodInfo = type.GetRuntimeMethods().FirstOrDefault((MethodInfo mi) => mi.Name == setterName && mi.IsPublic && mi.IsStatic);
							if (!(methodInfo != null))
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Attached property public static setter method \"",
									setterName,
									"\" not found, property \"",
									attribute.Name.LocalName,
									"\" in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length != 2 || !(parameters[0].ParameterType == typeof(Widget)))
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Attached property setter method must take 2 parameters and first one must be of type Widget, property \"",
									attribute.Name.LocalName,
									"\" in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							object obj = HumanReadableConverter.ConvertFromString(parameters[1].ParameterType, attribute.Value);
							methodInfo.Invoke(null, new object[]
							{
								this,
								obj
							});
						}
						else
						{
							PropertyInfo propertyInfo = (from pi in runtimeProperties
							where pi.Name == attribute.Name.LocalName
							select pi).FirstOrDefault<PropertyInfo>();
							if (!(propertyInfo != null))
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Property \"",
									attribute.Name.LocalName,
									"\" not found in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							if (attribute.Value.StartsWith("{") && attribute.Value.EndsWith("}"))
							{
								string name = attribute.Value.Substring(1, attribute.Value.Length - 2);
								object value = ContentManager.Get(propertyInfo.PropertyType, name, null);
								propertyInfo.SetValue(this, value, null);
							}
							else
							{
								object obj2 = HumanReadableConverter.ConvertFromString(propertyInfo.PropertyType, attribute.Value);
								if (propertyInfo.PropertyType == typeof(string))
								{
									obj2 = ((string)obj2).Replace("\\n", "\n").Replace("\\t", "\t");
								}
								propertyInfo.SetValue(this, obj2, null);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x000DE6A8 File Offset: 0x000DC8A8
		public void LoadChildren(object eventsTarget, XElement node)
		{
			if (node.HasElements)
			{
				ContainerWidget containerWidget = this as ContainerWidget;
				if (containerWidget == null)
				{
					throw new Exception("Type \"" + node.Name.LocalName + "\" is not a ContainerWidget, but it contains child widgets.");
				}
				foreach (XElement node2 in node.Elements())
				{
					if (Widget.IsNodeIncludedOnCurrentPlatform(node2))
					{
						Widget widget = null;
						string attributeValue = XmlUtils.GetAttributeValue<string>(node2, "Name", null);
						if (attributeValue != null)
						{
							widget = containerWidget.Children.Find(attributeValue, false);
						}
						if (widget != null)
						{
							widget.LoadContents(eventsTarget, node2);
						}
						else
						{
							Widget.LoadWidget(eventsTarget, node2, containerWidget);
						}
					}
				}
			}
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x000DE764 File Offset: 0x000DC964
		public bool IsChildWidgetOf(ContainerWidget containerWidget)
		{
			return containerWidget == this.ParentWidget || (this.ParentWidget != null && this.ParentWidget.IsChildWidgetOf(containerWidget));
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x000DE787 File Offset: 0x000DC987
		public virtual void ChangeParent(ContainerWidget parentWidget)
		{
			if (parentWidget != this.ParentWidget)
			{
				this.ParentWidget = parentWidget;
				if (parentWidget == null)
				{
					this.UpdateCeases();
				}
			}
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x000DE7A4 File Offset: 0x000DC9A4
		public void Measure(Vector2 parentAvailableSize)
		{
			if (this.MeasureOverride1 != null)
			{
				this.MeasureOverride1(parentAvailableSize);
				return;
			}
			this.MeasureOverride(parentAvailableSize);
			if (this.DesiredSize.X != float.PositiveInfinity && this.DesiredSize.Y != float.PositiveInfinity)
			{
				BoundingRectangle boundingRectangle = this.TransformBoundsToParent(this.DesiredSize);
				this.m_parentDesiredSize = boundingRectangle.Size();
				this.m_parentOffset = -boundingRectangle.Min;
				return;
			}
			this.m_parentDesiredSize = this.DesiredSize;
			this.m_parentOffset = Vector2.Zero;
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x000DE836 File Offset: 0x000DCA36
		public virtual void MeasureOverride(Vector2 parentAvailableSize)
		{
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x000DE838 File Offset: 0x000DCA38
		public void Arrange(Vector2 position, Vector2 parentActualSize)
		{
			float num = this.m_layoutTransform.M11 * this.m_layoutTransform.M11;
			float num2 = this.m_layoutTransform.M12 * this.m_layoutTransform.M12;
			float num3 = this.m_layoutTransform.M21 * this.m_layoutTransform.M21;
			float num4 = this.m_layoutTransform.M22 * this.m_layoutTransform.M22;
			this.m_actualSize.X = (num * parentActualSize.X + num3 * parentActualSize.Y) / (num + num3);
			this.m_actualSize.Y = (num2 * parentActualSize.X + num4 * parentActualSize.Y) / (num2 + num4);
			this.m_parentOffset = -this.TransformBoundsToParent(this.m_actualSize).Min;
			this.m_globalColorTransform = ((this.ParentWidget != null) ? (this.ParentWidget.m_globalColorTransform * this.m_colorTransform) : this.m_colorTransform);
			if (this.m_isRenderTransformIdentity)
			{
				this.m_globalTransform = this.m_layoutTransform;
			}
			else
			{
				this.m_globalTransform = (this.m_isLayoutTransformIdentity ? this.m_renderTransform : (this.m_renderTransform * this.m_layoutTransform));
			}
			this.m_globalTransform.M41 = this.m_globalTransform.M41 + (position.X + this.m_parentOffset.X);
			this.m_globalTransform.M42 = this.m_globalTransform.M42 + (position.Y + this.m_parentOffset.Y);
			if (this.ParentWidget != null)
			{
				this.m_globalTransform *= this.ParentWidget.GlobalTransform;
			}
			this.m_invertedGlobalTransform = null;
			this.m_globalScale = null;
			this.m_globalBounds = this.TransformBoundsToGlobal(this.m_actualSize);
			this.ArrangeOverride();
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x000DEA00 File Offset: 0x000DCC00
		public virtual void ArrangeOverride()
		{
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x000DEA02 File Offset: 0x000DCC02
		public virtual void UpdateCeases()
		{
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x000DEA04 File Offset: 0x000DCC04
		public virtual void Update()
		{
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x000DEA06 File Offset: 0x000DCC06
		public virtual void Draw(Widget.DrawContext dc)
		{
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x000DEA08 File Offset: 0x000DCC08
		public virtual void Overdraw(Widget.DrawContext dc)
		{
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x000DEA0C File Offset: 0x000DCC0C
		public virtual bool HitTest(Vector2 point)
		{
			Vector2 vector = this.ScreenToWidget(point);
			return vector.X >= 0f && vector.Y >= 0f && vector.X <= this.ActualSize.X && vector.Y <= this.ActualSize.Y;
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x000DEA66 File Offset: 0x000DCC66
		public Widget HitTestGlobal(Vector2 point, Func<Widget, bool> predicate = null)
		{
			return Widget.HitTestGlobal(this.RootWidget, point, predicate);
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x000DEA75 File Offset: 0x000DCC75
		public Vector2 ScreenToWidget(Vector2 p)
		{
			return Vector2.Transform(p, this.InvertedGlobalTransform);
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x000DEA83 File Offset: 0x000DCC83
		public Vector2 WidgetToScreen(Vector2 p)
		{
			return Vector2.Transform(p, this.GlobalTransform);
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x000DEA91 File Offset: 0x000DCC91
		public virtual void Dispose()
		{
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x000DEA94 File Offset: 0x000DCC94
		public static bool TestOverlap(Widget w1, Widget w2)
		{
			return w2.m_globalBounds.Min.X < w1.m_globalBounds.Max.X - 0.001f && w2.m_globalBounds.Min.Y < w1.m_globalBounds.Max.Y - 0.001f && w1.m_globalBounds.Min.X < w2.m_globalBounds.Max.X - 0.001f && w1.m_globalBounds.Min.Y < w2.m_globalBounds.Max.Y - 0.001f;
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x000DEB4C File Offset: 0x000DCD4C
		public static bool IsNodeIncludedOnCurrentPlatform(XElement node)
		{
			string attributeValue = XmlUtils.GetAttributeValue<string>(node, "_IncludePlatforms", null);
			string attributeValue2 = XmlUtils.GetAttributeValue<string>(node, "_ExcludePlatforms", null);
			if (attributeValue != null && attributeValue2 == null)
			{
				if (attributeValue.Split(new char[]
				{
					' '
				}).Contains(VersionsManager.Platform.ToString()))
				{
					return true;
				}
			}
			else
			{
				if (attributeValue2 == null || attributeValue != null)
				{
					return true;
				}
				if (!attributeValue2.Split(new char[]
				{
					' '
				}).Contains(VersionsManager.Platform.ToString()))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x000DEBDC File Offset: 0x000DCDDC
		public static void UpdateWidgetsHierarchy(Widget rootWidget)
		{
			if (rootWidget.IsUpdateEnabled)
			{
				bool isMouseVisible = false;
				Widget.UpdateWidgetsHierarchy(rootWidget, ref isMouseVisible);
				Mouse.IsMouseVisible = isMouseVisible;
			}
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x000DEC01 File Offset: 0x000DCE01
		public static void LayoutWidgetsHierarchy(Widget rootWidget, Vector2 availableSize)
		{
			rootWidget.Measure(availableSize);
			rootWidget.Arrange(Vector2.Zero, availableSize);
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x000DEC18 File Offset: 0x000DCE18
		public static void DrawWidgetsHierarchy(Widget rootWidget)
		{
			Widget.DrawContext drawContext = (Widget.m_drawContextsCache.Count > 0) ? Widget.m_drawContextsCache.Dequeue() : new Widget.DrawContext();
			try
			{
				drawContext.DrawWidgetsHierarchy(rootWidget);
			}
			finally
			{
				Widget.m_drawContextsCache.Enqueue(drawContext);
			}
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x000DEC6C File Offset: 0x000DCE6C
		public BoundingRectangle TransformBoundsToParent(Vector2 size)
		{
			float num = this.m_layoutTransform.M11 * size.X;
			float num2 = this.m_layoutTransform.M21 * size.Y;
			float x = num + num2;
			float num3 = this.m_layoutTransform.M12 * size.X;
			float num4 = this.m_layoutTransform.M22 * size.Y;
			float x2 = num3 + num4;
			float x3 = MathUtils.Min(0f, num, num2, x);
			float x4 = MathUtils.Max(0f, num, num2, x);
			float y = MathUtils.Min(0f, num3, num4, x2);
			float y2 = MathUtils.Max(0f, num3, num4, x2);
			return new BoundingRectangle(x3, y, x4, y2);
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x000DED1C File Offset: 0x000DCF1C
		public BoundingRectangle TransformBoundsToGlobal(Vector2 size)
		{
			float num = this.m_globalTransform.M11 * size.X;
			float num2 = this.m_globalTransform.M21 * size.Y;
			float x = num + num2;
			float num3 = this.m_globalTransform.M12 * size.X;
			float num4 = this.m_globalTransform.M22 * size.Y;
			float x2 = num3 + num4;
			float num5 = MathUtils.Min(0f, num, num2, x);
			float num6 = MathUtils.Max(0f, num, num2, x);
			float num7 = MathUtils.Min(0f, num3, num4, x2);
			float y = MathUtils.Max(0f, num3, num4, x2) + this.m_globalTransform.M42;
			return new BoundingRectangle(num5 + this.m_globalTransform.M41, num7 + this.m_globalTransform.M42, num6 + this.m_globalTransform.M41, y);
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x000DEDFC File Offset: 0x000DCFFC
		public static Type FindTypeFromXmlName(string name, string namespaceName)
		{
			if (string.IsNullOrEmpty(namespaceName))
			{
				throw new InvalidOperationException("Namespace must be specified when creating types in XML.");
			}
			Uri uri = new Uri(namespaceName);
			if (uri.Scheme == "runtime-namespace")
			{
				return TypeCache.FindType(uri.AbsolutePath + "." + name, false, true);
			}
			throw new InvalidOperationException("Unknown uri scheme when loading widget. Scheme must be runtime-namespace.");
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x000DEE58 File Offset: 0x000DD058
		public static Widget HitTestGlobal(Widget widget, Vector2 point, Func<Widget, bool> predicate)
		{
			if (widget != null && widget.IsVisible && (!widget.ClampToBounds || widget.HitTest(point)))
			{
				ContainerWidget containerWidget = widget as ContainerWidget;
				if (containerWidget != null)
				{
					WidgetsList children = containerWidget.Children;
					for (int i = children.Count - 1; i >= 0; i--)
					{
						Widget widget2 = Widget.HitTestGlobal(children[i], point, predicate);
						if (widget2 != null)
						{
							return widget2;
						}
					}
				}
				if (widget.IsHitTestVisible && widget.HitTest(point) && (predicate == null || predicate(widget)))
				{
					return widget;
				}
			}
			return null;
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x000DEEDC File Offset: 0x000DD0DC
		public static void UpdateWidgetsHierarchy(Widget widget, ref bool isMouseCursorVisible)
		{
			if (!widget.IsVisible || !widget.IsEnabled)
			{
				return;
			}
			if (widget.WidgetsHierarchyInput != null)
			{
				widget.WidgetsHierarchyInput.Update();
				isMouseCursorVisible |= widget.WidgetsHierarchyInput.IsMouseCursorVisible;
			}
			ContainerWidget containerWidget = widget as ContainerWidget;
			if (containerWidget != null)
			{
				WidgetsList children = containerWidget.Children;
				for (int i = children.Count - 1; i >= 0; i--)
				{
					if (i < children.Count)
					{
						Widget.UpdateWidgetsHierarchy(children[i], ref isMouseCursorVisible);
					}
				}
			}
			if (widget.Update1 == null)
			{
				widget.Update();
				return;
			}
			widget.Update1();
		}

		// Token: 0x0400135E RID: 4958
		public Action<Vector2> MeasureOverride1;

		// Token: 0x0400135F RID: 4959
		public Action Update1;

		// Token: 0x04001360 RID: 4960
		public bool m_isVisible;

		// Token: 0x04001361 RID: 4961
		public bool m_isEnabled;

		// Token: 0x04001362 RID: 4962
		public Vector2 m_actualSize;

		// Token: 0x04001363 RID: 4963
		public Vector2 m_desiredSize;

		// Token: 0x04001364 RID: 4964
		public Vector2 m_parentDesiredSize;

		// Token: 0x04001365 RID: 4965
		public BoundingRectangle m_globalBounds;

		// Token: 0x04001366 RID: 4966
		public Vector2 m_parentOffset;

		// Token: 0x04001367 RID: 4967
		public bool m_isLayoutTransformIdentity = true;

		// Token: 0x04001368 RID: 4968
		public bool m_isRenderTransformIdentity = true;

		// Token: 0x04001369 RID: 4969
		public Matrix m_layoutTransform = Matrix.Identity;

		// Token: 0x0400136A RID: 4970
		public Matrix m_renderTransform = Matrix.Identity;

		// Token: 0x0400136B RID: 4971
		public Matrix m_globalTransform = Matrix.Identity;

		// Token: 0x0400136C RID: 4972
		public Matrix? m_invertedGlobalTransform;

		// Token: 0x0400136D RID: 4973
		public float? m_globalScale;

		// Token: 0x0400136E RID: 4974
		public Color m_colorTransform = Color.White;

		// Token: 0x0400136F RID: 4975
		public Color m_globalColorTransform;

		// Token: 0x04001370 RID: 4976
		public WidgetInput m_widgetsHierarchyInput;

		// Token: 0x04001371 RID: 4977
		public static Queue<Widget.DrawContext> m_drawContextsCache = new Queue<Widget.DrawContext>();

		// Token: 0x04001372 RID: 4978
		public static int LayersLimit = -1;

		// Token: 0x04001373 RID: 4979
		public static bool DrawWidgetBounds = false;

		// Token: 0x0200058C RID: 1420
		public class DrawContext
		{
			// Token: 0x06002349 RID: 9033 RVA: 0x000EEA6A File Offset: 0x000ECC6A
			public void DrawWidgetsHierarchy(Widget rootWidget)
			{
				this.m_drawItems.Clear();
				this.CollateDrawItems(rootWidget, Display.ScissorRectangle);
				this.AssignDrawItemsLayers();
				this.RenderDrawItems();
				this.ReturnDrawItemsToCache();
			}

			// Token: 0x0600234A RID: 9034 RVA: 0x000EEA98 File Offset: 0x000ECC98
			public void CollateDrawItems(Widget widget, Rectangle scissorRectangle)
			{
				if (!widget.IsVisible || !widget.IsDrawEnabled)
				{
					return;
				}
				bool flag = widget.GlobalBounds.Intersection(new BoundingRectangle((float)scissorRectangle.Left, (float)scissorRectangle.Top, (float)scissorRectangle.Right, (float)scissorRectangle.Bottom));
				Rectangle? scissorRectangle2 = null;
				if (widget.ClampToBounds && flag)
				{
					scissorRectangle2 = new Rectangle?(scissorRectangle);
					int num = (int)MathUtils.Floor(widget.GlobalBounds.Min.X - 0.5f);
					int num2 = (int)MathUtils.Floor(widget.GlobalBounds.Min.Y - 0.5f);
					int num3 = (int)MathUtils.Ceiling(widget.GlobalBounds.Max.X - 0.5f);
					int num4 = (int)MathUtils.Ceiling(widget.GlobalBounds.Max.Y - 0.5f);
					scissorRectangle = Rectangle.Intersection(new Rectangle(num, num2, num3 - num, num4 - num2), scissorRectangle2.Value);
					Widget.DrawItem drawItemFromCache = this.GetDrawItemFromCache();
					drawItemFromCache.ScissorRectangle = new Rectangle?(scissorRectangle);
					this.m_drawItems.Add(drawItemFromCache);
				}
				if (widget.IsDrawRequired && flag)
				{
					Widget.DrawItem drawItemFromCache2 = this.GetDrawItemFromCache();
					drawItemFromCache2.Widget = widget;
					this.m_drawItems.Add(drawItemFromCache2);
				}
				if (flag || !widget.ClampToBounds)
				{
					ContainerWidget containerWidget = widget as ContainerWidget;
					if (containerWidget != null)
					{
						foreach (Widget widget2 in containerWidget.Children)
						{
							this.CollateDrawItems(widget2, scissorRectangle);
						}
					}
				}
				if (widget.IsOverdrawRequired && flag)
				{
					Widget.DrawItem drawItemFromCache3 = this.GetDrawItemFromCache();
					drawItemFromCache3.Widget = widget;
					drawItemFromCache3.IsOverdraw = true;
					this.m_drawItems.Add(drawItemFromCache3);
				}
				if (scissorRectangle2 != null)
				{
					Widget.DrawItem drawItemFromCache4 = this.GetDrawItemFromCache();
					drawItemFromCache4.ScissorRectangle = scissorRectangle2;
					this.m_drawItems.Add(drawItemFromCache4);
				}
				WidgetInput widgetsHierarchyInput = widget.WidgetsHierarchyInput;
				if (widgetsHierarchyInput == null)
				{
					return;
				}
				widgetsHierarchyInput.Draw(this);
			}

			// Token: 0x0600234B RID: 9035 RVA: 0x000EECAC File Offset: 0x000ECEAC
			public void AssignDrawItemsLayers()
			{
				for (int i = 0; i < this.m_drawItems.Count; i++)
				{
					Widget.DrawItem drawItem = this.m_drawItems[i];
					for (int j = i + 1; j < this.m_drawItems.Count; j++)
					{
						Widget.DrawItem drawItem2 = this.m_drawItems[j];
						if (drawItem.ScissorRectangle != null || drawItem2.ScissorRectangle != null)
						{
							drawItem2.Layer = MathUtils.Max(drawItem2.Layer, drawItem.Layer + 1);
						}
						else if (Widget.TestOverlap(drawItem.Widget, drawItem2.Widget))
						{
							drawItem2.Layer = MathUtils.Max(drawItem2.Layer, drawItem.Layer + 1);
						}
					}
				}
				this.m_drawItems.Sort();
			}

			// Token: 0x0600234C RID: 9036 RVA: 0x000EED74 File Offset: 0x000ECF74
			public void RenderDrawItems()
			{
				Rectangle scissorRectangle = Display.ScissorRectangle;
				int num = 0;
				foreach (Widget.DrawItem drawItem in this.m_drawItems)
				{
					if (Widget.LayersLimit >= 0 && drawItem.Layer > Widget.LayersLimit)
					{
						break;
					}
					if (drawItem.Layer != num)
					{
						num = drawItem.Layer;
						this.PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
						this.PrimitivesRenderer2D.Flush(true, int.MaxValue);
					}
					if (drawItem.Widget != null)
					{
						if (drawItem.IsOverdraw)
						{
							drawItem.Widget.Overdraw(this);
						}
						else
						{
							drawItem.Widget.Draw(this);
						}
					}
					else
					{
						Display.ScissorRectangle = Rectangle.Intersection(scissorRectangle, drawItem.ScissorRectangle.Value);
					}
				}
				this.PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
				this.PrimitivesRenderer2D.Flush(true, int.MaxValue);
				Display.ScissorRectangle = scissorRectangle;
				this.CursorPrimitivesRenderer2D.Flush(true, int.MaxValue);
			}

			// Token: 0x0600234D RID: 9037 RVA: 0x000EEEA0 File Offset: 0x000ED0A0
			public Widget.DrawItem GetDrawItemFromCache()
			{
				if (Widget.DrawContext.m_drawItemsCache.Count > 0)
				{
					Widget.DrawItem result = Widget.DrawContext.m_drawItemsCache[Widget.DrawContext.m_drawItemsCache.Count - 1];
					Widget.DrawContext.m_drawItemsCache.RemoveAt(Widget.DrawContext.m_drawItemsCache.Count - 1);
					return result;
				}
				return new Widget.DrawItem();
			}

			// Token: 0x0600234E RID: 9038 RVA: 0x000EEEEC File Offset: 0x000ED0EC
			public void ReturnDrawItemsToCache()
			{
				foreach (Widget.DrawItem drawItem in this.m_drawItems)
				{
					drawItem.Widget = null;
					drawItem.Layer = 0;
					drawItem.IsOverdraw = false;
					drawItem.ScissorRectangle = null;
					Widget.DrawContext.m_drawItemsCache.Add(drawItem);
				}
			}

			// Token: 0x040019F8 RID: 6648
			public List<Widget.DrawItem> m_drawItems = new List<Widget.DrawItem>();

			// Token: 0x040019F9 RID: 6649
			public static List<Widget.DrawItem> m_drawItemsCache = new List<Widget.DrawItem>();

			// Token: 0x040019FA RID: 6650
			public readonly PrimitivesRenderer2D PrimitivesRenderer2D = new PrimitivesRenderer2D();

			// Token: 0x040019FB RID: 6651
			public readonly PrimitivesRenderer3D PrimitivesRenderer3D = new PrimitivesRenderer3D();

			// Token: 0x040019FC RID: 6652
			public readonly PrimitivesRenderer2D CursorPrimitivesRenderer2D = new PrimitivesRenderer2D();
		}

		// Token: 0x0200058D RID: 1421
		public class DrawItem : IComparable<Widget.DrawItem>
		{
			// Token: 0x06002351 RID: 9041 RVA: 0x000EEFA4 File Offset: 0x000ED1A4
			public int CompareTo(Widget.DrawItem other)
			{
				return this.Layer - other.Layer;
			}

			// Token: 0x040019FD RID: 6653
			public int Layer;

			// Token: 0x040019FE RID: 6654
			public bool IsOverdraw;

			// Token: 0x040019FF RID: 6655
			public Widget Widget;

			// Token: 0x04001A00 RID: 6656
			public Rectangle? ScissorRectangle;
		}
	}
}
