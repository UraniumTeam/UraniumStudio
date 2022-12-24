namespace UraniumStudio.Model;

public readonly record struct Event
{
    public uint Index { get; }
    public EventType EventType { get; }
    public ulong TickTimestamp { get; init; }

    public Event(uint index, ulong tickTimestamp)
    {
        Index = index & 0xfffffff;
        TickTimestamp = tickTimestamp;
        EventType = index >> 28 == 0 ? EventType.Begin : EventType.End;
    }
}
