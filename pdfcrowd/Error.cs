using System;
using System.Net;

namespace pdfcrowd
{
	public class Error : Exception
	{
		private string error = "";

		private HttpStatusCode http_code = HttpStatusCode.Unused;

		public Error(string _error, HttpStatusCode _http_code)
		{
			this.error = _error;
			this.http_code = _http_code;
		}

		public override string ToString()
		{
			string result;
			if (this.http_code != HttpStatusCode.Unused)
			{
				result = string.Format("{0} - {1}", (int)this.http_code, this.error);
			}
			else
			{
				result = this.error;
			}
			return result;
		}
	}
}
