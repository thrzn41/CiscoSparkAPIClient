# Cisco Spark API Client for .NET

[![nuget](https://img.shields.io/nuget/v/Thrzn41.CiscoSpark.svg)](https://www.nuget.org/packages/Thrzn41.CiscoSpark) [![MIT license](https://img.shields.io/github/license/thrzn41/CiscoSparkAPIClient.svg)](https://github.com/thrzn41/CiscoSparkAPIClient/blob/master/LICENSE)

`Cisco Spark API Client`は、`Cisco Spark REST API`を利用しやすくしたライブラリです。  
基本的な機能のほかに、Cisco SparkのAPIを使いやすくするための機能を実装しています。

#### ほかの言語のREADME
* [English README](https://github.com/thrzn41/CiscoSparkAPIClient/blob/master/README.md) ([英語のREADME](https://github.com/thrzn41/CiscoSparkAPIClient/blob/master/README.md))

---
## 利用可能なプラットフォーム
* .NET Standard 1.3以降
* .NET Core 1.0以降
* .NET Framework 4.5.2以降

---
## 利用可能な機能

* Cisco Sparkの基本的なAPI(List/Get/Create Message, Spaceなど)。
* Cisco SparkのAdmin API(List/Get Event, Licenseなど)。
* ストレージに保存するTokenの暗号化と復号。
* List API用のPagination機能。
* Retry-after値の処理とRetry executor。
* Markdown builder
* エラーコードや詳細の取得。
* Webhook secretの検証とWebhook notification manager、Webhook event handler。
* OAuth2 helper
* 簡易Webhookサーバ機能(.NET Standard 2.0+, .NET Core 2.0+, .NET Framework 4.5.2+)。

### 基本機能

| Sparkのリソース名                  | 利用可能な機能                 | 説明                                 |
| :-------------------------------- | :---------------------------- | :---------------------------------- |
| Person/People                     | List/Get                      | Get Meも利用可能                     |
| Space(Room)                       | List/Create/Get/Update/Delete | -                                   |
| SpaceMembership(Membership)       | List/Create/Get/Update/Delete | -                                   |
| Message                           | List/Create/Get/Delete        | ローカルのstreamからファイル添付も可能 |
| Team                              | List/Create/Get/Update/Delete | -                                   |
| TeamMembership                    | List/Create/Get/Update/Delete | -                                   |
| Webhook                           | List/Create/Get/Update/Delete | -                                   |
| File                              | GetInfo/GetData/Upload        | -                                   |

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

### Retry-Afterの取得

`result.HasRetryAfter`と `result.RetryAfter`が、Cisco Spark API Clientで利用可能です。  
また、 `RetryExecutor`が利用可能です。  
詳細は後述。

### HTTP Statusコードの取得

`result.HttpStatusCode`が、Cisco Spark API Clientで利用可能です。  
詳細は後述。

### エラーコードと詳細の取得

Cisco Spark APIは、エラーコードと詳細を返す場合があります。  
`result.Data.HasErrors`と`result.Data.GetErrors()`が、Cisco Spark API Clientで利用可能です。

### 部分エラーの取得

Cisco Spark APIは、部分的なエラーを返す場合があります。  
部分エラーの詳細に関しては、[ここ](https://developer.ciscospark.com/errors.html)を参照。  
`Item.HasErrors`と`Item.GetPartialErrors()`が、Cisco Spark API Clientで利用可能です。

### trackingIdの取得

trackingIdは、Cisco Spark APIのテクニカルサポートで利用される可能性があります。

`result.TrackingId`が、Cisco Spark API Clientで利用可能です。  
詳細は後述。

### Validates webhook secret

`Webhook.CreateEventValidator()`が、Cisco Spark API Clientで利用可能です。   
詳細は後述。

Cisco Spark API Clientの`CreateWebhookAsync()`メソッドはデフォルトでは、webhook secretを動的に生成します。

### Webhook listener(.NET Standard 2.0+, .NET Core 2.0+, .NET Framework 4.5.2+)

Webhook listener機能は、簡易的なWebhookのサーバ機能を提供します。  
> **注記: この機能は、簡単なテスト時の利用を想定しています。  
> 運用環境等では、より信頼性のあるサーバをご利用ください。**

`WebhookListener`が、Cisco Spark API Clientで利用可能です。   
詳細は後述。

---
## 基本的な使い方

### Cisco Spark API Clientのインストール

`Cisco Spark API Client`は、以下のいずれかの方法で、`NuGet` package manager経由で入手できます。

* NuGet Package ManagerのGUI  
"`Thrzn41.CiscoSpark`"を検索してインストール。

* NuGet Package ManagerのCLI  
```
PM> Install-Package Thrzn41.CiscoSpark
```

* .NET Client  
```
> dotnet add package Thrzn41.CiscoSpark
```

### Cisco Spark API Client関連のusingディレクティブ

usingディレクティブを利用する場合は、以下の名前空間を指定します。

``` csharp
using Thrzn41.Util
using Thrzn41.CiscoSpark
using Thrzn41.CiscoSpark.Version1
```

必要に応じて、`Thrzn41.CiscoSpark.Version1.Admin`も利用可能です。

### Cisco Spark API Clientインスタンスの作成

Cisco Spark API Clientのインスタンスは可能な限り長い期間使いまわすようにします。

``` csharp
/// 基本API利用時。
SparkAPIClient spark = SparkAPI.CreateVersion1Client(token);
```

Admin APIを利用する場合は、Admin API用のインスタンスを作成する必要があります。  
`SparkAdminAPIClient`は、`SparkAPIClient`の全機能に加えて、Adminの機能が利用できます。  
``` csharp
/// Admin API利用時。
SparkAdminAPIClient spark = SparkAPI.CreateVersion1AdminClient(token);
```

> **注記: 'token'は、Cisco Spark APIでは、非常にセンシティブな情報です。  
> 'token'は、慎重に保護する必要があります。  
> ソースコード中に直接記載したり、安全ではない方法で保存しないようにします。  
> `Cisco Spark API Client`では、トークンを暗号化したり復号する方法を、いくつか提供しています。  
> 独自の方法で暗号化や復号、保護を実装する場合は、インスタンス作成時に、復号されたトークン文字列を利用することができます。**

### 暗号化したTokenをストレージに保存する

``` csharp
char[] tokens = GetBotTokenFromBotOwner();

var protectedToken = LocalProtectedString.FromChars(tokens);
LocalProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

**注記: LocalProtectedStringはメモリ内での保護は提供していません。  
Tokenを保存する際の、暗号化と復号での利用を想定しています。**


### 暗号化したTokenをストレージから読み込んで、Cisco Spark API Clientのインスタンスを作成する

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = LocalProtectedString.FromEncryptedData(encryptedData, entropy);

/// 基本API利用時。
SparkAPIClient spark = SparkAPI.CreateVersion1Client(protectedToken);
```

> **注記: オプションに応じて、暗号化されたデータは、暗号化したときと同じローカルユーザまたはローカルマシン上でのみ復号できます。**

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
var result = await spark.ListSpacesAsync();

if(result.IsSuccessStatus && result.Data.HasItems)
{
  foreach (var item in result.Data.Items) {
    Console.WriteLine("Space: title = {0}", item.Title);
  }  
}
```

### ファイルの情報やデータを取得する

ダウンロードせずにファイルの情報だけ入手する。

``` csharp
var result = await spark.GetFileInfoAsync(new Uri("https://api.example.com/path/to/file.png"));

if(result.IsSuccessStatus)
{
  var file = result.Data;

  Console.WriteLine("File: Name = {0}, Size = {1}, Type = {2}", file.Name, file.Size?.Value, file.MediaType?.Name);
}
```

ファイルをダウンロードする。
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
      // streamにファイルのデータが含まれる。
    }
  }
}
```

### Pagenation機能を利用する

``` csharp
var result = await spark.ListSpacesAsync();

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
var result = await spark.ListSpacesAsync();

Console.WriteLine("Status is {0}", result.HttpStatusCode);
```

### Retry afterを取得する.

``` csharp
var result = await spark.ListSpacesAsync();

if(result.IsSuccessStatus)
{
  //
  // ここで何かする...
  //
}
else if(result.HasRetryAfter)
{
  Console.WriteLine("{0}後にリトライしなきゃ!!", result.RetryAfter.Delta);  
}
```

### TrackingIdを取得する

``` csharp
var result = await spark.ListSpacesAsync();

Console.WriteLine("Tracking id: {0}", result.TrackingId);  
```

### Markdown Builder.

``` csharp
var md = new MarkdownBuilder();

// メンションと番号付きリストでMarkdownを作成。
md.Append("こんにちは、").AppendMentionToPerson("xyz_person_id", "〇〇さん").AppendLine();
md.AppendOrderedList("Item1");
md.AppendOrderedList("Item2");
md.AppendOrderedList("Item3");

var result = await spark.CreateMessageAsync("xyz_space_id", md.ToString());
```

### Webhookに通知されたデータを検証する

``` csharp
var webhook = await spark.GetWebhookAsync("xyz_webhook_id");

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
var webhook = await spark.GetWebhookAsync("xyz_webhook_id");

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

### Webhook Listener

* Webhook listenerのインスタンスの作成。

``` csharp
var listener = new WebhookListener();
```

* 待ち受けする、ホストとポートを登録する。

待ち受けには、TLS/httpsを利用すべきです。  
そのためには、まず、実行環境側で`netsh`ツールなどを利用して、有効な証明書をバインドしておく必要があります。

バインドされたアドレスとポート番号で、エンドポイントを追加します。

``` csharp
var endpointUri = listener.AddListenerEndpoint("yourwebhookserver.example.com", 8443);
```

* Webhook listener用のWebhookを作成します。

`listener.AddListenerEndpoint()`が返す`endpointUri`がWebhookの通知先Uriになります。  

``` csharp
var result = await spark.CreateWebhookAsync(
  "my webhook for test",
  endpointUri,
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

    if(eventData.Resouce == EventResouce.Message)
    {
      Console.WriteLine("Message, id = {0}", eventData.MessageData.Id);
    }
  }
);
```

* Listenerの開始。

Listenerを開始すると、イベント発生時に登録したfunctionに通知されます。

``` csharp
listener.Start();
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

ngrokは、`localhost`へフォワードします。

``` csharp
var endpointUri = listener.AddListenerEndpoint("localhost", 8080, false);
```

* Webhook listener用のWebhookを作成します。

この例では、ngronのトンネリングサービスを利用しています。  
`listener.AddListenerEndpoint()`が返したUriは、フォワード先のUriです。

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

    if(eventData.Resouce == EventResouce.Message)
    {
      Console.WriteLine("Message, id = {0}", eventData.MessageData.Id);
    }
  }
);
```

* Listenerの開始。

Listenerを開始すると、イベント発生時に登録したfunctionに通知されます。

``` csharp
listener.Start();
```
