# Autoexec

Autoexec is a Windows service to launch an executable.

If you provide the console argument it will show you the output.

Installation steps for Autoexec Service

Fill ExecutableFullPath and ExecutableArguments in Autoexec.exe.config

Execute the following command in Administrator mode:

sc create Autoexec displayname= "Autoexec" start= auto binpath= "YourPath\Autoexec.exe"
