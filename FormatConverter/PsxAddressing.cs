using System;

namespace PSX
{
	public static class PsxAddressing
	{
		public static uint FromPsxPtrToOffset(this uint value, SotnFileType fileType)
		{
			switch (fileType)
			{
			case SotnFileType.None:
				return value - 2147483648U;
			case SotnFileType.DraBin:
				return value - 2148139008U;
			case SotnFileType.MapBin:
				return value - 2149056512U;
			default:
				throw new ArgumentException("Invalid SotnFileType for converting from Psx pointer");
			}
		}
	}
}
