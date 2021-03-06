using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace MyToolkit.Networking
{
	public class HttpResponse
	{
		public HttpResponse(Exception exception)
		{
			this.exception = exception; 
		}

		public HttpResponse(IHttpRequest request)
		{
			Request = request;
			Cookies = new List<Cookie>();
		}

		public IHttpRequest Request { get; internal set; }
		internal HttpWebRequest WebRequest { get; set; }

		public bool IsConnected { get; internal set; }

		public bool IsPending
		{
			get { return !HasException && !Canceled && !Successful; } 
		}

		public void Abort()
		{
			if (Request != null && !Successful)
				WebRequest.Abort();
		}

		private Exception exception;
		public Exception Exception
		{
			get { return exception; }
			set
			{
				if (exception != null && exception is TimeoutException)
					return; // already set

				if (value is WebException && ((WebException)value).Status == WebExceptionStatus.RequestCanceled)
				{
					exception = null;
					Canceled = true;
				}
				else
				{
					exception = value;
					Canceled = false;
				}
			}
		}

		/// <summary>
		/// Can be used for polling (not recommended)
		/// </summary>
		public bool Processed { get { return Canceled || Successful || HasException; } }

		public bool Canceled { get; private set; }
		public bool Successful { get { return !Canceled && Exception == null && (Response != null || ResponseStream != null); } }
		public bool HasException { get { return Exception != null; } }

		/// <summary>
		/// If Response is null and Exception is null as well the request has been canceled
		/// </summary>
		public string Response { get { return RawResponse == null ? null : Request.Encoding.GetString(RawResponse, 0, RawResponse.Length); } }
		public byte[] RawResponse { get; internal set; }
		public Stream ResponseStream { get; internal set; }
		
		public List<Cookie> Cookies { get; private set; }

#if METRO
        internal void CreateTimeoutTimer()
        {
			if (Request.ConnectionTimeout > 0)
	            throw new NotImplementedException(); 
        }
#else
		private Timer timer; 
		internal void CreateTimeoutTimer()
		{
			if (Request.ConnectionTimeout > 0)
			{
				timer = new Timer(s =>
				{
					timer.Dispose();
					if (IsPending && !IsConnected)
					{
						Exception = new TimeoutException("The connection timed out.");
						Abort();
					}
				}, null, Request.ConnectionTimeout * 1000, Timeout.Infinite);
			}
		}
#endif
	}
}