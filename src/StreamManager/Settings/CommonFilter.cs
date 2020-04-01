using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Golem2.Manager.Settings
{
    public class CommonFilter
    {
        Regex[] regExps;
        bool negate = false;

        public CommonFilter(params String[] regExps)
        {
            this.regExps = (from q in regExps select new Regex(q)).ToArray();
        }

        public CommonFilter(bool negate, params String[] regExps) : this(regExps)
        {
            this.negate = negate;
        }

        public bool Test(String path)
        {
            bool valid = true;

            foreach (var regExp in regExps)
            {
                valid &= !regExp.IsMatch(path);
            }

            return (negate)?!valid:valid;
        }
        
    }
}
