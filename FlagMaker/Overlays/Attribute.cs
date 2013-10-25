using System;
using System.Collections.Generic;
using System.Linq;

namespace FlagMaker.Overlays
{
	public class Attribute
	{
		public string Name { get; private set; }
		public bool IsDiscrete { get; private set; }
		public double Value;
		public bool UseMaxX { get; private set; }

		public Attribute(string name, bool isDiscrete)
		{
			Name = name;
			IsDiscrete = isDiscrete;
			UseMaxX = true;
		}

		public Attribute(string name, bool isDiscrete, double initialValue, bool useMaxX)
		{
			Name = name;
			IsDiscrete = isDiscrete;
			Value = initialValue;
			UseMaxX = useMaxX;
		}

		public override string ToString()
		{
			return string.Format("{0}: {1}", Name, Value);
		}
	}

	public static class AttributeExtensions
	{
		public static Attribute Get(this IEnumerable<Attribute> attributes, string name)
		{
			return attributes.First(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) ?? attributes.First();
		}
	}
}
