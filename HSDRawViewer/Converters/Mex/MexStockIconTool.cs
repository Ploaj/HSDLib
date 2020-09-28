using HSDRaw;
using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.MEX;
using HSDRaw.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters.Mex
{
    public class MexStockIconTool
    {
        /// <summary>
        /// 
        /// </summary>
        public static void GenerateStockNode(string ifallPath)
        {
            var file = new HSDRawFile(ifallPath);
            var path = ifallPath;

            var stockSymbol = file.Roots.Find(e => e.Name.Equals("Stc_icns"));

            if (stockSymbol == null)
            {
                var stockModel = file.Roots.Find(e => e.Name.Equals("Stc_scemdls")).Data as HSDNullPointerArrayAccessor<HSD_JOBJDesc>;
                var texanim = stockModel[0].MaterialAnimations[0].Child.MaterialAnimation.TextureAnimation;

                file.Roots.Add(new HSDRootNode()
                {
                    Name = "Stc_icns",
                    Data = GenerateStockIconNodeFromVanilla(texanim)
                });
            }

            file.TrimData();
            file.Save(path + "_new.dat");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static MEX_Stock GenerateStockIconNodeFromVanilla(HSD_TexAnim texanim)
        {
            // extract tex anim assets
            var tobjs = texanim.ToTOBJs();
            var keys = texanim.AnimationObject.FObjDesc.GetDecodedKeys();
            FOBJ_Player player = new FOBJ_Player();
            player.Keys = keys;


            // generate stock node
            var stockNode = new MEX_Stock();

            stockNode.Reserved = 10;
            stockNode.Stride = 26;

            var newTobjs = new List<HSD_TOBJ>();
            var newKeys = new List<FOBJKey>();

            // get hardcoded images
            /*
             *  0 - blank
1 - smash ball
2 - master hand
3 - crazy hand
4 - target
5 - giga bowser
6 - sandbag
7 - red dot
8 - furby
            */

            int[] hardcoded = new[] { 250, 26, 27, 28, 57, 58, 59, 185, 185 };

            for (int i = 0; i < hardcoded.Length; i++)
            {
                newKeys.Add(new FOBJKey() { Frame = i, InterpolationType = GXInterpolationType.HSD_A_OP_CON, Value = newTobjs.Count });
                newTobjs.Add(tobjs[(int)player.GetValue(hardcoded[i])]);
            }

            // get fighter stock icons
            for (int i = 0; i < 27; i++)
            {
                int color = 0;
                while (true)
                {
                    var key = keys.Find(e => e.Frame == color * 30 + (i == 26 ? 29 : i));

                    if (key != null)
                    {
                        newKeys.Add(new FOBJKey() { Frame = 10 + color * 26 + i, InterpolationType = GXInterpolationType.HSD_A_OP_CON, Value = newTobjs.Count });
                        newTobjs.Add(tobjs[(int)key.Value]);
                        color++;
                    }
                    else
                        break;
                }
            }

            // order keys
            newKeys = newKeys.OrderBy(e => e.Frame).ToList();
            foreach (var k in newKeys)
                Console.WriteLine(k.Frame + " " + k.Value);

            // generate new tex anim
            var newTexAnim = new HSD_TexAnim();

            newTexAnim.AnimationObject = new HSD_AOBJ();
            newTexAnim.AnimationObject.FObjDesc = new HSD_FOBJDesc();
            newTexAnim.AnimationObject.FObjDesc.SetKeys(newKeys, (byte)TexTrackType.HSD_A_T_TIMG);
            newTexAnim.AnimationObject.FObjDesc.Next = new HSD_FOBJDesc();
            newTexAnim.AnimationObject.FObjDesc.Next.SetKeys(newKeys, (byte)TexTrackType.HSD_A_T_TCLT);

            newTexAnim.FromTOBJs(newTobjs, false);

            stockNode.MatAnimJoint = new HSD_MatAnimJoint();
            stockNode.MatAnimJoint.MaterialAnimation = new HSD_MatAnim();
            stockNode.MatAnimJoint.MaterialAnimation.TextureAnimation = newTexAnim;

            return stockNode;
        }

    }
}
