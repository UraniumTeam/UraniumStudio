using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UraniumStudio.Model;
using UraniumStudio.Utilities;

namespace UraniumStudio.Data;

public static class FileParser
{
    public static List<Function> ParseFile(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        using var reader = new BinaryReader(stream);
        var nsInTick = reader.ReadDouble();
        var funcNamesCount = reader.ReadUInt32();

        var funcNames = new string[funcNamesCount];

        for (var i = 0; i < funcNamesCount; i++)
        {
            var functionNameLength = reader.ReadUInt16();
            var functionName = reader.ReadBytes(functionNameLength);
            funcNames[i] = Encoding.ASCII.GetString(functionName);
        }

        var eventsCount = reader.ReadUInt32();
        var events = new Event[eventsCount];
        for (var i = 0; i < eventsCount; i++)
        {
            var indexBegin = reader.ReadUInt32();
            var tickTimestampBegin = reader.ReadUInt64();
            events[i] = new Event(indexBegin, tickTimestampBegin);
        }

        var min = events.Min(x => x.TickTimestamp);
        events = events
            .Select(x => x with { TickTimestamp = x.TickTimestamp - min })
            .OrderBy(x => x.TickTimestamp)
            .ToArray();

        return ConvertToRenderFunctions(events, funcNames, nsInTick);
    }

    private static List<Function> ConvertToRenderFunctions(ReadOnlySpan<Event> events, IReadOnlyList<string> names,
        double nsInTick)
    {
        var result = new List<Function>();
        var eventStack = new Stack<int>();
        for (var i = 0; i < events.Length; i++)
        {
            ref readonly var ev = ref events[i];
            if (ev.EventType == EventType.Begin)
            {
                eventStack.Push(i);
                continue;
            }

            var beginIndex = eventStack.Pop();
            ref readonly var beginEvent = ref events[beginIndex];

            var timeBeginMs = TimeConverter.TicksToMilliseconds(beginEvent.TickTimestamp, nsInTick);
            var timeEndMs = TimeConverter.TicksToMilliseconds(ev.TickTimestamp, nsInTick);
            var name = names[(int)ev.Index];
            result.Add(new Function(name, timeBeginMs, eventStack.Count, timeEndMs - timeBeginMs, Color.GetRandomColor()));
        }

        Debug.Assert(!eventStack.Any());

        return result;
    }
}
