using System.Windows.Media;

namespace UraniumStudio.Model;

public record Function(string Name, double StartPosX, int RowPosY, double Length, Color Color)
{
	public readonly string? Name = Name;
	public const int Height = 30;
	public readonly double StartPosX = StartPosX;
	public readonly int RowPosY = RowPosY;
	public readonly double Length = Length;

	public Color Color = Color;
}
