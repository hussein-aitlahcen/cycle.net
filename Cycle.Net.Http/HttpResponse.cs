using Cycle.Net.Core.Abstract;

namespace Cycle.Net.Http
{
    public sealed class HttpResponse : IHttpResponse
    {
        public IHttpRequest Origin { get; }
        public string Content { get; }
        public HttpResponse(IHttpRequest origin, string content)
        {
            Origin = origin;
            Content = content;
        }

        public override string ToString() =>
            $"HttpResponse(origin={Origin}, contentLength={Content.Length}";
    }
}