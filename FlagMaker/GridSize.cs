namespace FlagMaker
{
	internal class GridSize
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

		public GridSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Height, Width);
		}
	}
}