using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace PluginUpdater
{
	class Program
	{
		static ServerBinary Server;
		static TShockBinary TShock;
		static string ServerPath = Path.Combine("Binaries", "TerrariaServer.exe");
		static string TShockPath = Path.Combine("Binaries", "TShockAPI.dll");

		static void Main(string[] args)
		{
			if (!Disclaimer()) return;

			#region LoadEverything

			if (!Directory.Exists("Old")) Directory.CreateDirectory("Old");
			if (!Directory.Exists("New")) Directory.CreateDirectory("New");
			if (!Directory.Exists("Binaries")) Directory.CreateDirectory("Binaries");

			if (!File.Exists(ServerPath))
			{
				Console.WriteLine("Please put TerrariaServer.exe into the Binaries folder!");
				Console.Write("\r\nPress any key to exit...");
				Console.ReadKey();
				return;
			}

			if (!File.Exists(TShockPath))
			{
				Console.WriteLine("Please put TShockAPI.dll into the Binaries folder!");
				Console.Write("\r\nPress any key to exit...");
				Console.ReadKey();
				return;
			}

			Server = new ServerBinary(ServerPath);
			Console.WriteLine("Loaded TerrariaAPI binary!");
			TShock = new TShockBinary(TShockPath);
			Console.WriteLine("Loaded TShock binary!");
			Console.WriteLine(string.Format("Detected TShock API Version: {0}.{1}\r\n", TShock.Version.Major, TShock.Version.Minor));

			var Plugins = Directory.GetFiles("Old");

			if (Plugins.Count() == 0)
			{
				Console.WriteLine("Please put some old plugins into the Old folder!");
				Console.Write("\r\nPress any key to exit...");
				Console.ReadKey();
				return;
			}

			#endregion

			foreach (var PluginPath in Plugins)
			{
				var Plugin = new PluginBinary(PluginPath);

				var Classes = Plugin.Module.Types;

				foreach (var Class in Classes)
				{
					if (Class.HasCustomAttributes)
					{
						var I = Class.CustomAttributes.Count;
						for (int i = 0; i < I; i++)
						{
							var Attribute = Class.CustomAttributes[i];
							if (Attribute.AttributeType.Name == "ApiVersionAttribute")
							{
								var Version = new Version((int)Attribute.ConstructorArguments[0].Value, (int)Attribute.ConstructorArguments[1].Value);

								Console.WriteLine(string.Format("File: {0}, Class: {1}, Version: {2}.{3}:", Path.GetFileName(PluginPath), Class.Name, Version.Major, Version.Minor));

								if (Version != TShock.Version)
								{
									var ConstructorReference = Plugin.Module.Import(Server.VersionConstructor);
									var NewVersion = new CustomAttribute(ConstructorReference);
									
									NewVersion.ConstructorArguments.Add(new CustomAttributeArgument(Plugin.Module.TypeSystem.Int32, TShock.Version.Major));
									NewVersion.ConstructorArguments.Add(new CustomAttributeArgument(Plugin.Module.TypeSystem.Int32, TShock.Version.Minor));
									Class.CustomAttributes.Remove(Attribute);
									Class.CustomAttributes.Add(NewVersion);
									 
									Console.WriteLine("		Patched!");
									Plugin.Modified = true;
								}
								else Console.WriteLine("		Plugin is the same version as TShock!");
							}
						}
					}
				}
				if (Plugin.Modified)
				{
					Plugin.Assembly.Write(Path.Combine("New", Path.GetFileName(PluginPath)));
					Console.WriteLine("	File saved!\r\n");
				}
			}

			Console.Write("\r\nPress any key to exit...");
			Console.ReadKey();
		}

		static bool Disclaimer()
		{
			StringBuilder Disclaimer = new StringBuilder();
			Disclaimer.AppendLine("DISCLAIMER:\r\n");
			Disclaimer.AppendLine("This is a developer tool!\r\n");
			Disclaimer.AppendLine("Do not use this, UNLESS:");
			Disclaimer.AppendLine("	1) You're absotulely sure you don't want to use -ignoreversion.");
			Disclaimer.AppendLine("	2) You're absolutely sure that the plugins you're trying to patch can be patched.\r\n");
			Disclaimer.AppendLine("Do not use this, IF:");
			Disclaimer.AppendLine("	1) You have no idea what -ignoreversion is.");
			Disclaimer.AppendLine("	2) You have no idea about the API changes.\r\n");
			Disclaimer.AppendLine("No one but you is responsible for the damage you can do with this tool!");

			Console.WriteLine(Disclaimer);
			Console.WriteLine("\r\nIf you wish to continue - press Y, otherwise press any other key.");
			var K = Console.ReadKey();
			Console.Clear();
			return K.Key == ConsoleKey.Y;
		}
	}
}
