using System;
using System.Collections.Generic;
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
        var eventsTuples = new List<Tuple<Event, Event>>();

        for (var i = 0; i < funcNamesCount; i++)
        {
            var countLettersInFuncName = reader.ReadUInt16();
            var dataSliceFuncName = reader.ReadBytes(countLettersInFuncName);
            funcNames[i] = Encoding.ASCII.GetString(dataSliceFuncName);
        }

        var eventsCount = reader.ReadUInt32();
        for (var i = 0; i < eventsCount / 2; i++)
        {
            var indexBegin = reader.ReadUInt32();
            var tickTimestampBegin = reader.ReadUInt64();
            var eventBegin = new Event(indexBegin, tickTimestampBegin);

            var indexEnd = reader.ReadUInt32();
            var tickTimestampEnd = reader.ReadUInt64();
            var eventEnd = new Event(indexEnd, tickTimestampEnd);
            eventsTuples.Add(new Tuple<Event, Event>(eventBegin, eventEnd));
        }

        ToStartOfTimeline(eventsTuples);
        return ConvertToRenderFunctions(eventsTuples, funcNames, nsInTick);
    }

    private static void ToStartOfTimeline(List<Tuple<Event, Event>> eventsTuples)
    {
        double minX = eventsTuples.Min(e => e.Item1.TickTimestamp);
        foreach (var (eventBegin, eventEnd) in eventsTuples)
        {
            eventBegin.TickTimestamp -= (ulong)minX;
            eventEnd.TickTimestamp -= (ulong)minX;
        }
    }

    private static List<Function> ConvertToRenderFunctions(List<Tuple<Event, Event>> eventsTuples,
        IReadOnlyList<string> funcNames, double nsInTick)
    {
        var functions = new List<Function>();
        var rows = new Dictionary<uint, double>();
        foreach (var (eventBegin, eventEnd) in eventsTuples)
        {
            var timeBeginMs = TimeConverter.TicksToMilliseconds(eventBegin.TickTimestamp, nsInTick);
            var timeEndMs = TimeConverter.TicksToMilliseconds(eventEnd.TickTimestamp, nsInTick);
            var funcLengthMs = timeEndMs - timeBeginMs;
            var currentRowPosY = eventBegin.Index;
            if (rows.ContainsKey(currentRowPosY))
                if (rows[currentRowPosY] < timeBeginMs)
                {
                    rows[currentRowPosY] = timeEndMs;
                    functions.Add(
                        new Function(
                            funcNames[(int)eventBegin.Index], timeBeginMs, (int)currentRowPosY,
                            funcLengthMs, Color.GetRandomColor()));
                }
                else
                {
                    var stop = false;
                    while (rows.TryGetValue(currentRowPosY, out var value) && !stop)
                    {
                        if (value < timeBeginMs) stop = true;
                        else currentRowPosY++;
                    }

                    if (!stop) rows.Add(currentRowPosY, timeEndMs);
                    else rows[currentRowPosY] = timeEndMs;

                    functions.Add(
                        new Function(
                            funcNames[(int)eventBegin.Index], timeBeginMs,
                            (int)currentRowPosY,
                            funcLengthMs, Color.GetRandomColor()));
                }
            else
            {
                rows.Add(currentRowPosY, timeEndMs);
                functions.Add(
                    new Function(
                        funcNames[(int)eventBegin.Index], timeBeginMs, (int)currentRowPosY,
                        funcLengthMs, Color.GetRandomColor()));
            }
        }

        return functions;
    }
}
