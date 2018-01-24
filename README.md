# Cisco Spark API Client for .NET
---
[![nuget](https://img.shields.io/nuget/v/Thrzn41.CiscoSpark.svg?style=plastic)](https://www.nuget.org/packages/Thrzn41.CiscoSpark)

`Cisco Spark API Client` is a Library that wraps `Cisco Spark REST API`.   
Also, some useful features for developers are provided.

## Available Platforms
---
* .NET Standard 1.3 or later
* .NET Core 1.0 or later
* .NET Framework 4.5.2 or later

## Available Features
---
* Basic Cisco Spark APIs(List/Get/Create Message, Space, etc.).
* Cisco Spark Admin APIs(List/Get Event, License, etc.).
* Encrypt/Decrypt Cisco Spark token in storage.
* Pagination for list APIs.
* Retry-after value, Retry executor.
* Error code, error description.
* Webhook secret validator, Webhook notification manager, Webhook event handler.
* Simple Webhook Listener/Server(.NET Standard 2.0+, .NET Core 2.0+, .NET Framework 4.5.2+)

### Basic Features

| Spark Resource              | Available Feature             | Description                                     |
| :-------------------------- | :---------------------------- | :---------------------------------------------- |
| Person/People               | List/Get                      | Get Me is also available                        |
| Space(Room)                 | List/Create/Get/Update/Delete | -                                               |
| SpaceMembership(Membership) | List/Create/Get/Update/Delete | -                                               |
| Message                     | List/Create/Get/Delete        | Attach file from local stream is also available |
| Team                        | List/Create/Get/Update/Delete | -                                               |
| TeamMembership              | List/Create/Get/Update/Delete | -                                               |
| Webhook                     | List/Create/Get/Update/Delete | -                                               |
| File                        | GetInfo/GetData/Upload        | -                                               |

### Admin Features

| Spark Resource | Available Feature | Description |
| :-------------- | :---------------------------- | :---------------------------------------------- |
| Person/People   | Create/Update/Delete          | -                                               |
| Event           | List/Get                      | -                                               |
| Organization    | List/Get                      | -                                               |
| License         | List/Get                      | -                                               |
| Role            | List/Get                      | -                                               |

### Token encryption/decryption in storage
`ProtectedString` provides token encryption/decryption.  
More details are described later.

### Pagination

Cisco Spark API pagination is described on [here](https://developer.ciscospark.com/pagination.html).

`result.HasNext` and `result.ListNextAsync()` are available in the Cisco Spark API Client.  
More details are described later.

### Gets retry-after

`result.HasRetryAfter` and `result.RetryAfter` are available in the Cisco Spark API Client.  
Also, `RetryExecutor` is available.  
More details are described later.

### Gets HttpStatus code

`result.HttpStatusCode` is available in the Cisco Spark API Client.  
More details are described later.

### Gets Error code/description

There are cases when Cisco Spark API returns error with error code and description.  
`result.Data.HasErrors` and `result.Data.GetErrors()` are available in the Cisco Spark API Client.

### Gets Partial Errors

There are cases when Cisco Spark API returns partial errors.  
This is described on [here](https://developer.ciscospark.com/errors.html).    
`Item.HasErrors` and `Item.GetPartialErrors()` are available in the Cisco Spark API Client.

### Gets trackingId

The trackingId may be used on technical support of Cisco Spark API side.

`result.TrackingId` is available in the Cisco Spark API Client.  
More details are described later.

### Validates webhook secret

`Webhook.CreateEventValidator()` is available in the Cisco Spark API Client.  
Also, `WebhookNotificationManager` is available to facilicate event handling.  
More details are described later.

CreateWebhookAsync() method in the Cisco Spark API Client generates `webhook secret` dynamically by default option.

### Webhook Listener(.NET Standard 2.0+, .NET Core 2.0+, .NET Framework 4.5.2+)

Webhook listener feature provides simple Webhook server feature.  
> **NOTE: This feature is intended to be used for quick testing purpose.  
> In production environment, more reliable server solution should be used.**

`WebhookListener` is available in the Cisco Spark API Client.  
More details are described later.

## Basic Usage
---

### Install `Cisco Spark API Client`

You can install `Cisco Spark API Client` from `NuGet` package manager by any of the following methods.

* NuGet Package Manager GUI  
Search "`Thrzn41.CiscoSpark`" package and install.

* NuGet Package Manager CLI  
```
PM> Install-Package Thrzn41.CiscoSpark
```

* .NET Client  
```
> dotnet add package Thrzn41.CiscoSpark
```

### using Directive to import `Cisco Spark API Client` related types

If you want to use using directive, the following namespaces could be used.

``` csharp
using Thrzn41.Util
using Thrzn41.CiscoSpark
using Thrzn41.CiscoSpark.Version1
```

You can also use `Thrzn41.CiscoSpark.Version1.Admin` namespace, if needed.

### Create a Cisco Spark API Client instance

A Cisco Spark API Client instance should be re-used as long as possible.

``` csharp
/// Basic APIs.
SparkAPIClient spark = SparkAPI.CreateVersion1Client(token);
```

If you use Admin APIs, an instance for Admin APIs is required.  
`SparkAdminAPIClient` has all the features of `SparkAPIClient` and Admin Features.
``` csharp
/// Admin APIs.
SparkAdminAPIClient spark = SparkAPI.CreateVersion1AdminClient(token);
```

> **NOTE: The 'token' is very sensitive information for Cisco Spark API.  
> You MUST protect the 'token' carefully.  
> NEVER put it in source code directly or NEVER save it in unsecure manner.  
> `Cisco Spark API Client` provides some token encryption/decryption methods.  
> If you use your own token encryption/decryption/protection methods, you can create an instance with the decrypted token string.**

### Save encrypted token to storage

``` csharp
char[] tokens = GetBotTokenFromBotOwner();

var protectedToken = LocalProtectedString.FromChars(tokens);
LocalProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

> **NOTE: `LocalProtectedString` does not provide in-memory protection.  
> This is intended to be used to save and load encrypted token.**

### Load encrypted token from storage and create a Cisco Spark API Client

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = LocalProtectedString.FromEncryptedData(encryptedData, entropy);

/// Basic APIs.
SparkAPIClient spark = SparkAPI.CreateVersion1Client(protectedToken);
```

> **NOTE: The encrypted data can be decrypted only on the same local user or local machine as encrypted based on option parameter.**

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
    var result = await spark.CreateMessageAsync("xyz_space_id", "Hello Spark with Attachment", data);

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
var result = await spark.ListSpacesAsync();

if(result.IsSuccessStatus && result.Data.HasItems)
{
  foreach (var item in result.Data.Items) {
    Console.WriteLine("Space: title = {0}", item.Title);
  }  
}
```

### Get File info or File data

Get File info without downloading the file.
``` csharp
var result = await spark.GetFileInfoAsync(new Uri("https://api.example.com/path/to/file.png"));

if(result.IsSuccessStatus)
{
  var file = result.Data;

  Console.WriteLine("File: Name = {0}, Size = {1}, Type = {2}", file.Name, file.Size?.Value, file.MediaType?.Name);
}
```

Download the file.
``` csharp
var result = await spark.GetFileDataAsync(new Uri("https://api.example.com/path/to/file.png"));

if(result.IsSuccessStatus)
{
  var file = result.Data;

  if(result.IsSuccessStatus)
  {
    var file = result.Data;

    Console.WriteLine("File: Name = {0}, Size = {1}, Type = {2}", file.Name, file.Size?.Value, file.MediaType?.Name);

    using(var stream = file.Stream)
    {
      // File data will be contained in the stream...
    }
  }
}
```

### Pagenation

``` csharp
var result = await spark.ListSpacesAsync();

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
var result = await spark.ListSpacesAsync();

Console.WriteLine("Status is {0}", result.HttpStatusCode);
```

### Gets retry after value

``` csharp
var result = await spark.ListSpacesAsync();

if(result.IsSuccessStatus)
{
  //
  // Do something...
  //
}
else if(result.HasRetryAfter)
{
  Console.WriteLine("Let's retry: {0}", result.RetryAfter.Delta);  
}
```

### Gets TrackingId.

``` csharp
var result = await spark.ListSpacesAsync();

Console.WriteLine("Tracking id is {0}", result.TrackingId);  
```

### Validates webhook event data

``` csharp
var webhook = await spark.GetWebhookAsync("xyz_webhook_id");

var validator = webhook.CreateEventValidator();
```

When an event is notified on webhook uri,  
x-Spark-Signature will have hash code.

The validator can be used to validate the data.

``` csharp
byte[] webhookEventData = GetWebhookEventData();

if( validator.Validate(webhookEventData, "xyz_x_spark_signature_value") )
{
  Console.WriteLine("Notified data is valid!");
}
```

### Webhook Notification manager

Webhook notification manager manages webhooks and event notification.

* Create instance.

``` csharp
var notificationManager = new WebhookNotificationManager();
```

* Then, add webhook to manager with notification function.

``` csharp
var webhook = await spark.GetWebhookAsync("xyz_webhook_id");

notificationManager.AddNotification(
  webhook,
  (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);
  }
);
```

* On receiving webhook event.

``` csharp
byte[] webhookEventData = GetWebhookEventData();

// Signature will be checked and notified to function which is added earlier.
notificationManager.ValidateAndNotify(webhookEventData, "xyz_x_spark_signature_value", encodingOfData);
```

### Webhook Listener

* Create webhook listener instance.

``` csharp
var listener = new WebhookListener();
```

* Add listening host and port.

``` csharp
var endpointUri = listener.AddListenerEndpoint("yourwebhookserver.example.com", 8443);
```

* Create webhook for the listener.

The `endpointUri` returned by `listener.AddListenerEndpoint()` is Uri of the webhook.  

``` csharp
var result = await spark.CreateWebhookAsync(
  "my webhook for test",
  endpointUri,
  EventResource.Message,
  EventType.Created);
```

* Add webhook to listener with notification function.

``` csharp
var webhook = result.Data;

listener.AddNotification(
  webhook,
  async (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);

    if(eventData.Resouce == EventResouce.Message)
    {
      Console.WriteLine("Message, id = {0}", eventData.MessageData.Id);
    }
  }
);
```

* Start listener.

After starting listener, events will be notified to registered notification function.

``` csharp
listener.Start();
```

### Webhook Listener with ngrok

If you do not have global ip address.  
You may be able to use tunneling services such as [ngrok](https://ngrok.com/).

* Get ngrok and start it.

You can download ngrok command line tool from [here](https://ngrok.com/).

The following command will start tunneling that will forward to port 8080 of localhost.
```
prompt> ngrok http 8080 --bind-tls=true
```

* Create webhook listener instance.

``` csharp
var listener = new WebhookListener();
```

* Add listening host and port.

``` csharp
var endpointUri = listener.AddListenerEndpoint("localhost", 8080, false);
```

* Create webhook for the listener.

In this case, a tunneling service is used with ngrok,  
The `endpointUri` returned by `listener.AddListenerEndpoint()` is Uri to be forwarded.  

You should create webhook with ngrok uri.

If ngrok assigns `https://ngrok-xyz.example.com`,  
You need to create webhook with `String.Format("https://ngrok-xyz.example.com{0}", endpointUri.AbsolutePath)`.

``` csharp
var result = await spark.CreateWebhookAsync(
  "my webhook for test",
  new Uri(String.Format("https://ngrok-xyz.example.com{0}", endpointUri.AbsolutePath)),
  EventResource.Message,
  EventType.Created);
```

* Add webhook to listener with notification function.

``` csharp
var webhook = result.Data;

listener.AddNotification(
  webhook,
  async (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);

    if(eventData.Resouce == EventResouce.Message)
    {
      Console.WriteLine("Message, id = {0}", eventData.MessageData.Id);
    }
  }
);
```

* Start listener.

After starting listener, events will be notified to registered notification function.

``` csharp
listener.Start();
```

## Planned Features
---

| Feature | Description |
| :------ | :---------- |
| OAuth2 Helper | OAuth2 Helper to get integration token. |
| Markdown builder | Simple markdown builder to build Cisco Spark API specific markdown. |
