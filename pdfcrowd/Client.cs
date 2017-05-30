using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pdfcrowd
{
    public class Client
    {
        public static int SINGLE_PAGE = 1;

        public static int CONTINUOUS = 2;

        public static int CONTINUOUS_FACING = 3;

        public static int NONE_VISIBLE = 1;

        public static int THUMBNAILS_VISIBLE = 2;

        public static int FULLSCREEN = 3;

        public static int FIT_WIDTH = 1;

        public static int FIT_HEIGHT = 2;

        public static int FIT_PAGE = 3;

        private static string API_SELECTOR_BASE = "/api/";

        public string HOST = "pdfcrowd.com";

        public static int HTTP_PORT = 80;

        public static int HTTPS_PORT = 443;

        private StringDictionary fields = new StringDictionary();

        private string api_uri;

        private static string boundary = "----------ThIs_Is_tHe_bOUnDary_$";

        private static string multipart_content_type = "multipart/form-data; boundary=" + Client.boundary;

        private static string new_line = "\r\n";

        public Client(string username, string api_key)
        {
            this.useSSL(false);
            this.fields.Add("username", username);
            this.fields.Add("key", api_key);
            this.fields.Add("pdf_scaling_factor", "1");
            this.fields.Add("html_zoom", "200");
        }

        public Client(string username, string api_key, string hostname)
        {
            this.fields.Add("username", username);
            this.fields.Add("key", api_key);
            this.fields.Add("pdf_scaling_factor", "1");
            this.fields.Add("html_zoom", "200");
            this.HOST = hostname;
            this.useSSL(false);
        }

        public async Task convertURI(string uri, Stream out_stream)
        {
            await this.convert(out_stream, "uri", uri);
        }

        public async Task convertHtml(string content, Stream out_stream)
        {
            await this.convert(out_stream, "html", content);
        }

        public void convertFile(string fpath, Stream out_stream)
        {
            this.post_multipart(fpath, out_stream);
        }

        public async Task<int> numTokens()
        {
            string uri = string.Format("{0}user/{1}/tokens/", this.api_uri, this.fields["username"]);
            MemoryStream memoryStream = new MemoryStream();
            await this.call_api(uri, memoryStream, null);
            memoryStream.Position = 0L;
            string s = Client.read_stream(memoryStream);
            return int.Parse(s);
        }

        public void useSSL(bool use_ssl)
        {
            if (use_ssl)
            {
                this.api_uri = string.Format("https://{0}:{1}{2}", this.HOST, Client.HTTPS_PORT, Client.API_SELECTOR_BASE);
            }
            else
            {
                this.api_uri = string.Format("http://{0}:{1}{2}", this.HOST, Client.HTTP_PORT, Client.API_SELECTOR_BASE);
            }
        }

        public void setUsername(string username)
        {
            this.fields["username"] = username;
        }

        public void setApiKey(string key)
        {
            this.fields["key"] = key;
        }

        public void setPageWidth(string value)
        {
            this.fields["width"] = value;
        }

        public void setPageWidth(double value)
        {
            this.fields["width"] = value.ToString();
        }

        public void setPageHeight(string value)
        {
            this.fields["height"] = value;
        }

        public void setPageHeight(double value)
        {
            this.fields["height"] = value.ToString();
        }

        public void setHorizontalMargin(double value)
        {
            this.fields["margin_right"] = value.ToString();
            this.fields["margin_left"] = value.ToString();
        }

        public void setHorizontalMargin(string value)
        {
            this.fields["margin_right"] = value;
            this.fields["margin_left"] = value;
        }

        public void setVerticalMargin(double value)
        {
            this.fields["margin_top"] = value.ToString();
            this.fields["margin_bottom"] = value.ToString();
        }

        public void setVerticalMargin(string value)
        {
            this.fields["margin_top"] = value;
            this.fields["margin_bottom"] = value;
        }

        public void setPageMargins(string top, string right, string bottom, string left)
        {
            this.fields["margin_top"] = top;
            this.fields["margin_right"] = right;
            this.fields["margin_bottom"] = bottom;
            this.fields["margin_left"] = left;
        }

        public void setEncrypted(bool value)
        {
            this.fields["encrypted"] = (value ? "true" : null);
        }

        public void setEncrypted()
        {
            this.setEncrypted(true);
        }

        public void setUserPassword(string pwd)
        {
            this.fields["user_pwd"] = pwd;
        }

        public void setOwnerPassword(string pwd)
        {
            this.fields["owner_pwd"] = pwd;
        }

        public void setNoPrint(bool value)
        {
            this.fields["no_print"] = (value ? "true" : null);
        }

        public void setNoPrint()
        {
            this.setNoPrint(true);
        }

        public void setNoModify(bool value)
        {
            this.fields["no_modify"] = (value ? "true" : null);
        }

        public void setNoModify()
        {
            this.setNoModify(true);
        }

        public void setNoCopy(bool value)
        {
            this.fields["no_copy"] = (value ? "true" : null);
        }

        public void setNoCopy()
        {
            this.setNoCopy(true);
        }

        public void setPageLayout(int value)
        {
            this.fields["page_layout"] = value.ToString();
        }

        public void setPageMode(int value)
        {
            this.fields["page_mode"] = value.ToString();
        }

        public void setFooterText(string value)
        {
            this.fields["footer_text"] = value;
        }

        public void enableImages()
        {
            this.enableImages(true);
        }

        public void enableImages(bool value)
        {
            this.fields["no_images"] = (value ? null : "true");
        }

        public void enableBackgrounds()
        {
            this.enableBackgrounds(true);
        }

        public void enableBackgrounds(bool value)
        {
            this.fields["no_backgrounds"] = (value ? null : "true");
        }

        public void setHtmlZoom(float value)
        {
            this.fields["html_zoom"] = value.ToString();
        }

        public void enableJavaScript()
        {
            this.enableJavaScript(true);
        }

        public void enableJavaScript(bool value)
        {
            this.fields["no_javascript"] = (value ? null : "true");
        }

        public void enableHyperlinks()
        {
            this.enableHyperlinks(true);
        }

        public void enableHyperlinks(bool value)
        {
            this.fields["no_hyperlinks"] = (value ? null : "true");
        }

        public void setDefaultTextEncoding(string value)
        {
            this.fields["text_encoding"] = value;
        }

        public void usePrintMedia()
        {
            this.usePrintMedia(true);
        }

        public void usePrintMedia(bool value)
        {
            this.fields["use_print_media"] = (value ? "true" : null);
        }

        public void setMaxPages(int value)
        {
            this.fields["max_pages"] = value.ToString();
        }

        public void enablePdfcrowdLogo()
        {
            this.enablePdfcrowdLogo(true);
        }

        public void enablePdfcrowdLogo(bool value)
        {
            this.fields["pdfcrowd_logo"] = (value ? "true" : null);
        }

        public void setInitialPdfZoomType(int value)
        {
            this.fields["initial_pdf_zoom_type"] = value.ToString();
        }

        public void setInitialPdfExactZoom(float value)
        {
            this.fields["initial_pdf_zoom_type"] = "4";
            this.fields["initial_pdf_zoom"] = value.ToString();
        }

        public void setAuthor(string value)
        {
            this.fields["author"] = value;
        }

        public void setFailOnNon200(bool value)
        {
            this.fields["fail_on_non200"] = (value ? "true" : null);
        }

        public void setPdfScalingFactor(float value)
        {
            this.fields["pdf_scaling_factor"] = value.ToString();
        }

        public void setFooterHtml(string value)
        {
            this.fields["footer_html"] = value;
        }

        public void setFooterUrl(string value)
        {
            this.fields["footer_url"] = value;
        }

        public void setHeaderHtml(string value)
        {
            this.fields["header_html"] = value;
        }

        public void setHeaderUrl(string value)
        {
            this.fields["header_url"] = value;
        }

        public void setPageBackgroundColor(string value)
        {
            this.fields["page_background_color"] = value;
        }

        public void setTransparentBackground()
        {
            this.setTransparentBackground(true);
        }

        public void setTransparentBackground(bool val)
        {
            this.fields["transparent_background"] = (val ? "true" : null);
        }

        public void setPageNumberingOffset(int value)
        {
            this.fields["page_numbering_offset"] = value.ToString();
        }

        public void setHeaderFooterPageExcludeList(string value)
        {
            this.fields["header_footer_page_exclude_list"] = value;
        }

        public void setWatermark(string url, float offset_x, float offset_y)
        {
            this.fields["watermark_url"] = url;
            this.fields["watermark_offset_x"] = offset_x.ToString();
            this.fields["watermark_offset_y"] = offset_y.ToString();
        }

        public void setWatermark(string url, string offset_x, string offset_y)
        {
            this.fields["watermark_url"] = url;
            this.fields["watermark_offset_x"] = offset_x;
            this.fields["watermark_offset_y"] = offset_y;
        }

        public void setWatermarkRotation(double angle)
        {
            this.fields["watermark_rotation"] = angle.ToString();
        }

        public void setWatermarkInBackground()
        {
            this.setWatermarkInBackground(true);
        }

        public void setWatermarkInBackground(bool val)
        {
            this.fields["watermark_in_background"] = (val ? "true" : null);
        }

        private async Task convert(Stream out_stream, string method, string src)
        {
            string uri = string.Format("{0}pdf/convert/{1}/", this.api_uri, method);
            await this.call_api(uri, out_stream, src);
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] array = new byte[32768];
            while (true)
            {
                int num = input.Read(array, 0, array.Length);
                if (num <= 0)
                {
                    break;
                }
                output.Write(array, 0, num);
            }
        }

        private async Task call_api(string uri, Stream out_stream, string src)
        {
            StringDictionary stringDictionary = new StringDictionary();
            if (src != null)
            {
                stringDictionary["src"] = src;
            }
            string data = this.encode_post_data(stringDictionary);
            await do_requestAsync(uri, out_stream, data, "application/x-www-form-urlencoded");
        }

        private async Task do_requestAsync(string uri, Stream out_stream, object data, string content_type)
        {
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Method = "POST";
            ////HttpClient webRequest = new HttpClient();
            var url = new Uri(uri);
            byte[] array;
            if (data is byte[])
            {
                array = (byte[])data;
            }
            else
            {
                array = Encoding.UTF8.GetBytes((string)data);
            }

            webRequest.ContentType = content_type;
            webRequest.Headers[HttpResponseHeader.ContentLength] = array.Length.ToString();
            try
            {
                using (Stream stream = await webRequest.GetRequestStreamAsync())
                {
                    stream.Write(array, 0, array.Length);
                }
                using (WebResponse httpWebResponse = await webRequest.GetResponseAsync())
                {
                    //if (httpWebResponse.HttpStatusCode != HttpStatusCode.OK)
                    //{
                    //    throw new Error(httpWebResponse.StatusDescription, httpWebResponse.StatusCode);
                    //}
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        Client.CopyStream(stream, out_stream);
                        out_stream.Position = 0L;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                    MemoryStream memoryStream = new MemoryStream();
                    Client.CopyStream(httpWebResponse.GetResponseStream(), memoryStream);
                    memoryStream.Position = 0L;
                    string error = Client.read_stream(memoryStream);
                    throw new Error(error, httpWebResponse.StatusCode);
                }
                string str = "";
                if (ex.InnerException != null)
                {
                    str = "\n" + ex.InnerException.Message;
                }
                throw new Error(ex.Message + str, HttpStatusCode.Unused);
            }
        }

        private string encode_post_data(StringDictionary extra_data)
        {
            StringDictionary stringDictionary = new StringDictionary();
            if (extra_data != null)
            {
                IEnumerator enumerator = extra_data.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
                    if (dictionaryEntry.Value != null)
                    {
                        stringDictionary.Add(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                    }
                }
            }
            foreach (DictionaryEntry dictionaryEntry in this.fields)
            {
                ////DictionaryEntry dictionaryEntry;
                if (dictionaryEntry.Value != null)
                {
                    stringDictionary.Add(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
                }
            }
            string text = "";
            int num = 0;
            foreach (DictionaryEntry dictionaryEntry in stringDictionary)
            {
                ////DictionaryEntry dictionaryEntry;
                text = text + WebUtility.UrlEncode(dictionaryEntry.Key.ToString()) + "=" + WebUtility.UrlEncode(dictionaryEntry.Value.ToString());
                if (num < stringDictionary.Count)
                {
                    text += "&";
                }
            }
            return text.Substring(0, checked(text.Length - 1));
        }

        private static string get_mime_type(string fileName)
        {
            string result = "application/octet-stream";
            string name = Path.GetExtension(fileName).ToLower();
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(name);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                result = registryKey.GetValue("Content Type").ToString();
            }
            return result;
        }

        private byte[] encode_multipart_post_data(string filename)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            string text = "";
            foreach (DictionaryEntry dictionaryEntry in this.fields)
            {
                if (dictionaryEntry.Value != null)
                {
                    text = text + "--" + Client.boundary + Client.new_line;
                    text = text + string.Format("Content-Disposition: form-data; name=\"{0}\"", dictionaryEntry.Key) + Client.new_line;
                    text += Client.new_line;
                    text = text + dictionaryEntry.Value.ToString() + Client.new_line;
                }
            }
            text = text + "--" + Client.boundary + Client.new_line;
            text = text + string.Format("Content-Disposition: form-data; name=\"src\"; filename=\"{0}\"", filename) + Client.new_line;
            text = text + "Content-Type: " + Client.get_mime_type(filename) + Client.new_line;
            text += Client.new_line;
            binaryWriter.Write(uTF8Encoding.GetBytes(text));
            using (FileStream fileStream = File.OpenRead(filename))
            {
                byte[] array = new byte[8192];
                int count;
                while ((count = fileStream.Read(array, 0, array.Length)) > 0)
                {
                    binaryWriter.Write(array, 0, count);
                }
            }
            text = string.Concat(new string[]
            {
                Client.new_line,
                "--",
                Client.boundary,
                "--",
                Client.new_line
            });
            text += Client.new_line;
            binaryWriter.Write(uTF8Encoding.GetBytes(text));
            binaryWriter.Flush();
            return memoryStream.ToArray();
        }

        private static string read_stream(Stream stream)
        {
            string result;
            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }

        private void post_multipart(string fpath, Stream out_stream)
        {
            byte[] data = this.encode_multipart_post_data(fpath);
            //// Client.do_requestAsync(this.api_uri + "pdf/convert/html/", out_stream, data, Client.multipart_content_type);
        }
    }
}