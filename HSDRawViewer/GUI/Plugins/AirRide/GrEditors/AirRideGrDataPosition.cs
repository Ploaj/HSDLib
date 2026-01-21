using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    public class AirRideGrDataPositionProxy
    {
        public HSD_JOBJ _joint;
        public KAR_grPositionData _data;

        public float X
        {
            get => _joint == null ? _data.X : _joint.TX;
            set
            {
                if (_joint == null)
                    _data.X = value;

                else _joint.TX = value;
            }
        }

        public float Y
        {
            get => _joint == null ? _data.Y : _joint.TY;
            set
            {
                if (_joint == null)
                {
                    _data.Y = value;
                }
                else _joint.TY = value;
            }
        }

        public float Z
        {
            get => _joint == null ? _data.Z : _joint.TZ;
            set
            {
                if (_joint == null)
                {
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
            _joint = joint;
            _joint.Next = null;
            _joint.Child = null;
        }

        public AirRideGrDataPositionProxy(KAR_grPositionData data)
        {
            _data = data;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }

    public class AirRideGrDataAreaPositionProxy
    {
        public HSD_JOBJ _joint;
        public KAR_grAreaPositionData _areadata;

        public float X
        {
            get => _joint == null ? _areadata.X : _joint.TX;
            set
            {
                if (_joint == null)
                {
                    _areadata.X = value;
                }
                else _joint.TX = value;
            }
        }

        public float Y
        {
            get => _joint == null ? _areadata.Y : _joint.TY;
            set
            {
                if (_joint == null)
                {
                    _areadata.Y = value;
                }
                else _joint.TY = value;
            }
        }

        public float Z
        {
            get => _joint == null ? _areadata.Z : _joint.TZ;
            set
            {
                if (_joint == null)
                {
                    _areadata.Z = value;
                }
                else _joint.TZ = value;
            }
        }

        public AirRideGrDataAreaPositionProxy(HSD_JOBJ joint)
        {
            _joint = joint;
            _joint.Next = null;
            _joint.Child = null;
        }

        public AirRideGrDataAreaPositionProxy(KAR_grAreaPositionData data)
        {
            _areadata = data;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
