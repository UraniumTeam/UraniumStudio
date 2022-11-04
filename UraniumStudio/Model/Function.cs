using System.Windows.Media;

namespace UraniumStudio.Model;

public record Function(int StartPosX, int RowPosY, int Length, string? Name, Color Color)
{
	public const int Height = 30;
	public readonly int StartPosX = StartPosX;
	public readonly int RowPosY = RowPosY;
	public readonly int Length = Length;
	public readonly string? Name = Name;

	public Color Color = Color;
}
