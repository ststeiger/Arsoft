using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System
{

    // Environment.OSVersion.Platform == PlatformID.Unix

    public sealed class DotNetUtilities
    {
        public static Org.BouncyCastle.X509.X509Certificate FromX509Certificate(System.Security.Cryptography.X509Certificates.X509Certificate x509Cert)
        {
            // https://stackoverflow.com/questions/8136651/how-can-i-convert-a-bouncycastle-x509certificate-to-an-x509certificate2
            Org.BouncyCastle.X509.X509Certificate mycer = null;
            
            return null;
        }

    }


    public static class NetExtension
    {


        public static IAsyncResult AsApm(this Task task,
                                   AsyncCallback callback,
                                   object state)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<bool>(state);

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                 else tcs.TrySetResult(true);

                if (callback != null)
                    callback(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }

        public static IAsyncResult AsApm<T>(this Task<T> task,
                                    AsyncCallback callback,
                                    object state)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);

                if (callback != null)
                    callback(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }


        public static TOut[] ConvertAll<TIn, TOut>(this TIn[] thisArray, Func<TIn, TOut> converter)
        {
            if (thisArray == null)
                throw new ArgumentNullException(nameof(thisArray));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            TOut[] revVal = new TOut[thisArray.Length];

            for (int i = 0; i < thisArray.Length; i++)
                revVal[i] = converter(thisArray[i]);

            return revVal;
        }


        // public static byte[] GetRawCertData(this Org.BouncyCastle.X509.X509Certificate cert)
        public static byte[] GetRawCertData(this System.Security.Cryptography.X509Certificates.X509Certificate cert)
        {
            // https://stackoverflow.com/questions/1182612/what-is-the-difference-between-x509certificate2-and-x509certificate-in-net
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = (System.Security.Cryptography.X509Certificates.X509Certificate2)cert;
            return cert2.GetRawCertData();
        }


        public static void Connect(this UdpClient udpClient, System.Net.IPAddress address, int port)
        {
            // Just ignore it, new in SendAsync
        }


        //public async static Task<int> SendAsync(this UdpClient udpClient, byte[] datagram, int bytes)
        //{
        //    return 0;
        //}


        public static IAsyncResult BeginConnect(this TcpClient tcpClient, System.Net.IPAddress address, int port, AsyncCallback requestCallback, object state)
        {
            return tcpClient.ConnectAsync(address, port).AsApm(requestCallback, state);
        }

        
        public static bool EndConnect(this TcpClient tcpClient, IAsyncResult asyncResult)
        {
            return ((Task<bool>)asyncResult).Result;
        }
        


        public static bool WaitOne(this System.Threading.WaitHandle wh, TimeSpan timeout, bool exitContext)
        {
            return wh.WaitOne(timeout);
        }

        public static void Close(this UdpClient udpClient)
        {
        }


        public static void Close(this TcpClient tcpClient)
        {
        }


        public static void Close(this System.Threading.WaitHandle handle)
        {
        }
    }


}
