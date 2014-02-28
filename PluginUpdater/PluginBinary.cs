using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace PluginUpdater
{
	public class PluginBinary
	{
		public string Path { get; private set; }

		public PluginBinary(string path)
		{
			Path = path;
		}

		private AssemblyDefinition _assembly;
		private ModuleDefinition _module;

		public bool Modified = false;

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
	}
}
