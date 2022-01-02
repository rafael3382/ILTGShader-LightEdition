using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using GameEntitySystem;

namespace Game
{
	// Token: 0x0200038B RID: 907
	public class GameWidget : CanvasWidget
	{
		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06001AF6 RID: 6902 RVA: 0x000D3EB3 File Offset: 0x000D20B3
		// (set) Token: 0x06001AF7 RID: 6903 RVA: 0x000D3EBB File Offset: 0x000D20BB
		public ViewWidget ViewWidget { get; set; }

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001AF8 RID: 6904 RVA: 0x000D3EC4 File Offset: 0x000D20C4
		// (set) Token: 0x06001AF9 RID: 6905 RVA: 0x000D3ECC File Offset: 0x000D20CC
		public ContainerWidget GuiWidget { get; set; }

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001AFA RID: 6906 RVA: 0x000D3ED5 File Offset: 0x000D20D5
		// (set) Token: 0x06001AFB RID: 6907 RVA: 0x000D3EDD File Offset: 0x000D20DD
		public int GameWidgetIndex { get; set; }

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001AFC RID: 6908 RVA: 0x000D3EE6 File Offset: 0x000D20E6
		// (set) Token: 0x06001AFD RID: 6909 RVA: 0x000D3EEE File Offset: 0x000D20EE
		public SubsystemGameWidgets SubsystemGameWidgets { get; set; }

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06001AFE RID: 6910 RVA: 0x000D3EF7 File Offset: 0x000D20F7
		// (set) Token: 0x06001AFF RID: 6911 RVA: 0x000D3EFF File Offset: 0x000D20FF
		public PlayerData PlayerData { get; set; }

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06001B00 RID: 6912 RVA: 0x000D3F08 File Offset: 0x000D2108
		public ReadOnlyList<Camera> Cameras
		{
			get
			{
				return new ReadOnlyList<Camera>(this.m_cameras);
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06001B01 RID: 6913 RVA: 0x000D3F15 File Offset: 0x000D2115
		// (set) Token: 0x06001B02 RID: 6914 RVA: 0x000D3F20 File Offset: 0x000D2120
		public Camera ActiveCamera
		{
			get
			{
				return this.m_activeCamera;
			}
			set
			{
				if (value == null || value.GameWidget != this)
				{
					throw new InvalidOperationException("Invalid camera.");
				}
				if (!this.IsCameraAllowed(value))
				{
					value = this.FindCamera<FppCamera>(true);
				}
				if (value != this.m_activeCamera)
				{
					Camera activeCamera = this.m_activeCamera;
					this.m_activeCamera = value;
					this.m_activeCamera.Activate(activeCamera);
				}
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x06001B03 RID: 6915 RVA: 0x000D3F79 File Offset: 0x000D2179
		// (set) Token: 0x06001B04 RID: 6916 RVA: 0x000D3F81 File Offset: 0x000D2181
		public ComponentCreature Target { get; set; }

		// Token: 0x06001B05 RID: 6917 RVA: 0x000D3F8C File Offset: 0x000D218C
		public GameWidget(PlayerData playerData, int gameViewIndex)
		{
			this.PlayerData = playerData;
			this.GameWidgetIndex = gameViewIndex;
			this.SubsystemGameWidgets = playerData.SubsystemGameWidgets;
			base.LoadContents(this, ContentManager.Get<XElement>("Widgets/GameWidget", null));
			this.ViewWidget = this.Children.Find<ViewWidget>("View", true);
			this.GuiWidget = this.Children.Find<ContainerWidget>("Gui", true);
			this.m_cameras.Add(new FppCamera(this));
			this.m_cameras.Add(new DeathCamera(this));
			this.m_cameras.Add(new IntroCamera(this));
			this.m_cameras.Add(new TppCamera(this));
			this.m_cameras.Add(new DebugCamera(this));
			this.m_cameras.Add(new OrbitCamera(this));
			this.m_cameras.Add(new FixedCamera(this));
			this.m_cameras.Add(new LoadingCamera(this));
			this.m_activeCamera = this.FindCamera<LoadingCamera>(true);
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x000D409C File Offset: 0x000D229C
		public T FindCamera<T>(bool throwOnError = true) where T : Camera
		{
			T t = (T)((object)this.m_cameras.FirstOrDefault((Camera c) => c is T));
			if (t != null || !throwOnError)
			{
				return t;
			}
			throw new InvalidOperationException("Camera with type \"" + typeof(T).Name + "\" not found.");
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x000D4109 File Offset: 0x000D2309
		public bool IsEntityTarget(Entity entity)
		{
			return this.Target != null && this.Target.Entity == entity;
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x000D4123 File Offset: 0x000D2323
		public bool IsEntityFirstPersonTarget(Entity entity)
		{
			return this.IsEntityTarget(entity) && this.ActiveCamera is FppCamera;
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x000D4140 File Offset: 0x000D2340
		public override void Update()
		{
			WidgetInputDevice widgetInputDevice = this.DetermineInputDevices();
			if (base.WidgetsHierarchyInput == null || base.WidgetsHierarchyInput.Devices != widgetInputDevice)
			{
				base.WidgetsHierarchyInput = new WidgetInput(widgetInputDevice);
			}
			if (this.GuiWidget.ParentWidget == null)
			{
				Widget.UpdateWidgetsHierarchy(this.GuiWidget);
			}
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x000D4190 File Offset: 0x000D2390
		public WidgetInputDevice DetermineInputDevices()
		{
			if (this.PlayerData.SubsystemPlayers.PlayersData.Count > 0 && this.PlayerData == this.PlayerData.SubsystemPlayers.PlayersData[0])
			{
				WidgetInputDevice widgetInputDevice = WidgetInputDevice.None;
				foreach (PlayerData playerData in this.PlayerData.SubsystemPlayers.PlayersData)
				{
					if (playerData != this.PlayerData)
					{
						widgetInputDevice |= playerData.InputDevice;
					}
				}
				return (WidgetInputDevice.All & ~widgetInputDevice) | WidgetInputDevice.Touch | this.PlayerData.InputDevice;
			}
			WidgetInputDevice widgetInputDevice2 = WidgetInputDevice.None;
			foreach (PlayerData playerData2 in this.PlayerData.SubsystemPlayers.PlayersData)
			{
				if (playerData2 == this.PlayerData)
				{
					break;
				}
				widgetInputDevice2 |= playerData2.InputDevice;
			}
			return (this.PlayerData.InputDevice & ~widgetInputDevice2) | WidgetInputDevice.Touch;
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x000D42C8 File Offset: 0x000D24C8
		public bool IsCameraAllowed(Camera camera)
		{
			return !(camera is LoadingCamera);
		}

		// Token: 0x04001260 RID: 4704
		public List<Camera> m_cameras = new List<Camera>();

		// Token: 0x04001261 RID: 4705
		public Camera m_activeCamera;
	}
}
