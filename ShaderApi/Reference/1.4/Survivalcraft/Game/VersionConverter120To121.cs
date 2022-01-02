using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000344 RID: 836
	public class VersionConverter120To121 : VersionConverter
	{
		// Token: 0x170003AA RID: 938
		// (get) Token: 0x060018A3 RID: 6307 RVA: 0x000C1188 File Offset: 0x000BF388
		public override string SourceVersion
		{
			get
			{
				return "1.20";
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x060018A4 RID: 6308 RVA: 0x000C118F File Offset: 0x000BF38F
		public override string TargetVersion
		{
			get
			{
				return "1.21";
			}
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x000C1198 File Offset: 0x000BF398
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			foreach (XElement xelement in projectNode.Element("Entities").Elements())
			{
				foreach (XElement xelement2 in from e in xelement.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Body" || XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Frame"
				select e)
				{
					using (IEnumerator<XElement> enumerator3 = xelement2.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "LocalMatrix").GetEnumerator())
					{
						if (enumerator3.MoveNext())
						{
							XElement xelement3 = enumerator3.Current;
							Vector3 vector;
							Quaternion quaternion;
							Vector3 vector2;
							XmlUtils.GetAttributeValue<Matrix>(xelement3, "Value").Decompose(out vector, out quaternion, out vector2);
							XElement xelement4 = new XElement("Value");
							XElement xelement5 = new XElement("Value");
							XmlUtils.SetAttributeValue(xelement4, "Name", "Position");
							XmlUtils.SetAttributeValue(xelement4, "Type", "Microsoft.Xna.Framework.Vector3");
							XmlUtils.SetAttributeValue(xelement4, "Value", vector2);
							XmlUtils.SetAttributeValue(xelement5, "Name", "Rotation");
							XmlUtils.SetAttributeValue(xelement5, "Type", "Microsoft.Xna.Framework.Quaternion");
							XmlUtils.SetAttributeValue(xelement5, "Value", quaternion);
							xelement2.Add(xelement4);
							xelement2.Add(xelement5);
							xelement3.Remove();
						}
					}
				}
			}
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x000C13B0 File Offset: 0x000BF5B0
		public override void ConvertWorld(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}
	}
}
