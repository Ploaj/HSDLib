using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleeLib.IO;
using MeleeLib;

namespace MeleeLib.KAR
{
    public class CollisionData
    {

        public CollisionData(byte[] data)
        {
            Decompile(data);
        }

        public void Decompile(byte[] data)
        {
            DATReader r = new DATReader(data);
            int filesize = r.Int();
            int Relocoff = r.Int();
            int RelocCount = r.Int();
            int rootcount = r.Int();

            r.ReadRelocationTable(Relocoff + 0x20, RelocCount);

            r.Seek(Relocoff + 0x20 + RelocCount * 4);

            for(int i = 0; i < rootcount;i++)
            {
                int Offset = r.Int();
                int Stringoff = r.Int();
                string Name = r.String();
                Console.WriteLine(Name);
                r.Seek(Offset + 0x20);
                Read(r);
            }
        }

        public void Read(DATReader r)
        {
            if (r.Int() != 0) throw new Exception("Unk1");
            int FloatOffset = r.Int();

            for(int i = 0; i < 20; i++)
            {
                Console.WriteLine(r.Int().ToString("X"));
            }


            /*r.Seek(FloatOffset);
            int count = r.Int();
            for (int i = 0; i < count; i++)
                Console.WriteLine(r.Float());
            Console.WriteLine(r.Pos().ToString("X"));*/
        }

    }
}
