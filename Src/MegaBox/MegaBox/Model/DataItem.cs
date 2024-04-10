﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CG.Web.MegaApiClient; // MEGA Api !

namespace MegaBox.Model
{
    public class DataItem
    {
        public int Id { get; set; }
        public string Category { get; set; }

        public string Headline { get; set; }

        public string Subhead { get; set; }

        public string DateLine { get; set; }

        public string Image { get; set; }

        public long FileSize { get; set; }

        public bool IsLast { get; set; } = false;

        public DateTime? DateModified { get; set; }
    }

    
}
