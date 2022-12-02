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
	public static IEnumerable<Function> ParseFile(string path)
	{
		byte[] data = File.ReadAllBytes(path);
		int cursor = 0;
		double nsInTick = BitConverter.ToDouble(data, cursor);
		cursor += 8;
		uint funcNamesCount = BitConverter.ToUInt32(data, cursor);
		cursor += 4;

		string[] funcNames = new string[funcNamesCount];
		var eventsTuples = new List<Tuple<Event, Event>>();

		for (int i = 0; i < funcNamesCount; i++)
		{
			ushort countLettersInFuncName = BitConverter.ToUInt16(data, cursor);
			cursor += 2;
			byte[] dataSliceFuncName = data.Skip(cursor).Take(countLettersInFuncName).ToArray();
			cursor += countLettersInFuncName;
			funcNames[i] = Encoding.ASCII.GetString(dataSliceFuncName);
		}

		uint eventsCount = BitConverter.ToUInt32(data, cursor);
		cursor += 4;
		for (int i = 0; i < eventsCount / 2; i++)
		{
			uint indexBegin = BitConverter.ToUInt32(data, cursor);
			cursor += 4;
			ulong tickTimestampBegin = BitConverter.ToUInt64(data, cursor);
			cursor += 8;
			var eventBegin = new Event(indexBegin, tickTimestampBegin);

			uint indexEnd = BitConverter.ToUInt32(data, cursor);
			cursor += 4;
			ulong tickTimestampEnd = BitConverter.ToUInt64(data, cursor);
			cursor += 8;
			var eventEnd = new Event(indexEnd, tickTimestampEnd);
			eventsTuples.Add(new Tuple<Event, Event>(eventBegin, eventEnd));
		}

		ToStartOfTimeline(eventsTuples);
		return ConvertToRenderFunctions(eventsTuples, funcNames, nsInTick);
	}

	static void ToStartOfTimeline(List<Tuple<Event, Event>> eventsTuples)
	{
		double minX = eventsTuples.Min(e => e.Item1.TickTimestamp);
		foreach (var (eventBegin, eventEnd) in eventsTuples)
		{
			eventBegin.TickTimestamp -= (ulong)minX;
			eventEnd.TickTimestamp -= (ulong)minX;
		}
	}

	static IEnumerable<Function> ConvertToRenderFunctions(
		List<Tuple<Event, Event>> eventsTuples,
		IReadOnlyList<string> funcNames,
		double nsInTick)
	{

		var functions = new List<Function>();
		var rows = new Dictionary<uint, double>();
		foreach (var (eventBegin, eventEnd) in eventsTuples)
		{
			double timeBeginMs = TimeConverter.TicksToMilliseconds(eventBegin.TickTimestamp, nsInTick),
				timeEndMs = TimeConverter.TicksToMilliseconds(eventEnd.TickTimestamp, nsInTick);
			double funcLengthMs = timeEndMs - timeBeginMs;
			uint currentRowPosY = eventBegin.Index;
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
					bool stop = false;
					while (rows.TryGetValue(currentRowPosY, out double value) && !stop)
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
