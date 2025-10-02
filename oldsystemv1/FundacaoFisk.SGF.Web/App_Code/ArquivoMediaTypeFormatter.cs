using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

namespace ComponentesApiController.App_Start
{
    public class ArquivoMediaTypeFormatter : MediaTypeFormatter
    {
        public ArquivoMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/jpeg"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/gif"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/bmp"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/tif"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/tiff"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/pdf"));
        }

        public override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return false;
        }

        public override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type == typeof(byte[]);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent contentHeaders, TransportContext transportContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("readStream");
            }

            return Task.Factory.StartNew(() =>
            {
                byte[] data = value as byte[];
                stream.Write(data, 0, data.Length);
            });
        }
    }
}