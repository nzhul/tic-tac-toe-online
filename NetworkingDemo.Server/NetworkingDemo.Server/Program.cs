﻿using NetworkingDemo.Server;

var server = new NetworkServer();
server.Start();

while (true)
{
    server.PollEvents();
    Thread.Sleep(1000);
}