using System;
using UnityEngine;

namespace CompilerGenerated
{
	[Serializable]
	internal sealed class _0024adaptor_0024__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35___0024WindowFunction_00240
	{
		protected __UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ _0024from;

		public _0024adaptor_0024__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35___0024WindowFunction_00240(__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ from)
		{
			_0024from = from;
		}

		public void Invoke(int capacity)
		{
			_0024from();
		}

		public static GUI.WindowFunction Adapt(__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ from)
		{
			return new _0024adaptor_0024__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35___0024WindowFunction_00240(from).Invoke;
		}
	}
}
