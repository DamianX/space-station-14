using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Content.Server.GameObjects.Components.DCPU
{
	public interface IHardware
	{
		UInt32 ID { get; }
		UInt32 Manufactorer { get; }
		ushort Version { get; }

		void Interrupt();
	}
}
