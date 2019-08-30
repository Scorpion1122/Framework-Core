using System.Text;

namespace System
{
	public static class StringUtility
	{
		private const char formatSeperator = '_';

		public static string AllCapsFormatter(string value)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				if (i != 0 && value[i - 1] != formatSeperator)
				{
					if (char.IsWhiteSpace(value[i - 1]))
						builder.Append(formatSeperator);
					else if (char.IsUpper(value[i]) && !char.IsUpper(value[i - 1]))
						builder.Append(formatSeperator);
				}

				if (!char.IsWhiteSpace(value[i]))
					builder.Append(char.ToUpper(value[i]));
			}
			return builder.ToString();
		}
	}

}
