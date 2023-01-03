namespace UraniumStudio.Utilities;

public static class TimeConverter
{
	public static double TicksToNanoseconds(ulong ticks, double nanosecondsPerTick)
	{
		return ticks * nanosecondsPerTick;
	}

	public static double TicksToMilliseconds(ulong ticks, double nanosecondsPerTick)
	{
		return TicksToNanoseconds(ticks, nanosecondsPerTick) / 1000000;
	}
}