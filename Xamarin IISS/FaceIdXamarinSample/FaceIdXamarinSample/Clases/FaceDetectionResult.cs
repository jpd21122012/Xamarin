using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceIdXamarinSample.Clases
{
    class FaceDetectionResult
    {
        public double Age { get; set; }
        public string Gender { get; set; }
        public double Smile { get; set; }
        public string Glasses { get; set; }
        public double Beard { get; set; }
        public double Moustache { get; set; }

        public string FaceId { get; set; }
    }
}
