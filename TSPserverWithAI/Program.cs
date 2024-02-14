using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настройка и запуск TCP сервера
            TcpListener tcpServer = new TcpListener(IPAddress.Any, 13000);
            tcpServer.Start();

            // Настройка и запуск HTTP сервера
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .Configure(app =>
                {
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapPost("/generate", async context =>
                        {
                            // Обработка HTTP запроса и свистопляси с AI
                            // модель AI, если чё свой сделаем)
                            var modelId = "b-mc2/sql-create-context";
                            //крч запрос туда сюда,
                            //хуй знает как его сюда засунуть придумаем
                            var prompt = "";

                            var maxTokens = 50;

                            using var reader = new StreamReader(context.Request.Body);
                            var requestBody = await reader.ReadToEndAsync();

                            dynamic requestData = JsonConvert.DeserializeObject(requestBody);

                            if (requestData != null && requestData.model != null && requestData.inputs != null)
                            {
                                modelId = requestData.model;
                                prompt = requestData.inputs;
                                if (requestData.options != null && requestData.options.max_tokens != null)
                                {
                                    maxTokens = requestData.options.max_tokens;
                                }
                            }

                            string response = ProcessCommand(prompt, modelId, maxTokens);

                            await context.Response.WriteAsync(response);
                        });
                    });
                })
                .Build();
            webHost.Start();

            Console.WriteLine("Серверы запущены. Ожидание подключений...");

            // Обработка TCP подключений
            while (true)
            {
                Console.Write("Жду подключения...");
                TcpClient tcpClient = tcpServer.AcceptTcpClient();
                Console.WriteLine("Подключение состоялось");

                string clientIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                Console.WriteLine("IP отправителя: " + clientIP);

                Stream stream = tcpClient.GetStream();
                var br = new BinaryReader(stream);
                string data = br.ReadString();
                Console.WriteLine("Получено:{0}", data);

                data = ProcessCommand(data, "default_model_id", 50);

                var bw = new BinaryWriter(stream);
                bw.Write(data);
                Console.WriteLine("Отправлено: {0}", data);

                tcpClient.Close();
                br.Close();
                bw.Close();
            }
        }

        public static string ProcessCommand(string data, string modelId, int maxTokens)
        {
            // Обработка сообщения
            // В данном примере сервер просто возвращает строку в верхнем регистре
            return Console.ReadLine();
        }
    }
}
