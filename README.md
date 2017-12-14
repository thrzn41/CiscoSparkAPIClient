# Cisco Spark API Client

## Available features for now

### Basic features

| Spark Resource | Available Feature | Description |
| :-------------- | :---------------------------- | :---------------------------------------------- |
| Person/People   | List/Get/Delete               | Get Me is also available                        |
| Space/Room      | List/Create/Get/Update/Delete | -                                               |
| SpaceMembership | List/Create/Get/Update/Delete | -                                               |
| Message         | List/Create/Get/Delete        | Attach file from local stream is also available |
| Team            | List/Create/Get/Update/Delete | -                                               |
| TeamMembership  | List/Create/Get/Update/Delete | -                                               |
| Webhook         | List/Create/Get/Update/Delete | -                                               |
| File            | GetInfo/GetData/Upload        | -                                               |

For now, Admin or Event APIs are not available in the Cisco Spark API Client.  
It will be available in the future.

### Token encryption in storage
`ProtectedString` provides token encryption/decryption to/from storage.  
More details are described later.

### Pagination

Cisco Spark API pagination is described on [here](https://developer.ciscospark.com/pagination.html).

`result.HasNext` and `result.ListNextAsync()` are available in the Cisco Spark API Client.  
More details are described later.

### Gets HttpStatus code

`result.HttpStatusCode` is available in the Cisco Spark API Client.  
More details are described later.

I am thinking of more programmers-friendly status code.  
But, currently the client returns Http Status code.

### Gets retry-after

`result.HasRetryAfter` and `result.RetryAfter` are available in the Cisco Spark API Client.  
More details are described later.

### Gets trackingId

The trackingId may be used on technical support of Cisco Spark API side.

`result.TrackingId` is available in the Cisco Spark API Client.  
More details are described later.



## Basic Usage

### Save encrypted token to storage

``` csharp
char[] tokens = GetTokensFromUser();

var protectedToken = ProtectedString.FromChars(tokens);
ProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

**NOTE: ProtectedString does not provide in-memory protection.  
This is intended to use to save and load encrypted token.**


### Load encrypted token from storage

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = ProtectedString.FromEncryptedData(encryptedData, entropy);
```

### Create a Cisco Spark API Client instance

A Cisco Spark API Client instance should be re-used as long as possible.

``` csharp
SparkAPIClient spark = SparkAPI.CreateVersion1Client(protectedToken);
```

### Post a message to a Cisco Spark Space

``` csharp
var result = await spark.CreateMessageAsync("xyz_space_id", "Hello, Spark!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message was posted: id = {0}", result.Data.Id);
}
```

### Post a message with attachment to a Cisco Spark Space

``` csharp
using (var fs   = new FileStream("path/myfile.png", FileMode.Open, FileAccess.Read, FileShare.Read))
using (var data = new SparkFileData(fs, "imagefile.png", SparkMediaType.ImagePNG))
{
    var result = spark.CreateMessageAsync("xyz_space_id", "With Attachment", data);

    if(result.IsSuccessStatus)
    {
       Console.WriteLine("Message was posted with attachment: id = {0}", result.Data.Id);
    }
}
```

### Post a message to a Cisco Spark 1:1 Space

``` csharp
var result = await spark.CreateDirectMessageAsync("targetuser@example.com", "Hello, Spark!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message was posted: id = {0}", result.Data.Id);
}
```

### List spaces

``` csharp
var result = await spark.ListSpaces();

if(result.IsSuccessStatus && result.Data.HasItems)
{
  foreach (var item in result.Data.Items) {
    Console.WriteLine("Space: title = {0}", item.Title);
  }  
}
```

### Pagenation

``` csharp
var result = await spark.ListSpaces();

if(result.IsSuccessStatus)
{
  //
  // Do something...
  //

  if(result.HasNext)
  {
    // List next result.
    result = await result.ListNextAsync();

    if(result.IsSuccessStatus)
    {
      // ...
    }
  }
}
```

### Gets Http status code of the request

``` csharp
var result = await spark.ListSpaces();

Console.WriteLine("Status is {0}", result.HttpStatusCode);
```

### Gets retry after value.

``` csharp
var result = await spark.ListSpaces();

if(result.IsSuccessStatus)
{
  //
  // Do something...
  //
}
else if(result.HasRetryAfter)
{
  Console.WriteLine("Let's retry at {0}", result.RetryAfter.Date);  
}
```

### Gets TrackingId.

``` csharp
var result = await spark.ListSpaces();

Console.WriteLine("Tracking id is {0}", result.TrackingId);  
```



## Planned Features

| Feature | Description |
| :------ | :---------- |
| Webhook secret validation | To validate webhook secret. |
| Webhook manager | To manage webhooks. |
| Markdown builder | Simple markdown builder to build Cisco Spark API specific markdown. |
| Gets error code and description | To get error code and description from Cisco Spark Json body on an error. |
| Admin APIs | Admin and Event APIs. |
| Simple Webhook server | A simple webhook server that can be used on test or demo.|
