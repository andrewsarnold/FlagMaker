using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using FlagMaker.Localization;
using FlagMaker.Overlays.OverlayTypes.PathTypes;
using FlagMaker.Overlays.OverlayTypes.ShapeTypes;

namespace FlagMaker.Overlays
{
	public static class OverlayFactory
	{
		private static Dictionary<string, Type> _typeMap;

		public static Dictionary<string, OverlayPath> CustomTypes;

		public static void SetUpTypeMap()
		{
			var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(typeof(Overlay)) && t.IsSealed).OrderBy(t => t.Name));
			_typeMap = new Dictionary<string, Type>();

			foreach (var type in types)
			{
				var instance = (Overlay) Activator.CreateInstance(type, 0, 0);
				_typeMap.Add(instance.Name, type);
			}
		}

		public static Type GetOverlayType(string name)
		{
			var result = CustomTypes.Any(t => t.Key == name)
				? CustomTypes[name].GetType()
				: _typeMap.First(t => t.Key == name).Value;

			if (result == null)
			{
				throw new Exception(string.Format(strings.OverlayLoadError, name));
			}

			return result;
		}

		public static Overlay GetInstance(string name, int maxX = 1, int maxY = 1)
		{
			return GetInstance(GetOverlayType(name), maxX, maxY, name);
		}

		public static Overlay GetFlagInstance(string path, int maxX = 1, int maxY = 1)
		{
			return new OverlayFlag(Flag.LoadFromFile(path), path, maxX, maxY);
		}

		public static Overlay GetImageInstance(string path, string directory, int maxX = 1, int maxY = 1)
		{
			return new OverlayImage(path, directory, maxX, maxY);
		}

		public static Overlay GetInstance(Type type, int maxX = 1, int maxY = 1, string name = "")
		{
			if (type == typeof(OverlayPath)) // custom overlay
			{
				var overlay = CustomTypes[name];

				// Create a unique copy
				var overlayCopy = overlay.Copy();
				overlayCopy.SetMaximum(maxX, maxY);
				return overlayCopy;
			}

			return (Overlay)Activator.CreateInstance(type, maxX, maxY);
		}

		public static void FillCustomOverlays()
		{
			CustomTypes = new Dictionary<string, OverlayPath>();

			var path = string.Format("{0}Custom", AppDomain.CurrentDomain.BaseDirectory);

			foreach (var file in Directory.GetFiles(path, "*.ovr"))
			{
				try
				{
					var name = string.Empty;
					double width = 0;
					double height = 0;
					var pathData = string.Empty;

					using (var sr = new StreamReader(file))
					{
						string line;
						while ((line = sr.ReadLine()) != null)
						{
							switch (line.Split('=')[0].ToLower())
							{
								case "name":
									name = line.Split('=')[1];
									break;
								case "width":
									width = int.Parse(line.Split('=')[1]);
									break;
								case "height":
									height = int.Parse(line.Split('=')[1]);
									break;
								case "path":
									pathData = line.Split('=')[1];
									break;
							}
						}
					}

					if (CustomTypes.Any(t => String.Equals(t.Key, name, StringComparison.InvariantCultureIgnoreCase)) ||
					    _typeMap.Any(t => String.Equals(t.Key, name, StringComparison.InvariantCultureIgnoreCase)))
					{
						throw new DuplicateNameException(string.Format(strings.OverlayNameExists, name));
					}

					var overlay = new OverlayPath(name, pathData, new Vector(width, height), 1, 1);
					CustomTypes.Add(name, overlay);
				}
				catch (DuplicateNameException)
				{
					throw;
				}
				catch (Exception)
				{
					throw new Exception(string.Format(strings.OverlayLoadError, Path.GetFileNameWithoutExtension(file)));
				}
			}
		}

		public static IEnumerable<Type> GetOverlaysByType(Type type)
		{
			return _typeMap.Where(o => o.Value.IsSubclassOf(type)).Select(o => o.Value);
		}

		public static IEnumerable<Type> GetOverlaysNotInTypes(IEnumerable<Type> types)
		{
			return _typeMap.Where(o => !types.Any(t => o.Value.IsSubclassOf(t))).Select(o => o.Value);
		} 
	}
}
