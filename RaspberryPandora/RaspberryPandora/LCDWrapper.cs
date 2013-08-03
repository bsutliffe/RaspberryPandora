using RaspberryPiDotNet;
using RaspberryPiDotNet.MicroLiquidCrystal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RaspberryPandora {
	public class LCDWrapper : Object {
		public DisplayMode DisplayMode { get; set; }
		public int Columns { get; private set; }
		public int Rows { get; private set; }

		private ILcdTransferProvider transferProvider;
		private Lcd lcd;
		private StringWriter consoleContent = new StringWriter();
		private bool backlightOn = true;

		public LCDWrapper(GPIOPins rs, GPIOPins enable, GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7, int columns = 40, int rows = 2, DisplayMode displayMode = DisplayMode.LCD_ONLY) {
			DisplayMode = displayMode;
			Columns = columns;
			Rows = rows;
			if (DisplayMode != DisplayMode.CONSOLE_ONLY) {
				transferProvider = new RaspPiGPIOMemLcdTransferProvider(
					fourBitMode: true,
					rs: rs,
					rw: GPIOPins.GPIO_NONE,
					enable: enable,
					d0: GPIOPins.GPIO_NONE,
					d1: GPIOPins.GPIO_NONE,
					d2: GPIOPins.GPIO_NONE,
					d3: GPIOPins.GPIO_NONE,
					d4: d4,
					d5: d5,
					d6: d6,
					d7: d7
				);
				lcd = new Lcd(transferProvider);
				lcd.Begin(Convert.ToByte(columns), Convert.ToByte(rows));
				lcd.Backlight = true;
				lcd.BlinkCursor = false;
				lcd.ShowCursor = false;
				lcd.Visible = true;
			}
			if(DisplayMode != DisplayMode.LCD_ONLY){
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.CursorVisible = false;
			}
		}
		public LCDWrapper(GPIOPins rs, GPIOPins enable, GPIOPins d0, GPIOPins d1, GPIOPins d2, GPIOPins d3, GPIOPins d4, GPIOPins d5, GPIOPins d6, GPIOPins d7, int columns = 40, int rows = 2, DisplayMode displayMode = DisplayMode.LCD_ONLY) {
			DisplayMode = displayMode;
			Columns = columns;
			Rows = rows;
			if (DisplayMode != DisplayMode.CONSOLE_ONLY) {
				transferProvider = new RaspPiGPIOMemLcdTransferProvider(
					fourBitMode: false,
					rs: rs,
					rw: GPIOPins.GPIO_NONE,
					enable: enable,
					d0: d0,
					d1: d1,
					d2: d2,
					d3: d3,
					d4: d4,
					d5: d5,
					d6: d6,
					d7: d7
				);
				lcd = new Lcd(transferProvider);
				lcd.Begin(Convert.ToByte(columns), Convert.ToByte(rows));
				lcd.Backlight = true;
				lcd.BlinkCursor = false;
				lcd.ShowCursor = false;
				lcd.Visible = true;
			}
			if (DisplayMode != DisplayMode.LCD_ONLY) {
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.CursorVisible = false;
			}
		}

		public void Write(string message) {
			if (DisplayMode != DisplayMode.LCD_ONLY) {
				int linesWritten = 0;
				string messageLeft = message;
				Console.Clear();
				Console.BackgroundColor = backlightOn ? ConsoleColor.Black : ConsoleColor.Gray;
				Console.ForegroundColor = ConsoleColor.White;
				consoleContent = new StringWriter();
				while (messageLeft.Length > 0 && linesWritten < Rows) {
					string thisLine = messageLeft.Substring(0, Math.Min(messageLeft.Length, Columns));
					Console.WriteLine(thisLine);
					consoleContent.WriteLine(thisLine);
					messageLeft = messageLeft.Substring(thisLine.Length);
				}
			}
			if (DisplayMode != DisplayMode.CONSOLE_ONLY) {
				lcd.Clear();
				lcd.Write(message);
			}
		}

		public void Write(IEnumerable<string> messages) {
			if (DisplayMode != DisplayMode.LCD_ONLY) {
				Console.Clear();
				Console.BackgroundColor = backlightOn ? ConsoleColor.Black : ConsoleColor.Gray;
				Console.ForegroundColor = ConsoleColor.White;
				consoleContent = new StringWriter();
				if (messages.Count() > Rows)
					messages = messages.Take(Rows);
				foreach (string message in messages){
					string thisLine = message.Substring(0, Math.Min(Columns, message.Length));
					Console.WriteLine(thisLine);
					consoleContent.WriteLine(thisLine);
				}
			}
			if (DisplayMode != DisplayMode.CONSOLE_ONLY) {
				lcd.Clear();
				int linesWritten = 0;
				while (linesWritten < Rows) {
					lcd.Write(messages.ElementAt(linesWritten));
					linesWritten++;
					if (linesWritten < Rows)
						lcd.SetCursorPosition(linesWritten, 0);
				}
			}
		}

		public void Backlight(bool on) {
			if (DisplayMode != DisplayMode.LCD_ONLY) {
				backlightOn = on;
				Console.Clear();
				Console.BackgroundColor = on ? ConsoleColor.Black : ConsoleColor.Gray;
				Console.ForegroundColor = ConsoleColor.White;
				Console.Write(consoleContent.ToString());
			}
			if(DisplayMode != DisplayMode.CONSOLE_ONLY)
				lcd.Backlight = on;
		}

		public void Power(bool on) {
			if (DisplayMode != DisplayMode.LCD_ONLY) {
				if (on)
					Console.Write(consoleContent.ToString());
				else
					Console.Clear();
			}
			if (DisplayMode != DisplayMode.CONSOLE_ONLY) {
				lcd.Visible = on;
				lcd.Backlight = on;
			}
		}
	}

	public enum DisplayMode {
		LCD_ONLY = 1,
		CONSOLE_ONLY,
		BOTH
	}
}
