# Cisco Spark API Client

## 現在利用可能な機能

### 基本機能

| Sparkのリソース名 | 利用可能な機能                | 説明 |
| :-------------- | :---------------------------- | :---------------------------------- |
| Person/People   | List/Get/Delete               | Get Meも利用可能                     |
| Space/Room      | List/Create/Get/Update/Delete | -                                   |
| SpaceMembership | List/Create/Get/Update/Delete | -                                   |
| Message         | List/Create/Get/Delete        | ローカルのstreamからファイル添付も可能 |
| Team            | List/Create/Get/Update/Delete | -                                   |
| TeamMembership  | List/Create/Get/Update/Delete | -                                   |
| Webhook         | List/Create/Get/Update/Delete | -                                   |
| File            | GetInfo/GetData/Upload        | -                                   |

現時点では、Cisco Spark API Clientでは、Admin/Event APIは利用可能ではありません。  
将来的には実装予定です。

### ストレージのTokenの暗号化と復号

`ProtectedString`が、ストレージへのToken保存時の暗号化と、読み込み時の復号の機能を提供します。  
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
詳細は後述。

### trackingIdの取得

trackingIdは、Cisco Spark APIのテクニカルサポートで利用される可能性があります。

`result.TrackingId`が、Cisco Spark API Clientで利用可能です。  
詳細は後述。




## 基本的な使い方

### 暗号化したTokenをストレージに保存する

``` csharp
char[] tokens = GetTokensFromUser();

var protectedToken = ProtectedString.FromChars(tokens);
ProtectedString.ClearChars(tokens);

Save("token.dat",   protectedToken.EncryptedData);
Save("entropy.dat", protectedToken.Entropy);
```

**注意: ProtectedStringはメモリ内での保護は提供していません。  
Tokenを保存する際の、暗号化と復号での利用を想定しています。**


### 暗号化したTokenをストレージから読み込む

``` csharp
byte[] encryptedData = Load("token.dat");
byte[] entropy       = Load("entropy.dat");

var protectedToken = ProtectedString.FromEncryptedData(encryptedData, entropy);
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
    var result = spark.CreateMessageAsync("xyz_space_id", "添付ファイル付き", data);

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



## 計画中の機能

| 機能 | 概要 |
| :--- | :--- |
| Webhook secretの整合性検証 | webhook secretの整合性を検証する機能。 |
| Webhook manager | webhooksを管理する。 |
| Markdown builder | Cisco Spark API特有のMarkdownのBuilder。 |
| Error codeとdescriptionの取得 | エラー発生時に、Cisco SparkのJson bodyに含まれるerror codeとdescriptionを取得する。 |
| Admin APIs | AdminとEvent APIの機能。 |
| Simple Webhook server | テストやデモで利用できる簡単なWebhookサーバ機能。 |
