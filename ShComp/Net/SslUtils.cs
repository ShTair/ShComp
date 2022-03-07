using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ShComp.Net;

public static class SslUtils
{
    public static async Task<X509Certificate2?> GetCertificateAsync(string host, int port = 443, CancellationToken cancellationToken = default)
    {
        using var client = new TcpClient(host, port);
        using var stream = client.GetStream();
        using var sslStream = new SslStream(stream);

        var options = new SslClientAuthenticationOptions
        {
            TargetHost = host,
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        };

        await sslStream.AuthenticateAsClientAsync(options, cancellationToken);
        return sslStream.RemoteCertificate as X509Certificate2;
    }

    public static Task<X509Certificate2?> GetCertificateAsync(string host, CancellationToken cancellationToken) => GetCertificateAsync(host, 443, cancellationToken);
}
