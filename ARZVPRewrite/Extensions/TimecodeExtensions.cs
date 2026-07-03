using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Extensions
{
    public static class TimecodeExtensions
    {
        public static TimeSpan ToTimeSpan(this Timecode tc)
        {
            return TimeSpan.FromMilliseconds(tc.ToMilliseconds());
        }
    }
}
