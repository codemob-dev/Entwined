# Entwined
An advanced Bopl Battle networking library.

### Example:
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

#### More info and documentation can be found on the [github repo](github.com/codemob-dev/Entwined).