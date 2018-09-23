using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MeleeLib.DAT;
using MeleeLib.IO;
using MeleeLib.DAT.Script;
using MeleeLib;
using MeleeLib.KAR;
using HSDLib;
using HSDLib.Common;

namespace Modlee
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //HSDFile HSD = new HSDFile("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\PlMrNr.dat");
            /*foreach(HSD_JOBJ jobj in ((HSD_JOBJ)HSD.Roots[0].Node).DepthFirstList)
            {
                if(jobj.DOBJ != null)
                foreach (HSD_DOBJ dobj in jobj.DOBJ.List)
                {
                    Console.WriteLine(dobj.POBJ.List.Length);
                }
            }*/
            //HSDFile HSD2 = new HSDFile("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\TyCoin.dat");
            //DatTexture t = new DatTexture();
            //t.SetFromBitmap(new System.Drawing.Bitmap("13.png"), TPL_TextureFormat.RGB5A3, TPL_PaletteFormat.RGB565);
            //TriangleConverter tc = new TriangleConverter(true, 256, 2, true);
            //Facepoint p = new Facepoint();
            //p._vertex = new Vertex3();

            //tc.GroupPrimitives();
            /*DATReader read = new DATReader(File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTexture.raw"));
            
            read.BigEndian = false;
            DATWriter write = new DATWriter();
            for (int i = 0; i < read.Size(); i += 8)
            {
                write.Short((short)read.Short());
                write.Short((short)read.Short());
                write.Int(0);
                read.Int();
            }
            write.Save("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTextureSwap.raw");*/

            // File.WriteAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTextureSwapRecomp.raw", TPL.ToCMP(File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTextureSwap.raw"), 128, 64));
            //TPL.ConvertFromTextureMelee(File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTexture.raw"), 128, 64, 14, null, 0, 0).Save("C:\\Users\\Owen\\Desktop\\Modlee\\Tests\\CompTexture.png");
            /*DATFile d = Decompiler.Decompile(File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\PlNs.dat"));

            Console.WriteLine(d.Roots[0].FighterData[0].Scripts[52].SubActions.Count);
            foreach(DatFighterScript script in d.Roots[0].FighterData[0].Scripts)
            foreach (SubAction a in script.SubActions)
            {
                CompileError err;
                SubAction adc = MeleeCMD.CompileCommand(MeleeCMD.DecompileSubAction(a), out err);
                    //Console.WriteLine(err + " " + MeleeCMD.DecompileSubAction(a));
                if(!MeleeCMD.DecompileSubAction(adc).Equals(MeleeCMD.DecompileSubAction(a)))
                    Console.WriteLine(MeleeCMD.DecompileSubAction(adc));
            }*/

            //CollisionData cd = new CollisionData(File.ReadAllBytes("C:\\Users\\Owen\\Desktop\\GrTest7.dat"));

            //DATFile f = Decompiler.Decompile(File.ReadAllBytes("PlFcNrOrg.dat"));
            //Compiler.Compile(f, "Test.bin");

            /*string[] files = Directory.GetFiles("C:\\Users\\Owen\\Desktop\\Modlee\\Melee\\root\\");
            
            foreach(string file in files)
            {
                if (Path.GetFileName(file).StartsWith("Pl") && Path.GetFileName(file).EndsWith("Nr.dat"))
                {
                    Console.WriteLine(Path.GetFileName(file));
                    DATFile f = Decompiler.Decompile(File.ReadAllBytes(file));
                    Compiler.Compile(f, "Test.bin");
                }
            }*/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
