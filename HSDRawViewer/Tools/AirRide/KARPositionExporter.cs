using HSDRaw.AirRide.Gr.Data;
using IONET.Core;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using System.Numerics;

namespace HSDRawViewer.Tools.AirRide
{
    public class KARPositionExporter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Quaternion LookRotation(Vector3 forward, Vector3 up)
        {
            forward = Vector3.Normalize(forward); // Ensure forward vector is normalized
            up = Vector3.Cross(up, forward); // Ensure up vector is orthogonal to forward, then normalize
            up = Vector3.Normalize(up);

            Vector3 right = Vector3.Cross(forward, up); // Calculate the right vector

            // Create a rotation matrix
            Matrix4x4 rotationMatrix = new(
                right.X, up.X, -forward.X, 0,
                right.Y, up.Y, -forward.Y, 0,
                right.Z, up.Z, -forward.Z, 0,
                0, 0, 0, 1
            );

            return Quaternion.CreateFromRotationMatrix(rotationMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="list"></param>
        public static void ExportAreaPositions(string filePath, KAR_grAreaPositionList list)
        {
            // generate io model
            IOModel iomodel = new();

            // setup skeleton
            iomodel.Skeleton = new IOSkeleton();
            IOBone root = new()
            {
                Name = "root"
            };
            iomodel.Skeleton.RootBones.Add(root);

            int index = 0;
            foreach (KAR_grAreaPositionData e in list.AreaPosition.Array)
            {
                root.AddChild(new IOBone()
                {
                    Name = $"Area_{index}",
                    Translation = new Vector3(e.X, e.Y, e.Z),
                    Rotation = LookRotation(new Vector3(e.DX, e.DY, e.DZ), Vector3.UnitY),
                });
                index++;
            }

            IOScene ioscene = new();
            ioscene.Models.Add(iomodel);
            IONET.IOManager.ExportScene(ioscene, filePath, new IONET.ExportSettings()
            {

            });
        }
    }
}
