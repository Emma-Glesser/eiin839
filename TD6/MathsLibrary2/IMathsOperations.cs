using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MathsLibraryRest
{
    [ServiceContract]
    public interface IMathsOperations
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Add?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int Add(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Sub?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int Substract(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Mul?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int Multiply(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Div?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        double Divide(double a, double b);



        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "AddXML?a={a}&b={b}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int AddXML(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "SubXML?a={a}&b={b}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int SubstractXML(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "MulXML?a={a}&b={b}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]
        int MultiplyXML(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "DivXML?a={a}&b={b}", ResponseFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Wrapped)]
        double DivideXML(double a, double b);



        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "AddBody?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int AddBody(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "SubBody?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int SubstractBody(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "MulBody?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int MultiplyBody(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "DivBody?a={a}&b={b}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        double DivideBody(double a, double b);



        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AddPost", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        int AddPost(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SubPost", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        int SubstractPost(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "MulPost", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        int MultiplyPost(int a, int b);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "DivPost", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json)]
        double DividePost(double a, double b);

    }
}
