using System.Collections.Generic;

namespace ViscaLib.Data
{
    public class CameraResponseModel
    {
        public int ResponseType { get; set; }
        public List<int> Response { get; set; } = new List<int>();
    }
}
