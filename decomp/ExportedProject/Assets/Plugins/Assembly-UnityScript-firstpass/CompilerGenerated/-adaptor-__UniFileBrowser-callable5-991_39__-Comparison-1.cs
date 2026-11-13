using System;

namespace CompilerGenerated
{
	[Serializable]
	internal sealed class _0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241
	{
		protected __UniFileBrowser_0024callable5_0024991_39__ _0024from;

		public _0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241(__UniFileBrowser_0024callable5_0024991_39__ from)
		{
			_0024from = from;
		}

		public int Invoke(UniFileBrowser.FileData x, UniFileBrowser.FileData y)
		{
			return _0024from(x, y);
		}

		public static Comparison<UniFileBrowser.FileData> Adapt(__UniFileBrowser_0024callable5_0024991_39__ from)
		{
			return new _0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241(from).Invoke;
		}
	}
}
