namespace UraniumStudio.Model;

public class Event
{
	public Event(uint index, ulong tickTimestamp)
	{
		Index = index & 0xfffffff;
		TickTimestamp = tickTimestamp;
		EventType = index >> 28 == 0x0 ? EventType.Begin : EventType.End;
	}

	public uint Index { get; }

	public EventType EventType { get; }

	public ulong TickTimestamp { get; set; } // 
}

public enum EventType
{
	Begin,
	End
}
