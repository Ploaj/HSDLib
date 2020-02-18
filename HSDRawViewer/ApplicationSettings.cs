
using HSDRaw;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HSDRawViewer
{
    public class ApplicationSettings
    {
        public static bool UnlockedViewport { get; set; } = false;

        public static string HSDFileFilter { get; } = "HSD DAT Archive File|*.dat;*.usd";

        public static List<Type> HSDTypes { get; internal set; } = new List<Type>();

        public static void Init()
        {
            List<Type> types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                from assemblyType in domainAssembly.GetTypes()
                                where typeof(HSDAccessor).IsAssignableFrom(assemblyType)
                                select assemblyType).ToList();
            HSDTypes.AddRange(types);
        }
    }
}
