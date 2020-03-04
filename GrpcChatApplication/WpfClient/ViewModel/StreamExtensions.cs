using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Google.Protobuf.Collections;
using Grpc.Core;
using GrpcServer;

namespace WpfClient.ViewModel
{
    internal static class StreamExtensions
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
            this IAsyncStreamReader<T> stream,
            [EnumeratorCancellation] CancellationToken token)
        {
            while (await stream.MoveNext(token))
            {
                yield return stream.Current;
            }
        }

        public static string ToUserString(this RepeatedField<User> userlist)
        {
            string s = userlist.Count.ToString() + " Teilnehmer:";
            foreach (var user in userlist)
                s += " " + user.Name;
            return s;
        }
    }


}