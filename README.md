biz.dfch.CS.Infoblox.Wapi
=========================

Object Mapper for IPAM Infoblox REST API

Module: biz.dfch.CS.Infoblox.Wapi

d-fens GmbH, General-Guisan-Strasse 6, CH-6300 Zug, Switzerland

This .NET/C# assembly provide a very simple object mapper for the Infoblox WAPI (REST API).

You can download the assembly from [NuGet](https://www.nuget.org/profiles/ronald.rink) at [biz.dfch.CS.Infoblox.Wapi](https://www.nuget.org/packages/biz.dfch.CS.Infoblox.Wapi/). If you prefer to download the DLL directly you can also download it from the repository's [Releases](https://github.com/dfch/biz.dfch.CS.Infoblox.Wapi/releases).

This assembly is used by the [PowerShell module biz.dfch.PS.Ipam.Infoblox.Api](https://github.com/dfch/biz.dfch.PS.Ipam.Infoblox.Api) and the [Cumulus Automation Portal](https://github.com/dfch/biz.dfch.CS.Cumulus.Server).

Usage
-----

You install the package via NuGet:

	Install-Package biz.dfch.CS.Infoblox.Wapi

After adding the reference to the project (this would have been done automatically via NuGet) you can instantiate the the `RestHelper` object and submit arbitrary queries to your Infoblox server:

	var Username = "EdgarSchnittenfittich";
	var Password = "infoblox";
	var UriServer = "https://www.example.com/";

	var rest = new RestHelper();
	var nc = new System.Net.NetworkCredential(Username, Password);
	rest.Credential = nc;
	rest.UriServer = new System.Uri(UriServer);
	rest.ReturnType = RestHelper.ReturnTypes.JsonPretty;

	var q = new Hashtable();
	// optionally you can add query string parameters
	q.Add("_max_return", "2");
	var s = rest.Invoke("networkview", q);

If you are using this assembly the PowerShell you can also specify the ReturnType as a string:

	rest.ReturnTypeString = 'json-pretty';
	# or
	rest.ReturnTypeString = 'JsonPretty';

You can also very easily extract fields from the JSON response via [JSON.Net](http://json.net) and its *LINQ to JSON* feature:

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	JToken jv = JObject.Parse(contentError);
	var MessageError = jv.SelectToken("Error", true).ToString();
	var MessageCode = jv.SelectToken("code", true).ToString();
	var MessageText = jv.SelectToken("text", true).ToString();

Most of the time when something goes wrong (i.e. your query is malformed) Infoblox will just return with an `HTTP 400`. You will then have to look at the response to get further information. The assembly will throw an exception and extract the error parameters for you.

For further information have a look at the [unit tests](./biz.dfch.CS.Infoblox.Wapi.Tests/WapiTests.cs) that are part of the repository.
