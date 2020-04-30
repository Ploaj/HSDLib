using HSDRaw;
using HSDRaw.Melee.Pl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSDRawViewer.Converters
{
    public class MexFighterConverter
    {

        /// <summary>
        /// Takes a fighter file and converts it for easier use with mex with optional rename
        /// 1. Renames strings to new fighter names
        /// TODO: 2. Replace ids with personal ids in subactions
        /// 3. 
        /// </summary>
        public static void ConvertToMEXFighter(SBM_FighterData fighterData, string oldName, string newName, string AJPath, string RstPath)
        {
            byte[] ajdata = File.ReadAllBytes(AJPath);
            var rstFile = new HSDRawFile(RstPath).Roots[0].Data;

            var sa = fighterData.SubActionTable.Subactions;
            foreach (var v in sa)
            {
                v.Name = v.Name.Replace(oldName, newName);
            }
            fighterData.SubActionTable.Subactions = sa;

            var wsa = fighterData.WinSubAction.Subactions;
            foreach (var v in wsa)
            {
                v.Name = v.Name.Replace(oldName, newName);

            }
            fighterData.WinSubAction.Subactions = wsa;
        }

    }
}
