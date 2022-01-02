using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x0200013E RID: 318
	public static class DialogsManager
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x00022FAE File Offset: 0x000211AE
		public static ReadOnlyList<Dialog> Dialogs
		{
			get
			{
				return new ReadOnlyList<Dialog>(DialogsManager.m_dialogs);
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00022FBC File Offset: 0x000211BC
		public static bool HasDialogs(Widget parentWidget)
		{
			if (parentWidget == null)
			{
				parentWidget = (ScreensManager.CurrentScreen ?? ScreensManager.RootWidget);
			}
			using (List<Dialog>.Enumerator enumerator = DialogsManager.m_dialogs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.ParentWidget == parentWidget)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00023028 File Offset: 0x00021228
		public static void ShowDialog(ContainerWidget parentWidget, Dialog dialog)
		{
			Dispatcher.Dispatch(delegate
			{
				if (!DialogsManager.m_dialogs.Contains(dialog))
				{
					if (parentWidget == null)
					{
						parentWidget = (ScreensManager.CurrentScreen ?? ScreensManager.RootWidget);
					}
					dialog.WidgetsHierarchyInput = null;
					DialogsManager.m_dialogs.Add(dialog);
					DialogsManager.AnimationData animationData = new DialogsManager.AnimationData
					{
						Direction = 1
					};
					DialogsManager.m_animationData[dialog] = animationData;
					parentWidget.Children.Add(animationData.CoverRectangle);
					if (dialog.ParentWidget != null)
					{
						dialog.ParentWidget.Children.Remove(dialog);
					}
					parentWidget.Children.Add(dialog);
					DialogsManager.UpdateDialog(dialog, animationData);
					dialog.Input.Clear();
				}
			}, false);
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002304E File Offset: 0x0002124E
		public static void HideDialog(Dialog dialog)
		{
			Dispatcher.Dispatch(delegate
			{
				if (DialogsManager.m_dialogs.Contains(dialog))
				{
					dialog.ParentWidget.Input.Clear();
					dialog.WidgetsHierarchyInput = new WidgetInput(WidgetInputDevice.None);
					DialogsManager.m_dialogs.Remove(dialog);
					DialogsManager.m_animationData[dialog].Direction = -1;
				}
			}, false);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x00023070 File Offset: 0x00021270
		public static void HideAllDialogs()
		{
			Dialog[] array = DialogsManager.m_dialogs.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				DialogsManager.HideDialog(array[i]);
			}
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x000230A0 File Offset: 0x000212A0
		public static void Update()
		{
			foreach (KeyValuePair<Dialog, DialogsManager.AnimationData> keyValuePair in DialogsManager.m_animationData)
			{
				Dialog key = keyValuePair.Key;
				DialogsManager.AnimationData value = keyValuePair.Value;
				if (value.Direction > 0)
				{
					value.Factor = MathUtils.Min(value.Factor + 6f * Time.FrameDuration, 1f);
				}
				else if (value.Direction < 0)
				{
					value.Factor = MathUtils.Max(value.Factor - 6f * Time.FrameDuration, 0f);
					if (value.Factor <= 0f)
					{
						DialogsManager.m_toRemove.Add(key);
					}
				}
				DialogsManager.UpdateDialog(key, value);
			}
			foreach (Dialog dialog in DialogsManager.m_toRemove)
			{
				DialogsManager.AnimationData animationData = DialogsManager.m_animationData[dialog];
				DialogsManager.m_animationData.Remove(dialog);
				dialog.ParentWidget.Children.Remove(dialog);
				animationData.CoverRectangle.ParentWidget.Children.Remove(animationData.CoverRectangle);
			}
			DialogsManager.m_toRemove.Clear();
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0002320C File Offset: 0x0002140C
		public static void UpdateDialog(Dialog dialog, DialogsManager.AnimationData animationData)
		{
			if (animationData.Factor < 1f)
			{
				float factor = animationData.Factor;
				float num = 0.75f + 0.25f * MathUtils.Pow(animationData.Factor, 0.25f);
				dialog.RenderTransform = Matrix.CreateTranslation((0f - dialog.ActualSize.X) / 2f, (0f - dialog.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(dialog.ActualSize.X / 2f, dialog.ActualSize.Y / 2f, 0f);
				dialog.ColorTransform = Color.White * factor;
				animationData.CoverRectangle.ColorTransform = Color.White * factor;
				return;
			}
			dialog.RenderTransform = Matrix.Identity;
			dialog.ColorTransform = Color.White;
			animationData.CoverRectangle.ColorTransform = Color.White;
		}

		// Token: 0x040002B0 RID: 688
		public static Dictionary<Dialog, DialogsManager.AnimationData> m_animationData = new Dictionary<Dialog, DialogsManager.AnimationData>();

		// Token: 0x040002B1 RID: 689
		public static List<Dialog> m_dialogs = new List<Dialog>();

		// Token: 0x040002B2 RID: 690
		public static List<Dialog> m_toRemove = new List<Dialog>();

		// Token: 0x0200041D RID: 1053
		public class AnimationData
		{
			// Token: 0x0400154D RID: 5453
			public float Factor;

			// Token: 0x0400154E RID: 5454
			public int Direction;

			// Token: 0x0400154F RID: 5455
			public RectangleWidget CoverRectangle = new RectangleWidget
			{
				OutlineColor = Color.Transparent,
				FillColor = new Color(0, 0, 0, 192),
				IsHitTestVisible = true
			};
		}
	}
}
