using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PluginUpdater
{
	public class TShockBinary
	{
		public string Path { get; private set; }

		public TShockBinary(string path)
		{
			Path = path;
		}

		private AssemblyDefinition _assembly;
		private ModuleDefinition _module;
		private TypeDefinition _plugintype;
		private CustomAttribute _versionattribute;
		private Version _version;

		public AssemblyDefinition Assembly
		{
			get
			{
				if (_assembly == null) _assembly = AssemblyDefinition.ReadAssembly(Path);
				return _assembly;
			}
		}

		public ModuleDefinition Module
		{
			get
			{
				if (_module == null) _module = Assembly.MainModule;
				return _module;
			}
		}

		public TypeDefinition PluginType
		{
			get
			{
				if (_plugintype == null) _plugintype = Module.GetType("TShockAPI.TShock");
				return _plugintype;
			}
		}

		public CustomAttribute VersionAttribute
		{
			get
			{
				if (_versionattribute == null) _versionattribute = PluginType.CustomAttributes.First(Attr => Attr.AttributeType.Name == "ApiVersionAttribute");
				return _versionattribute;
			}
		}

		public Version Version
		{
			get
			{
				if (_version == null) _version = new Version((int)VersionAttribute.ConstructorArguments[0].Value, (int)VersionAttribute.ConstructorArguments[1].Value);
				return _version;
			}
		}
	}
}
