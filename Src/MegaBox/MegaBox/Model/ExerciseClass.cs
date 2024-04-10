using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaBox.Model
{
    public class ExerciseClass
    {
        public DateTime ClassTime { get; set; }
        public string ClassName { get; set; }
        public string Instructor { get; set; }

        public int FileSize { get; set; }

        public bool IsLast { get; set; } = false;
    }
}
