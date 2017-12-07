# Cisco Spark API client
---

For now this is experimental version and has only create a message feature.  
More features will be implemented shortly.


## Usage

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


### Post a message to a Cisco Spark Space.

``` csharp

var spark = SparkAPI.CreateVersion1Client(protectedToken);

var result = await spark.CreateMessageAsync("xyz_space_id", "Hello, Spark!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message is posted: id = {0}", result.Data.Id);
}

```


### Post a message to a Cisco Spark 1:1 Space.

``` csharp

var result = await spark.CreateMessageAsync("targetuser@example.com", "Hello, Spark!", targetType : MessageTargetType.PersonEmail);

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message is posted: id = {0}", result.Data.Id);
}

```
