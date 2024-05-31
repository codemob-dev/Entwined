# Entwined
An advanced Bopl Battle networking library

## How to use
Instead of a big text wall on how everything works, there will be an example Hello World program to demonstrate the basic functionality.

```c#
[BepInDependency("com.entwinedteam.entwined")]
[BepInPlugin("com.yourname.myFirstEntwinedPlugin", "My First Entwined Plugin", 1.0.0)]
internal class MyPlugin : MonoBehaviour
{
    static PacketChannel helloWorldChannel;
    private void Awake()
    {
        helloWorldChannel = new PacketChannel(this);

        helloWorldChannel.OnMessage += PacketType_OnMessage;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(15, 120, 100, 40), "Send Packet"))
        {
            helloWorldPacket.SendMessage(Encoding.ASCII.GetBytes("Hello World!"));
            Logger.LogInfo("Sent message!");
        }
    }

    private static void PacketType_OnMessage(byte[] payload)
    {
        Logger.LogInfo(Encoding.ASCII.GetString(payload));
    }
}
```

The first line, `[BepInDependency("com.entwinedteam.entwined")]`, ensures that your plugin will only load if the user has installed Entwine.

Next up is:
```c#
static PacketChannel helloWorldChannel;
private void Awake()
{
    helloWorldChannel = new PacketChannel(this);

    helloWorldChannel.OnMessage += PacketType_OnMessage;
}
```
This creates a new isolated channel and adds the function `PacketType_OnMessage`. This function is run whenever a client broadcasts a packet on your channel.

```c#
private static void PacketType_OnMessage(byte[] payload)
{
    Logger.LogInfo(Encoding.ASCII.GetString(payload));
}
```
The function decodes the payload of the packet to a string and logs it to the console.

`OnGUI` creates a button and runs the following code when it is pressed:
```c#
helloWorldPacket.SendMessage(Encoding.ASCII.GetBytes("Hello World!"));
Logger.LogInfo("Sent message!");
```
This converts the string to a byte array and broadcasts it to all connected clients.