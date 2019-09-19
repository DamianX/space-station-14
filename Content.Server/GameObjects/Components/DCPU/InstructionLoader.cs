using System;
using System.IO;
using System.Linq;
using Robust.Shared.Interfaces.Resources;
using Robust.Shared.IoC;

namespace Content.Server.GameObjects.Components.DCPU
{
    public class InstructionLoader
    {
        public static string RemoveWhitespace(string input)
        {
            return new string(input
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static byte[] StringToByteArray(String hex)
        {
            hex = RemoveWhitespace(hex);
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        /// <summary>
        /// Loads a binary as DCPU-16 instructions
        /// </summary>
        /// <param name="pathToBinary">The relative or absolute path to the file
        /// which stores binary DCPU-16 instructions. If relative, the path is
        /// relative to the application's current directory</param>
        /// <returns>An array of 16 bit words which represent binary instructions</returns>
        public static ushort[] Load (string pathToBinary)
        {
            byte[] buffer = StringToByteArray("01 98 02 7C 5D 00 02 90 21 8C 22 94 22 7C 32 00 41 04 42 00");

            ushort[] instructions = new ushort[buffer.Length / sizeof (ushort)];
            Buffer.BlockCopy (buffer, 0, instructions, 0, buffer.Length);

            return instructions;
        }
    }
}
