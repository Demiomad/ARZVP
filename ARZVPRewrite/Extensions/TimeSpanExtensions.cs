using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Extensions
{
    public static class TimeSpanExtensions
    {
        public static Timecode ToTimecode(this TimeSpan ts)
        {
            return Timecode.FromSeconds(ts.TotalSeconds);
        }
    }
}
