This program will run `gdb-server.exe` through `wine` in a given `pressure-vessel` for a steam game. To simplify, this lets you debug steam games across their sandbox and proton with full debug symbols using gdb. 

## How To Use

To use, first run the game, then run this with `--appId` set to the games app id, `--gdb` with the path to `gdb-server.exe`, then `--processName` with the name of the process to debug.
Example:
```
--appId=1237970 --processName=Titanfall2.exe --gdb="/mnt/ExtraSpace/SteamLibrary/steamapps/compatdata/1237970/pfx/drive_c/Program Files/w64devkit/bin/gdbserver.exe"
```

Once you've done that, you should see your game freeze. Once its frozen, attach to it via your IDE's remote debug feature. 
If you are for example using a Jetbrains product,you could even include this as a script to launch before running the remote debug, allowing 1 click debugging of steam games running through proton.

## Arguments

`--appId={appId}` The AppId of the application, used to find its `pressure-vessel` to run everything in.

`--processName=App.exe` The name of the process to attach the debugger to.

`--pressureVesselCmd="/path/to/vessel/` The path to the `pressure-vessel` to use. By default runs `SteamLinuxRuntime_sniper`.

`--gdb="/path/to/gdb-server.exe"` The path to the `gdb-server` to use. This must be the windows version of the `gdb-server` and will be run via `wine gdb-server.exe --once localhost:{port} --attach {pid}`

`--port="1234"` Specifies the port to use for remote debugging. Defaults to `1234`.
