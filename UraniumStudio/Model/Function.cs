using System.Windows.Media;

namespace UraniumStudio.Model;

public record Function(int StartPosX, int PosY, int Length, string Name, Color Color)
{
	public int StartPosX = StartPosX;
	public int PosY = PosY;
	public const int Width = 30;
	public int Length = Length;
	public string Name;

	public Color Color = Color;
}
