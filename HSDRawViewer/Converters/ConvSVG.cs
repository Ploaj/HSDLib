using HSDRaw.Melee.Gr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HSDRawViewer.Converters
{
    public class ConvSVG
    {
        private static Dictionary<CollMaterial, string> materialToColor = new Dictionary<CollMaterial, string>()
        {
            { CollMaterial.Basic, "stroke:#808080" },
            { CollMaterial.Rock, "stroke:#806060)" },
            { CollMaterial.Grass, "stroke:#40ff40" },
            { CollMaterial.Dirt, "stroke:#c06060" },
            { CollMaterial.Wood, "stroke:#c08040" },
            { CollMaterial.HeavyMetal, "stroke:#604040" },
            { CollMaterial.LightMetal, "stroke:#404040" },
            { CollMaterial.UnkFlatZone, "stroke:#c0c0c0" },
            { CollMaterial.AlienGoop, "stroke:#df8f7f" },
            { CollMaterial.Water, "stroke:#3030ff" },
            { CollMaterial.Glass, "stroke:#c0c0ff" },
            { CollMaterial.Checkered, "stroke:#ffffc0" },
            { CollMaterial.FlatZone, "stroke:#c0c0c0" },
        };


        public static void CollDataToSVG(string filename, SBM_Coll_Data colldata)
        {
            svg svg = new svg();

            var groups = new List<svgShape>();
            foreach (var area in colldata.LineGroups)
            {
                svgGroup g = new svgGroup();
                List<svgShape> lines = new List<svgShape>();
                lines.AddRange(AreaToList(colldata, area.TopLineIndex, area.TopLineCount).ToArray());
                lines.AddRange(AreaToList(colldata, area.LeftLineIndex, area.LeftLineCount).ToArray());
                lines.AddRange(AreaToList(colldata, area.RightLineIndex, area.RightLineCount).ToArray());
                lines.AddRange(AreaToList(colldata, area.BottomLineIndex, area.BottomLineCount).ToArray());
                g.shapes = lines.ToArray();
                groups.Add(g);
            }
            svg.groups = groups.ToArray();

            using (var settings = new XmlTextWriter(new FileStream(filename, FileMode.Create), Encoding.UTF8))
            {
                settings.Indentation = 4;
                settings.Formatting = Formatting.Indented;
                //settings.Namespaces = false;
                //settings.Settings.OmitXmlDeclaration = true;

                XmlSerializer serializer = new XmlSerializer(typeof(svg));
                serializer.Serialize(settings, svg);
            }
        }

        private static List<svgShape> AreaToList(SBM_Coll_Data colldata, int start, int count)
        {
            var list = new List<svgShape>();
            for (int i = start; i < start + count; i++)
            {
                var link = colldata.Links[i];
                svgLine line = new svgLine()
                {
                    x1 = 500 + colldata.Vertices[link.VertexIndex1].X,
                    y1 = 500 - colldata.Vertices[link.VertexIndex1].Y,
                    x2 = 500 + colldata.Vertices[link.VertexIndex2].X,
                    y2 = 500 - colldata.Vertices[link.VertexIndex2].Y
                };
                line.style = materialToColor[(CollMaterial)link.Material];
                list.Add(line);
            }
            return list;
        }
    }
    
    public class svg
    {
        [XmlAttribute]
        public float width { get; set; } = 1000;

        [XmlAttribute]
        public float height { get; set; } = 1000;
        
        [XmlElement("line", typeof(svgLine))]
        public svgShape[] shapes { get; set; }
        
        [XmlElement("g", typeof(svgGroup))]
        public svgShape[] groups { get; set; }
    }

    public class svgShape
    {

    }

    public class svgGroup : svgShape
    {
        [XmlElement("line", typeof(svgLine))]
        public svgShape[] shapes { get; set; }
    }

    public class svgLine : svgShape
    {
        [XmlAttribute]
        public float x1 { get; set; }

        [XmlAttribute]
        public float y1 { get; set; }

        [XmlAttribute]
        public float x2 { get; set; }

        [XmlAttribute]
        public float y2 { get; set; }

        [XmlAttribute]
        public string style { get; set; } = "stroke:rgb(255,0,0);";
    }
    
}
