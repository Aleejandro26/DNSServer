using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DNSServer {
    class Program {
        static void Main(string[] args)
        {
            StartDNSServer();
        }

        static void StartDNSServer()
        {
            try
            {
                int dnsPort = 53;
                UdpClient udpListener = new UdpClient(dnsPort);
                Console.WriteLine("Servidor DNS iniciado. Esperando consultas...");

                while (true)
                {
                    IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedData = udpListener.Receive(ref clientEndpoint);
                    string domain = Encoding.ASCII.GetString(receivedData);
                    Console.WriteLine($"Consulta recibida: {domain}");

                    // Responder con una dirección IP estática (por ejemplo, 127.0.0.1)
                    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                    byte[] ipBytes = ipAddress.GetAddressBytes();
                    byte[] responseData = new byte[receivedData.Length];
                    Array.Copy(receivedData, responseData, receivedData.Length);
                    responseData[2] = 0x81; // Asignar el bit de respuesta
                    responseData[3] = 0x80; // Asignar el bit de autoridad

                    // Construir la respuesta DNS con la dirección IP estática
                    responseData[6] = 0;
                    responseData[7] = 1;
                    responseData[10] = 0;
                    responseData[11] = 1;

                    // Enviar la respuesta al cliente
                    udpListener.Send(responseData, responseData.Length, clientEndpoint);
                    Console.WriteLine($"Respuesta enviada: {ipAddress}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}