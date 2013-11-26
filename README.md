# Restcoration

A wrapper for RestSharp to provide more domain-oriented design for fetching restful data.
NuGet package: https://www.nuget.org/packages/Restcoration/

# Usage

### Step 1: Take a POCO for the request

```c#
public class HttpBinIpRequest
{
}
```

### Step 2: Make sure you have a class you want a response for
```c#
public class HttpBinIpSuccessResponse
{
  public string Origin { get; set; }
}
```

### Step 3: Decorate the request class
```c#
[Rest(Method = Method.GET, Resource = "/ip", OK = typeof(HttpBinIpSuccessResponse))]
public class HttpBinIpRequest
{
}
```

### Step 4: Use the factory to fetch the object!
```c#
var factory = new RestClientFactory("http://httpbin.org/");
var response = factory.Get<HttpBinIpSuccessResponse, HttpBinIpRequest>(new HttpBinIpRequest());

// Alternative: Use an anonymous object:
var response = factory.Get(new HttpBinIpRequest());
Assert.That(response, Is.TypeOf<HttpBinIpSuccessResponse>());
```


#### You can specify a default class to parse to, if all different status codes have same structure:
```c#
[Rest(ResponseType = typeof(DefaultResponse))]
```

#### A data format can be specified, too:
```c#
[Rest(RequestFormat = DataFormat.xml)]
```

#### Use it together with a Blueprint POCO generator
I have another project here which is capable of parsing Blueprint API format and converting it into POCOs suitable for use with this library.

Check it out: https://github.com/Grinderofl/BluePOCO
The ApiTransformer from that project, to convert Api into usable ApiObjects, is also available on NuGet: https://www.nuget.org/packages/BlueprintApiTransformer/
