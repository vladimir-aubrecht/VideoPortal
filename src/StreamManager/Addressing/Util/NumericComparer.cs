// (c) Vasian Cepa 2005
// Version 2

using System;
using System.Collections; // required for NumericComparer : IComparer only

namespace Golem2.Manager.Addressing.Util
{

	public class NumericComparer : IComparer
	{
		public NumericComparer()
		{}
		
		public int Compare(object x, object y)
		{
            int ret = StringLogicalComparer.Compare(x.ToString(), y.ToString());

            return ret;
		}
	}//EOC

}