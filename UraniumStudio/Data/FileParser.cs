using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using UraniumStudio.Model;
using static UraniumStudio.Utilities.Utilities;

namespace UraniumStudio.Data;

public static class FileParser
{
	public static List<Function> ParseFile(string path)
	{
		byte[] data = File.ReadAllBytes(path);
		int cursor = 0;
		ulong nsInTick = BitConverter.ToUInt64(data, cursor);
		cursor += 8;
		uint funcsCount = BitConverter.ToUInt32(data, cursor);
		cursor += 4;

		var funcs = new List<Tuple<string, int>>();
		var events = new List<Event>();

		for (int i = 0; i < funcsCount; i++)
		{
			ushort countLettersInFuncName = BitConverter.ToUInt16(data, cursor);
			cursor += 2;
			byte[] dataSliceFuncName = data.Skip(cursor).Take(countLettersInFuncName).ToArray();
			cursor += countLettersInFuncName;
			funcs.Add(new Tuple<string, int>(Encoding.ASCII.GetString(dataSliceFuncName), dataSliceFuncName.Length));
		}

		uint eventsCount = BitConverter.ToUInt32(data, cursor) * 2;
		cursor += 4;
		for (int i = 0; i < eventsCount; i++)
		{
			ushort index = BitConverter.ToUInt16(data, cursor);
			cursor += 2;
			ushort state = BitConverter.ToUInt16(data, cursor);
			cursor += 2;
			ulong tickTimestamp = BitConverter.ToUInt64(data, cursor);
			cursor += 8;
			events.Add(new Event(index, state, tickTimestamp));
		}

		var functions = new List<Function>();
		const int experimentTickTime = 2;
		foreach (var func in funcs)
		{
			var funcEvents = new List<Event>();
			foreach (var e in events)
			{
				if (e.Index == funcs.IndexOf(func))
					funcEvents.Add(e);
			}

			functions.Add(
				new Function(
					func.Item1, funcEvents, funcEvents.Find(e => e.State == 0).TickTimestamp / 1000000000000,
					funcs.IndexOf(func),
					(funcEvents.Find(e => e.State == 4096).TickTimestamp -
					 funcEvents.Find(e => e.State == 0).TickTimestamp) / experimentTickTime, GetRandomColor()));
		}

		return functions;
	}
}
