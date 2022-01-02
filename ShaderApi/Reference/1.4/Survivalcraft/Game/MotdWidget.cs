using System;
using System.Collections.Generic;
using Engine;
using Engine.Input;
using SimpleJson;

namespace Game
{
	// Token: 0x02000395 RID: 917
	public class MotdWidget : CanvasWidget
	{
		// Token: 0x06001B99 RID: 7065 RVA: 0x000D7370 File Offset: 0x000D5570
		public MotdWidget()
		{
			this.m_containerWidget = new CanvasWidget();
			this.Children.Add(this.m_containerWidget);
			MotdManager.MessageOfTheDayUpdated += this.MotdManager_MessageOfTheDayUpdated;
			this.MotdManager_MessageOfTheDayUpdated();
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x000D73C4 File Offset: 0x000D55C4
		public override void Update()
		{
			if (!this.Noticed && MotdManager.UpdateResult != null)
			{
				JsonObject jsonObj = MotdManager.UpdateResult;
				if (jsonObj["update"].ToString() == "1")
				{
					try
					{
						DialogsManager.ShowDialog(ScreensManager.m_screens["MainMenu"], new MessageDialog(jsonObj["title"].ToString(), jsonObj["content"].ToString(), jsonObj["btn"].ToString(), LanguageControl.Cancel, delegate(MessageDialogButton btn)
						{
							if (btn == MessageDialogButton.Button1)
							{
								WebBrowserManager.LaunchBrowser(jsonObj["url"].ToString());
							}
						}));
					}
					catch (Exception ex)
					{
						Log.Error("Failed processing Update check. Reason: " + ex.Message);
					}
					finally
					{
						this.Noticed = true;
					}
				}
			}
			Vector2? tap = base.Input.Tap;
			if (tap != null)
			{
				tap = base.Input.Tap;
				Widget widget = base.HitTestGlobal(tap.Value, null);
				if (widget != null && (widget == this || widget.IsChildWidgetOf(this)))
				{
					this.m_tapsCount++;
				}
			}
			if (this.m_tapsCount >= 5)
			{
				this.m_tapsCount = 0;
				MotdManager.ForceRedownload();
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
			}
			if (base.Input.IsKeyDownOnce(Key.PageUp))
			{
				this.GotoLine(this.m_currentLineIndex - 1);
			}
			if (base.Input.IsKeyDownOnce(Key.PageDown))
			{
				this.GotoLine(this.m_currentLineIndex + 1);
			}
			if (this.m_lines.Count > 0)
			{
				this.m_currentLineIndex %= this.m_lines.Count;
				double realTime = Time.RealTime;
				if (this.m_lastLineChangeTime == 0.0 || realTime - this.m_lastLineChangeTime >= (double)this.m_lines[this.m_currentLineIndex].Time)
				{
					this.GotoLine((this.m_lastLineChangeTime != 0.0) ? (this.m_currentLineIndex + 1) : 0);
				}
				float num = (float)(realTime - this.m_lastLineChangeTime);
				float num2 = (float)(this.m_lastLineChangeTime + (double)this.m_lines[this.m_currentLineIndex].Time - 0.33000001311302185 - realTime);
				tap = new Vector2?(new Vector2((num >= num2) ? (base.ActualSize.X * (1f - MathUtils.PowSign(MathUtils.Sin(MathUtils.Saturate(1.5f * num2) * 3.14159274f / 2f), 0.33f))) : (base.ActualSize.X * (MathUtils.PowSign(MathUtils.Sin(MathUtils.Saturate(1.5f * num) * 3.14159274f / 2f), 0.33f) - 1f)), 0f));
				base.SetWidgetPosition(this.m_containerWidget, tap);
				this.m_containerWidget.Size = base.ActualSize;
				return;
			}
			this.m_containerWidget.Children.Clear();
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x000D76F4 File Offset: 0x000D58F4
		public void GotoLine(int index)
		{
			if (this.m_lines.Count > 0)
			{
				this.m_currentLineIndex = MathUtils.Max(index, 0) % this.m_lines.Count;
				this.m_containerWidget.Children.Clear();
				this.m_containerWidget.Children.Add(this.m_lines[this.m_currentLineIndex].Widget);
				this.m_lastLineChangeTime = Time.RealTime;
				this.m_tapsCount = 0;
			}
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x000D7770 File Offset: 0x000D5970
		public void Restart()
		{
			this.m_currentLineIndex = 0;
			this.m_lastLineChangeTime = 0.0;
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x000D7788 File Offset: 0x000D5988
		public void MotdManager_MessageOfTheDayUpdated()
		{
			this.m_lines.Clear();
			if (MotdManager.MessageOfTheDay != null)
			{
				foreach (MotdManager.Line line in MotdManager.MessageOfTheDay.Lines)
				{
					try
					{
						MotdWidget.LineData item = this.ParseLine(line);
						this.m_lines.Add(item);
					}
					catch (Exception ex)
					{
						Log.Warning(string.Format("Error loading MOTD line {0}. Reason: {1}", MotdManager.MessageOfTheDay.Lines.IndexOf(line) + 1, ex.Message));
					}
				}
			}
			this.Restart();
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x000D7844 File Offset: 0x000D5A44
		public MotdWidget.LineData ParseLine(MotdManager.Line line)
		{
			MotdWidget.LineData lineData = new MotdWidget.LineData();
			lineData.Time = line.Time;
			if (line.Node != null)
			{
				lineData.Widget = Widget.LoadWidget(null, line.Node, null);
			}
			else
			{
				if (string.IsNullOrEmpty(line.Text))
				{
					throw new InvalidOperationException("Invalid MOTD line.");
				}
				StackPanelWidget stackPanelWidget = new StackPanelWidget
				{
					Direction = LayoutDirection.Vertical,
					HorizontalAlignment = WidgetAlignment.Center,
					VerticalAlignment = WidgetAlignment.Center
				};
				string[] array = line.Text.Replace("\r", "").Split(new string[]
				{
					"\n"
				}, StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (!string.IsNullOrEmpty(text))
					{
						LabelWidget widget = new LabelWidget
						{
							Text = text,
							HorizontalAlignment = WidgetAlignment.Center,
							VerticalAlignment = WidgetAlignment.Center,
							DropShadow = true
						};
						stackPanelWidget.Children.Add(widget);
					}
				}
				lineData.Widget = stackPanelWidget;
			}
			return lineData;
		}

		// Token: 0x040012B7 RID: 4791
		public CanvasWidget m_containerWidget;

		// Token: 0x040012B8 RID: 4792
		public List<MotdWidget.LineData> m_lines = new List<MotdWidget.LineData>();

		// Token: 0x040012B9 RID: 4793
		public int m_currentLineIndex;

		// Token: 0x040012BA RID: 4794
		public double m_lastLineChangeTime;

		// Token: 0x040012BB RID: 4795
		public int m_tapsCount;

		// Token: 0x040012BC RID: 4796
		public bool Noticed;

		// Token: 0x02000586 RID: 1414
		public class LineData
		{
			// Token: 0x040019ED RID: 6637
			public float Time;

			// Token: 0x040019EE RID: 6638
			public Widget Widget;
		}
	}
}
