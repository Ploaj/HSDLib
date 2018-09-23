using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Text.RegularExpressions;

namespace Modlee.Rendering.Tristripper
{
    public class StripperOptions
    {
        public ulong[]          DFaces				= null;
        public uint[]			WFaces				= null;
        public uint NbFaces				= 0;
        public bool AskForWords			= true;
        public bool OneSided			= true;
        public bool SGIAlgorithm		= true;
        public bool ConnectAllStrips	= false;
    }

    public class StripperResult<T>
    {
        public ulong NbStrips;            // #strips created
        public ulong[] StripLengths;       // Lengths of the strips (NbStrips values)
        public T[] StripRuns;            // The strips in words or dwords, depends on AskForWords
        public bool AskForWords;		    // true => results are in words (else dwords)
    }

    public class Tristripper
    {
        Adjacencies mAdj;
        public uint NbFaces = 0;
        public bool mAskForWords = true;
        public bool mOneSided = true;
        public bool mSGIAlgorithm = true;
        public bool mConnectAllStrips = false;

        public bool Init(StripperOptions create)
        {
            // Create adjacencies
            {
                mAdj = new Adjacencies();
                if (mAdj == null)
                    return false;

                AdjacencyInput ac = new AdjacencyInput() ;
                ac.NbFaces = create.NbFaces;
                ac.DFaces = create.DFaces;
                ac.WFaces = create.WFaces;
                bool Status = mAdj.Init(ac);

                Status = mAdj.CreateDatabase();

                mAskForWords = create.AskForWords;
                mOneSided = create.OneSided;
                mSGIAlgorithm = create.SGIAlgorithm;
                mConnectAllStrips = create.ConnectAllStrips;
            }

            return true;
        }
        public bool[] mTags;
        public int mNbStrips;


        public bool IS_BOUNDARY(ulong x) { return x == ulong.MaxValue; }

        public bool Compute(StripperResult<ulong> result)
        {
            // You must call Init() first
            if (mAdj == null) return false;

            // Get some bytes
            mStripLengths = new CustomArray; if (mStripLengths == null) return false;
            mStripRuns = new CustomArray; if (mStripRuns == null) return false;
            mTags = new bool[mAdj.mNbFaces]; if (mTags == null) return false;
            ulong[] Connectivity = new ulong[mAdj.mNbFaces]; if (Connectivity == null) return false;

            // mTags contains one bool/face. True=>the face has already been included in a strip
            //ZeroMemory(mTags, mAdj->mNbFaces * sizeof(bool));

            // Compute the number of connections for each face. This buffer is further recycled into
            // the insertion order, ie contains face indices in the order we should treat them
            //ZeroMemory(Connectivity, mAdj->mNbFaces * sizeof(ulong));
            if (mSGIAlgorithm)
            {
                // Compute number of adjacent triangles for each face
                for (ulong i = 0; i < mAdj.mNbFaces; i++)
                {
                    AdjTriangle Tri = mAdj.mFaces[i];
                    if (!IS_BOUNDARY(Tri.ATri[0])) Connectivity[i]++;
                    if (!IS_BOUNDARY(Tri.ATri[1])) Connectivity[i]++;
                    if (!IS_BOUNDARY(Tri.ATri[2])) Connectivity[i]++;
                }

                // Sort by number of neighbors
                RadixSort RS = new RadixSort();
                ulong[] Sorted = RS.Sort(Connectivity, mAdj.mNbFaces).GetIndices();

                // The sorted indices become the order of insertion in the strips
                CopyMemory(Connectivity, Sorted, mAdj.mNbFaces * sizeof(ulong));
            }
            else
            {
                // Default order
                for (ulong i = 0; i < mAdj.mNbFaces; i++) Connectivity[i] = i;
            }

            mNbStrips = 0;  // #strips created
            ulong TotalNbFaces = 0; // #faces already transformed into strips
            ulong Index = 0;    // Index of first face

            while (TotalNbFaces != mAdj.mNbFaces)
            {
                // Look for the first face [could be optimized]
                while (mTags[Connectivity[Index]]) Index++;
                ulong FirstFace = Connectivity[Index];

                // Compute the three possible strips from this face and take the best
                TotalNbFaces += ComputeBestStrip(FirstFace);

                // Let's wrap
                mNbStrips++;
            }

            // Free now useless ram
            //RELEASEARRAY(Connectivity);
            //RELEASEARRAY(mTags);

            // Fill result structure and exit
            result.NbStrips = (ulong)mNbStrips;
            //result.StripLengths = (ulong[])mStripLengths.Collapse();
            //result.StripRuns = mStripRuns.Collapse();

            //if (mConnectAllStrips) ConnectAllStrips(result);

            return true;
        }

        public ulong ComputeBestStrip(ulong face)
        {
            ulong[] Strip = new ulong[3];       // Strips computed in the 3 possible directions
            ulong[] Faces = new ulong[3];       // Faces involved in the 3 previous strips
            ulong[] Length = new ulong[3];        // Lengths of the 3 previous strips

            ulong[] FirstLength = new ulong[3];   // Lengths of the first parts of the strips are saved for culling

            // Starting references
            ulong[] Refs0 = new ulong[3];
            ulong[] Refs1 = new ulong[3];
            Refs0[0] = mAdj.mFaces[face].VRef[0];
            Refs1[0] = mAdj.mFaces[face].VRef[1];

            // Bugfix by Eric Malafeew!
            Refs0[1] = mAdj.mFaces[face].VRef[2];
            Refs1[1] = mAdj.mFaces[face].VRef[0];

            Refs0[2] = mAdj.mFaces[face].VRef[1];
            Refs1[2] = mAdj.mFaces[face].VRef[2];

            // Compute 3 strips
            for (ulong j = 0; j < 3; j++)
            {
                // Get some bytes for the strip and its faces
                Strip[j] = new ulong[mAdj.mNbFaces + 2 + 1 + 2];   // max possible length is NbFaces+2, 1 more if the first index gets replicated
                Faces[j] = new ulong[mAdj.mNbFaces + 2];
                //FillMemory(Strip[j], (mAdj.mNbFaces + 2 + 1 + 2) * sizeof(ulong), 0xff);
                //FillMemory(Faces[j], (mAdj.mNbFaces + 2) * sizeof(ulong), 0xff);

                // Create a local copy of the tags
                bool[] Tags = new bool[mAdj.mNbFaces];
                mTags = Tags; //CopyMemory(Tags, mTags, mAdj->mNbFaces * sizeof(bool));

                // Track first part of the strip
                Length[j] = TrackStrip(face, Refs0[j], Refs1[j], &Strip[j][0], &Faces[j][0], Tags);

                // Save first length for culling
                FirstLength[j] = Length[j];
                //		if(j==1)	FirstLength[j]++;	// ...because the first face is written in reverse order for j==1

                // Reverse first part of the strip
                for (ulong i = 0; i < Length[j] / 2; i++)
                {
                    Strip[j][i] ^= Strip[j][Length[j] - i - 1];
                    Strip[j][Length[j] - i - 1] ^= Strip[j][i];
                    Strip[j][i] ^= Strip[j][Length[j] - i - 1];
                }
                for (i = 0; i < (Length[j] - 2) / 2; i++)
                {
                    Faces[j][i] ^= Faces[j][Length[j] - i - 3];
                    Faces[j][Length[j] - i - 3] ^= Faces[j][i];
                    Faces[j][i] ^= Faces[j][Length[j] - i - 3];
                }

                // Track second part of the strip
                ulong NewRef0 = Strip[j][Length[j] - 3];
                ulong NewRef1 = Strip[j][Length[j] - 2];
                ulong ExtraLength = TrackStrip(face, NewRef0, NewRef1, &Strip[j][Length[j] - 3], &Faces[j][Length[j] - 3], Tags);
                Length[j] += ExtraLength - 3;

                // Free temp ram
                RELEASEARRAY(Tags);
            }

            // Look for the best strip among the three
            ulong Longest = Length[0];
            ulong Best = 0;
            if (Length[1] > Longest) { Longest = Length[1]; Best = 1; }
            if (Length[2] > Longest) { Longest = Length[2]; Best = 2; }

            ulong NbFaces = Longest - 2;

            // Update global tags
            for (j = 0; j < Longest - 2; j++) mTags[Faces[Best][j]] = true;

            // Flip strip if needed ("if the length of the first part of the strip is odd, the strip must be reversed")
            if (mOneSided && FirstLength[Best] & 1)
            {
                // Here the strip must be flipped. I hardcoded a special case for triangles and quads.
                if (Longest == 3 || Longest == 4)
                {
                    // Flip isolated triangle or quad
                    Strip[Best][1] ^= Strip[Best][2];
                    Strip[Best][2] ^= Strip[Best][1];
                    Strip[Best][1] ^= Strip[Best][2];
                }
                else
                {
                    // "to reverse the strip, write it in reverse order"
                    for (j = 0; j < Longest / 2; j++)
                    {
                        Strip[Best][j] ^= Strip[Best][Longest - j - 1];
                        Strip[Best][Longest - j - 1] ^= Strip[Best][j];
                        Strip[Best][j] ^= Strip[Best][Longest - j - 1];
                    }

                    // "If the position of the original face in this new reversed strip is odd, you're done"
                    ulong NewPos = Longest - FirstLength[Best];
                    if (NewPos & 1)
                    {
                        // "Else replicate the first index"
                        for (j = 0; j < Longest; j++) Strip[Best][Longest - j] = Strip[Best][Longest - j - 1];
                        Longest++;
                    }
                }
            }

            // Copy best strip in the strip buffers
            for (j = 0; j < Longest; j++)
            {
                ulong Ref = Strip[Best][j];
                if (mAskForWords) mStripRuns->Store((uword)Ref);    // Saves word reference
                else mStripRuns->Store(Ref);            // Saves dword reference
            }
            mStripLengths->Store(Longest);

            // Free local ram
            for (j = 0; j < 3; j++)
            {
                RELEASEARRAY(Faces[j]);
                RELEASEARRAY(Strip[j]);
            }

            // Returns #faces involved in the strip
            return NbFaces;
        }

    }

    public class AdjacencyInput
    {
        public ulong NbFaces;     // #faces in source topo
        public ulong[] DFaces;         // list of faces (dwords) or null
        public uint[] WFaces;			// list of faces (words) or null
    }

    public class AdjTriangle
    {                                   // Should be derived from a triangle structure
        public ulong[] VRef = new ulong[3];     // Vertex-references
        public ulong[] ATri = new ulong[3];     // Links/References of adjacent triangles. The 2 most significant bits contains


        public byte FindEdge(ulong vref0, ulong vref1)
        {
            byte EdgeNb = 0xff;
            if (VRef[0] == vref0 && VRef[1] == vref1) EdgeNb = 0;
            else if (VRef[0] == vref1 && VRef[1] == vref0) EdgeNb = 0;
            else if (VRef[0] == vref0 && VRef[2] == vref1) EdgeNb = 1;
            else if (VRef[0] == vref1 && VRef[2] == vref0) EdgeNb = 1;
            else if (VRef[1] == vref0 && VRef[2] == vref1) EdgeNb = 2;
            else if (VRef[1] == vref1 && VRef[2] == vref0) EdgeNb = 2;
            return EdgeNb;
        }
    };

    public struct AdjEdge
    {
        public ulong Ref0;            // Vertex reference
        public ulong Ref1;            // Vertex reference
        public ulong FaceNb;          // Owner face
    };

    public class Adjacencies
    {
        public ulong mNbEdges;
        public ulong mCurrentNbFaces;
        public AdjEdge[] mEdges;

        public ulong mNbFaces;
        public AdjTriangle[] mFaces;

        public bool Init(AdjacencyInput create)
        {
            // Get some bytes
            mNbFaces = create.NbFaces;
            mFaces = new AdjTriangle[mNbFaces]; if (mFaces == null) return false;
            mEdges = new AdjEdge[mNbFaces * 3]; if (mEdges == null) return false;

            // Feed me with triangles.....
            for (ulong i = 0; i < mNbFaces; i++)
            {
                // Get correct vertex references
                ulong Ref0 = create.DFaces == null ? create.DFaces[i * 3 + 0] : create.WFaces == null ? create.WFaces[i * 3 + 0] : 0;
                ulong Ref1 = create.DFaces == null ? create.DFaces[i * 3 + 1] : create.WFaces == null ? create.WFaces[i * 3 + 1] : 1;
                ulong Ref2 = create.DFaces == null ? create.DFaces[i * 3 + 2] : create.WFaces == null ? create.WFaces[i * 3 + 2] : 2;

                // Add a triangle to the database
                AddTriangle(Ref0, Ref1, Ref2);
            }

            // At this point of the process we have mFaces & mEdges filled with input data. That is:
            // - a list of triangles with 3 null links (i.e. -1)
            // - a list of mNbFaces*3 edges, each edge having 2 vertex references and an owner face.

            return true;
        }

        public bool AddTriangle(ulong ref0, ulong ref1, ulong ref2)
        {
            // Store vertex-references
            mFaces[mCurrentNbFaces].VRef[0] = ref0;
            mFaces[mCurrentNbFaces].VRef[1] = ref1;
            mFaces[mCurrentNbFaces].VRef[2] = ref2;

            // Reset links
            mFaces[mCurrentNbFaces].ATri[0] = ulong.MaxValue;
            mFaces[mCurrentNbFaces].ATri[1] = ulong.MaxValue;
            mFaces[mCurrentNbFaces].ATri[2] = ulong.MaxValue;

            // Add edge 01 to database
            if (ref0 < ref1) AddEdge(ref0, ref1, mCurrentNbFaces);
            else AddEdge(ref1, ref0, mCurrentNbFaces);
            // Add edge 02 to database
            if (ref0 < ref2) AddEdge(ref0, ref2, mCurrentNbFaces);
            else AddEdge(ref2, ref0, mCurrentNbFaces);
            // Add edge 12 to database
            if (ref1 < ref2) AddEdge(ref1, ref2, mCurrentNbFaces);
            else AddEdge(ref2, ref1, mCurrentNbFaces);

            mCurrentNbFaces++;

            return true;
        }

        public bool AddEdge(ulong ref0, ulong ref1, ulong face)
        {
            // Store edge data
            mEdges[mNbEdges].Ref0 = ref0;
            mEdges[mNbEdges].Ref1 = ref1;
            mEdges[mNbEdges].FaceNb = face;
            mNbEdges++;
            return true;
        }

        public bool CreateDatabase()
        {
            // Here mNbEdges should be equal to mCurrentNbFaces*3.

            RadixSort Core = new RadixSort();

            ulong[] FaceNb = new ulong[mNbEdges]; if (FaceNb == null) return false;
            ulong[] VRefs0 = new ulong[mNbEdges]; if (VRefs0 == null) return false;
            ulong[] VRefs1 = new ulong[mNbEdges]; if (VRefs1 == null) return false;

            for (ulong i = 0; i < mNbEdges; i++)
            {
                FaceNb[i] = mEdges[i].FaceNb;
                VRefs0[i] = mEdges[i].Ref0;
                VRefs1[i] = mEdges[i].Ref1;
            }

            // Multiple sort
            ulong[] Sorted = Core.Sort(FaceNb, mNbEdges).Sort(VRefs0, mNbEdges).Sort(VRefs1, mNbEdges).GetIndices();

            // Read the list in sorted order, look for similar edges
            ulong LastRef0 = VRefs0[Sorted[0]];
            ulong LastRef1 = VRefs1[Sorted[0]];
            ulong Count = 0;
            ulong[] TmpBuffer = new ulong[3];

            for (int i = 0; i < (int)mNbEdges; i++)
            {
                ulong Face = FaceNb[Sorted[i]];     // Owner face
                ulong Ref0 = VRefs0[Sorted[i]];     // Vertex ref #1
                ulong Ref1 = VRefs1[Sorted[i]];     // Vertex ref #2
                if (Ref0 == LastRef0 && Ref1 == LastRef1)
                {
                    // Current edge is the same as last one
                    TmpBuffer[Count++] = Face;              // Store face number
                    if (Count == 3)
                    {
                        /*RELEASEARRAY(VRefs1);
                        RELEASEARRAY(VRefs0);
                        RELEASEARRAY(FaceNb);*/
                        return false;               // Only works with manifold meshes (i.e. an edge is not shared by more than 2 triangles)
                    }
                }
                else
                {
                    // Here we have a new edge (LastRef0, LastRef1) shared by Count triangles stored in TmpBuffer
                    if (Count == 2)
                    {
                        // if Count==1 => edge is a boundary edge: it belongs to a single triangle.
                        // Hence there's no need to update a link to an adjacent triangle.
                        bool Status2 = UpdateLink(TmpBuffer[0], TmpBuffer[1], LastRef0, LastRef1);
                        if (!Status2)
                        {
                            /*RELEASEARRAY(VRefs1);
                            RELEASEARRAY(VRefs0);
                            RELEASEARRAY(FaceNb);*/
                            return Status2;
                        }
                    }
                    // Reset for next edge
                    Count = 0;
                    TmpBuffer[Count++] = Face;
                    LastRef0 = Ref0;
                    LastRef1 = Ref1;
                }
            }
            bool Status = true;
            if (Count == 2) Status = UpdateLink(TmpBuffer[0], TmpBuffer[1], LastRef0, LastRef1);

            /*RELEASEARRAY(VRefs1);
            RELEASEARRAY(VRefs0);
            RELEASEARRAY(FaceNb);*/

            // We don't need the edges anymore
            //RELEASEARRAY(mEdges);

            return Status;
        }


        public bool UpdateLink(ulong firsttri, ulong secondtri, ulong ref0, ulong ref1)
        {
            AdjTriangle Tri0 = mFaces[firsttri];      // Catch the first triangle
            AdjTriangle Tri1 = mFaces[secondtri];     // Catch the second triangle

            // Get the edge IDs. 0xff means input references are wrong.
            byte EdgeNb0 = Tri0.FindEdge(ref0, ref1); if (EdgeNb0 == 0xff) return false;
            byte EdgeNb1 = Tri1.FindEdge(ref0, ref1); if (EdgeNb1 == 0xff) return false;

            // Update links. The two most significant bits contain the counterpart edge's ID.
            Tri0.ATri[EdgeNb0] = secondtri | ((ulong)(EdgeNb1) << 30);
            Tri1.ATri[EdgeNb1] = firsttri | ((ulong)(EdgeNb0) << 30);

            return true;
        }
    }


    public struct SMDVertex
    {
        public Vector3 P;
        public Vector3 N;
        public Vector2 UV;
        public float[] Weights;
        public int[] Bones;
    }

    public struct SMDTriangle
    {
        public SMDVertex v1, v2, v3;
        public String Material;
    }

    public class SMD
    {
        //public VBN Bones;
        //public Animation Animation; // todo
        public List<SMDTriangle> Triangles;

        public SMD()
        {
            Triangles = new List<SMDTriangle>();
        }

        public SMD(string fname)
        {
            Read(fname);
        }

        public void Read(string fname)
        {
            StreamReader reader = File.OpenText(fname);
            string line;

            string current = "";

            //Bones = new VBN();
            Triangles = new List<SMDTriangle>();
            //Dictionary<int, Bone> BoneList = new Dictionary<int, Bone>();
            
            while ((line = reader.ReadLine()) != null)
            {
                line = Regex.Replace(line, @"\s+", " ");
                string[] args = line.Replace(";", "").TrimStart().Split(' ');

                if (args[0].Equals("triangles") || args[0].Equals("end") || args[0].Equals("skeleton") || args[0].Equals("nodes"))
                {
                    current = args[0];
                    continue;
                }
                
                if (current.Equals("triangles"))
                {
                    string meshName = args[0];
                    if (args[0].Equals(""))
                        continue;

                    SMDTriangle t = new SMDTriangle();
                    Triangles.Add(t);
                    t.Material = meshName;

                    for (int j = 0; j < 3; j++)
                    {
                        line = reader.ReadLine();
                        line = Regex.Replace(line, @"\s+", " ");
                        args = line.Replace(";", "").TrimStart().Split(' ');

                        int parent = int.Parse(args[0]);
                        SMDVertex vert = new SMDVertex();
                        vert.P = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        vert.N = new Vector3(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]));
                        vert.UV = new Vector2(float.Parse(args[7]), float.Parse(args[8]));
                        vert.Bones = new int[0];
                        vert.Weights = new float[0];
                        if (args.Length > 9)
                        {
                            int wCount = int.Parse(args[9]);
                            int w = 10;
                            vert.Bones = new int[wCount];
                            vert.Weights = new float[wCount];
                            for (int i = 0; i < wCount; i++)
                            {
                                vert.Bones[i] = (int.Parse(args[w++]));
                                vert.Weights[i] = (float.Parse(args[w++]));
                            }
                        }
                        switch (j)
                        {
                            case 0: t.v1 = vert; break;
                            case 1: t.v2 = vert; break;
                            case 2: t.v3 = vert; break;
                        }
                    }
                }
            }
        }

        public void Save(string FileName)
        {
            StringBuilder o = new StringBuilder();

            if (Triangles != null)
            {
                o.AppendLine("triangles");
                foreach (SMDTriangle tri in Triangles)
                {
                    o.AppendLine(tri.Material);
                    WriteVertex(o, tri.v1);
                    WriteVertex(o, tri.v2);
                    WriteVertex(o, tri.v3);
                }
                o.AppendLine("end");
            }

            File.WriteAllText(FileName, o.ToString());
        }

        private void WriteVertex(StringBuilder o, SMDVertex v)
        {
            o.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8} ",
                        v.P.X, v.P.Y, v.P.Z,
                        v.N.X, v.N.Y, v.N.Z,
                        v.UV.X, v.UV.Y);
            if (v.Weights == null)
            {
                o.AppendLine("0");
            }
            else
            {
                string weights = v.Weights.Length + "";
                for (int i = 0; i < v.Weights.Length; i++)
                {
                    weights += " " + v.Bones[i] + " " + v.Weights[i];
                }
                o.AppendLine(weights);
            }
        }
    }
}
