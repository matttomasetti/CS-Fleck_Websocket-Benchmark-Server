using System;
using Fleck;
using Newtonsoft.Json.Linq;

namespace fleck_socket
{
    class Program
    {
        /// <summary>
        /// Creates the websocket server object with the necessary event handlers, and
        /// starts listening on the given port
        /// </summary>
        /// <param name="args"> </param>
        static void Main(string[] args)
        {
            // Create a listen server on localhost with port 8080
            var server = new WebSocketServer("ws://0.0.0.0:8080");
            server.Start(socket =>
            {
                
                /*
                 * Bind required events for the server
                 */
            
                // Event triggered whenever a client connects to the websocket
                socket.OnOpen = () =>
                {
                    // send newly connected client initial timestamp
                    Notify(socket, 0);
                };
                
                // Event triggered whenever a client disconnects to the websocket
                socket.OnClose = () => Console.WriteLine("Close!");
                
                // Event triggered whenever the websocket server receives a message form a client
                socket.OnMessage = message =>
                {
                    // decode incoming message into an object
                    JObject json = JObject.Parse(message);
                    var c = json.GetValue("c").ToString();
                
                    // notify client with event for message with count "c"
                    Notify(socket, int.Parse(c));
                };
            });

            Console.ReadLine();
        }
        
        /// <summary>
        /// Gets the current unix timestamp of the server
        /// </summary>
        /// <returns> The current unix timestamp </returns>
        private static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        
        /// <summary>
        /// Creates a JSON string containing the message count and the current timestamp
        /// </summary>
        /// <param name="c"> The message count </param>
        /// <returns>  A JSON string containing the message count and the current timestamp </returns>
        private static string GetEvent(int c)
        {
            //create an event array for the time that message "c" is received by the server
            return String.Format("{{\"c\":{0},\"ts\":{1}}}", c.ToString(), GetTimestamp().ToString());
        }

        /// <summary>
        /// Send a connected client an event JSON string
        /// </summary>
        /// <param name="ws"> The client connection the outgoing message is for </param>
        /// <param name="c"> The message count </param>
        private static void Notify(IWebSocketConnection ws, int c)
        {
            //send the given connection the event timestamp for message "c"
            ws.Send(GetEvent(c));
        }
    }
}