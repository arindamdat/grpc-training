using Grpc.Net.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Privileges;

namespace GrpcClient
{
    class Program
    {
        static string gRPCServerAdddress = "https://localhost:5001";

        static GrpcChannel Channel = null;
        static GrpcChannel GetChannel()
        {
            if (Channel == null)
            {
                Channel = GrpcChannel.ForAddress(gRPCServerAdddress);
            }
            return Channel;
        }
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press any key to continue........");
            Console.ReadLine();
            var reply = await GetHelloReply();
            Console.WriteLine($"Got reply: {reply.Message} {reply.TimeOfDay}");

            Console.WriteLine("Press any key to continue........");
            Console.ReadLine();
            await GetPrivilegeSetsStream();

            Console.WriteLine("Press any key to continue........");
            Console.ReadLine();
            await GetPrivilegeSetResponseObject();

            Console.ReadLine();
        }

        static async Task GetPrivilegeSetResponseObject()
        {
            var channel = GetChannel();
            var client = new UserManagement.Privileges.PrivilegeSetService.PrivilegeSetServiceClient(channel);
            var reply = await client.GetPrivilegeSetListAsync(new GetPrivilegeSetsRequest());
            foreach(var ps in reply.Items)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ps));
            }
        }
        static async Task GetPrivilegeSetsStream()
        {
            var channel = GetChannel();
            var client = new UserManagement.Privileges.PrivilegeSetService.PrivilegeSetServiceClient(channel);
            var reply = client.GetPrivilegeSets(new UserManagement.Privileges.GetPrivilegeSetsRequest());
            await foreach (var ps in ReadStreamAsync(reply.ResponseStream))
            {
                Console.WriteLine(JsonConvert.SerializeObject(ps));
            }

        }
        static async IAsyncEnumerable<T> ReadStreamAsync<T>(Grpc.Core.IAsyncStreamReader<T> streamReader)
        {
            // you can see the documentation for Grpc.Core.IAsyncStremReader
            while (await streamReader.MoveNext(CancellationToken.None))
            {
                yield return streamReader.Current;
            }
            yield break;
        }
        static async Task<HelloReply> GetHelloReply()
        {
            var channel = GetChannel();
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "Arindam" });
            return reply;
        }
    }
}
