# SignalR の学習

SignalR は Web に双方向のリアルタイム通信を可能にするライブラリとして作成されましたが、.NET Framework のクライアントが用意されていますので、Web 以外でも使用できます。

ということで、簡単なサンプルプログラムでも作ってみます。

## 作るもの

リアルタイム通信なので、複数台のPCに 予定/実績/差異 を表示させてみます。
表示は Web で行います。
サーバは IIS で SignalR を動作させます。_(SignalR は ASP.NET の仲間) Self-Host でも可能_
予定、実績の更新は .NET のコンソールアプリから行うようにします。

## xxx

### まずは SignalR のサーバから

VS2013 の新規プロジェクトで、ASP.NET Web アプリケーション を選び テンプレートを Empty で作成。
NuGet で Microsoft.AspNet.SignalR をインストール。
Hubs フォルダを追加して、PlanHub.cs を追加。
追加した PlanHub.cs はこれ。
``` csharp
using Microsoft.AspNet.SignalR;

namespace PaSignalR.Hubs
{
    public class PlanHub : Hub
    {
        private static int _plan = 0;
        private static int _result = 0;

        public void GetPlan()
        {
            Clients.Caller.Updated(_plan, _result);
        }

        public void Update(int plan, int result)
        {
            _plan = plan;
            _result = result;
            Clients.All.Updated(plan, result);
        }
    }
}
```
計画、実績は共有メンバで保持。GetPlan が呼ばれると Updated を呼び出して計画と実績を通知。
Update が呼ばれると、計画と実績を保持し、Updated を呼び出してオウム返しする。
GetPlan は呼ばれてたクライアントだけに、Update は全てのクライアントに Updated を呼び出します。

後は、Startup クラスを作って、SigralR が機能するようにしたりしますが、ほぼ定型的な作業です。

次に、ブラウザに表示する html ですが、MVCでもWebFormsでもなく静的なファイルでOKです。

``` html
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>xxx</title>
</head>
<body>
    <table border="0">
        <tr><th>予定</th><td><span id="plan" /></td></tr>
        <tr><th>実績</th><td><span id="result" /></td></tr>
        <tr><th>差異</th><td><span id="diff" /></td></tr>
    </table>
    <script src="Scripts/jquery-1.6.4.min.js"></script>
    <script src="Scripts/jquery.signalR-2.2.0.min.js"></script>
    <script src="signalr/hubs"></script>
    <script type="text/javascript">
        $(function () {
            var planHub = $.connection.planHub;
            planHub.client.Updated = function (plan, result) {
                $("#plan").text(plan);
                $("#result").text(result);
                $("#diff").text(result - plan);
            };
            $.connection.hub.start().done(function () {
                planHub.server.getPlan();
            });
        });
    </script>
</body>
</html>
```
サーバに繋がった後、GetPlan を呼び出して現在の値を取得しています。
GetPlan　を呼び出したり、値が変更されると Updated が呼び出されますので、各々のtextを書き換えています。

次に、値を更新するコンソールアプリですが、ソリューションにコンソールアプリのプロジェクトを追加します。
今回は、SignalR のサンプルですので、コマンドライン引数に計画と実績を指定してサーバに通知して終了する単純なプログラムとします。
``` csharp
using System;
using Microsoft.AspNet.SignalR.Client;

namespace PsSignalR.CommandLine
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var conn = new HubConnection("http://localhost:55740/");
            var hub = conn.CreateHubProxy("planHub");
            hub.On<int, int>("Updated", (plan, result) =>
            {
                Console.WriteLine("PLAN  :{0}", plan);
                Console.WriteLine("RESULT:{0}", result);
                Console.WriteLine("DIFF  :{0}", result - plan);
            });
            conn.Start().ContinueWith(t =>
            {
                if (args.Length == 2)
                {
                    hub.Invoke("Update", int.Parse(args[0]), int.Parse(args[1]));
                }
                else
                {
                    hub.Invoke("GetPlan");
                }
            });
            Console.ReadLine();
        }
    }
}
```