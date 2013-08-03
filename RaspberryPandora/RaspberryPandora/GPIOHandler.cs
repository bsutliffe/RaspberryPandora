using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using RaspberryPiDotNet;
using System.Threading;

namespace RaspberryPandora {
	public static class GPIOHandler {
		public static System.Timers.Timer refreshTimer = new System.Timers.Timer(10);

		private static Dictionary<GPIO, bool> currentValues = new Dictionary<GPIO, bool>();
		private static Dictionary<GPIO, Action<GPIO, bool>[]> listeners = new Dictionary<GPIO, Action<GPIO, bool>[]>();
		private static GPIOPins restricted = GPIOPins.GPIO_NONE;

		public static bool Enabled {
			get { return refreshTimer.Enabled; }
			set {
				refreshTimer.Enabled = value;
			}
		}

		static GPIOHandler() {
			refreshTimer.Elapsed += refreshTimer_Elapsed;
			refreshTimer.Enabled = false;
		}

		private static void refreshTimer_Elapsed(object sender, ElapsedEventArgs e) {
			Dictionary<GPIO, Action<GPIO, bool>[]> pinsToPing = listeners;
			if (restricted != GPIOPins.GPIO_NONE) {
				pinsToPing = new Dictionary<GPIO, Action<GPIO, bool>[]>();
				GPIO restrictedGPIO = listeners.Where(l => l.Key.Pin == restricted).Select(l => l.Key).FirstOrDefault();
				if (restrictedGPIO != null)
					pinsToPing.Add(restrictedGPIO, listeners[restrictedGPIO]);
			}
			foreach (KeyValuePair<GPIO, Action<GPIO, bool>[]> kvp in pinsToPing) {
				bool cachedValue = currentValues[kvp.Key];
				bool value = kvp.Key.Read();
				if (value != cachedValue) {
					currentValues[kvp.Key] = value;
					foreach (Action<GPIO, bool> listener in kvp.Value) {
						ThreadPool.QueueUserWorkItem(o => listener.Invoke(kvp.Key, value));
					}
				}
			}
		}

		public static void AddChangeListener(this GPIO pin, Action<GPIO, bool> listener) {
			lock (listeners) {
				Action<GPIO, bool>[] theseListeners = listeners.ContainsKey(pin) ? listeners[pin] : new Action<GPIO, bool>[] { listener };
				if (!theseListeners.Contains(listener))
					theseListeners = theseListeners.Concat(new Action<GPIO, bool>[] { listener }).ToArray();
				if (!currentValues.ContainsKey(pin))
					currentValues[pin] = pin.Read();
				listeners[pin] = theseListeners;
				refreshTimer.Enabled = true;
			}
		}

		public static void RemoveChangeListener(this GPIO pin, Action<GPIO, bool> listener) {
			lock (listeners) {
				Action<GPIO, bool>[] theseListeners = listeners.ContainsKey(pin) ? listeners[pin] : null;
				if (theseListeners != null && theseListeners.Contains(listener))
					theseListeners = theseListeners.Where(l => l != listener).ToArray();
				if (theseListeners.Length == 0) {
					listeners.Remove(pin);
					if (listeners.Count == 0)
						refreshTimer.Enabled = false;
				} else
					listeners[pin] = theseListeners;
			}
		}

		public static void Restrict(GPIOPins onlyThisPin) {
			restricted = onlyThisPin;
		}
	}
}
