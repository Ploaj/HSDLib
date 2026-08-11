using HSDRaw.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HSDRawViewer.IO.Splines
{
    public sealed class SplineObj
    {
        public List<HSD_Vector3> Vertices { get; } = new();

        public List<SplineObjObject> Objects { get; } = new();

        public void Open(string filePath)
        {
            SplineObjObject currentGroup = null;

            foreach (string line in File.ReadLines(filePath))
            {
                string[] parts = line.Split(
                    (char[])null,
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    continue;

                switch (parts[0])
                {
                    case "o":
                        currentGroup = new SplineObjObject
                        {
                            Name = parts.Length > 1
                                ? string.Join(' ', parts.Skip(1))
                                : string.Empty
                        };

                        Objects.Add(currentGroup);
                        break;

                    case "v":
                        if (parts.Length >= 4 &&
                            float.TryParse(parts[1], NumberStyles.Float,
                                CultureInfo.InvariantCulture, out float x) &&
                            float.TryParse(parts[2], NumberStyles.Float,
                                CultureInfo.InvariantCulture, out float y) &&
                            float.TryParse(parts[3], NumberStyles.Float,
                                CultureInfo.InvariantCulture, out float z))
                        {
                            Vertices.Add(new HSD_Vector3(x, y, z));
                        }
                        break;

                    case "l":
                        if (currentGroup != null)
                        {
                            SplineObjLine lineData = new();

                            for (int i = 1; i < parts.Length; i++)
                            {
                                if (!int.TryParse(parts[i], out int index))
                                    continue;

                                // OBJ indices are 1-based.
                                // Negative indices are relative to the end.
                                index = index >= 0
                                    ? index - 1
                                    : Vertices.Count + index;

                                if (index >= 0 && index < Vertices.Count)
                                    lineData.Indices.Add(index);
                            }

                            if (lineData.Indices.Count > 0)
                                currentGroup.Lines.Add(lineData);
                        }
                        break;
                }
            }
        }
        public void Save(string filePath)
        {
            using StreamWriter writer = new(
                filePath,
                false,
                new UTF8Encoding(false));

            foreach (HSD_Vector3 vertex in Vertices)
            {
                writer.WriteLine(
                    $"v {vertex.X.ToString(CultureInfo.InvariantCulture)} " +
                    $"{vertex.Y.ToString(CultureInfo.InvariantCulture)} " +
                    $"{vertex.Z.ToString(CultureInfo.InvariantCulture)}");
            }

            foreach (SplineObjObject obj in Objects)
                obj.Save(writer);
        }
    }

    public sealed class SplineObjObject
    {
        public string Name { get; set; } = string.Empty;

        public List<SplineObjLine> Lines { get; } = new();

        /// <summary>
        /// Gets all vertex indices connected to the first line.
        /// </summary>
        public IEnumerable<int> GetConnectedIndices()
        {
            if (Lines.Count == 0)
                yield break;

            HashSet<int> visited = new();
            Queue<int> queue = new();

            foreach (int index in Lines[0].Indices)
            {
                if (visited.Add(index))
                {
                    queue.Enqueue(index);
                    yield return index;
                }
            }

            while (queue.Count > 0)
            {
                int currentIndex = queue.Dequeue();

                foreach (SplineObjLine line in Lines)
                {
                    if (!line.Indices.Contains(currentIndex))
                        continue;

                    foreach (int index in line.Indices)
                    {
                        if (visited.Add(index))
                        {
                            queue.Enqueue(index);
                            yield return index;
                        }
                    }
                }
            }
        }

        public void Save(StreamWriter writer)
        {
            writer.WriteLine($"o {Name}");

            foreach (SplineObjLine line in Lines)
            {
                writer.Write("l");

                foreach (int index in line.Indices)
                    writer.Write($" {index + 1}");

                writer.WriteLine();
            }
        }
    }

    public sealed class SplineObjLine
    {
        public List<int> Indices { get; } = new();
    }
}
