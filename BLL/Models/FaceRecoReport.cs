using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class FaceRecoReport
    {
        public string filePath { get; set; }        
        public Rectangle[] visages { get; set; }

    }
}
