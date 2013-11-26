# Restcoration

A wrapper for RestSharp to provide more domain-oriented design for fetching restful data.


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
