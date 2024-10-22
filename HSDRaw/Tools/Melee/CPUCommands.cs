using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSDRaw.Tools.Melee
{
    public class CPUCommandDesc
    {
        public int CommandByte { get; }

        public string Name { get; }

        public int ParamCount { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandByte"></param>
        /// <param name="name"></param>
        /// <param name="paramCount"></param>
        public CPUCommandDesc(int commandByte, string name, int paramCount)
        {
            CommandByte = commandByte;
            Name = name;
            ParamCount = paramCount;
        }
    }

    public class CPUCommands
    {
        private static List<CPUCommandDesc> _commands = new List<CPUCommandDesc>()
        {
            new CPUCommandDesc(0x00, "None", 0),
            new CPUCommandDesc(0x01, "PressA", 0),
            new CPUCommandDesc(0x02, "ReleaseA", 0),
            new CPUCommandDesc(0x03, "PressB", 0),
            new CPUCommandDesc(0x04, "ReleaseB", 0),
            new CPUCommandDesc(0x05, "PressX", 0),
            new CPUCommandDesc(0x06, "ReleaseX", 0),
            new CPUCommandDesc(0x07, "PressY", 0),
            new CPUCommandDesc(0x08, "ReleaseY", 0),
            new CPUCommandDesc(0x09, "PressR", 0),
            new CPUCommandDesc(0x0A, "ReleaseR", 0),
            new CPUCommandDesc(0x0B, "PressL", 0),
            new CPUCommandDesc(0x0C, "ReleaseL", 0),
            new CPUCommandDesc(0x0D, "PressZ", 0),
            new CPUCommandDesc(0x0E, "ReleaseZ", 0),
            new CPUCommandDesc(0x0F, "PressUp", 0),
            new CPUCommandDesc(0x10, "ReleaseUp", 0),
            new CPUCommandDesc(0x11, "PressDown", 0),
            new CPUCommandDesc(0x12, "ReleaseDown", 0),
            new CPUCommandDesc(0x13, "PressRight", 0),
            new CPUCommandDesc(0x14, "ReleaseRight", 0),
            new CPUCommandDesc(0x15, "PressLeft", 0),
            new CPUCommandDesc(0x16, "ReleaseLeft", 0),
            new CPUCommandDesc(0x17, "PressStart", 0),
            new CPUCommandDesc(0x18, "ReleaseStart", 0),
            new CPUCommandDesc(0x19, "ReleaseAll", 0),

            new CPUCommandDesc(0x7F, "End", 0),

            new CPUCommandDesc(0x80, "SetLStickX", 1),
            new CPUCommandDesc(0x81, "SetLStickY", 1),
            new CPUCommandDesc(0x82, "SetCStickX", 1),
            new CPUCommandDesc(0x83, "SetCStickY", 1),
            new CPUCommandDesc(0x84, "SetTriggerR", 1),
            new CPUCommandDesc(0x85, "SetTriggerL", 1),
            new CPUCommandDesc(0x86, "HoldA", 1),
            new CPUCommandDesc(0x87, "HoldReleaseA", 1),
            new CPUCommandDesc(0x88, "HoldB", 1),
            new CPUCommandDesc(0x89, "HoldReleaseB", 1),
            new CPUCommandDesc(0x8A, "HoldX", 1),
            new CPUCommandDesc(0x8B, "HoldReleaseX", 1),
            new CPUCommandDesc(0x8C, "HoldY", 1),
            new CPUCommandDesc(0x8D, "HoldReleaseY", 1),
            new CPUCommandDesc(0x8E, "Wait", 1),
            new CPUCommandDesc(0x8F, "SetLStickAngleTarget", 1),
            new CPUCommandDesc(0x90, "SetLStickXTarget", 1),
            new CPUCommandDesc(0x91, "SetLStickXFacing", 1),
            new CPUCommandDesc(0x92, "WaitInState", 1),
            new CPUCommandDesc(0x93, "SetScenario", 1),
            new CPUCommandDesc(0x94, "SetLStickAngleFighter", 1),
            new CPUCommandDesc(0x95, "SetLStickXFighter", 1),

            new CPUCommandDesc(0xC0, "AddLStickAngleTarget", 2),
            new CPUCommandDesc(0xC1, "AddLStickXTarget", 2),
            new CPUCommandDesc(0xC2, "AddLStickXFacing", 2),
        };

        public static string Decompile(byte[] data)
        {
            StringBuilder b = new StringBuilder();

            for (int i = 0; i < data.Length;)
            {
                var command = data[i++];
                var desc = _commands.Find(e => e.CommandByte == command);

                if (desc == null)
                    break;

                b.AppendLine($"{desc.Name}({string.Join(",", new byte[desc.ParamCount].Select(e => (sbyte)data[i++]))})");

                if (command == 0x7F)
                    break;
            }

            return b.ToString();
        }
    }
}
