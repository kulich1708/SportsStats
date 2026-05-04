using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Tools
{
	public static class PhotoHelper
	{
		public static string GetMimeTypeFromExtension(string filePath)
		{
			string extension = Path.GetExtension(filePath).ToLower();
			return extension switch
			{
				".png" => "image/png",
				".jpg" or ".jpeg" => "image/jpeg",
				".gif" => "image/gif",
				_ => "application/octet-stream",
			};
		}
	}
}
