using ServidorPostIt.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace ServidorPostIt.Services
{
    internal class NotaServer
    {
        HttpListener server = new();

        public NotaServer()
        {
            server.Prefixes.Add("http://*:12345/notas/");
        }

        public void Iniciar()
        {
            if (!server.IsListening)
            {
                server.Start();

                new Thread(Escuchar)
                {
                    IsBackground = true
                }.Start();

            }
        }

        public event EventHandler<Nota>? NotaRecibida;

        private void Escuchar(object? obj)
        {
            while (true)
            {
                var context = server.GetContext();
                var pagina = File.ReadAllText("assets/index.html");
                var bufferPagina = Encoding.UTF8.GetBytes(pagina);

                if (context.Request.Url != null)
                {
                    if (context.Request.Url.LocalPath == "/notas/")
                    {
                        context.Response.ContentLength64 = bufferPagina.Length;
                        context.Response.OutputStream.Write(bufferPagina, 0, bufferPagina.Length);
                        context.Response.StatusCode = 200;
                        context.Response.OutputStream.Close();
                    } else if (context.Request.HttpMethod == "POST" && context.Request.Url.LocalPath == "/notas/crear")
                    {
                        var bufferDatos = new byte[context.Request.ContentLength64];
                        context.Request.InputStream.Read(bufferDatos, 0, bufferDatos.Length);
                        string datos = Encoding.UTF8.GetString(bufferDatos);
               
                        var diccionario = HttpUtility.ParseQueryString(datos);
                        Nota nota = new()
                        {
                           Titulo = diccionario["titulo"] ?? "",
                           Contenido = diccionario["contenido"] ?? "",
                           Remitente = Dns.GetHostEntry(context.Request.RemoteEndPoint.Address).HostName,
                           X = int.Parse(diccionario["x"] ?? "0"),
                           Y = int.Parse(diccionario["y"] ?? "0")

                        };

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            NotaRecibida?.Invoke(this, nota);
                        });

                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                }

            }
        }

        public void Detener()
        {
            server.Stop();
        }
    }
}
