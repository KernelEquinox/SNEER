using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SotNEditor
{
	public static class ConfigFile
	{
		public static ConfigValues ReadConfig(string path)
		{
			string localPath = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath;
			string path2 = Path.GetDirectoryName(localPath) + "/" + path;
			ConfigValues configValues = new ConfigValues();
			FileStream fileStream;
			try
			{
				fileStream = new FileStream(path2, FileMode.Open);
			}
			catch (FileNotFoundException)
			{
				throw;
			}
			XmlReader xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
			{
				IgnoreWhitespace = true
			});
			if (!ConfigFile.ValidateFile(xmlReader))
			{
				throw new InvalidDataException("Invalid config file");
			}
			while (xmlReader.Read())
			{
				XmlNodeType nodeType = xmlReader.NodeType;
				if (nodeType == XmlNodeType.Element && xmlReader.Name == "Param")
				{
					ConfigFile.ReadParam(xmlReader, configValues);
				}
			}
			xmlReader.Close();
			fileStream.Close();
			return configValues;
		}

		public static bool WriteConfig(string path, ConfigValues data)
		{
			if (data == null)
			{
				return false;
			}
			string localPath = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath;
			string path2 = Path.GetDirectoryName(localPath) + "/" + path;
			FileStream fileStream;
			try
			{
				fileStream = new FileStream(path2, FileMode.Create);
			}
			catch
			{
				return false;
			}
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = " "
			};
			XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings);
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("SotNEditorConfig");
			xmlWriter.WriteStartElement("Param");
			xmlWriter.WriteStartElement("F_GAME");
			xmlWriter.WriteString(data.F_Game_BinPath);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Param");
			xmlWriter.WriteStartElement("DRA");
			xmlWriter.WriteString(data.Dra_BinPath);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("Param");
			xmlWriter.WriteStartElement("FlushColor");
			xmlWriter.WriteString(data.FlushColor.ToString());
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Close();
			fileStream.Close();
			return true;
		}

		private static bool ValidateFile(XmlReader reader)
		{
			while (reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType <= XmlNodeType.Comment)
				{
					if (nodeType == XmlNodeType.Element)
					{
						return !(reader.Name != "SotNEditorConfig");
					}
					if (nodeType != XmlNodeType.Comment)
					{
					}
				}
				else if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.XmlDeclaration)
				{
				}
			}
			return false;
		}

		private static void ReadParam(XmlReader reader, ConfigValues cv)
		{
			while (reader.Read())
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					if (reader.Name == "F_GAME")
					{
						reader.Read();
						cv.F_Game_BinPath = reader.ReadContentAsString();
						return;
					}
					if (reader.Name == "DRA")
					{
						reader.Read();
						cv.Dra_BinPath = reader.ReadContentAsString();
						return;
					}
					if (reader.Name == "FlushColor")
					{
						reader.Read();
						cv.FlushColor = uint.Parse(reader.ReadContentAsString());
						return;
					}
				}
			}
		}

		private const string RootElementName = "SotNEditorConfig";

		private const string ParamElementName = "Param";

		private const string FGAME_ElementName = "F_GAME";

		private const string DRA_ElementName = "DRA";

		private const string FlushColor_ElementName = "FlushColor";
	}
}
