using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000345 RID: 837
	public class VersionConverter121To122 : VersionConverter
	{
		// Token: 0x170003AC RID: 940
		// (get) Token: 0x060018A8 RID: 6312 RVA: 0x000C143C File Offset: 0x000BF63C
		public override string SourceVersion
		{
			get
			{
				return "1.21";
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x060018A9 RID: 6313 RVA: 0x000C1443 File Offset: 0x000BF643
		public override string TargetVersion
		{
			get
			{
				return "1.22";
			}
		}

		// Token: 0x060018AA RID: 6314 RVA: 0x000C144C File Offset: 0x000BF64C
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			foreach (XElement xelement in projectNode.Element("Subsystems").Elements())
			{
				foreach (XElement xelement2 in from e in xelement.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CreatureSpawn"
				select e)
				{
					XmlUtils.SetAttributeValue(xelement2, "Name", "Spawn");
					foreach (XElement node in xelement2.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CreaturesData"))
					{
						XmlUtils.SetAttributeValue(node, "Name", "SpawnsData");
					}
					foreach (XElement node2 in xelement2.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CreaturesGenerated"))
					{
						XmlUtils.SetAttributeValue(node2, "Name", "IsSpawned");
					}
				}
			}
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x000C1644 File Offset: 0x000BF844
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
