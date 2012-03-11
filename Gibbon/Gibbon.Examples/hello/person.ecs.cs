using System;
using System.Text;
using Gibbon.Http;
using System.Collections.Generic;

/*
Debug|x86
Release|x86
*/
namespace Gorilla.Rendered.Ecs //
{
	public class hello_person
	{
		public static string Render(Dictionary<string, object> @params)
		{
			 var ___output = new StringBuilder();
			___output.Append("hello, ");
			___output.Append(( @params["name"] ).ToString());//tag[7, 29]->[360, 379]
			___output.Append("!\r\n");
			 return ___output.ToString();
		}
	}
}