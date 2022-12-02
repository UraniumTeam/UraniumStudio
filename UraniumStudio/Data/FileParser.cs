using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UraniumStudio.Model;
using static UraniumStudio.Utilities.Color;

namespace UraniumStudio.Data;

public static class FileParser
{
	public static List<Function> ParseFile(string path)
	{
		byte[] data = File.ReadAllBytes(path);
		int cursor = 0;
		double nsInTick = BitConverter.ToDouble(data, cursor);
		cursor += 8;
		uint funcNamesCount = BitConverter.ToUInt32(data, cursor);
		cursor += 4;

		var funcNames = new string[funcNamesCount];
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

		double minX = eventsTuples.Min(e => e.Item1.TickTimestamp);
		foreach (var e in eventsTuples)
		{
			e.Item1.TickTimestamp -= (ulong)minX;
			e.Item2.TickTimestamp -= (ulong)minX;
		}

		var functions = new List<Function>();
		var rows = new Dictionary<uint, double>();
		for (int i = 0; i < eventsTuples.Count; i++)
		{
			double timeBeginMs = eventsTuples[i].Item1.TickTimestamp * nsInTick / 1000000;
			double timeEndMs = eventsTuples[i].Item2.TickTimestamp * nsInTick / 1000000;
			double funcLengthMs = timeEndMs - timeBeginMs;
			uint currentRowPosY = eventsTuples[i].Item1.Index;
			if (rows.ContainsKey(currentRowPosY))
			{
				if (rows[currentRowPosY] < timeBeginMs)
				{
					rows[currentRowPosY] = timeEndMs;
					functions.Add(
						new Function(
							funcNames[(int)eventsTuples[i].Item1.Index], timeBeginMs, (int)currentRowPosY,
							funcLengthMs, GetRandomColor()));
				}
				else
				{
					bool stop = false;
					while (rows.TryGetValue(currentRowPosY, out double value) && !stop)
					{
						if (value < timeBeginMs)
							stop = true;
						else
						{
							currentRowPosY++;
						}
					}

					if (stop == false)
						rows.Add(currentRowPosY, timeEndMs);
					else
						rows[currentRowPosY] = timeEndMs;

					functions.Add(
						new Function(
							funcNames[(int)eventsTuples[i].Item1.Index], timeBeginMs,
							(int)currentRowPosY,
							funcLengthMs, GetRandomColor()));
				}
			}

			else
			{
				rows.Add(currentRowPosY, timeEndMs);
				functions.Add(
					new Function(
						funcNames[(int)eventsTuples[i].Item1.Index], timeBeginMs, (int)currentRowPosY,
						funcLengthMs, GetRandomColor()));
			}
		}

		return functions.ToList();
	}
}
