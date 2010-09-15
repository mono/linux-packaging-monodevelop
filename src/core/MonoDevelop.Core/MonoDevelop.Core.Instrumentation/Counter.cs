// 
// Counter.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;

namespace MonoDevelop.Core.Instrumentation
{
	[Serializable]
	public class Counter: MarshalByRefObject
	{
		internal int count;
		int totalCount;
		string name;
		bool logMessages;
		CounterCategory category;
		protected List<CounterValue> values = new List<CounterValue> ();
		TimeSpan resolution = TimeSpan.FromMilliseconds (0);
		DateTime lastValueTime = DateTime.MinValue;
		CounterDisplayMode displayMode = CounterDisplayMode.Block;
		bool disposed;
		
		internal Counter (string name, CounterCategory category)
		{
			this.name = name;
			this.category = category;
		}
		
		public string Name {
			get { return name; }
		}
		
		public CounterCategory Category {
			get { return category; }
		}
		
		public TimeSpan Resolution {
			get { return resolution; }
			set { resolution = value; }
		}

		public bool LogMessages {
			get { return this.logMessages; }
			set { this.logMessages = value; }
		}
		
		public int Count {
			get { return count; }
			set {
				lock (values) {
					if (value > count)
						totalCount += value - count;
					count = value;
				}
			}
		}
		
		public bool Disposed {
			get { return disposed; }
			internal set { disposed = value; }
		}
		
		public int TotalCount {
			get { return totalCount; }
		}
		
		public CounterDisplayMode DisplayMode {
			get { return this.displayMode; }
			set { this.displayMode = value; }
		}
		
		public IEnumerable<CounterValue> GetValues ()
		{
			lock (values) {
				return new List<CounterValue> (values);
			}
		}
		
		public IEnumerable<CounterValue> GetValuesAfter (DateTime time)
		{
			List<CounterValue> res = new List<CounterValue> ();
			lock (values) {
				for (int n=values.Count - 1; n >= 0; n--) {
					CounterValue val = values [n];
					if (val.TimeStamp > time)
						res.Add (val);
					else
						break;
				}
			}
			res.Reverse ();
			return res;
		}
		
		public IEnumerable<CounterValue> GetValuesBetween (DateTime startTime, DateTime endTime)
		{
			List<CounterValue> res = new List<CounterValue> ();
			lock (values) {
				if (values.Count == 0 || startTime > values[values.Count - 1].TimeStamp)
					return res;
				for (int n=0; n<values.Count; n++) {
					CounterValue val = values[n];
					if (val.TimeStamp > endTime)
						break;
					if (val.TimeStamp >= startTime)
						res.Add (val);
				}
			}
			return res;
		}
		
		public CounterValue GetValueAt (DateTime time)
		{
			lock (values) {
				if (values.Count == 0 || time < values[0].TimeStamp)
					return new CounterValue (0, 0, time);
				if (time >= values[values.Count - 1].TimeStamp)
					return values[values.Count - 1];
				for (int n=0; n<values.Count; n++) {
					if (values[n].TimeStamp > time)
						return values [n - 1];
				}
			}
			return new CounterValue (0, 0, time);
		}
		
		public CounterValue LastValue {
			get {
				lock (values) {
					if (values.Count > 0)
						return values [values.Count - 1];
					else
						return new CounterValue (0, 0, DateTime.MinValue);
				}
			}
		}
		
		internal int StoreValue (string message, TimerTraceList traces)
		{
			DateTime now = DateTime.Now;
			if (resolution.Ticks != 0) {
				if (now - lastValueTime < resolution)
					return -1;
			}
			values.Add (new CounterValue (count, totalCount, now, message, traces));
			return values.Count - 1;
		}
		
		internal void RemoveValue (int index)
		{
			lock (values) {
				values.RemoveAt (index);
				for (int n=index; n<values.Count; n++) {
					CounterValue val = values [n];
					val.UpdateValueIndex (n);
				}
			}
		}
		
		public void Inc ()
		{
			Inc (1, null);
		}
		
		public void Inc (string message)
		{
			Inc (1, message);
		}
		
		public void Inc (int n)
		{
			Inc (n, null);
		}
		
		public void Inc (int n, string message)
		{
			if (InstrumentationService.Enabled) {
				lock (values) {
					count += n;
					totalCount += n;
					StoreValue (message, null);
				}
			}
			if (logMessages && message != null)
				InstrumentationService.LogMessage (message);
		}
		
		public void Dec ()
		{
			Dec (1);
		}
		
		public void Dec (string message)
		{
			Dec (1, message);
		}
		
		public void Dec (int n)
		{
			Dec (n, null);
		}
		
		public void Dec (int n, string message)
		{
			if (InstrumentationService.Enabled) {
				lock (values) {
					count -= n;
					StoreValue (message, null);
				}
			}
			if (logMessages && message != null)
				InstrumentationService.LogMessage (message);
		}
		
		public void SetValue (int value)
		{
			SetValue (value, null);
		}
		
		public void SetValue (int value, string message)
		{
			if (InstrumentationService.Enabled) {
				lock (values) {
					count = value;
					StoreValue (message, null);
				}
			}
			if (logMessages && message != null)
				InstrumentationService.LogMessage (message);
		}
		
		public static Counter operator ++ (Counter c)
		{
			c.Inc (1, null);
			return c;
		}
		
		public static Counter operator -- (Counter c)
		{
			c.Dec (1, null);
			return c;
		}
		
		public MemoryProbe CreateMemoryProbe ()
		{
			return new MemoryProbe (this);
		}
		
		public virtual void Trace (string message)
		{
			if (InstrumentationService.Enabled) {
				lock (values) {
					StoreValue (message, null);
				}
			}
			if (logMessages && message != null)
				InstrumentationService.LogMessage (message);
		}
		
		public override object InitializeLifetimeService ()
		{
			return null;
		}

	}
	
	[Serializable]
	public struct CounterValue
	{
		int value;
		int totalCount;
		DateTime timestamp;
		string message;
		TimerTraceList traces;
		int threadId;
		
		internal CounterValue (int value, int totalCount, DateTime timestamp)
		{
			this.value = value;
			this.timestamp = timestamp;
			this.totalCount = totalCount;
			this.message = null;
			traces = null;
			threadId = 0;
		}
		
		internal CounterValue (int value, int totalCount, DateTime timestamp, string message, TimerTraceList traces)
		{
			this.value = value;
			this.timestamp = timestamp;
			this.totalCount = totalCount;
			this.message = message;
			this.traces = traces;
			this.threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
		}
		
		public DateTime TimeStamp {
			get { return timestamp; }
		}
		
		public int Value {
			get { return this.value; }
		}
		
		public int TotalCount {
			get { return totalCount; }
		}
		
		public int ThreadId {
			get { return this.threadId; }
		}
		
		public string Message {
			get { return message; }
		}
		
		public bool HasTimerTraces {
			get { return traces != null; }
		}
		
		public TimeSpan Duration {
			get {
				if (traces == null)
					return new TimeSpan (0);
				else
					return traces.TotalTime;
			}
		}
		
		public IEnumerable<TimerTrace> GetTimerTraces ()
		{
			if (traces == null)
				yield break;
			TimerTrace trace = traces.FirstTrace;
			while (trace != null) {
				yield return trace;
				trace = trace.Next;
			}
		}
		
		internal void UpdateValueIndex (int newIndex)
		{
			if (traces != null)
				traces.ValueIndex = newIndex;
		}
	}
	
	public enum CounterDisplayMode
	{
		Block,
		Line
	}
}
