This sample shows an MVC website sending commands (or events) via a Windows Azure Service Bus.  
In order to run this sample you'll need to create a Windows Azure Service Bus.  Once you've 
created your service bus endpoint you'll need to update the web.config file and the 
ServiceConfiguration.Local.cscfg files with your ACS Connection String.  

After you've updated both files to point to your service bus, you need to set both the 
SampleWebsite and the SampleAzureMessageProcessor projects to start up (Right click the 
solution and select multiple startup projects and set both to start).

When the website and Azure Emulator load, click the "Create a new account" button.  Doing 
so will send a message through the azure bus.  The emulator will respond to the message
and marshal the message through the default in-memory SeekU providers.  Check the 
emulator's console output for confirmation that commands are indeed being issued (but
keep in mind the emulator can be flakey).

You'll also notice that creating a new redirects the website to an async action that 
waits for 2 seconds before showing the web page.  This is a sample lifed from the 
CQRS Journey project.  It's a nice snippet of code that shows how you can poll the 
read model repository to wait for the database to be updated after you issue a command.

There's no actual read model in this example; it's just being simulated.  The take away
is the command being send via Azure to be handled by a long running process.

Finally, if you want to see what sending events is like instead of sending commands, 
you need to make two simple changes.  First, go into the DependencyResolution\IoC.cs 
file and swap out the lines that are commented.  Second, swap the commented line in the
WorkerRoleQueueSample\WorkerRole.cs file to deserialize events.  Thats all it takes.