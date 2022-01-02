using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000252 RID: 594
	public class BuildFurnitureDialog : Dialog
	{
		// Token: 0x06001375 RID: 4981 RVA: 0x00092A1C File Offset: 0x00090C1C
		public BuildFurnitureDialog(FurnitureDesign design, FurnitureDesign sourceDesign, Action<bool> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/BuildFurnitureDialog", null);
			base.LoadContents(this, node);
			this.m_nameLabel = this.Children.Find<LabelWidget>("BuildFurnitureDialog.Name", true);
			this.m_statusLabel = this.Children.Find<LabelWidget>("BuildFurnitureDialog.Status", true);
			this.m_designWidget2d = this.Children.Find<FurnitureDesignWidget>("BuildFurnitureDialog.Design2d", true);
			this.m_designWidget3d = this.Children.Find<FurnitureDesignWidget>("BuildFurnitureDialog.Design3d", true);
			this.m_nameButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.NameButton", true);
			this.m_axisButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.AxisButton", true);
			this.m_leftButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.LeftButton", true);
			this.m_rightButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.RightButton", true);
			this.m_upButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.UpButton", true);
			this.m_downButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.DownButton", true);
			this.m_mirrorButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.MirrorButton", true);
			this.m_turnRightButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.TurnRightButton", true);
			this.m_increaseResolutionButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.IncreaseResolutionButton", true);
			this.m_decreaseResolutionButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.DecreaseResolutionButton", true);
			this.m_resolutionLabel = this.Children.Find<LabelWidget>("BuildFurnitureDialog.ResolutionLabel", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.CancelButton", true);
			this.m_buildButton = this.Children.Find<ButtonWidget>("BuildFurnitureDialog.BuildButton", true);
			this.m_handler = handler;
			this.m_design = design;
			this.m_sourceDesign = sourceDesign;
			this.m_axis = 1;
			int num = 0;
			num += this.m_design.Geometry.SubsetOpaqueByFace.Sum(delegate(BlockMesh b)
			{
				if (b == null)
				{
					return 0;
				}
				return b.Indices.Count / 3;
			});
			num += this.m_design.Geometry.SubsetAlphaTestByFace.Sum(delegate(BlockMesh b)
			{
				if (b == null)
				{
					return 0;
				}
				return b.Indices.Count / 3;
			});
			this.m_isValid = (num <= 8192);
			this.m_statusLabel.Text = string.Format(LanguageControl.Get(BuildFurnitureDialog.fName, 1), num, 8192, this.m_isValid ? LanguageControl.Get(BuildFurnitureDialog.fName, 2) : LanguageControl.Get(BuildFurnitureDialog.fName, 3));
			this.m_designWidget2d.Design = this.m_design;
			this.m_designWidget3d.Design = this.m_design;
		}

		// Token: 0x06001376 RID: 4982 RVA: 0x00092CD4 File Offset: 0x00090ED4
		public override void Update()
		{
			this.m_nameLabel.Text = (string.IsNullOrEmpty(this.m_design.Name) ? this.m_design.GetDefaultName() : this.m_design.Name);
			this.m_designWidget2d.Mode = (FurnitureDesignWidget.ViewMode)this.m_axis;
			this.m_designWidget3d.Mode = FurnitureDesignWidget.ViewMode.Perspective;
			if (this.m_designWidget2d.Mode == FurnitureDesignWidget.ViewMode.Side)
			{
				this.m_axisButton.Text = LanguageControl.Get(BuildFurnitureDialog.fName, 4);
			}
			if (this.m_designWidget2d.Mode == FurnitureDesignWidget.ViewMode.Top)
			{
				this.m_axisButton.Text = LanguageControl.Get(BuildFurnitureDialog.fName, 5);
			}
			if (this.m_designWidget2d.Mode == FurnitureDesignWidget.ViewMode.Front)
			{
				this.m_axisButton.Text = LanguageControl.Get(BuildFurnitureDialog.fName, 6);
			}
			this.m_leftButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(0, this.m_axis));
			this.m_rightButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(1, this.m_axis));
			this.m_upButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(2, this.m_axis));
			this.m_downButton.IsEnabled = this.IsShiftPossible(BuildFurnitureDialog.DirectionAxisToDelta(3, this.m_axis));
			this.m_decreaseResolutionButton.IsEnabled = this.IsDecreaseResolutionPossible();
			this.m_increaseResolutionButton.IsEnabled = this.IsIncreaseResolutionPossible();
			this.m_resolutionLabel.Text = string.Format("{0}", this.m_design.Resolution);
			this.m_buildButton.IsEnabled = this.m_isValid;
			if (this.m_nameButton.IsClicked)
			{
				List<Tuple<string, Action>> list = new List<Tuple<string, Action>>();
				if (this.m_sourceDesign != null)
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(BuildFurnitureDialog.fName, 7), delegate()
					{
						this.Dismiss(false);
						DialogsManager.ShowDialog(base.ParentWidget, new TextBoxDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 10), this.m_sourceDesign.Name, 20, delegate(string s)
						{
							try
							{
								if (s != null)
								{
									this.m_sourceDesign.Name = s;
								}
							}
							catch (Exception ex)
							{
								DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Error, ex.Message, LanguageControl.Ok, null, null));
							}
						}));
					}));
					list.Add(new Tuple<string, Action>(LanguageControl.Get(BuildFurnitureDialog.fName, 8), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new TextBoxDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 11), this.m_design.Name, 20, delegate(string s)
						{
							try
							{
								if (s != null)
								{
									this.m_design.Name = s;
								}
							}
							catch (Exception ex)
							{
								DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Error, ex.Message, LanguageControl.Ok, null, null));
							}
						}));
					}));
				}
				else
				{
					list.Add(new Tuple<string, Action>(LanguageControl.Get(BuildFurnitureDialog.fName, 9), delegate()
					{
						DialogsManager.ShowDialog(base.ParentWidget, new TextBoxDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 11), this.m_design.Name, 20, delegate(string s)
						{
							try
							{
								if (s != null)
								{
									this.m_design.Name = s;
								}
							}
							catch (Exception ex)
							{
								DialogsManager.ShowDialog(base.ParentWidget, new MessageDialog(LanguageControl.Error, ex.Message, LanguageControl.Ok, null, null));
							}
						}));
					}));
				}
				if (list.Count == 1)
				{
					list[0].Item2();
				}
				else
				{
					DialogsManager.ShowDialog(base.ParentWidget, new ListSelectionDialog(LanguageControl.Get(BuildFurnitureDialog.fName, 11), list, 64f, (object t) => ((Tuple<string, Action>)t).Item1, delegate(object t)
					{
						((Tuple<string, Action>)t).Item2();
					}));
				}
			}
			if (this.m_axisButton.IsClicked)
			{
				this.m_axis = (this.m_axis + 1) % 3;
			}
			if (this.m_leftButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(0, this.m_axis));
			}
			if (this.m_rightButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(1, this.m_axis));
			}
			if (this.m_upButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(2, this.m_axis));
			}
			if (this.m_downButton.IsClicked)
			{
				this.Shift(BuildFurnitureDialog.DirectionAxisToDelta(3, this.m_axis));
			}
			if (this.m_mirrorButton.IsClicked)
			{
				this.m_design.Mirror(this.m_axis);
			}
			if (this.m_turnRightButton.IsClicked)
			{
				this.m_design.Rotate(this.m_axis, 1);
			}
			if (this.m_decreaseResolutionButton.IsClicked)
			{
				this.DecreaseResolution();
			}
			if (this.m_increaseResolutionButton.IsClicked)
			{
				this.IncreaseResolution();
			}
			if (this.m_buildButton.IsClicked && this.m_isValid)
			{
				this.Dismiss(true);
			}
			if (base.Input.Back || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(false);
			}
		}

		// Token: 0x06001377 RID: 4983 RVA: 0x000930A8 File Offset: 0x000912A8
		public bool IsShiftPossible(Point3 delta)
		{
			int resolution = this.m_design.Resolution;
			Box box = this.m_design.Box;
			box.Location += delta;
			return box.Left >= 0 && box.Top >= 0 && box.Near >= 0 && box.Right <= resolution && box.Bottom <= resolution && box.Far <= resolution;
		}

		// Token: 0x06001378 RID: 4984 RVA: 0x0009311E File Offset: 0x0009131E
		public void Shift(Point3 delta)
		{
			if (this.IsShiftPossible(delta))
			{
				this.m_design.Shift(delta);
			}
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x00093138 File Offset: 0x00091338
		public bool IsDecreaseResolutionPossible()
		{
			int resolution = this.m_design.Resolution;
			if (resolution > 2)
			{
				int num = MathUtils.Max(this.m_design.Box.Width, this.m_design.Box.Height, this.m_design.Box.Depth);
				return resolution > num;
			}
			return false;
		}

		// Token: 0x0600137A RID: 4986 RVA: 0x00093194 File Offset: 0x00091394
		public void DecreaseResolution()
		{
			if (this.IsDecreaseResolutionPossible())
			{
				int resolution = this.m_design.Resolution;
				Point3 zero = Point3.Zero;
				if (this.m_design.Box.Right >= resolution)
				{
					zero.X = -1;
				}
				if (this.m_design.Box.Bottom >= resolution)
				{
					zero.Y = -1;
				}
				if (this.m_design.Box.Far >= resolution)
				{
					zero.Z = -1;
				}
				this.m_design.Shift(zero);
				this.m_design.Resize(resolution - 1);
			}
		}

		// Token: 0x0600137B RID: 4987 RVA: 0x00093232 File Offset: 0x00091432
		public bool IsIncreaseResolutionPossible()
		{
			return this.m_design.Resolution < 8192;
		}

		// Token: 0x0600137C RID: 4988 RVA: 0x00093246 File Offset: 0x00091446
		public void IncreaseResolution()
		{
			if (this.IsIncreaseResolutionPossible())
			{
				this.m_design.Resize(this.m_design.Resolution + 1);
			}
		}

		// Token: 0x0600137D RID: 4989 RVA: 0x00093268 File Offset: 0x00091468
		public static Point3 DirectionAxisToDelta(int direction, int axis)
		{
			if (direction == 0)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, 0, 1);
				case 1:
					return new Point3(1, 0, 0);
				case 2:
					return new Point3(1, 0, 0);
				}
			}
			if (direction == 1)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, 0, -1);
				case 1:
					return new Point3(-1, 0, 0);
				case 2:
					return new Point3(-1, 0, 0);
				}
			}
			if (direction == 2)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, 1, 0);
				case 1:
					return new Point3(0, 0, 1);
				case 2:
					return new Point3(0, 1, 0);
				}
			}
			if (direction == 3)
			{
				switch (axis)
				{
				case 0:
					return new Point3(0, -1, 0);
				case 1:
					return new Point3(0, 0, -1);
				case 2:
					return new Point3(0, -1, 0);
				}
			}
			return Point3.Zero;
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x00093345 File Offset: 0x00091545
		public void Dismiss(bool result)
		{
			DialogsManager.HideDialog(this);
			this.m_handler(result);
		}

		// Token: 0x04000C2D RID: 3117
		public FurnitureDesign m_design;

		// Token: 0x04000C2E RID: 3118
		public FurnitureDesign m_sourceDesign;

		// Token: 0x04000C2F RID: 3119
		public int m_axis;

		// Token: 0x04000C30 RID: 3120
		public static string fName = "BuildFurnitureDialog";

		// Token: 0x04000C31 RID: 3121
		public Action<bool> m_handler;

		// Token: 0x04000C32 RID: 3122
		public bool m_isValid;

		// Token: 0x04000C33 RID: 3123
		public LabelWidget m_nameLabel;

		// Token: 0x04000C34 RID: 3124
		public LabelWidget m_statusLabel;

		// Token: 0x04000C35 RID: 3125
		public FurnitureDesignWidget m_designWidget2d;

		// Token: 0x04000C36 RID: 3126
		public FurnitureDesignWidget m_designWidget3d;

		// Token: 0x04000C37 RID: 3127
		public ButtonWidget m_axisButton;

		// Token: 0x04000C38 RID: 3128
		public ButtonWidget m_leftButton;

		// Token: 0x04000C39 RID: 3129
		public ButtonWidget m_rightButton;

		// Token: 0x04000C3A RID: 3130
		public ButtonWidget m_upButton;

		// Token: 0x04000C3B RID: 3131
		public ButtonWidget m_downButton;

		// Token: 0x04000C3C RID: 3132
		public ButtonWidget m_mirrorButton;

		// Token: 0x04000C3D RID: 3133
		public ButtonWidget m_turnRightButton;

		// Token: 0x04000C3E RID: 3134
		public ButtonWidget m_increaseResolutionButton;

		// Token: 0x04000C3F RID: 3135
		public ButtonWidget m_decreaseResolutionButton;

		// Token: 0x04000C40 RID: 3136
		public LabelWidget m_resolutionLabel;

		// Token: 0x04000C41 RID: 3137
		public ButtonWidget m_nameButton;

		// Token: 0x04000C42 RID: 3138
		public ButtonWidget m_buildButton;

		// Token: 0x04000C43 RID: 3139
		public ButtonWidget m_cancelButton;
	}
}
