# Cisco Spark API client
---

現在、実験中のバージョンで、メッセージ作成の単純な機能のみ実装されています。  
多くの機能も間もなく実装予定です。

## 使い方

### Tokenを暗号化して保存する

``` csharp
char[] tokens = GetTokensFromUser();

var protectedToken = ProtectedString.FromChars(tokens);
ProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

**NOTE: ProtectedStringは、メモリ内での暗号化を提供していません。  
暗号化したTokenを保存したり、読み込んだりすることを意図しています。**


### 暗号化したTokenを読み込む

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = ProtectedString.FromEncryptedData(encryptedData, entropy);
```


### Cisco Sparkのスペースにメッセージを投稿する

``` csharp

var spark = SparkAPI.CreateVersion1Client(protectedToken);

var result = await spark.CreateMessageAsync("xyz_space_id", "Hello, Spark!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message is posted: id = {0}", result.Data.Id);
}

```


### Cisco Sparkの1:1スペースにメッセージを投稿する

``` csharp

var result = await spark.CreateMessageAsync("targetuser@example.com", "Hello, Spark!", targetType : MessageTargetType.PersonEmail);

if(result.IsSuccessStatus)
{
   Console.WriteLine("Message is posted: id = {0}", result.Data.Id);
}

```
