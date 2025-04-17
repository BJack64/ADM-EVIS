using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.WebPages;


using System.Web.Routing;
using System.Web.Mvc.Properties;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Data;

namespace eFakturADM.Web.Helpers
{
    public static class MSDSService
    {
        public static itemType[] GetMSDSItemsbyName(String Name, out String cid)
        {
            String URL = System.Configuration.ConfigurationManager.AppSettings["MSDSServiceURL"].ToString();
            String UserName = System.Configuration.ConfigurationManager.AppSettings["MSDSServiceUserName"].ToString();
            String Password = System.Configuration.ConfigurationManager.AppSettings["MSDSServicePassword"].ToString();

            MSDS_Generic msdsService = new MSDS_Generic();
            msdsService.Url = URL;
            System.Net.CredentialCache myCredentials = new System.Net.CredentialCache();
            System.Net.NetworkCredential netCred = new System.Net.NetworkCredential(UserName, Password);
            myCredentials.Add(new Uri(msdsService.Url), "Basic", netCred);
            msdsService.Credentials = myCredentials;
            MSDS_GenericMSDS_GenericType msdsType = new MSDS_GenericMSDS_GenericType
            {
                MATERIALNAME = Name,
                MSDSID = null
            };
            return msdsService.MSDS_GetList(msdsType, out cid);
        }

        public static itemType GetMSDSItembyID(String ID, out String cid)
        {
            String URL = System.Configuration.ConfigurationManager.AppSettings["MSDSServiceURL"].ToString();
            String UserName = System.Configuration.ConfigurationManager.AppSettings["MSDSServiceUserName"].ToString();
            String Password = System.Configuration.ConfigurationManager.AppSettings["MSDSServicePassword"].ToString();

            MSDS_Generic msdsService = new MSDS_Generic();
            msdsService.Url = URL;
            System.Net.CredentialCache myCredentials = new System.Net.CredentialCache();
            System.Net.NetworkCredential netCred = new System.Net.NetworkCredential(UserName, Password);
            myCredentials.Add(new Uri(msdsService.Url), "Basic", netCred);
            msdsService.Credentials = myCredentials;
            MSDS_GenericMSDS_GenericType msdsType = new MSDS_GenericMSDS_GenericType
            {
                MATERIALNAME = null,
                MSDSID = ID
            };
            return msdsService.MSDS_GetList(msdsType, out cid).Where(a=>a.ID.Equals(ID)).FirstOrDefault();
        }

        public static chapterType[] GetMSDSChapterbyID(String ID)
        {
            String URL = System.Configuration.ConfigurationManager.AppSettings["MSDSServiceURL"].ToString();
            String UserName = System.Configuration.ConfigurationManager.AppSettings["MSDSServiceUserName"].ToString();
            String Password = System.Configuration.ConfigurationManager.AppSettings["MSDSServicePassword"].ToString();

            MSDS_Generic msdsService = new MSDS_Generic();
            msdsService.Url = URL;
            System.Net.CredentialCache myCredentials = new System.Net.CredentialCache();
            System.Net.NetworkCredential netCred = new System.Net.NetworkCredential(UserName, Password);
            myCredentials.Add(new Uri(msdsService.Url), "Basic", netCred);
            msdsService.Credentials = myCredentials;
            
            return msdsService.GetMSDSChapter(ID);
        }
        
    }
    public static class TotalEmployeeName 
    {
        public static String GetNameByGGI(String GGI) 
        {
#if TOTAL_DEV
            if (String.IsNullOrEmpty(GGI))
                return "";
            //from PIR
            eFakturADM.Web.PIRWCFService.PIRWCFServiceClient pirservice = new PIRWCFService.PIRWCFServiceClient();
            eFakturADM.Web.PIRWCFService.EmployeeInformation pirEmployee = null;

            //from AD
            eFakturADM.Web.ADDataService.ADDataServiceClient adService = new ADDataService.ADDataServiceClient();
            eFakturADM.Web.ADDataService.User adUser = null;
            try
            {
                pirEmployee = pirservice.GetDataByGGI(GGI);
            }
            catch (Exception)
            {

            }
            try
            {
                adUser = adService.AuthenticateUser(GGI);
            }
            catch (Exception)
            {

            }
            if (pirEmployee != null)
                return pirEmployee.FirstName + " " + pirEmployee.LastName;
            else if (adUser != null)
                return adUser.Name;
            else
                return GGI;
#else
            return GGI;
#endif

        }

        public static String GetEmailByGGI(String GGI)
        {
#if TOTAL_DEV
            if (String.IsNullOrEmpty(GGI))
                return "";
            
            //from AD
            eFakturADM.Web.ADDataService.ADDataServiceClient adService = new ADDataService.ADDataServiceClient();
            eFakturADM.Web.ADDataService.User adUser = null;
            try
            {
                adUser = adService.AuthenticateUser(GGI);
            }
            catch (Exception)
            {

            }
            if (adUser != null)
                return adUser.Email;
            else
                return null;
#else
            return null;
#endif

        }
    }
    public static class Renders
    {
        public static string RenderPartialView(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }

    public static class Extensions
    {       
        public static string ToRelativeTime(this DateTime date)
        {
            int Minute = 60;
            int Hour = Minute * 60;
            int Day = Hour * 24;
            int Year = Day * 365;

            var thresholds = new Dictionary<long, Func<TimeSpan, string>>
                {
                    {2, t => "a second ago"},
                    {Minute,  t => String.Format("{0} seconds ago", (int)t.TotalSeconds)},
                    {Minute * 2,  t => "a minute ago"},
                    {Hour,  t => String.Format("{0} minutes ago", (int)t.TotalMinutes)},
                    {Hour * 2,  t => "an hour ago"},
                    {Day,  t => String.Format("{0} hours ago", (int)t.TotalHours)},
                    {Day * 2,  t => "yesterday"},
                    {Day * 30,  t => String.Format("{0} days ago", (int)t.TotalDays)},
                    {Day * 60,  t => "last month"},
                    {Year,  t => String.Format("{0} months ago", (int)t.TotalDays / 30)},
                    {Year * 2,  t => "last year"},
                    {Int64.MaxValue,  t => String.Format("{0} years ago", (int)t.TotalDays / 365)}
                };
            var difference = DateTime.UtcNow - date.ToUniversalTime();
            return thresholds.First(t => difference.TotalSeconds < t.Key).Value(difference);

        }

        public static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        public static IEnumerable IndexOfAll(this string haystack, string needle)
        {
            int pos, offset = 0;
            while ((pos = haystack.IndexOf(needle)) > 0)
            {
                haystack = haystack.Substring(pos + needle.Length);
                offset += pos;
                yield return offset;
            }
        }

        public static void ReplaceBAckground(string source, string search)
        {
            foreach (int Pos in source.IndexOfAll(search))
            {

            }

        }

    }

    /// <summary>
    /// A JsonResult with ContentType of text/html and the serialized object contained within textarea tags
    /// </summary>
    /// <remarks>
    /// It is not possible to upload files using the browser's XMLHttpRequest
    /// object. So the jQuery Form Plugin uses a hidden iframe element. For a
    /// JSON response, a ContentType of application/json will cause bad browser
    /// behavior so the content-type must be text/html. Browsers can behave badly
    /// if you return JSON with ContentType of text/html. So you must surround
    /// the JSON in textarea tags. All this is handled nicely in the browser
    /// by the jQuery Form Plugin. But we need to overide the default behavior
    /// of the JsonResult class in order to achieve the desired result.
    /// </remarks>
    public class FileUploadJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            this.ContentType = "text/html";
            context.HttpContext.Response.Write("<textarea>");
            base.ExecuteResult(context);
            context.HttpContext.Response.Write("</textarea>");
        }
    }    

    public static class TreeViewHelper
    {
        /// <summary>
        /// Create an HTML tree from a recursive collection of items
        /// </summary>
        public static TreeView<T> TreeView<T>(this HtmlHelper html, IEnumerable<T> items)
        {
            return new TreeView<T>(html, items);
        }
    }

    /// <summary>
    /// Create an HTML tree from a resursive collection of items
    /// </summary>
    public class TreeView<T> : IHtmlString
    {
        private readonly HtmlHelper _html;
        private readonly IEnumerable<T> _items = Enumerable.Empty<T>();
        private Func<T, string> _displayProperty = item => item.ToString();
        private Func<T, IEnumerable<T>> _childrenProperty;
        private string _emptyContent = "No children";
        private IDictionary<string, object> _htmlAttributes = new Dictionary<string, object>();
        private IDictionary<string, object> _childHtmlAttributes = new Dictionary<string, object>();
        private Func<T, HelperResult> _itemTemplate;

        public TreeView(HtmlHelper html, IEnumerable<T> items)
        {
            if (html == null) throw new ArgumentNullException("html");
            _html = html;
            _items = items;
            // The ItemTemplate will default to rendering the DisplayProperty
            _itemTemplate = item => new HelperResult(writer => writer.Write(_displayProperty(item)));
        }

        /// <summary>
        /// The property which will display the text rendered for each item
        /// </summary>
        public TreeView<T> ItemText(Func<T, string> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            _displayProperty = selector;
            return this;
        }


        /// <summary>
        /// The template used to render each item in the tree view
        /// </summary>
        public TreeView<T> ItemTemplate(Func<T, HelperResult> itemTemplate)
        {
            if (itemTemplate == null) throw new ArgumentNullException("itemTemplate");
            _itemTemplate = itemTemplate;
            return this;
        }


        /// <summary>
        /// The property which returns the children items
        /// </summary>
        public TreeView<T> Children(Func<T, IEnumerable<T>> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            _childrenProperty = selector;
            return this;
        }

        /// <summary>
        /// Content displayed if the list is empty
        /// </summary>
        public TreeView<T> EmptyContent(string emptyContent)
        {
            if (emptyContent == null) throw new ArgumentNullException("emptyContent");
            _emptyContent = emptyContent;
            return this;
        }

        /// <summary>
        /// HTML attributes appended to the root ul node
        /// </summary>
        public TreeView<T> HtmlAttributes(object htmlAttributes)
        {
            HtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            return this;
        }

        /// <summary>
        /// HTML attributes appended to the root ul node
        /// </summary>
        public TreeView<T> HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) throw new ArgumentNullException("htmlAttributes");
            _htmlAttributes = htmlAttributes;
            return this;
        }

        /// <summary>
        /// HTML attributes appended to the children items
        /// </summary>
        public TreeView<T> ChildrenHtmlAttributes(object htmlAttributes)
        {
            ChildrenHtmlAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            return this;
        }

        /// <summary>
        /// HTML attributes appended to the children items
        /// </summary>
        public TreeView<T> ChildrenHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) throw new ArgumentNullException("htmlAttributes");
            _childHtmlAttributes = htmlAttributes;
            return this;
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        public void Render()
        {
            var writer = _html.ViewContext.Writer;
            using (var textWriter = new HtmlTextWriter(writer))
            {
                textWriter.Write(ToString());
            }
        }

        private void ValidateSettings()
        {
            if (_childrenProperty == null)
            {
                throw new InvalidOperationException("You must call the Children() method to tell the tree view how to find child items");
            }
        }

        public override string ToString()
        {
            ValidateSettings();

            var listItems = _items.ToList();

            var ul = new TagBuilder("ul");
            ul.MergeAttributes(_htmlAttributes);

            if (listItems.Count == 0)
            {
                var li = new TagBuilder("li")
                {
                    InnerHtml = _emptyContent
                };
                ul.InnerHtml += li.ToString();
            }

            foreach (var item in listItems)
            {
                BuildNestedTag(ul, item, _childrenProperty);
            }

            return ul.ToString();
        }

        private void AppendChildren(TagBuilder parentTag, T parentItem, Func<T, IEnumerable<T>> childrenProperty)
        {
            var children = childrenProperty(parentItem).ToList();
            if (children.Count() == 0)
            {
                return;
            }

            var innerUl = new TagBuilder("ul");
            innerUl.MergeAttributes(_childHtmlAttributes);

            foreach (var item in children)
            {
                BuildNestedTag(innerUl, item, childrenProperty);
            }

            parentTag.InnerHtml += innerUl.ToString();
        }

        private void BuildNestedTag(TagBuilder parentTag, T parentItem, Func<T, IEnumerable<T>> childrenProperty)
        {
            var li = GetLi(parentItem);
            parentTag.InnerHtml += li.ToString(TagRenderMode.StartTag);
            AppendChildren(li, parentItem, childrenProperty);
            parentTag.InnerHtml += li.InnerHtml + li.ToString(TagRenderMode.EndTag);
        }

        private TagBuilder GetLi(T item)
        {
            var li = new TagBuilder("li")
            {
                InnerHtml = _itemTemplate(item).ToHtmlString()
            };

            return li;
        }
    }    

    public static class Select2InputExtensions
    {
        private const string DATA_PLACEHOLDER = "data-placeholder";
        private const string DATA_OPTION = "data-option";

        public static IHtmlString Select2For<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.Select2For(expression, format: null);
        }

        public static IHtmlString Select2For<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return htmlHelper.Select2For(expression, format, (IDictionary<string, object>)null);
        }

        public static IHtmlString Select2For<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.Select2For(expression, format: null, htmlAttributes: htmlAttributes);
        }

        public static IHtmlString Select2For<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return htmlHelper.Select2For(expression, format: format, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), multiple: false);
        }

        public static IHtmlString Select2For<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.Select2For(expression, format: null, htmlAttributes: htmlAttributes, multiple: false);
        }

        public static IHtmlString Select2MultipleFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.Select2MultipleFor(expression, format: null);
        }

        public static IHtmlString Select2MultipleFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            return htmlHelper.Select2MultipleFor(expression, format, (IDictionary<string, object>)null);
        }

        public static IHtmlString Select2MultipleFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return htmlHelper.Select2MultipleFor(expression, format: null, htmlAttributes: htmlAttributes);
        }

        public static IHtmlString Select2MultipleFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            return htmlHelper.Select2For(expression, format: format, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), multiple: true);
        }

        public static IHtmlString Select2MultipleFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.Select2For(expression, format: null, htmlAttributes: htmlAttributes, multiple: true);
        }

        public static IHtmlString Select2For<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string format, IDictionary<string, object> htmlAttributes, bool multiple)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string name = ExpressionHelper.GetExpressionText(expression);
            string value = string.Empty;
            string dataPlaceHolder = string.Empty;

            if (metadata.Model != null)
            {
                if ((metadata.ModelType == typeof(Guid)) && ((Guid)metadata.Model == Guid.Empty))
                {
                    value = string.Empty;
                }
                else
                {
                    value = metadata.Model.ToString();
                }
            }

            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Cannot find fullName.");
            }

            System.Web.Mvc.TagBuilder tagBuilder = null;

            tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
            tagBuilder.MergeAttribute("name", !multiple ? fullName : fullName + "[]", true);
            if (multiple)
            {
                tagBuilder.MergeAttribute("multiple", "", true);
            }

            if (metadata.AdditionalValues != null)
            {
                if (metadata.AdditionalValues.ContainsKey(DATA_PLACEHOLDER))
                {
                    tagBuilder.MergeAttribute(DATA_PLACEHOLDER, metadata.AdditionalValues[DATA_PLACEHOLDER] as string);
                }
                if (metadata.AdditionalValues.ContainsKey(DATA_OPTION))
                {
                    string fieldName = metadata.AdditionalValues[DATA_OPTION] as string;
                    var parentType = metadata.ContainerType;
                    var parentMetaData = ModelMetadataProviders.Current
                        .GetMetadataForProperties(htmlHelper.ViewData.Model, parentType);

                    var dataOptionValue = (string)parentMetaData.FirstOrDefault(p => p.PropertyName == fieldName).Model;

                    tagBuilder.MergeAttribute(DATA_OPTION, dataOptionValue ?? "");
                }
                //if (multiple && metadata.AdditionalValues.ContainsKey(DATA_SPLIT))
                //{
                //    tagBuilder.MergeAttribute(DATA_SPLIT, metadata.AdditionalValues[DATA_SPLIT].ToString());
                //}
            }

            string valueParameter = htmlHelper.FormatValue(value, format);
            // bool usedModelState = false;
            bool useViewData = false;
            bool isExplicitValue = true;

            string attemptedValue = string.Empty;

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Value != null)
                {
                    attemptedValue = (string)modelState.Value.ConvertTo(typeof(string), null /* culture */);
                }
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            var validationAttribs = htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata);
            tagBuilder.MergeAttributes(validationAttribs);
            // tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(string.Empty), metadata));

            tagBuilder.MergeAttribute("value", value ?? ((useViewData) ? attemptedValue : valueParameter), isExplicitValue);

            tagBuilder.GenerateId(fullName);

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }

    }    
    
    public static class Util
    {
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}