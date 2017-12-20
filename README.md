# Cisco Spark API Client

## Available features for now

* Basic Cisco Spark APIs(List/Get/Create Message, Space, etc.).
* Encrypt Cisco Spark token in storage.
* Pagination for list APIs.
* Retry-after value, Retry executor.
* Webhook secret validator, Webhook notification manager.
* Simple Webhook Listener/Server

### Basic features

| Spark Resource | Available Feature | Description |
| :-------------- | :---------------------------- | :---------------------------------------------- |
| Person/People   | List/Get                      | Get Me is also available                        |
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
Also, `RetryExecutor` is available.
More details are described later.

### Gets trackingId

The trackingId may be used on technical support of Cisco Spark API side.

`result.TrackingId` is available in the Cisco Spark API Client.  
More details are described later.

### Validates webhook secret

`Webhook.CreateEventValidator()` is available in the Cisco Spark API Client.  
Also, `WebhookNotificationManager` is available.
More details are described later.

### Webhook listner

Webhook listner feature provides simple Webhook server feature.  
**NOTE: This feature is intended to be used for quick testing purpose.  
In production environment, more reliable server solution should be used.**

`WebhookListener` is available in the Cisco Spark API Client.  
More details are described later.




## Basic Usage

### Save encrypted token to storage

``` csharp
char[] tokens = GetBotTokenFromUser();

var protectedToken = ProtectedString.FromChars(tokens);
ProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

**NOTE: ProtectedString does not provide in-memory protection.  
This is intended to be used to save and load encrypted token.**


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
    var result = await spark.CreateMessageAsync("xyz_space_id", "With Attachment", data);

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

### Validates webhook event data

``` csharp
var webhook = await spark.GetWebhook("xyz_webhook_id");

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
var webhook = await spark.GetWebhook("xyz_webhook_id");

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

### Webhook listener with ngrok

If you do not have global ip address.
You may be able to use tunneling services such as [ngrok](https://ngrok.com/).

* Get ngrok and start it.

You can downlod ngrok command line tool from [here](https://ngrok.com/).

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
var endpointUri = listener.AddListnerEndpoint("localhost", 8080, false);
```

* Create webhook for the listner.

In this case, a tunneling service is uses with ngrok,  
The endpointUri returned by listener.AddListnerEndpoint() is uri to be forwarded.  

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

notificationManager.AddNotification(
  webhook,
  async (eventData) =>
  {
    Console.WriteLine("Event is notified, id = {0}", eventData.Id);
  }
);
```

* Start listner.

After starting listener, events will be notified to registered notification function.

``` csharp
listener.Start();
```



## Planned Features

| Feature | Description |
| :------ | :---------- |
| Markdown builder | Simple markdown builder to build Cisco Spark API specific markdown. |
| Gets error code and description | To get error code and description from Cisco Spark Json body on an error. |
| Admin APIs | Admin and Event APIs. |
