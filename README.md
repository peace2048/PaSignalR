# SignalR �̊w�K

SignalR �� Web �ɑo�����̃��A���^�C���ʐM���\�ɂ��郉�C�u�����Ƃ��č쐬����܂������A.NET Framework �̃N���C�A���g���p�ӂ���Ă��܂��̂ŁAWeb �ȊO�ł��g�p�ł��܂��B

�Ƃ������ƂŁA�ȒP�ȃT���v���v���O�����ł�����Ă݂܂��B

## ������

���A���^�C���ʐM�Ȃ̂ŁA�������PC�� �\��/����/���� ��\�������Ă݂܂��B
�\���� Web �ōs���܂��B
�T�[�o�� IIS �� SignalR �𓮍삳���܂��B_(SignalR �� ASP.NET �̒���) Self-Host �ł��\_
�\��A���т̍X�V�� .NET �̃R���\�[���A�v������s���悤�ɂ��܂��B

## xxx

### �܂��� SignalR �̃T�[�o����

VS2013 �̐V�K�v���W�F�N�g�ŁAASP.NET Web �A�v���P�[�V���� ��I�� �e���v���[�g�� Empty �ō쐬�B
NuGet �� Microsoft.AspNet.SignalR ���C���X�g�[���B
Hubs �t�H���_��ǉ����āAPlanHub.cs ��ǉ��B
�ǉ����� PlanHub.cs �͂���B
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
�v��A���т͋��L�����o�ŕێ��BGetPlan ���Ă΂��� Updated ���Ăяo���Čv��Ǝ��т�ʒm�B
Update ���Ă΂��ƁA�v��Ǝ��т�ێ����AUpdated ���Ăяo���ăI�E���Ԃ�����B
GetPlan �͌Ă΂�Ă��N���C�A���g�����ɁAUpdate �͑S�ẴN���C�A���g�� Updated ���Ăяo���܂��B

��́AStartup �N���X������āASigralR ���@�\����悤�ɂ����肵�܂����A�قڒ�^�I�ȍ�Ƃł��B

���ɁA�u���E�U�ɕ\������ html �ł����AMVC�ł�WebForms�ł��Ȃ��ÓI�ȃt�@�C����OK�ł��B

``` html
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>xxx</title>
</head>
<body>
    <table border="0">
        <tr><th>�\��</th><td><span id="plan" /></td></tr>
        <tr><th>����</th><td><span id="result" /></td></tr>
        <tr><th>����</th><td><span id="diff" /></td></tr>
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
�T�[�o�Ɍq��������AGetPlan ���Ăяo���Č��݂̒l���擾���Ă��܂��B
GetPlan�@���Ăяo������A�l���ύX������ Updated ���Ăяo����܂��̂ŁA�e�X��text�����������Ă��܂��B

���ɁA�l���X�V����R���\�[���A�v���ł����A�\�����[�V�����ɃR���\�[���A�v���̃v���W�F�N�g��ǉ����܂��B
����́ASignalR �̃T���v���ł��̂ŁA�R�}���h���C�������Ɍv��Ǝ��т��w�肵�ăT�[�o�ɒʒm���ďI������P���ȃv���O�����Ƃ��܂��B
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