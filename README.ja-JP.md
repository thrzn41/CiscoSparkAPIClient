# Cisco Spark API Client

![nuget](https://img.shields.io/nuget/vpre/Thrzn41.CiscoSpark.svg)

## 現在利用可能な機能

* Cisco Sparkの基本的なAPI(List/Get/Create Message, Spaceなど)
* Cisco SparkのAdmin API(List/Get Eventなど)
* ストレージに保存するTokenの暗号化。
* List API用のPagination機能。
* Retry-after値の処理とRetry executor。
* Webhook secretの検証とWebhook notification manager。
* 簡易Webhookサーバ機能。

### 基本機能

| Sparkのリソース名 | 利用可能な機能                | 説明 |
| :-------------- | :---------------------------- | :---------------------------------- |
| Person/People   | List/Get                      | Get Meも利用可能                     |
| Space/Room      | List/Create/Get/Update/Delete | -                                   |
| SpaceMembership | List/Create/Get/Update/Delete | -                                   |
| Message         | List/Create/Get/Delete        | ローカルのstreamからファイル添付も可能 |
| Team            | List/Create/Get/Update/Delete | -                                   |
| TeamMembership  | List/Create/Get/Update/Delete | -                                   |
| Webhook         | List/Create/Get/Update/Delete | -                                   |
| File            | GetInfo/GetData/Upload        | -                                   |

### Admin機能
| Sparkのリソース名 | 利用可能な機能                | 説明 |
| :-------------- | :---------------------------- | :---------------------------------------------- |
| Person/People   | Create/Update/Delete          | -                                               |
| Event           | List/Get                      | -                                               |
| Organization    | List/Get                      | -                                               |
| License         | List/Get                      | -                                               |
| Role            | List/Get                      | -                                               |

### ストレージのTokenの暗号化と復号

`ProtectedString`が、Token保存時の暗号化と、読み込み時の復号の機能を提供します。  
詳細は後述。

### Pagination機能

Cisco Spark APIのpaginationに関しては、[ここ](https://developer.ciscospark.com/pagination.html)を参照。

`result.HasNext`と`result.ListNextAsync()`が、Cisco Spark API Clientで利用可能です。  
詳細は後述。

### HTTP Statusコードの取得

`result.HttpStatusCode`が、Cisco Spark API Clientで利用可能です。  
詳細は後述。

よりプログラマが利用しやすいステータスコードを検討中ですが、  
現時点では、HTTP Statusコードを返します。

### Retry-Afterの取得

`result.HasRetryAfter`と `result.RetryAfter`が、Cisco Spark API Clientで利用可能です。  
また、 `RetryExecutor`が利用可能です。  
詳細は後述。

### trackingIdの取得

trackingIdは、Cisco Spark APIのテクニカルサポートで利用される可能性があります。

`result.TrackingId`が、Cisco Spark API Clientで利用可能です。  
詳細は後述。

### Validates webhook secret

`Webhook.CreateEventValidator()`が、Cisco Spark API Clientで利用可能です。   
詳細は後述。

### Webhook listner

Webhook listner機能は、簡易的なWebhookのサーバ機能を提供します。  
**注意: この機能は、簡単なテスト時の利用を想定しています。  
運用環境等では、より信頼性のあるサーバをご利用ください。**

`WebhookListener`が、Cisco Spark API Clientで利用可能です。   
詳細は後述。



## 基本的な使い方

### 暗号化したTokenをストレージに保存する

``` csharp
char[] tokens = GetBotTokensFromUser();

var protectedToken = LocalProtectedString.FromChars(tokens);
LocalProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

**注意: LocalProtectedStringはメモリ内での保護は提供していません。  
Tokenを保存する際の、暗号化と復号での利用を想定しています。**


### 暗号化したTokenをストレージから読み込む

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = LocalProtectedString.FromEncryptedData(encryptedData, entropy);
```

### Cisco Spark API Clientのインスタンスを作成する

Cisco Spark API Clientのインスタンスは可能な限り再利用してください。

``` csharp
SparkAPIClient spark = SparkAPI.CreateVersion1Client(protectedToken);
```

### Cisco Sparkのスペースにメッセージを投稿する

``` csharp
var result = await spark.CreateMessageAsync("xyz_space_id", "こんにちは, Spark!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("メッセージが投稿されました: id = {0}", result.Data.Id);
}
```

### Cisco Sparkのスペースに添付ファイル付きでメッセージを投稿する

``` csharp
using (var fs   = new FileStream("path/myfile.png", FileMode.Open, FileAccess.Read, FileShare.Read))
using (var data = new SparkFileData(fs, "imagefile.png", SparkMediaType.ImagePNG))
{
    var result = await spark.CreateMessageAsync("xyz_space_id", "添付ファイル付き", data);

    if(result.IsSuccessStatus)
    {
       Console.WriteLine("添付ファイル付きでメッセージが投稿されました: id = {0}", result.Data.Id);
    }
}
```

### Cisco Sparkの1:1スペースにメッセージを投稿する

``` csharp
var result = await spark.CreateDirectMessageAsync("targetuser@example.com", "こんにちは, Spark!");

if(result.IsSuccessStatus)
{
   Console.WriteLine("メッセージが投稿されました: id = {0}", result.Data.Id);
}
```

### スペースの一覧を取得する

``` csharp
var result = await spark.ListSpaces();

if(result.IsSuccessStatus && result.Data.HasItems)
{
  foreach (var item in result.Data.Items) {
    Console.WriteLine("Space: title = {0}", item.Title);
  }  
}
```

### Pagenation機能を利用する

``` csharp
var result = await spark.ListSpaces();

if(result.IsSuccessStatus)
{
  //
  // ここで何か処理する...
  //

  if(result.HasNext)
  {
    // 続きのリストがあれば取得する。
    result = await result.ListNextAsync();

    if(result.IsSuccessStatus)
    {
      // ...
    }
  }
}
```

### Http status codeを取得する

``` csharp
var result = await spark.ListSpaces();

Console.WriteLine("Status is {0}", result.HttpStatusCode);
```

### Retry afterを取得する.

``` csharp
var result = await spark.ListSpaces();

if(result.IsSuccessStatus)
{
  //
  // ここで何かする...
  //
}
else if(result.HasRetryAfter)
{
  Console.WriteLine("{0}にリトライしなきゃ!!", result.RetryAfter.Date);  
}
```

### TrackingIdを取得する

``` csharp
var result = await spark.ListSpaces();

Console.WriteLine("Tracking id: {0}", result.TrackingId);  
```


### Webhookに通知されたデータを検証する

``` csharp
var webhook = await spark.GetWebhook("xyz_webhook_id");

var validator = webhook.CreateEventValidator();
```

イベントがWebhookのURIに通知された際には、
x-Spark-Signatureがハッシュ値を持っています。

validatorを利用して、データの整合性を確認できます。

``` csharp
byte[] webhookEventData = GetWebhookEventData();

if( validator.Validate(webhookEventData, "xyz_x_spark_signature_value") )
{
  Console.WriteLine("通知されたイベントデータの検証に成功!");
}
```

### Webhookの通知管理

Webhook notification managerを使ってWebhookへの通知を管理します。


* インスタンスを作成する。

``` csharp
var notificationManager = new WebhookNotificationManager();
```

* 通知用のfunctionを登録します。

``` csharp
var webhook = await spark.GetWebhook("xyz_webhook_id");

notificationManager.AddNotification(
  webhook,
  (eventData) =>
  {
    Console.WriteLine("イベントを受信, id = {0}", eventData.Id);
  }
);
```

* イベントの受信時。

``` csharp
byte[] webhookEventData = GetWebhookEventData();

// Signatureが確認され登録したfunctionにイベントデータが通知されます。
notificationManager.ValidateAndNotify(webhookEventData, "xyz_x_spark_signature_value", encodingOfData);
```
### Webhook listenerをngrokと共に利用する

グローバルIPアドレスが利用できない場合、
[ngrok](https://ngrok.com/)などのトンネリングサービスが便利な場合があります。

* ngrokの入手と起動。

ngrokのコマンドラインツールは、[ここから入手](https://ngrok.com/)できます。

以下のコマンドで、トンネリングサービスを起動して、localhostの8080ポートにフォワードされます。

```
prompt> ngrok http 8080 --bind-tls=true
```

* Webhook listenerのインスタンスの作成。

``` csharp
var listener = new WebhookListener();
```

* 待ち受けする、ホストとポートを登録する。

``` csharp
var endpointUri = listener.AddListnerEndpoint("localhost", 8080, false);
```

* Webhook listener用のWebhookを作成します。

この例では、ngronのトンネリングサービスを利用しています。  
`listener.AddListnerEndpoint()`が返したUriは、フォワード先のUriです。

Webhookには、ngrok側のUriを指定する必要があります。

ngrokが、`https://ngrok-xyz.example.com`を割り当てた場合、  
`String.Format("https://ngrok-xyz.example.com{0}", endpointUri.AbsolutePath)`をWebhookの宛先として指定します。

``` csharp
var result = await spark.CreateWebhookAsync(
  "テスト用のWebhook",
  new Uri(String.Format("https://ngrok-xyz.example.com{0}", endpointUri.AbsolutePath)),
  EventResource.Message,
  EventType.Created);
```

* Webhook listenerにWebhookと通知先funcを登録します。


``` csharp
var webhook = result.Data;

listener.AddNotification(
  webhook,
  async (eventData) =>
  {
    Console.WriteLine("Eventが通知されました, id = {0}", eventData.Id);
  }
);
```

* Listnerの開始。

Listenerを開始すると、イベント発生時に登録したfunctionに通知されます。

``` csharp
listener.Start();
```



## 計画中の機能

| 機能 | 概要 |
| :--- | :--- |
| OAuth2 Helper | Integrationのトークン取得用のOAuth2 Helper。 |
| Markdown builder | Cisco Spark API特有のMarkdownのBuilder。 |
| Error codeとdescriptionの取得 | エラー発生時に、Cisco SparkのJson bodyに含まれるerror codeとdescriptionを取得する。 |
