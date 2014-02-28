using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PluginUpdater
{
	public class ServerBinary
	{
		public string Path { get; private set; }

		public ServerBinary(string path)
		{
			Path = path;
		}

		private AssemblyDefinition _assembly;
		private ModuleDefinition _module;
		private TypeDefinition _versiontype;

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

		public TypeDefinition VersionType
		{
			get
			{
				if (_versiontype == null) _versiontype = Module.GetType("TerrariaApi.Server.ApiVersionAttribute");
				return _versiontype;
			}
		}

		public MethodDefinition VersionConstructor
		{
			get { return VersionType.Methods[1]; }
		}
	}
}
