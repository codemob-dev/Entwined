# Entwined
An advanced Bopl Battle networking library.

## How to use
Instead of a big text wall on how everything works, there will be an example Hello World program to demonstrate the basic functionality.
```c#
[BepInDependency("com.entwinedteam.entwined")]
[BepInPlugin("com.yourname.myFirstEntwinedPlugin", "My First Entwined Plugin", 1.0.0)]
internal class MyPlugin : MonoBehaviour
{
    static EntwinedPacketChannel<string> helloWorldChannel;
    private void Awake()
    {
        helloWorldChannel = new EntwinedPacketChannel<string>(this, new StringEntwiner());

        helloWorldChannel.OnMessage += OnMessage;
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(15, 120, 100, 40), "Send Packet"))
        {
            helloWorldChannel.SendMessage("Hello World!");
        }
    }

    private static void OnMessage(string payload, PacketSourceInfo sourceInfo)
    {
        Logger.LogInfo($"{sourceInfo.SteamName}: {payload}");
    }
}
```

The first line, `[BepInDependency("com.entwinedteam.entwined")]`, ensures that your plugin will load after Entwined.

Next up is:
```c#
static EntwinedPacketChannel<string> helloWorldChannel;
private void Awake()
{
    helloWorldChannel = new EntwinedPacketChannel<string>(this, new StringEntwiner());

    helloWorldChannel.OnMessage += OnMessage;
}
```
This creates a new isolated channel that automatically encodes and decodes strings and adds the function `OnMessage`. This function is run whenever a client broadcasts a packet on the channel. `PacketSourceInfo` contains various information about the client who sent the message.

```c#
private static void OnMessage(string payload, PacketSourceInfo sourceInfo)
{
    Logger.LogInfo($"{sourceInfo.SteamName}: {payload}");
}
```
Because we passed `StringEntwiner` to our channel we do not need to decode a byte array, and can easily log the string to the console.

`OnGUI` creates a button and runs the following code when it is pressed:
```c#
helloWorldChannel.SendMessage("Hello World!");
```
This automatically encodes the string and broadcasts it to all of the connected clients.