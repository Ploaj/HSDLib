using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.AirRide
{
    public class AirRideGrDataPositionProxy
    {
        public HSD_JOBJ _joint;
        public KAR_grPositionData _data;
        public KAR_grAreaPositionData _areadata;

        public float X
        {
            get => _joint == null ? (_data == null ? _areadata.X : _data.X) : _joint.TX;
            set
            {
                if (_joint == null)
                {
                    if (_data == null)
                        _areadata.X = value;
                    else
                        _data.X = value;
                }
                else _joint.TX = value;
            }
        }

        public float Y
        {
            get => _joint == null ? (_data == null ? _areadata.Y : _data.Y) : _joint.TY;
            set
            {
                if (_joint == null)
                {
                    if (_data == null)
                        _areadata.Y = value;
                    else
                        _data.Y = value;
                }
                else _joint.TY = value;
            }
        }

        public float Z
        {
            get => _joint == null ? (_data == null ? _areadata.Z : _data.Z) : _joint.TZ;
            set
            {
                if (_joint == null)
                {
                    if (_data == null)
                        _areadata.Z = value;
                    else
                        _data.Z = value;
                }
                else _joint.TZ = value;
            }
        }

        public AirRideGrDataPositionProxy()
        {
            _data = new KAR_grPositionData();
        }

        public AirRideGrDataPositionProxy(HSD_JOBJ joint)
        {
            _joint = HSDAccessor.DeepClone<HSD_JOBJ>(joint);
            _joint.Next = null;
            _joint.Child = null;
        }

        public AirRideGrDataPositionProxy(KAR_grPositionData data)
        {
            _data = data;
        }

        public AirRideGrDataPositionProxy(KAR_grAreaPositionData data)
        {
            _areadata = data;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AirRideGrDataPosition
    {
        public AirRideGrDataPositionProxy[] _positions { get; set; } = new AirRideGrDataPositionProxy[0];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="list"></param>
        public AirRideGrDataPosition(HSD_JOBJ joint, KAR_grAreaPositionList list)
        {
            if (list == null)
                return;

            if (list.JointIndices != null)
            {
                var joints = joint.TreeList;
                _positions = list.JointIndices.Array.Select(e => new AirRideGrDataPositionProxy(joints[e])).ToArray();
            }
            else
            if (list.AreaPositionArray != null)
                _positions = list.AreaPositionArray.Select(e => new AirRideGrDataPositionProxy(e)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="list"></param>
        public AirRideGrDataPosition(HSD_JOBJ joint, KAR_grPositionList list)
        {
            if (list == null)
                return;

            if (list.JointIndices != null)
            {
                var joints = joint.TreeList;
                _positions = list.JointIndices.Array.Select(e => new AirRideGrDataPositionProxy(joints[e])).ToArray();
            }
            else
            if (list.PositionDataArray != null)
                _positions = list.PositionDataArray.Select(e => new AirRideGrDataPositionProxy(e)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rootJoint"></param>
        /// <returns></returns>

        public KAR_grPositionList ToKarPositionList(HSD_JOBJ rootJoint)
        {
            if (_positions.Length == 0)
                return null;

            var list = new KAR_grPositionList()
            {
                Count = _positions.Length
            };

            foreach (var v in _positions)
            {
                // joint index
                if (v._joint != null)
                {
                    rootJoint.AddChild(v._joint);
                    if (list.JointIndices == null)
                        list.JointIndices = new HSDIntArray() { _s = new HSDStruct() };
                    list.JointIndices.Add(rootJoint.TreeList.Count - 1);
                }
                else
                // position data
                if (v._data != null)
                {
                    if (list.PositionData == null)
                        list.PositionData = new HSDArrayAccessor<KAR_grPositionData>();

                    list.PositionData.Add(v._data);
                }
                else
                //
                if (v._areadata != null)
                {

                }
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rootJoint"></param>
        /// <returns></returns>

        public KAR_grAreaPositionList ToKarAreaPositionList(HSD_JOBJ rootJoint)
        {
            if (_positions.Length == 0)
                return null;

            var list = new KAR_grAreaPositionList()
            {
                Count = _positions.Length
            };

            foreach (var v in _positions)
            {
                // joint index
                if (v._joint != null)
                {
                    rootJoint.AddChild(v._joint);
                    if (list.JointIndices == null)
                        list.JointIndices = new HSDIntArray() { _s = new HSDStruct() };
                    list.JointIndices.Add(rootJoint.TreeList.Count - 1);
                }
                else
                // position data
                if (v._data != null)
                {
                }
                else
                //
                if (v._areadata != null)
                {
                    if (list.AreaPosition == null)
                        list.AreaPosition = new HSDArrayAccessor<KAR_grAreaPositionData>();

                    list.AreaPosition.Add(v._areadata);
                }
            }

            return list;
        }
    }
}
